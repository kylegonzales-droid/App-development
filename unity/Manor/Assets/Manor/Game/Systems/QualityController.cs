using UnityEngine;

namespace Manor.Game.Systems
{
    public enum QualityTier { Low, Medium, High }

    /// <summary>
    /// Three quality tiers applied at runtime.
    ///
    /// The rule from docs/graphics/08-mobile-performance-strategy.md: degrade fidelity,
    /// never identity. Low must look like the same game — same palette, same mood —
    /// with less of it.
    /// </summary>
    public sealed class QualityController : MonoBehaviour
    {
        [SerializeField] private QualityTier tier = QualityTier.Medium;
        [SerializeField] private WeatherDriver weather;

        [Header("Render scale")]
        [SerializeField] private float lowScale = 0.65f;
        [SerializeField] private float mediumScale = 0.8f;
        [SerializeField] private float highScale = 1.0f;

        public QualityTier Tier => tier;

        private void Start() => Apply(tier);

        public void Apply(QualityTier newTier)
        {
            tier = newTier;

            switch (tier)
            {
                case QualityTier.Low:
                    QualitySettings.shadowDistance = 35f;
                    QualitySettings.shadowCascades = 1;
                    QualitySettings.lodBias = 0.7f;
                    QualitySettings.skinWeights = SkinWeights.TwoBones;
                    Application.targetFrameRate = 30;
                    SetRenderScale(lowScale);
                    weather?.ScaleParticleBudget(0.3f);
                    break;

                case QualityTier.Medium:
                    QualitySettings.shadowDistance = 60f;
                    QualitySettings.shadowCascades = 2;
                    QualitySettings.lodBias = 1.0f;
                    QualitySettings.skinWeights = SkinWeights.FourBones;
                    Application.targetFrameRate = 60;
                    SetRenderScale(mediumScale);
                    weather?.ScaleParticleBudget(0.6f);
                    break;

                default:
                    QualitySettings.shadowDistance = 90f;
                    QualitySettings.shadowCascades = 4;
                    QualitySettings.lodBias = 1.6f;
                    QualitySettings.skinWeights = SkinWeights.FourBones;
                    Application.targetFrameRate = 60;
                    SetRenderScale(highScale);
                    break;
            }
        }

        /// <summary>
        /// Render scale lives on the URP asset. Set via the asset so this compiles
        /// without a hard reference to a specific URP version's API surface.
        /// </summary>
        private static void SetRenderScale(float scale)
        {
            var pipeline = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline;
            if (pipeline == null) return;

            var property = pipeline.GetType().GetProperty("renderScale");
            if (property != null && property.CanWrite)
            {
                property.SetValue(pipeline, Mathf.Clamp(scale, 0.4f, 2f));
            }
        }
    }
}
