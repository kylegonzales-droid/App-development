# Graphics architecture

Technical direction for the game's visual systems. Produced in response to the
graphics directive; extends the Phase 1 design package in `../design/`.

## Read this first

**[`00-engine-decision.md`](00-engine-decision.md)** — the directive's feature set
requires a 3D renderer. The project is currently specified on SpriteKit, which is
2D. That contradiction must be resolved before any implementation begins.

## Documents

| Doc | Covers |
|---|---|
| [`00-engine-decision.md`](00-engine-decision.md) | **Blocking.** Engine options, scope reality, four packages, recommendation |
| [`01-graphics-and-rendering-architecture.md`](01-graphics-and-rendering-architecture.md) | Frame budget, clustered forward pipeline, frame graph, resolution strategy, thermal, colour pipeline, quality tiers, modules |
| [`02-material-architecture.md`](02-material-architecture.md) | Surface state as data, the four wetness operations, porosity, drying curve, puddles, snow accumulation |
| [`03-lighting-architecture.md`](03-lighting-architecture.md) | Light budget, twelve moods, the six Kingston colour grades, shadows, ambient, readability floor |
| [`04-weather-architecture.md`](04-weather-architecture.md) | Weather state machine, transitions, fan-out, seasons, twelve rain layers costed, fog |
| [`05-vfx-architecture.md`](05-vfx-architecture.md) | Effect contracts, pooling, budgets, overdraw, GPU vs CPU, decals, LOD |
| [`06-animation-architecture.md`](06-animation-architecture.md) | Layer stack, locomotion, motion-matching principles, inertialisation, IK, latency budget |
| [`07-world-interaction-architecture.md`](07-world-interaction-architecture.md) | Interaction tiers, shared accumulation buffers, stateful objects, priority list |
| [`08-mobile-performance-strategy.md`](08-mobile-performance-strategy.md) | Device tiers, the quality ladder, thermal management, memory, bandwidth, measurement |
| [`09-kingston-environment-pipeline.md`](09-kingston-environment-pipeline.md) | Kit-of-parts, street furniture, authoring pipeline, LOD, what Claude can and cannot build |
| [`10-vertical-slice-graphics-plan.md`](10-vertical-slice-graphics-plan.md) | Phases A–R sequenced, the feel gate, acceptance criteria |
| [`11-first-prototype.md`](11-first-prototype.md) | **"One Wet Street"** — the first thing to build |
| [`12-character-pipeline.md`](12-character-pipeline.md) | Character tiers, budgets, skin/eye/hair/cloth shading, import validation |
| [`13-character-sourcing.md`](13-character-sourcing.md) | Topology and licence checklists for acquiring base meshes |

## Principles carried through all of them

1. **Visual quality is a product of systems**, not of any single axis.
2. **Weather is world state, not a screen filter.** It lives in `KingstonCore` as
   pure data and fans out to rendering, audio, NPCs, vehicles and gameplay.
3. **Pure state, engine-facing rendering.** Weather, materials and interaction
   state are testable without a GPU, which is where coverage belongs.
4. **Every effect declares its meaning**, its budget, its pooling and its
   fallback, or it does not ship.
5. **There is no spare frame budget.** A new effect names its donor.
6. **Degrade fidelity, never identity.** LOW and ULTRA are the same street.
7. **Cinematic never beats readable.** The grade has an accessibility floor.
8. **Measure frame time and variance, cold and after 30 minutes.**
9. **Prove the visual result before building the system behind it.**
10. **Original work only.** Techniques are studied from public SIGGRAPH and GDC
    material; no assets, code, artwork or colour grading is reproduced from any
    existing game.

## Research basis

Public SIGGRAPH 2020 material on GPU-driven effects, lighting technology, froxel
volumetric fog, screen-space wetness and deformable snow was used as **technique
reference only**. Specific talk contents were not verified line-by-line in this
session; where a claim mattered it is attributed generally rather than precisely.
Nothing in these documents reproduces any studio's assets, implementation or look.
