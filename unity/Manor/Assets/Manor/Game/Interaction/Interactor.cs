using UnityEngine;
using UnityEngine.InputSystem;

namespace Manor.Game.Interaction
{
    /// <summary>
    /// Finds the single best thing to interact with and exposes it for the HUD.
    ///
    /// One context action at a time, chosen by distance and facing — never a wall of
    /// buttons. See docs/design/11-mobile-controls.md.
    /// </summary>
    public sealed class Interactor : MonoBehaviour
    {
        [SerializeField] private float radius = 2.6f;
        [SerializeField] private LayerMask mask = ~0;

        [Tooltip("How much being in front of the player matters versus being close.")]
        [SerializeField, Range(0f, 1f)] private float facingWeight = 0.55f;

        [Tooltip("Rescan interval. Every frame is wasteful; this is imperceptible.")]
        [SerializeField] private float scanInterval = 0.1f;

        private readonly Collider[] _hits = new Collider[24];
        private float _nextScan;

        public IInteractable Best { get; private set; }

        /// <summary>Raised when an interaction actually fires.</summary>
        public event System.Action<IInteractable> Interacted;

        private void Update()
        {
            if (Time.time >= _nextScan)
            {
                _nextScan = Time.time + scanInterval;
                Best = FindBest();
            }

            if (Best != null && WasPressed()) Fire();
        }

        private void Fire()
        {
            IInteractable target = Best;
            target.Interact(gameObject);
            Interacted?.Invoke(target);
        }

        private IInteractable FindBest()
        {
            int count = Physics.OverlapSphereNonAlloc(
                transform.position, radius, _hits, mask, QueryTriggerInteraction.Collide);

            IInteractable best = null;
            float bestScore = float.NegativeInfinity;
            Vector3 forward = transform.forward;

            for (int i = 0; i < count; i++)
            {
                if (_hits[i] == null) continue;

                // GetComponent rather than TryGetComponent: the generic TryGetComponent
                // overload is fussy about interface types across Unity versions.
                var candidate = _hits[i].GetComponent<IInteractable>();
                if (candidate == null) continue;
                if (!candidate.IsAvailable || candidate.Anchor == null) continue;

                Vector3 toTarget = candidate.Anchor.position - transform.position;
                float distance = toTarget.magnitude;
                if (distance > radius) continue;

                Vector3 flat = Vector3.ProjectOnPlane(toTarget, Vector3.up);
                float facing = flat.sqrMagnitude > 0.0001f
                    ? Vector3.Dot(forward, flat.normalized)
                    : 1f;

                // Behind the player is still reachable, just heavily penalised.
                float score = (1f - distance / radius) * (1f - facingWeight)
                              + Mathf.Max(facing, 0f) * facingWeight;

                if (score > bestScore)
                {
                    bestScore = score;
                    best = candidate;
                }
            }
            return best;
        }

        private static bool WasPressed()
        {
            Keyboard k = Keyboard.current;
            if (k != null && k.eKey.wasPressedThisFrame) return true;
            Gamepad pad = Gamepad.current;
            return pad != null && pad.buttonWest.wasPressedThisFrame;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.63f, 0.24f, 0.35f);
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
