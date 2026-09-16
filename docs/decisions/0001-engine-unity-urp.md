# ADR 0001 — Engine: Unity 6 + URP

**Status:** Accepted — 2026-09-16
**Supersedes:** the SpriteKit recommendation in `../design/12-technical-architecture.md`
**Decided by:** project owner

## Decision

The game is built in **Unity 6 (6000.x LTS) with the Universal Render Pipeline**,
targeting iOS via Metal. It is a **3D game**. It is not a 2D game.

## The instruction this records

> "ensure that this will be using 3d designing and not a 2d game. characters need
> to be good render"

Those two constraints settle the four packages in `../graphics/00-engine-decision.md`:

| Package | Why it is out |
|---|---|
| **D — SpriteKit 2D** | 2D. Directly excluded by the instruction. |
| **C — RealityKit** | 3D, but exposes no custom render passes, so the weather and material architecture cannot be built in it. |
| **B — Metal custom renderer** | 3D, but has no skeletal animation, skinning or character shading until we write them. "Characters need to be good render" is 6–9 months away on this path. |
| **A — Unity 6 + URP** | 3D, mature character rendering and animation on day one, mature mobile pipeline. |

## Consequences — accepted

**Gained**
- Character rendering, skinning, animation, IK, LOD, culling, instancing, quality
  tiers and Addressables streaming exist on day one rather than being written.
- The "One Wet Street" prototype moves from ~6–9 months (Metal) to ~4–7 weeks.
- A large ecosystem of licensable modular environment and character assets, which
  matters because art volume — not rendering technique — is this project's
  binding constraint.

**Lost**
- **The project is no longer Swift-native.** Gameplay is C#.
- Most of Phase 0's *engine* skills (SpriteKit, SwiftUI, Swift concurrency,
  Swift Testing) no longer apply to gameplay code. They are **not deleted** —
  see "Phase 0 skills" below.
- Unity licensing terms and runtime fees are the owner's to review. Not assessed
  here.

**Unchanged and still binding**
- The layering rule: pure, engine-free game logic separated from presentation.
  `KingstonCore` becomes `Manor.Core`, a C# assembly compiled with
  `noEngineReferences`, so the compiler enforces the boundary.
- Every architecture document in `../graphics/` was written engine-agnostic at the
  system level precisely so this decision would not invalidate them. They stand;
  their "Package A" implementation notes are now the live ones.
- All Phase 1 world, story, faction and mission design is unaffected.
- The eleven operating principles in `../../CLAUDE.md`.

## Phase 0 skills — kept, not deleted

Of the 30 project-local skills, these still apply directly and must not be removed:

`ios-simulator` · `debugging-instruments` · `ios-ettrace-performance` ·
`ios-memgraph-analysis` · `metrickit` · `ios-accessibility` · `apple-hig` ·
`ui-review` · the six TDD workflow skills

They cover profiling, device testing, accessibility, HIG and App Store work, none
of which the engine change affects. The Swift and SpriteKit skills are now dormant
reference rather than active guidance.

## What this does not decide

- Unity licence tier.
- Exact Unity 6 LTS patch version — pinned on first Editor open.
- Art resourcing, which remains the project's largest open risk
  (`../design/15-open-decisions.md`).
- iOS deployment target.
