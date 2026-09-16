using Manor.Core.Missions;
using Manor.Core.Time;
using Manor.Game.Interaction;
using Manor.Game.NPC;
using Manor.Game.Systems;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Manor.Game.UI
{
    /// <summary>
    /// The HUD, drawn with IMGUI.
    ///
    /// IMGUI is chosen deliberately for the prototype: it needs no Canvas prefabs, no
    /// font assets and no scene wiring, so the slice has nothing that can break on
    /// import. It is NOT the shipping HUD — that is a UI Toolkit or Canvas pass once
    /// the layout in docs/design/08-ui-system.md is settled.
    /// </summary>
    public sealed class HUDController : MonoBehaviour
    {
        [SerializeField] private Interactor interactor;
        [SerializeField] private Player.PlayerController player;
        [SerializeField] private CameraRig.ThirdPersonCamera cameraRig;

        private const float Pad = 18f;

        private GUIStyle _objective, _prompt, _meta, _dialogue, _title, _menuItem;
        private bool _stylesReady;
        private Manor.Core.Missions.MissionRunner _subscribedRunner;
        private bool _paused;
        private string _dialogueLine = string.Empty;
        private float _dialogueUntil;

        // Subscribe in Start, not OnEnable: GameDirector creates the MissionRunner in
        // Awake, and OnEnable can run before another object's Awake has completed.
        private void Start()
        {
            GameDirector director = GameDirector.Instance;
            if (director != null)
            {
                director.Missions.BeatChanged += OnBeatChanged;
                _subscribedRunner = director.Missions;
            }

            // Listen to the Interactor rather than to every NPC. NPCs are created by
            // NPCSpawner.Start(), and the relative order of two Start() calls is
            // undefined — subscribing to NPCs here would silently find none.
            if (interactor != null) interactor.Interacted += OnInteracted;
        }

        private void OnDestroy()
        {
            if (_subscribedRunner != null) _subscribedRunner.BeatChanged -= OnBeatChanged;
            if (interactor != null) interactor.Interacted -= OnInteracted;
        }

        private void OnInteracted(IInteractable target)
        {
            if (target is NPCController npc) ShowDialogue($"{npc.DisplayName}:  {npc.LastLine}", 4.5f);
        }

        private void OnBeatChanged(Beat beat)
        {
            if (beat == null) ShowDialogue("Objective complete.", 4f);
        }

        private void ShowDialogue(string text, float seconds)
        {
            _dialogueLine = text;
            _dialogueUntil = Time.time + seconds;
        }

        private void Update()
        {
            Keyboard k = Keyboard.current;
            if (k != null && k.escapeKey.wasPressedThisFrame) TogglePause();
        }

        private void TogglePause()
        {
            _paused = !_paused;
            Time.timeScale = _paused ? 0f : 1f;
            Cursor.lockState = _paused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = _paused;
            if (player != null) player.InputEnabled = !_paused;
            if (cameraRig != null) cameraRig.InputEnabled = !_paused;
        }

        private void OnGUI()
        {
            EnsureStyles();
            GameDirector director = GameDirector.Instance;
            if (director == null) return;

            DrawObjective(director);
            DrawMeta(director);
            DrawPrompt();
            DrawDialogue();
            if (_paused) DrawPauseMenu();
        }

        private void DrawObjective(GameDirector director)
        {
            string text = director.Missions.ObjectiveText;
            if (string.IsNullOrEmpty(text)) return;

            GUI.Label(new Rect(Pad + 16f, Pad, 620f, 30f), text, _objective);
            GUI.color = new Color(0.95f, 0.63f, 0.24f);
            GUI.Label(new Rect(Pad, Pad + 3f, 16f, 20f), "●", _objective);
            GUI.color = Color.white;
        }

        private void DrawMeta(GameDirector director)
        {
            int hour = Mathf.FloorToInt(director.Clock.Hour);
            int minute = Mathf.FloorToInt((director.Clock.Hour - hour) * 60f);
            DayPart part = director.Clock.Part;
            var w = director.WeatherNow;

            string line = $"{hour:00}:{minute:00}   {part}   {w.Dominant}";
            var size = _meta.CalcSize(new GUIContent(line));
            GUI.Label(new Rect(Screen.width - size.x - Pad, Pad, size.x, 24f), line, _meta);
        }

        private void DrawPrompt()
        {
            IInteractable best = interactor != null ? interactor.Best : null;
            if (best == null) return;

            string text = $"[E]  {best.Prompt}";
            var size = _prompt.CalcSize(new GUIContent(text));
            GUI.Label(
                new Rect((Screen.width - size.x) * 0.5f, Screen.height * 0.62f, size.x, 32f),
                text, _prompt);
        }

        private void DrawDialogue()
        {
            if (Time.time > _dialogueUntil || string.IsNullOrEmpty(_dialogueLine)) return;

            const float W = 720f, H = 54f;
            var rect = new Rect((Screen.width - W) * 0.5f, Screen.height - H - 64f, W, H);
            GUI.Label(rect, _dialogueLine, _dialogue);
        }

        private void DrawPauseMenu()
        {
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), GUIContent.none);

            const float W = 340f, H = 250f;
            var panel = new Rect((Screen.width - W) * 0.5f, (Screen.height - H) * 0.5f, W, H);

            GUI.Label(new Rect(panel.x, panel.y, W, 48f), "MANOR KINGSTON", _title);

            float y = panel.y + 76f;
            GUI.Label(new Rect(panel.x, y, W, 26f), "Esc      resume", _menuItem); y += 30f;
            GUI.Label(new Rect(panel.x, y, W, 26f), "WASD     move", _menuItem); y += 30f;
            GUI.Label(new Rect(panel.x, y, W, 26f), "Shift    run", _menuItem); y += 30f;
            GUI.Label(new Rect(panel.x, y, W, 26f), "Mouse    look", _menuItem); y += 30f;
            GUI.Label(new Rect(panel.x, y, W, 26f), "E        interact", _menuItem);
        }

        private void EnsureStyles()
        {
            if (_stylesReady) return;
            _stylesReady = true;

            _objective = Tinted(19, new Color(0.92f, 0.94f, 0.96f), FontStyle.Normal);
            _meta = Tinted(15, new Color(0.62f, 0.68f, 0.75f), FontStyle.Normal);
            _prompt = Tinted(20, new Color(0.95f, 0.78f, 0.45f), FontStyle.Bold);
            _dialogue = Tinted(18, new Color(0.90f, 0.93f, 0.96f), FontStyle.Normal);
            _dialogue.alignment = TextAnchor.MiddleCenter;
            _dialogue.wordWrap = true;
            _title = Tinted(24, new Color(0.95f, 0.63f, 0.24f), FontStyle.Bold);
            _title.alignment = TextAnchor.MiddleCenter;
            _menuItem = Tinted(16, new Color(0.80f, 0.85f, 0.90f), FontStyle.Normal);
            _menuItem.alignment = TextAnchor.MiddleCenter;
        }

        private static GUIStyle Tinted(int size, Color colour, FontStyle style) =>
            new(GUI.skin.label)
            {
                fontSize = size,
                fontStyle = style,
                normal = { textColor = colour }
            };
    }
}
