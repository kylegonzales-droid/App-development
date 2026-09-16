using UnityEngine;

namespace Manor.Game.Systems
{
    /// <summary>
    /// Makes one renderer respond to wetness: darker albedo, higher smoothness.
    ///
    /// Deliberately implemented with a MaterialPropertyBlock on URP's standard Lit
    /// shader rather than a custom Shader Graph, so the prototype has no shader that
    /// can fail to compile. The four physically-grounded operations described in
    /// docs/graphics/02-material-architecture.md move into a Shader Graph later.
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public sealed class WetSurface : MonoBehaviour
    {
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int SmoothnessId = Shader.PropertyToID("_Smoothness");

        [Tooltip("How much this material darkens when wet. Brick high, glass near zero.")]
        [SerializeField, Range(0f, 1f)] private float porosity = 0.75f;

        [SerializeField, Range(0f, 1f)] private float drySmoothness = 0.12f;

        [Tooltip("Rescan interval. Wetness moves slowly; per-frame updates are wasted work.")]
        [SerializeField] private float updateInterval = 0.25f;

        private Renderer _renderer;
        private MaterialPropertyBlock _block;
        private Color _dryColour = Color.white;
        private float _nextUpdate;
        private float _applied = -1f;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _block = new MaterialPropertyBlock();

            if (_renderer.sharedMaterial != null && _renderer.sharedMaterial.HasProperty(BaseColorId))
            {
                _dryColour = _renderer.sharedMaterial.GetColor(BaseColorId);
            }
        }

        private void Update()
        {
            if (Time.time < _nextUpdate) return;
            _nextUpdate = Time.time + updateInterval;

            float wetness = Shader.GetGlobalFloat(Shader.PropertyToID("_ManorWetness"));
            if (Mathf.Abs(wetness - _applied) < 0.005f) return;
            _applied = wetness;

            // Water fills microsurface (smoother) and reduces diffuse scattering (darker).
            float darkening = Mathf.Lerp(1f, Mathf.Lerp(0.95f, 0.42f, porosity), wetness);

            _renderer.GetPropertyBlock(_block);
            _block.SetColor(BaseColorId, _dryColour * darkening);
            _block.SetFloat(SmoothnessId, Mathf.Lerp(drySmoothness, 0.85f, wetness));
            _renderer.SetPropertyBlock(_block);
        }
    }
}
