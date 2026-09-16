using Manor.Core.Time;
using UnityEngine;

namespace Manor.Game.Systems
{
    /// <summary>
    /// Turns the pure clock into a sun angle, light colour and ambient level.
    ///
    /// Lighting moods are parameter sets, not brightness values — see
    /// docs/graphics/03-lighting-architecture.md. This is the runtime that blends them.
    /// </summary>
    [RequireComponent(typeof(Light))]
    public sealed class TimeOfDayDriver : MonoBehaviour
    {
        [Header("Sun path")]
        [Tooltip("Compass direction the sun swings around. 170 is roughly south for the UK.")]
        [SerializeField] private float southYaw = 170f;
        [SerializeField] private float maxElevation = 58f;

        [Header("Colour")]
        [SerializeField] private Gradient sunColour;
        [SerializeField] private Gradient ambientColour;
        [SerializeField] private AnimationCurve sunIntensity =
            AnimationCurve.Linear(0f, 0f, 1f, 1f);

        private Light _sun;

        private void Awake()
        {
            _sun = GetComponent<Light>();
            if (sunColour == null || sunColour.colorKeys.Length == 0) sunColour = DefaultSunGradient();
            if (ambientColour == null || ambientColour.colorKeys.Length == 0) ambientColour = DefaultAmbientGradient();
        }

        private void Update()
        {
            GameDirector director = GameDirector.Instance;
            if (director?.Clock == null) return;

            float hour = director.Clock.Hour;
            float t = hour / 24f;

            // Elevation peaks at noon and goes negative at night.
            float elevation = Mathf.Sin((hour - 6f) / 12f * Mathf.PI) * maxElevation;
            transform.rotation = Quaternion.Euler(elevation, southYaw, 0f);

            float daylight = Mathf.Clamp01(Mathf.Sin((hour - 6f) / 12f * Mathf.PI));

            // Overcast and rain flatten the directional contribution — that is what
            // makes an overcast British day cheap to light and correct to look at.
            float cloud = director.WeatherNow.CloudCover;
            float directional = daylight * Mathf.Lerp(1f, 0.28f, cloud);

            _sun.color = sunColour.Evaluate(t);
            _sun.intensity = sunIntensity.Evaluate(daylight) * Mathf.Lerp(1.15f, 0.35f, cloud);
            _sun.shadowStrength = Mathf.Lerp(0.35f, 0.85f, directional);

            RenderSettings.ambientLight = ambientColour.Evaluate(t) * Mathf.Lerp(0.85f, 1.15f, cloud);
        }

        private static Gradient DefaultSunGradient()
        {
            var g = new Gradient();
            g.SetKeys(
                new[]
                {
                    new GradientColorKey(new Color(0.16f, 0.22f, 0.36f), 0.00f), // night
                    new GradientColorKey(new Color(0.95f, 0.62f, 0.35f), 0.29f), // dawn
                    new GradientColorKey(new Color(1.00f, 0.96f, 0.90f), 0.50f), // noon
                    new GradientColorKey(new Color(0.94f, 0.58f, 0.30f), 0.79f), // dusk
                    new GradientColorKey(new Color(0.16f, 0.22f, 0.36f), 1.00f)
                },
                new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
            return g;
        }

        private static Gradient DefaultAmbientGradient()
        {
            var g = new Gradient();
            g.SetKeys(
                new[]
                {
                    new GradientColorKey(new Color(0.06f, 0.09f, 0.14f), 0.00f),
                    new GradientColorKey(new Color(0.35f, 0.38f, 0.44f), 0.30f),
                    new GradientColorKey(new Color(0.55f, 0.58f, 0.62f), 0.50f),
                    new GradientColorKey(new Color(0.33f, 0.34f, 0.42f), 0.80f),
                    new GradientColorKey(new Color(0.06f, 0.09f, 0.14f), 1.00f)
                },
                new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
            return g;
        }
    }
}
