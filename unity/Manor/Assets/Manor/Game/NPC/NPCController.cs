using Manor.Game.Interaction;
using UnityEngine;

namespace Manor.Game.NPC
{
    /// <summary>
    /// Reactive-tier NPC: wanders between points, idles, turns to face the player when
    /// spoken to. Deliberately simple — the architecture matters more than the AI at
    /// this stage. See docs/design/04-npc-system.md for the three-tier plan.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class NPCController : MonoBehaviour, IInteractable
    {
        [SerializeField] private NPCDefinition definition;
        [SerializeField] private Transform anchor;

        private enum State { Idle, Walking, Talking }

        private CharacterController _controller;
        private Vector3 _home;
        private Vector3 _destination;
        private State _state = State.Idle;
        private float _stateTimer;
        private Transform _player;
        private int _lineIndex;

        public string Prompt => "TALK";
        public Transform Anchor => anchor != null ? anchor : transform;
        public bool IsAvailable => _state != State.Talking;

        /// <summary>Mission target id, blank if this NPC is scenery.</summary>
        public string MissionId => definition != null ? definition.missionId : string.Empty;

        public string DisplayName => definition != null ? definition.displayName : "Local";

        /// <summary>Most recent line spoken. Read by the HUD after an interaction.</summary>
        public string LastLine { get; private set; } = string.Empty;

        /// <summary>Raised with the spoken line when the player talks to this NPC.</summary>
        public event System.Action<NPCController, string> Spoke;

        public void Configure(NPCDefinition def) => definition = def;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _home = transform.position;
            EnterIdle();
        }

        private void Update()
        {
            _stateTimer -= Time.deltaTime;

            switch (_state)
            {
                case State.Idle:
                    if (_stateTimer <= 0f) EnterWalking();
                    break;

                case State.Walking:
                    StepTowardsDestination();
                    break;

                case State.Talking:
                    FacePlayer();
                    if (_stateTimer <= 0f) EnterIdle();
                    break;
            }

            // Keep NPCs on the ground without a full gravity model.
            if (!_controller.isGrounded) _controller.Move(Vector3.down * (9.81f * Time.deltaTime));
        }

        private void StepTowardsDestination()
        {
            Vector3 flat = _destination - transform.position;
            flat.y = 0f;

            if (flat.sqrMagnitude < 0.36f || _stateTimer <= 0f)
            {
                EnterIdle();
                return;
            }

            Vector3 direction = flat.normalized;
            float speed = definition != null ? definition.walkSpeed : 1.3f;
            _controller.Move(direction * (speed * Time.deltaTime));
            transform.rotation = Quaternion.Slerp(
                transform.rotation, Quaternion.LookRotation(direction), 8f * Time.deltaTime);
        }

        private void FacePlayer()
        {
            if (_player == null) return;
            Vector3 flat = _player.position - transform.position;
            flat.y = 0f;
            if (flat.sqrMagnitude < 0.01f) return;
            transform.rotation = Quaternion.Slerp(
                transform.rotation, Quaternion.LookRotation(flat), 10f * Time.deltaTime);
        }

        private void EnterIdle()
        {
            _state = State.Idle;
            Vector2 range = definition != null ? definition.idleRange : new Vector2(2f, 7f);
            _stateTimer = Random.Range(range.x, range.y);
        }

        private void EnterWalking()
        {
            float radius = definition != null ? definition.wanderRadius : 14f;
            Vector2 offset = Random.insideUnitCircle * radius;
            _destination = _home + new Vector3(offset.x, 0f, offset.y);
            _state = State.Walking;
            _stateTimer = 20f; // give up rather than push a wall forever
        }

        public void Interact(GameObject interactor)
        {
            _player = interactor != null ? interactor.transform : null;
            _state = State.Talking;
            _stateTimer = 4f;
            LastLine = NextLine();
            Spoke?.Invoke(this, LastLine);
        }

        private string NextLine()
        {
            if (definition == null || definition.lines == null || definition.lines.Length == 0)
            {
                return "Alright?";
            }
            string line = definition.lines[_lineIndex % definition.lines.Length];
            _lineIndex++;
            return line;
        }
    }
}
