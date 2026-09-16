using System.Collections.Generic;

namespace Manor.Core.Weather
{
    /// <summary>The authored look of one settled weather condition.</summary>
    public readonly struct WeatherProfile
    {
        public float Precipitation { get; }
        public float Snowfall { get; }
        public float CloudCover { get; }
        public float Visibility { get; }
        public float WindSpeed { get; }
        public float RelativeHumidity { get; }

        /// <summary>Mean real seconds this condition holds before the chain rolls again.</summary>
        public float MeanDwellSeconds { get; }

        public WeatherProfile(
            float precipitation, float snowfall, float cloudCover, float visibility,
            float windSpeed, float relativeHumidity, float meanDwellSeconds)
        {
            Precipitation = precipitation;
            Snowfall = snowfall;
            CloudCover = cloudCover;
            Visibility = visibility;
            WindSpeed = windSpeed;
            RelativeHumidity = relativeHumidity;
            MeanDwellSeconds = meanDwellSeconds;
        }
    }

    public static class WeatherProfiles
    {
        //                                                     precip snow  cloud  vis    wind  humid  dwell(s)
        private static readonly WeatherProfile ClearP     = new(0.00f, 0f,   0.10f, 2000f, 2.0f, 0.45f, 300f);
        private static readonly WeatherProfile CloudyP    = new(0.00f, 0f,   0.55f, 1600f, 3.0f, 0.60f, 420f);
        private static readonly WeatherProfile OvercastP  = new(0.00f, 0f,   0.95f, 1100f, 3.5f, 0.72f, 600f);
        private static readonly WeatherProfile LightRainP = new(0.30f, 0f,   0.95f, 800f,  4.0f, 0.85f, 300f);
        private static readonly WeatherProfile HeavyRainP = new(0.85f, 0f,   1.00f, 420f,  6.5f, 0.95f, 240f);
        private static readonly WeatherProfile StormP     = new(1.00f, 0f,   1.00f, 260f, 12.0f, 0.97f, 180f);
        private static readonly WeatherProfile FogP       = new(0.00f, 0f,   0.80f, 90f,   0.8f, 0.98f, 360f);
        private static readonly WeatherProfile LightSnowP = new(0.05f, 0.35f, 0.95f, 500f, 3.0f, 0.88f, 300f);
        private static readonly WeatherProfile HeavySnowP = new(0.10f, 0.90f, 1.00f, 220f, 5.5f, 0.92f, 240f);

        private static readonly Dictionary<WeatherCondition, WeatherProfile> Map = new()
        {
            { WeatherCondition.Clear, ClearP },
            { WeatherCondition.Cloudy, CloudyP },
            { WeatherCondition.Overcast, OvercastP },
            { WeatherCondition.LightRain, LightRainP },
            { WeatherCondition.HeavyRain, HeavyRainP },
            { WeatherCondition.Storm, StormP },
            { WeatherCondition.Fog, FogP },
            { WeatherCondition.LightSnow, LightSnowP },
            { WeatherCondition.HeavySnow, HeavySnowP }
        };

        public static WeatherProfile For(WeatherCondition condition) => Map[condition];
    }
}
