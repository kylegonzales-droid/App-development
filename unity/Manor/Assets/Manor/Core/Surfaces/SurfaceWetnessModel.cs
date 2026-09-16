using System;

namespace Manor.Core.Surfaces
{
    /// <summary>
    /// The wet/dry state model. Pure arithmetic over plain data — no GPU, no engine,
    /// no frame coupling — so the drying curve is unit-testable rather than something
    /// you verify by squinting at a screenshot.
    ///
    /// The renderer never owns this. It reads <see cref="SurfacePatch.Wetness"/> and
    /// shades accordingly (see docs/graphics/02-material-architecture.md).
    /// </summary>
    public static class SurfaceWetnessModel
    {
        /// <summary>
        /// Wetness lost per second by a fully exposed, freely draining surface with
        /// DryingRate 1.0 under <see cref="DryingConditions.Temperate"/>.
        /// Tuned so such a surface takes roughly four minutes to dry from saturated.
        /// </summary>
        public const float ReferenceEvaporationPerSecond = 0.0045f;

        /// <summary>Sheltered ground still dries, just far more slowly. Never zero.</summary>
        public const float MinExposureFactor = 0.25f;

        /// <summary>Water that cannot drain away still evaporates. Never zero.</summary>
        public const float MinDrainageFactor = 0.30f;

        /// <summary>Wetness gained this step. Rain that cannot reach the surface cannot wet it.</summary>
        public static float WettingDelta(in SurfacePatch patch, float rainIntensity, float deltaSeconds)
        {
            if (rainIntensity <= 0f || deltaSeconds <= 0f) return 0f;
            return rainIntensity * patch.SkyExposure * patch.Material.AbsorptionRate * deltaSeconds;
        }

        /// <summary>Wetness lost this step. Returned positive; callers subtract it.</summary>
        public static float DryingDelta(
            in SurfacePatch patch, in DryingConditions conditions, float deltaSeconds)
        {
            if (deltaSeconds <= 0f) return 0f;

            float exposureFactor = MinExposureFactor + (1f - MinExposureFactor) * patch.SkyExposure;
            float drainageFactor = MinDrainageFactor + (1f - MinDrainageFactor) * patch.Drainage;

            return ReferenceEvaporationPerSecond
                   * patch.Material.DryingRate
                   * exposureFactor
                   * drainageFactor
                   * EvaporationFactor(conditions)
                   * deltaSeconds;
        }

        /// <summary>
        /// Ambient multiplier on evaporation. Warm, windy and dry air removes water fastest;
        /// near-freezing saturated air barely removes any.
        /// </summary>
        public static float EvaporationFactor(in DryingConditions c)
        {
            float temperature = Clamp((c.TemperatureC + 5f) / 30f, 0.05f, 1.5f);
            float wind = 1f + c.WindSpeed * 0.08f;
            float humidity = Clamp(1f - c.RelativeHumidity * 0.7f, 0.2f, 1f);
            return temperature * wind * humidity;
        }

        /// <summary>
        /// Advance one patch by one step. Wetting and drying both apply every step;
        /// during rain wetting dominates, and when it stops only drying remains.
        /// </summary>
        public static SurfacePatch Advance(
            SurfacePatch patch, float rainIntensity, in DryingConditions conditions, float deltaSeconds)
        {
            if (deltaSeconds < 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(deltaSeconds), deltaSeconds, "Time does not run backwards.");
            }

            float next = patch.Wetness
                         + WettingDelta(patch, rainIntensity, deltaSeconds)
                         - DryingDelta(patch, conditions, deltaSeconds);

            patch.Wetness = SurfacePatch.Clamp01(next);
            return patch;
        }

        /// <summary>
        /// Seconds for a patch to fall from its current wetness to <paramref name="target"/>
        /// with no rain. Used by tests and tuning; the runtime integrates per frame instead.
        /// Returns <see cref="float.PositiveInfinity"/> if the patch cannot reach the target.
        /// </summary>
        public static float SecondsToDryTo(
            in SurfacePatch patch, float target, in DryingConditions conditions)
        {
            float clampedTarget = SurfacePatch.Clamp01(target);
            if (patch.Wetness <= clampedTarget) return 0f;

            float perSecond = DryingDelta(patch, conditions, 1f);
            if (perSecond <= 0f) return float.PositiveInfinity;

            return (patch.Wetness - clampedTarget) / perSecond;
        }

        private static float Clamp(float v, float min, float max) => v < min ? min : (v > max ? max : v);
    }
}
