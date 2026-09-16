namespace Manor.Core.Weather
{
    /// <summary>
    /// The world's weather, as plain data. Every consumer — materials, lighting, VFX,
    /// audio, NPC appearance and behaviour, vehicles, gameplay — reads this.
    /// There is deliberately no "rain effect enabled" flag anywhere: if rain is falling,
    /// it is because <see cref="Precipitation"/> is above zero.
    /// </summary>
    public readonly struct WeatherState
    {
        public WeatherCondition From { get; }
        public WeatherCondition To { get; }

        /// <summary>0 at the start of a transition, 1 when it has completed.</summary>
        public float TransitionProgress { get; }

        /// <summary>0..1 blended rainfall intensity.</summary>
        public float Precipitation { get; }

        /// <summary>0..1 blended snowfall intensity.</summary>
        public float Snowfall { get; }

        /// <summary>0..1 blended cloud cover.</summary>
        public float CloudCover { get; }

        /// <summary>Metres.</summary>
        public float Visibility { get; }

        public float TemperatureC { get; }

        /// <summary>Metres per second.</summary>
        public float WindSpeed { get; }

        /// <summary>0..1.</summary>
        public float RelativeHumidity { get; }

        public WeatherState(
            WeatherCondition from, WeatherCondition to, float transitionProgress,
            float precipitation, float snowfall, float cloudCover, float visibility,
            float temperatureC, float windSpeed, float relativeHumidity)
        {
            From = from;
            To = to;
            TransitionProgress = transitionProgress;
            Precipitation = precipitation;
            Snowfall = snowfall;
            CloudCover = cloudCover;
            Visibility = visibility;
            TemperatureC = temperatureC;
            WindSpeed = windSpeed;
            RelativeHumidity = relativeHumidity;
        }

        public bool IsTransitioning => TransitionProgress < 1f;

        /// <summary>The condition currently contributing most to the blend.</summary>
        public WeatherCondition Dominant => TransitionProgress >= 0.5f ? To : From;

        public Surfaces.DryingConditions ToDryingConditions() =>
            new(TemperatureC, WindSpeed, RelativeHumidity);

        public override string ToString() =>
            IsTransitioning
                ? $"{From}->{To} {TransitionProgress:0.00} precip={Precipitation:0.00}"
                : $"{To} precip={Precipitation:0.00}";
    }
}
