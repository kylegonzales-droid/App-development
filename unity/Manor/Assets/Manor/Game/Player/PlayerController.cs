using UnityEngine;
using UnityEngine.InputSystem;

namespace Manor.Game.Player
{
    /// <summary>
    /// Third-person locomotion on a CharacterController. Movement is camera-relative.
    ///
    /// Responsiveness is the point: acceleration is fast and the character turns toward
    /// the move direction quickly, because on a touchscreen there is no controller
    /// tactility to cover for input latency.
    /// See docs/graphics/06-animation-architecture.md for the latency budget.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        [Header("Speed")]
        [SerializeField] private float walkSpeed = 1.9f;
        [SerializeField] private float runSpeed = 4.8f;
        [SerializeField] private float acceleration = 22f;
        [SerializeField] private float deceleration = 28f;

        [Header("Turning")]
        [SerializeField] private float turnSmoothTime = 0.08f;

        [Header("Gravity")]
        [SerializeField] private float gravity = -19.6f;
        [SerializeField] private float groundedStick = -2f;

        private CharacterController _controller;
        private Transform _camera;
        private Vector3 _horizontalVelocity;
        private float _verticalVelocity;
        private float _turnVelocity;

        /// <summary>0 when still, 1 at full run. Drives the animator and footstep rate.</summary>
        public float NormalisedSpeed { get; private set; }

        public bool IsGrounded => _controller != null && _controller.isGrounded;

        /// <summary>Set false while a menu or dialogue owns input.</summary>
        public bool InputEnabled { get; set; } = true;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            if (UnityEngine.Camera.main != null) _camera = UnityEngine.Camera.main.transform;
        }

        private void Update()
        {
            Vector2 input = InputEnabled ? ReadMove() : Vector2.zero;
            bool running = InputEnabled && ReadRun();

            Vector3 desired = DesiredDirection(input);
            float targetSpeed = input.sqrMagnitude > 0.01f ? (running ? runSpeed : walkSpeed) : 0f;

            Vector3 target = desired * targetSpeed;
            float rate = targetSpeed > 0.01f ? acceleration : deceleration;
            _horizontalVelocity = Vector3.MoveTowards(_horizontalVelocity, target, rate * Time.deltaTime);

            if (desired.sqrMagnitude > 0.01f) FaceDirection(desired);

            ApplyGravity();

            Vector3 motion = _horizontalVelocity;
            motion.y = _verticalVelocity;
            _controller.Move(motion * Time.deltaTime);

            NormalisedSpeed = runSpeed > 0f ? Mathf.Clamp01(_horizontalVelocity.magnitude / runSpeed) : 0f;
        }

        private Vector3 DesiredDirection(Vector2 input)
        {
            if (input.sqrMagnitude < 0.0001f) return Vector3.zero;

            Vector3 forward = Vector3.forward;
            Vector3 right = Vector3.right;

            if (_camera != null)
            {
                forward = Vector3.ProjectOnPlane(_camera.forward, Vector3.up).normalized;
                right = Vector3.ProjectOnPlane(_camera.right, Vector3.up).normalized;
            }

            return (forward * input.y + right * input.x).normalized;
        }

        private void FaceDirection(Vector3 direction)
        {
            float targetYaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float yaw = Mathf.SmoothDampAngle(
                transform.eulerAngles.y, targetYaw, ref _turnVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        }

        private void ApplyGravity()
        {
            if (_controller.isGrounded && _verticalVelocity < 0f) _verticalVelocity = groundedStick;
            else _verticalVelocity += gravity * Time.deltaTime;
        }

        // Read devices directly rather than through an .inputactions asset, so the
        // project has no binary input asset to get out of sync with the code.
        private static Vector2 ReadMove()
        {
            Vector2 move = Vector2.zero;

            Keyboard k = Keyboard.current;
            if (k != null)
            {
                if (k.wKey.isPressed || k.upArrowKey.isPressed) move.y += 1f;
                if (k.sKey.isPressed || k.downArrowKey.isPressed) move.y -= 1f;
                if (k.dKey.isPressed || k.rightArrowKey.isPressed) move.x += 1f;
                if (k.aKey.isPressed || k.leftArrowKey.isPressed) move.x -= 1f;
            }

            Gamepad pad = Gamepad.current;
            if (pad != null) move += pad.leftStick.ReadValue();

            return Vector2.ClampMagnitude(move, 1f);
        }

        private static bool ReadRun()
        {
            Keyboard k = Keyboard.current;
            if (k != null && k.leftShiftKey.isPressed) return true;
            Gamepad pad = Gamepad.current;
            return pad != null && pad.leftStickButton.isPressed;
        }
    }
}
