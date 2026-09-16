# Known issues — Manor Kingston

Updated 2026-09-16. Honest status; nothing here is aspirational.

## Verification state

| Claim | Status |
|---|---|
| C# compiles | **NOT VERIFIED** — no compiler has ever seen it |
| 39 NUnit tests pass | **NOT VERIFIED** — written, never executed |
| Scene generates | **NOT VERIFIED** |
| Play Mode works | **NOT VERIFIED** |
| Windows build works | **NOT VERIFIED** |
| Numeric models correct | **VERIFIED** — 32 checks in `tools/model-validation/` |
| C# structurally sound | **VERIFIED** — 40 files brace-balanced, namespaced |
| `Manor.Core` engine-free | **VERIFIED** — no `UnityEngine` reference |
| Editor code confined to Editor assembly | **VERIFIED** |

Everything in the first group needs a machine with Unity. All authoring so far
happened in a Linux cloud container with no Unity, no .NET SDK and no GUI.

## Defects found and fixed by review (still uncompiled)

1. **HUD dialogue would never have appeared.** `HUDController.Start()` called
   `FindObjectsByType<NPCController>()` to subscribe to dialogue events, but
   NPCs are created in `NPCSpawner.Start()`. The relative order of two `Start()`
   calls is undefined, so the HUD could subscribe to zero NPCs and fail silently.
   Fixed by subscribing to `Interactor.Interacted` — which exists from `Awake` —
   and reading `NPCController.LastLine`.
2. **`OnEnable` subscribed to `GameDirector.Instance`** before another object's
   `Awake` was guaranteed to have run. Moved to `Start`, with unsubscribe in
   `OnDestroy` against a cached reference.
3. **`Object.DestroyImmediate` / `Object.FindFirstObjectByType`** left
   unqualified in editor scripts — ambiguous between `UnityEngine.Object` and
   `System.Object` if the project enables ImplicitUsings. Fully qualified.
4. **`Shader.PropertyToID` recomputed every update** in `WetSurface`. Cached.
5. **`GetBonesPerVertex()`** read without checking `IsCreated` in the character
   validator — would throw on an unskinned mesh. Guarded.

## Expected problems on first open

Ranked by likelihood. None are confirmed; all are predictions from static review.

1. **URP API drift across 6000.x.** `QualityController.SetRenderScale` already
   uses reflection to avoid a hard dependency, but the URP assembly name in
   `Manor.Game.asmdef` (`Unity.RenderPipelines.Universal.Runtime`) may differ.
2. **Input System namespace.** `Manor.Game.asmdef` references
   `Unity.InputSystem`. If the package resolves under a different assembly name
   the reference must be re-pointed in the Inspector.
3. **`SerializedObject.FindProperty` names** in `KingstonSceneBuilder`. Any
   rename of a `[SerializeField]` field silently returns null, then throws.
4. **Unvalidated version pins.** `ProjectVersion.txt` says `6000.0.0f1` and the
   package versions in `manifest.json` were written without an Editor. Unity
   resolves both on first open.
5. **URP asset may not exist.** The project has no URP pipeline asset committed.
   If `GraphicsSettings.currentRenderPipeline` is null, materials fall back to
   the built-in shader and the look will be wrong. Create a URP asset and assign
   it in Project Settings → Graphics.

## Known design limitations

- Player and NPCs are capsules. Real characters need
  `docs/graphics/13-character-sourcing.md`.
- No animation. The locomotion state machine in
  `docs/graphics/06-animation-architecture.md` is designed, not built.
- No audio at all.
- No save/load, no minimap, no LODs, no occlusion culling, no baked lighting.
- Wetness darkens and glosses URP Lit via `MaterialPropertyBlock`. The
  four-operation Shader Graph in `docs/graphics/02-material-architecture.md`
  comes later.
- Snow is modelled in `Manor.Core` but has no accumulation rendering.
- Seasons exist in the data model only.
- **No performance data of any kind.** Every performance figure in the docs is a
  target, not a measurement.
