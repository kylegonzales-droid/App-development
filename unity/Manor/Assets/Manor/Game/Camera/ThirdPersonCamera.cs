using UnityEngine;
using UnityEngine.InputSystem;

namespace Manor.Game.CameraRig
{
    /// <summary>
    /// Damped third-person follow with velocity look-ahead and wall collision.
    ///
    /// Look-ahead is the detail that makes a follow camera feel good: the camera leads
    /// the direction of travel so the player sees where they are going rather than
    /// where they have been.
    /// </summary>
    public sealed class ThirdPersonCamera : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 pivotOffset = new(0f, 1.55f, 0f);

        [Header("Framing")]
        [SerializeField] private float distance = 4.2f;
        [SerializeField] private float minPitch = -25f;
        [SerializeField] private float maxPitch = 65f;
        [SerializeField] private float startPitch = 14f;

        [Header("Feel")]
        [SerializeField] private float followDamping = 0.12f;
        [SerializeField] private float lookSensitivity = 0.14f;
        [SerializeField] private float lookAheadStrength = 0.55f;
        [SerializeField] private float lookAheadDamping = 0.35f;

        [Header("Collision")]
        [SerializeField] private LayerMask collisionMask = ~0;
        [SerializeField] private float collisionRadius = 0.28f;
        [SerializeField] private float collisionBuffer = 0.22f;

        private float _yaw;
        private float _pitch;
        private Vector3 _positionVelocity;
        private Vector3 _lookAhead;
        private Vector3 _lookAheadVelocity;
        private Vector3 _lastTargetPosition;

        public bool InputEnabled { get; set; } = true;

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            if (target != null) _lastTargetPosition = target.position;
        }

        private void Start()
        {
            _pitch = startPitch;
            if (target != null)
            {
                _yaw = target.eulerAngles.y;
                _lastTargetPosition = target.position;
            }
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            if (InputEnabled)
            {
                Vector2 look = ReadLook();
                _yaw += look.x * lookSensitivity;
                _pitch = Mathf.Clamp(_pitch - look.y * lookSensitivity, minPitch, maxPitch);
            }

            Vector3 pivot = target.position + pivotOffset;

            // Look-ahead from actual movement, not from input, so it survives being pushed.
            Vector3 frameMove = target.position - _lastTargetPosition;
            _lastTargetPosition = target.position;
            Vector3 desiredAhead = Time.deltaTime > 0f
                ? Vector3.ProjectOnPlane(frameMove / Time.deltaTime, Vector3.up) * lookAheadStrength
                : Vector3.zero;
            _lookAhead = Vector3.SmoothDamp(_lookAhead, desiredAhead, ref _lookAheadVelocity, lookAheadDamping);
            pivot += _lookAhead;

            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            Vector3 wanted = pivot - rotation * Vector3.forward * distance;
            wanted = ResolveCollision(pivot, wanted);

            transform.position = Vector3.SmoothDamp(
                transform.position, wanted, ref _positionVelocity, followDamping);
            transform.rotation = rotation;
        }

        private Vector3 ResolveCollision(Vector3 pivot, Vector3 wanted)
        {
            Vector3 direction = wanted - pivot;
            float length = direction.magnitude;
            if (length < 0.001f) return wanted;

            if (Physics.SphereCast(pivot, collisionRadius, direction / length,
                    out RaycastHit hit, length, collisionMask, QueryTriggerInteraction.Ignore))
            {
                float safe = Mathf.Max(hit.distance - collisionBuffer, 0.4f);
                return pivot + direction / length * safe;
            }
            return wanted;
        }

        private static Vector2 ReadLook()
        {
            Vector2 look = Vector2.zero;
            Mouse m = Mouse.current;
            if (m != null) look += m.delta.ReadValue();
            Gamepad pad = Gamepad.current;
            if (pad != null) look += pad.rightStick.ReadValue() * 6f;
            return look;
        }
    }
}
