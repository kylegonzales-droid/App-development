using System;

namespace Manor.Core.Surfaces
{
    /// <summary>
    /// Authored, immutable description of how one material family responds to water.
    /// Porosity is the highest-leverage number here: it is what makes brick darken hard
    /// and glazed tile barely darken, from the same global wetness value.
    /// </summary>
    public readonly struct SurfaceMaterialProfile : IEquatable<SurfaceMaterialProfile>
    {
        public string Name { get; }

        /// <summary>0..1. How strongly the surface darkens and holds water. Brick high, glass ~0.</summary>
        public float Porosity { get; }

        /// <summary>Wetness gained per second at full rain and full sky exposure.</summary>
        public float AbsorptionRate { get; }

        /// <summary>Multiplier on the base evaporation rate. Porous materials release slowly.</summary>
        public float DryingRate { get; }

        /// <summary>0..1. How much accumulating snow sticks to this material.</summary>
        public float SnowAffinity { get; }

        public SurfaceMaterialProfile(
            string name, float porosity, float absorptionRate, float dryingRate, float snowAffinity)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Material profiles must be named.", nameof(name));
            }

            Name = name;
            Porosity = Clamp01(porosity);
            AbsorptionRate = absorptionRate < 0f ? 0f : absorptionRate;
            DryingRate = dryingRate < 0f ? 0f : dryingRate;
            SnowAffinity = Clamp01(snowAffinity);
        }

        private static float Clamp01(float v) => v < 0f ? 0f : (v > 1f ? 1f : v);

        public bool Equals(SurfaceMaterialProfile other) =>
            string.Equals(Name, other.Name, StringComparison.Ordinal);

        public override bool Equals(object obj) => obj is SurfaceMaterialProfile p && Equals(p);
        public override int GetHashCode() => Name?.GetHashCode() ?? 0;
        public override string ToString() => Name;
    }
}
