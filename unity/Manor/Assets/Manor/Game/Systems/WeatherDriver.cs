using Manor.Core.Surfaces;
using Manor.Core.Weather;
using UnityEngine;

namespace Manor.Game.Systems
{
    /// <summary>
    /// Turns weather state into what you can see: rain and snow particles, fog, and
    /// ground wetness.
    ///
    /// Wetness is NOT a screen filter. It is integrated by the pure model in
    /// Manor.Core and pushed to materials as global shader values, so surfaces soak
    /// and dry at their own rates. See docs/graphics/02-material-architecture.md.
    /// </summary>
    public sealed class WeatherDriver : MonoBehaviour
    {
        private static readonly int WetnessId = Shader.PropertyToID("_ManorWetness");
        private static readonly int SnowId = Shader.PropertyToID("_ManorSnow");

        [Header("Particles")]
        [SerializeField] private ParticleSystem rain;
        [SerializeField] private ParticleSystem rainSplash;
        [SerializeField] private ParticleSystem snow;
        [SerializeField] private float maxRainRate = 2200f;
        [SerializeField] private float maxSplashRate = 650f;
        [SerializeField] private float maxSnowRate = 900f;

        [Header("Fog")]
        [SerializeField] private float clearFogDensity = 0.004f;
        [SerializeField] private float stormFogDensity = 0.032f;
        [SerializeField] private Color dryFog = new(0.60f, 0.65f, 0.70f);
        [SerializeField] private Color wetFog = new(0.44f, 0.48f, 0.54f);

        [Header("Wet look")]
        [Tooltip("How glossy fully wet ground becomes. Drives URP smoothness globally.")]
        [SerializeField, Range(0f, 1f)] private float maxWetSmoothness = 0.82f;

        /// <summary>Representative ground wetness, 0..1. Read by the HUD and audio.</summary>
        public float GroundWetness => _road.Wetness;

        private SurfacePatch _road;
        private bool _initialised;

        private void Awake()
        {
            // Representative open, well-drained tarmac. Individual materials will later
            // carry their own patches; one is enough to drive the global look.
            _road = new SurfacePatch(SurfaceLibrary.Asphalt, skyExposure: 1f, drainage: 0.85f);
            _initialised = true;
        }

        private void Update()
        {
            GameDirector director = GameDirector.Instance;
            if (director == null || !_initialised) return;

            WeatherState w = director.WeatherNow;

            _road = SurfaceWetnessModel.Advance(
                _road, w.Precipitation, w.ToDryingConditions(), Time.deltaTime);

            Shader.SetGlobalFloat(WetnessId, _road.Wetness);
            Shader.SetGlobalFloat(SnowId, _road.SnowDepth);

            ApplyParticles(w);
            ApplyFog(w);
        }

        private void ApplyParticles(in WeatherState w)
        {
            SetRate(rain, w.Precipitation * maxRainRate);
            SetRate(rainSplash, w.Precipitation * maxSplashRate);
            SetRate(snow, w.Snowfall * maxSnowRate);
        }

        private static void SetRate(ParticleSystem system, float rate)
        {
            if (system == null) return;

            ParticleSystem.EmissionModule emission = system.emission;
            emission.rateOverTime = rate;

            bool shouldPlay = rate > 1f;
            if (shouldPlay && !system.isPlaying) system.Play();
            else if (!shouldPlay && system.isPlaying) system.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        private void ApplyFog(in WeatherState w)
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;

            float wetFactor = Mathf.Clamp01(Mathf.Max(w.Precipitation, w.Snowfall));
            float visibilityFactor = Mathf.InverseLerp(2000f, 90f, w.Visibility);

            RenderSettings.fogDensity = Mathf.Lerp(clearFogDensity, stormFogDensity, visibilityFactor);
            RenderSettings.fogColor = Color.Lerp(dryFog, wetFog, wetFactor);
        }

        /// <summary>Exposed so the quality tiers can dial particle budgets down.</summary>
        public void ScaleParticleBudget(float scale)
        {
            maxRainRate *= scale;
            maxSplashRate *= scale;
            maxSnowRate *= scale;
        }

        /// <summary>Global smoothness target for wet-aware materials.</summary>
        public float WetSmoothness => _road.Wetness * maxWetSmoothness;
    }
}
