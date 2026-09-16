using Manor.Core.Surfaces;
using NUnit.Framework;

namespace Manor.Core.Tests
{
    [TestFixture]
    public class SurfaceWetnessModelTests
    {
        private static readonly DryingConditions Temperate = DryingConditions.Temperate;

        private static SurfacePatch OpenRoadCrown() =>
            new(SurfaceLibrary.Asphalt, skyExposure: 1.0f, drainage: 1.0f);

        private static SurfacePatch OpenGutter() =>
            new(SurfaceLibrary.WornAsphalt, skyExposure: 1.0f, drainage: 0.05f);

        private static SurfacePatch UnderTheArch() =>
            new(SurfaceLibrary.Concrete, skyExposure: 0.05f, drainage: 0.20f);

        private static SurfacePatch Soaked(SurfacePatch p)
        {
            p.Wetness = 1f;
            return p;
        }

        private static SurfacePatch Simulate(
            SurfacePatch patch, float rainIntensity, float seconds, float step = 0.1f)
        {
            for (float t = 0f; t < seconds; t += step)
            {
                patch = SurfaceWetnessModel.Advance(patch, rainIntensity, Temperate, step);
            }
            return patch;
        }

        // ---------- wetting ----------

        [Test]
        public void ExposedSurface_WetsInHeavyRain()
        {
            SurfacePatch p = Simulate(OpenRoadCrown(), rainIntensity: 1f, seconds: 30f);
            Assert.That(p.Wetness, Is.GreaterThan(0.5f), "30 s of heavy rain should visibly wet open tarmac.");
        }

        [Test]
        public void ShelteredSurface_StaysDryInRain()
        {
            SurfacePatch p = Simulate(UnderTheArch(), rainIntensity: 1f, seconds: 120f);
            Assert.That(p.Wetness, Is.LessThan(0.15f),
                "Rain cannot reach ground under the railway arch, so it must not wet it.");
        }

        [Test]
        public void Wetness_SaturatesAtOne()
        {
            SurfacePatch p = Simulate(OpenRoadCrown(), rainIntensity: 1f, seconds: 600f);
            Assert.That(p.Wetness, Is.EqualTo(1f).Within(1e-4f));
        }

        [Test]
        public void NoRain_DoesNotWet()
        {
            Assert.That(SurfaceWetnessModel.WettingDelta(OpenRoadCrown(), 0f, 10f), Is.Zero);
        }

        // ---------- drying ----------

        [Test]
        public void Drying_IsSlowerThanWetting()
        {
            var patch = OpenRoadCrown();
            float wetPerSecond = SurfaceWetnessModel.WettingDelta(patch, 1f, 1f);
            float dryPerSecond = SurfaceWetnessModel.DryingDelta(patch, Temperate, 1f);

            Assert.That(wetPerSecond, Is.GreaterThan(dryPerSecond * 4f),
                "Wetting must be substantially faster than drying, or rain reads as a toggle.");
        }

        [Test]
        public void Wetness_FloorsAtZero()
        {
            SurfacePatch p = Simulate(OpenRoadCrown(), rainIntensity: 0f, seconds: 3600f);
            Assert.That(p.Wetness, Is.EqualTo(0f).Within(1e-6f));
        }

        /// <summary>
        /// The prototype's key systemic acceptance criterion, as a test:
        /// stop the rain and the street must dry UNEVENLY. If these three ever come out
        /// equal, the material architecture has failed its central promise.
        /// </summary>
        [Test]
        public void AfterRainStops_StreetDriesUnevenly()
        {
            SurfacePatch crown = Soaked(OpenRoadCrown());
            SurfacePatch gutter = Soaked(OpenGutter());
            SurfacePatch arch = Soaked(UnderTheArch());

            const float FiveMinutes = 300f;
            crown = Simulate(crown, 0f, FiveMinutes);
            gutter = Simulate(gutter, 0f, FiveMinutes);
            arch = Simulate(arch, 0f, FiveMinutes);

            Assert.Multiple(() =>
            {
                Assert.That(crown.Wetness, Is.LessThan(gutter.Wetness),
                    "The road crown sheds water and must dry before the gutter.");
                Assert.That(gutter.Wetness, Is.LessThan(arch.Wetness),
                    "The sheltered arch must stay wettest of all.");
                Assert.That(arch.Wetness - crown.Wetness, Is.GreaterThan(0.2f),
                    "The spread must be large enough to actually see on screen.");
            });
        }

        [Test]
        public void Porosity_ChangesDryingTime()
        {
            var brick = Soaked(new SurfacePatch(SurfaceLibrary.Brick, 1f, 1f));
            var glass = Soaked(new SurfacePatch(SurfaceLibrary.Glass, 1f, 1f));

            float brickSeconds = SurfaceWetnessModel.SecondsToDryTo(brick, 0f, Temperate);
            float glassSeconds = SurfaceWetnessModel.SecondsToDryTo(glass, 0f, Temperate);

            Assert.That(brickSeconds, Is.GreaterThan(glassSeconds * 3f),
                "Porous brick must hold water far longer than glass.");
        }

        [Test]
        public void ColdSaturatedAir_DriesFarSlowerThanWarmDryAir()
        {
            var patch = Soaked(OpenRoadCrown());
            var winter = new DryingConditions(1f, 1f, 0.97f);
            var summer = new DryingConditions(26f, 4f, 0.35f);

            Assert.That(
                SurfaceWetnessModel.SecondsToDryTo(patch, 0f, winter),
                Is.GreaterThan(SurfaceWetnessModel.SecondsToDryTo(patch, 0f, summer) * 3f));
        }

        [Test]
        public void SecondsToDryTo_IsZeroWhenAlreadyDrier()
        {
            var patch = OpenRoadCrown();
            patch.Wetness = 0.1f;
            Assert.That(SurfaceWetnessModel.SecondsToDryTo(patch, 0.5f, Temperate), Is.Zero);
        }

        // ---------- integration stability ----------

        [Test]
        public void StepSize_DoesNotMateriallyChangeResult()
        {
            SurfacePatch fine = Simulate(Soaked(OpenRoadCrown()), 0f, 120f, step: 1f / 120f);
            SurfacePatch coarse = Simulate(Soaked(OpenRoadCrown()), 0f, 120f, step: 1f / 15f);

            Assert.That(fine.Wetness, Is.EqualTo(coarse.Wetness).Within(0.02f),
                "Frame rate must not change how fast the world dries.");
        }
    }
}
