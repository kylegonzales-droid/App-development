# Technical architecture

## Decision 1 — is SwiftUI + SpriteKit still right?

**Yes, with one condition that must be stated plainly.**

### The condition

**SpriteKit is a 2D engine.** The brief asks for "third-person movement." In
SpriteKit that means a **high-angle top-down or isometric camera** — the player
seen from above and slightly behind. It does **not** mean an over-the-shoulder 3D
camera. That is not a limitation to work around; it is what the engine is.

| If you want | Engine | Consequence |
|---|---|---|
| **High-angle 2D** *(recommended)* | **SpriteKit** | Phase 0 stack stands. Achievable by a small team. Distinctive. |
| Isometric 2.5D | SpriteKit | Same engine, more art per asset, harder collision authoring |
| True 3D third-person | SceneKit / RealityKit / Metal | Full rebuild of the technical plan; 3D art pipeline; SpriteKit skills mostly wasted |
| True 3D, fastest path | Unity | Abandons the entire Phase 0 environment and the Swift-native premise |

**Recommendation: high-angle 2D on SpriteKit.** Reasons:

1. **It is the only option a small team can finish.** A 3D open world is an
   order of magnitude more art, tooling and engineering. The art budget, not the
   code, is what kills projects in this genre.
2. **The art direction already depends on it.** Wet tarmac, reflected signage and
   a strong colour script are cheap and gorgeous in authored 2D, and expensive
   and mediocre in low-budget 3D.
3. **Performance is a solved problem**, so the frame budget can be spent on crowd
   density and lighting rather than on drawing the world at all.
4. **It is distinctive.** A beautiful 2D British crime game looks like itself. A
   budget 3D one looks like every asset-pack game on the store.
5. **Phase 0's environment fits it exactly.**

**This is a decision, not a detail — it gates everything downstream. Confirm it
before any implementation begins.**

### Framework roles

| Framework | Role | Status |
|---|---|---|
| **SpriteKit** | World, player, NPCs, vehicles, physics, particles, camera, **and the HUD** | Core |
| **SwiftUI** | Menus, settings, pause, phone, map, meta screens. Non-realtime only. | Core |
| **Swift Concurrency** | Asset loading, save I/O, mission graph evaluation off the hot path | Core |
| **Swift Testing** | Unit tests on `KingstonCore` | Core |
| **XCUITest** | Launch, menu flow, settings, accessibility smoke | Core |
| GameKit | Achievements, leaderboards | **Deferred** — post-slice |
| SwiftData | — | **Not adopted.** See Decision 3 |
| Core Haptics | Haptic table in `10-audio-haptics.md` | **Deferred** — adopt at VS3 |
| AVFoundation | Music and ambience | **Deferred** — adopt at VS2 |
| GameplayKit | Possibly pathfinding / state machines | **Deferred** — evaluate at VS2 |
| Metal | — | **Not adopted.** SpriteKit already sits on it |
| CloudKit, StoreKit, any backend | — | **Not adopted.** No requirement |

## Decision 2 — the HUD is SpriteKit, not SwiftUI

Mixing SwiftUI into a 60 fps HUD is a well-known hitch source: SwiftUI's update
cycle is not frame-synchronised with SpriteKit's, and Observation-driven redraws
land unpredictably inside the frame.

**Rule:** anything that updates per-frame or overlays live gameplay is drawn in
SpriteKit. SwiftUI owns only screens where a dropped frame does not matter —
menus, settings, the phone, the map, results.

## Decision 3 — save data is Codable, not SwiftData

A single-player save is **one document**, written occasionally, read at launch. It
is not a queryable multi-record store.

| | Codable + atomic write *(recommended)* | SwiftData |
|---|---|---|
| Fit | One versioned document | Many queryable records |
| Migration | Explicit version field, plain Swift | `VersionedSchema` + migration plan ceremony |
| Testability | Pure, trivial, no container | Needs a container, even in-memory |
| Failure mode | Readable JSON you can inspect | Opaque store |
| Cost | Near zero | Real complexity for no present benefit |

Per `.claude/rules/game-stack.md`, a framework is adopted only when a present
requirement needs it. **None does.** The `swiftdata` skill stays installed as
reference; if a future feature genuinely needs record-level queries (a large
collectible database, say), revisit it then.

Save: versioned `Codable` struct → JSON → atomic write to Application Support,
with a rotating backup and a corruption fallback. Autosave on mission boundaries
and zone transitions, never mid-beat.

## Module structure

Per `.claude/rules/game-stack.md`. Four Swift packages, enforced by what each can
import.

```
┌──────────────────────────────────────────────────┐
│  App target — thin. Lifecycle, DI wiring only.   │
└───────────┬──────────────────────┬───────────────┘
            │                      │
   ┌────────▼────────┐   ┌─────────▼─────────┐
   │  KingstonUI     │   │  KingstonWorld    │
   │  SwiftUI        │   │  SpriteKit        │
   │  menus, phone,  │   │  scenes, nodes,   │
   │  map, settings  │   │  camera, HUD,     │
   │                 │   │  physics, FX      │
   └────────┬────────┘   └─────────┬─────────┘
            │                      │
            └──────────┬───────────┘
                       │
            ┌──────────▼───────────┐     ┌────────────────────┐
            │   KingstonCore       │◄────│ KingstonServices   │
            │   PURE SWIFT         │     │ save, audio,       │
            │   no SpriteKit       │     │ haptics, GameKit   │
            │   no SwiftUI         │     │ behind protocols   │
            │   no UIKit           │     └────────────────────┘
            │                      │
            │ rules · world state  │
            │ mission runtime      │
            │ faction standing     │
            │ NPC decision logic   │
            │ heat · economy       │
            │ save model           │
            └──────────────────────┘
```

**`KingstonCore` is the rule that matters.** It imports no Apple UI or game
framework. Consequences:

- A whole mission graph runs in a unit test in milliseconds, no simulator.
- Faction standing, heat escalation, the alarm model and economy are all testable
  as pure functions over state.
- The presentation layer can be replaced (or the engine decision revisited)
  without touching the rules.

**Enforcement:** a CI grep for `import SpriteKit|import SwiftUI|import UIKit`
under `KingstonCore/` fails the build. The `architecture-auditor` agent checks
the same boundary on review.

## Concurrency posture

Swift 6, strict concurrency on from commit one — retrofitting it is painful.

- **The game loop is synchronous and stays synchronous.** No `await` in
  `update(_:)`, `didSimulatePhysics`, or contact delegates. Per
  `.claude/rules/game-stack.md`, an actor hop per frame is a frame-time bug.
- Scene, nodes and HUD are `@MainActor`.
- Async is for: asset and chunk loading, save I/O, audio preparation, and mission
  graph evaluation that is not needed this frame.
- `KingstonCore` types are `Sendable` value types wherever possible, which makes
  most of the isolation question disappear.
- The `concurrency-auditor` agent reviews this boundary.

## World streaming

The map exceeds a comfortable memory footprint as one scene.

- Map divided into a **chunk grid**; chunks load and unload by camera proximity
  with a one-chunk hysteresis band so you cannot thrash by standing on a boundary.
- **One texture atlas per zone**, preloaded on zone entry — never a texture load
  in a per-frame path.
- NPCs, vehicles, particles and projectiles come from **pre-warmed object pools**.
  Steady-state allocation in the game loop is treated as a bug.
- Target: **60 fps sustained**, 120 where ProMotion allows; measured as frame
  time *and variance*, never an average.

## Testing strategy

| Layer | Tool | What |
|---|---|---|
| `KingstonCore` | Swift Testing | Mission graphs end-to-end, standing arithmetic, heat escalation, alarm model, save round-trip and version migration, economy. **This is where coverage lives.** |
| `KingstonWorld` | Swift Testing | Pool correctness, chunk load/unload, collision categories, camera maths |
| `KingstonUI` | XCUITest | Launch, settings, pause, accessibility smoke |
| Visual | Manual + capture | Simulator screenshots and video per `/sim-run` |
| Performance | Instruments, ETTrace | Frame time and variance at each milestone; never an average alone |

`swift-snapshot-testing` is a **candidate, not adopted** — revisit when there is
stable UI worth pinning.

## Target and tooling

- **iOS deployment target:** to be set at scaffold. Recommend the **lowest target
  that supports Swift 6 strict concurrency comfortably and covers the intended
  device base** — decide with real device-share data, not aspiration. The game
  must run well on a five-year-old iPhone; that is a harder constraint than the
  API level.
- SwiftLint from commit one, `--strict` in CI.
- CI: build, unit tests, lint, `KingstonCore` import-boundary check.
