namespace Manor.Core.Surfaces
{
    /// <summary>Ambient conditions that govern how fast water leaves a surface.</summary>
    public readonly struct DryingConditions
    {
        public float TemperatureC { get; }

        /// <summary>Metres per second.</summary>
        public float WindSpeed { get; }

        /// <summary>0..1.</summary>
        public float RelativeHumidity { get; }

        public DryingConditions(float temperatureC, float windSpeed, float relativeHumidity)
        {
            TemperatureC = temperatureC;
            WindSpeed = windSpeed < 0f ? 0f : windSpeed;
            RelativeHumidity = relativeHumidity < 0f ? 0f : (relativeHumidity > 1f ? 1f : relativeHumidity);
        }

        /// <summary>A mild, still, damp British afternoon. The default for tests and tuning.</summary>
        public static DryingConditions Temperate => new(15f, 2f, 0.6f);
    }
}
