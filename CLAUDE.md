# App-development — iOS Mobile Game

## Current state — read this first

**Engine decided: Unity 6 + URP. This is a 3D game, not a 2D game.**
See `docs/decisions/0001-engine-unity-urp.md`.

The project has a small amount of real code: `unity/Manor/Assets/Manor/Core/` —
game clock, surface wetness model, weather simulation — plus 33 NUnit tests.
There is **no scene, no renderer, no character, no shader, no gameplay** yet.

**Nothing has been compiled or run.** This container has no Unity and no .NET SDK
(the network policy blocks Microsoft's distribution host). The C# is unverified by
a compiler. Its *arithmetic* is validated separately — see "Verification" below.
Never describe the code as working, tested, or building.

## Target stack (declared, not yet implemented)

| Aspect | Status |
|---|---|
| Engine | **Unity 6 (6000.x LTS) + URP**, iOS via Metal |
| Dimension | **3D.** Third-person. Not 2D — this is explicit and not open for drift. |
| Language | C# |
| Game logic | `Manor.Core` — pure C#, `noEngineReferences: true` |
| Testing | NUnit via the Unity Test Runner |
| Deployment target | **Undecided.** Lowest that covers the intended device base. |
| Persistence, audio, haptics, Game Center, monetisation | **Not adopted.** See "Adding frameworks". |

**Swift, SwiftUI and SpriteKit are no longer the stack.** Phase 0 and Phase 1
assumed them; `docs/decisions/0001-engine-unity-urp.md` supersedes that. The Swift
and SpriteKit skills under `.claude/skills/` are dormant reference — do not follow
them for gameplay code, and do not delete them.

**Still live from Phase 0:** `ios-simulator`, `debugging-instruments`,
`ios-ettrace-performance`, `ios-memgraph-analysis`, `metrickit`,
`ios-accessibility`, `apple-hig`, `ui-review`, and the TDD skills. Profiling,
device testing, accessibility and App Store work are unaffected by the engine.

## Non-negotiable operating principles

1. **Inspect before modifying.** Read the actual files. Never assume structure.
2. **Preserve existing functionality.** No drive-by rewrites of working code.
3. **Prefer reusable systems** over one-off copies of the same logic.
4. **Avoid unnecessary dependencies.** See "Adding frameworks" below.
5. **Avoid speculative architecture.** Build for the requirement in front of you.
6. **Build incrementally.** Small, reviewable, independently verifiable changes.
7. **Test every meaningful change.** A change without a test is unfinished.
8. **Use the iOS Simulator when available** to exercise real behaviour.
9. **Validate visual UI changes** by looking at them, not by reading the diff.
10. **Profile performance when appropriate** rather than guessing at it.
11. **Compiling is not completion.** Never report a feature done because it built.

These are expanded in `.claude/rules/`, which is imported below and is binding.

@.claude/rules/engineering-principles.md
@.claude/rules/game-stack.md
@.claude/rules/hig-for-games.md
@.claude/rules/verification.md

## Adding frameworks

Adopt a package only when a real, present requirement needs it — never because it
is idiomatic, available, or listed as a possibility. This applies to Unity
packages and Asset Store purchases as much as to frameworks.

**Not adopted, pending justification:** analytics, ads, IAP/monetisation, any
backend, cloud save, DOTS/ECS, and any third-party rendering or animation package.
`Packages/manifest.json` is the current agreed set.

**Any third-party asset must have its licence checked for commercial iOS
distribution before it enters the repository**, and the provenance recorded.

When adopting one, record in the same change: what requires it, what was
considered instead, and how it is tested.

## Verification and the tooling reality

This container has **no Unity, no .NET SDK, no Xcode, no Simulator**. It runs
Linux, and the network policy blocks Microsoft's .NET host. Consequences:

- **C# cannot be compiled or run here.** Say so. Never imply otherwise.
- The NUnit suite requires the Unity Editor. It has not been run.
- Building for iOS requires a macOS host with Unity and Xcode.

What *can* be verified here: the numeric models, via
`tools/model-validation/validate_core_model.py`, a Python port of the same
arithmetic asserting the same properties. **A pass there means the model is
right, not that the C# compiles.** Keep it in step with the C# or delete it.

Run it after any change to the weather or surface models:

```sh
python3 tools/model-validation/validate_core_model.py
```

## Where things are

| Path | Contents |
|---|---|
| `unity/Manor/` | The game. See `unity/README.md`. |
| `docs/decisions/` | ADRs. Start at 0001. |
| `docs/design/` | World, story, factions, missions, UI *(Swift-era engine sections superseded)* |
| `docs/graphics/` | Rendering, materials, weather, lighting, VFX, animation, characters, performance |
| `tools/model-validation/` | Python validation of the core numeric models |

## Skills

30 skills are installed project-local under `.claude/skills/` and load
automatically. Provenance and licensing: `.claude/SKILLS-ATTRIBUTION.md`.
Several are now dormant — see "Target stack" above.
