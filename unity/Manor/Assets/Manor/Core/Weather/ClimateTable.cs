using System.Collections.Generic;

namespace Manor.Core.Weather
{
    /// <summary>
    /// Transition weights and seasonal bias for the Kingston climate.
    ///
    /// Tuning intent: overcast is the default state and should hold the majority of
    /// playtime. A world that is constantly dramatic is a world with no weather at all.
    /// </summary>
    public static class ClimateTable
    {
        /// <summary>Relative weights for the next condition, given the current one.</summary>
        private static readonly Dictionary<WeatherCondition, (WeatherCondition Next, float Weight)[]> Transitions = new()
        {
            [WeatherCondition.Clear] = new[]
            {
                (WeatherCondition.Cloudy, 0.60f), (WeatherCondition.Overcast, 0.25f),
                (WeatherCondition.Clear, 0.15f)
            },
            [WeatherCondition.Cloudy] = new[]
            {
                (WeatherCondition.Overcast, 0.45f), (WeatherCondition.Clear, 0.25f),
                (WeatherCondition.LightRain, 0.20f), (WeatherCondition.Cloudy, 0.10f)
            },
            [WeatherCondition.Overcast] = new[]
            {
                (WeatherCondition.LightRain, 0.34f), (WeatherCondition.Cloudy, 0.30f),
                (WeatherCondition.Overcast, 0.20f), (WeatherCondition.Fog, 0.10f),
                (WeatherCondition.LightSnow, 0.06f)
            },
            [WeatherCondition.LightRain] = new[]
            {
                (WeatherCondition.Overcast, 0.44f), (WeatherCondition.HeavyRain, 0.26f),
                (WeatherCondition.LightRain, 0.18f), (WeatherCondition.Cloudy, 0.12f)
            },
            [WeatherCondition.HeavyRain] = new[]
            {
                (WeatherCondition.LightRain, 0.50f), (WeatherCondition.Overcast, 0.26f),
                (WeatherCondition.Storm, 0.14f), (WeatherCondition.HeavyRain, 0.10f)
            },
            [WeatherCondition.Storm] = new[]
            {
                (WeatherCondition.HeavyRain, 0.62f), (WeatherCondition.LightRain, 0.28f),
                (WeatherCondition.Overcast, 0.10f)
            },
            [WeatherCondition.Fog] = new[]
            {
                (WeatherCondition.Overcast, 0.62f), (WeatherCondition.Cloudy, 0.22f),
                (WeatherCondition.Fog, 0.16f)
            },
            [WeatherCondition.LightSnow] = new[]
            {
                (WeatherCondition.Overcast, 0.48f), (WeatherCondition.HeavySnow, 0.22f),
                (WeatherCondition.LightSnow, 0.18f), (WeatherCondition.Cloudy, 0.12f)
            },
            [WeatherCondition.HeavySnow] = new[]
            {
                (WeatherCondition.LightSnow, 0.58f), (WeatherCondition.Overcast, 0.30f),
                (WeatherCondition.HeavySnow, 0.12f)
            }
        };

        /// <summary>
        /// Multiplier applied to a candidate's weight for the current season.
        /// Snow outside winter is multiplied to zero, which is what keeps a Kingston
        /// summer free of blizzards without a special case in the simulation.
        /// </summary>
        public static float SeasonalWeight(WeatherCondition condition, Season season) =>
            condition switch
            {
                WeatherCondition.Clear => season switch
                {
                    Season.Summer => 2.0f, Season.Spring => 1.2f,
                    Season.Autumn => 0.7f, _ => 0.5f
                },
                WeatherCondition.LightRain or WeatherCondition.HeavyRain => season switch
                {
                    Season.Autumn => 1.4f, Season.Winter => 1.2f,
                    Season.Spring => 1.1f, _ => 0.7f
                },
                WeatherCondition.Storm => season switch
                {
                    Season.Autumn => 1.5f, Season.Winter => 1.2f, _ => 0.8f
                },
                WeatherCondition.Fog => season switch
                {
                    Season.Winter => 2.0f, Season.Autumn => 1.6f, _ => 0.4f
                },
                WeatherCondition.LightSnow or WeatherCondition.HeavySnow =>
                    season == Season.Winter ? 1.0f : 0f,
                _ => 1.0f
            };

        public static (WeatherCondition Next, float Weight)[] CandidatesFor(WeatherCondition current) =>
            Transitions[current];

        /// <summary>Base air temperature for a season, before time-of-day variation.</summary>
        public static float BaseTemperature(Season season) => season switch
        {
            Season.Spring => 11f,
            Season.Summer => 20f,
            Season.Autumn => 10f,
            _ => 4f
        };
    }
}
