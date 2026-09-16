namespace Manor.Core.Surfaces
{
    /// <summary>
    /// One place on one material. Sky exposure and drainage are authored per location
    /// during environment work; they are the reason a street dries unevenly rather than
    /// fading out as one flat value.
    /// </summary>
    public struct SurfacePatch
    {
        public SurfaceMaterialProfile Material;

        /// <summary>0 fully sheltered (under the railway arch), 1 fully open (middle of the Loop).</summary>
        public float SkyExposure;

        /// <summary>0 holds water (gutter, pothole), 1 sheds it (road crown, camber).</summary>
        public float Drainage;

        /// <summary>0..1. Current water held by the surface.</summary>
        public float Wetness;

        /// <summary>0..1. Accumulated snow, normalised to this surface's maximum.</summary>
        public float SnowDepth;

        public SurfacePatch(
            SurfaceMaterialProfile material, float skyExposure, float drainage,
            float wetness = 0f, float snowDepth = 0f)
        {
            Material = material;
            SkyExposure = Clamp01(skyExposure);
            Drainage = Clamp01(drainage);
            Wetness = Clamp01(wetness);
            SnowDepth = Clamp01(snowDepth);
        }

        internal static float Clamp01(float v) => v < 0f ? 0f : (v > 1f ? 1f : v);

        public override string ToString() =>
            $"{Material.Name} exposure={SkyExposure:0.00} drainage={Drainage:0.00} wetness={Wetness:0.000}";
    }
}
