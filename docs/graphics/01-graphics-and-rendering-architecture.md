# Graphics and rendering architecture

Engine-agnostic at system level. See `00-engine-decision.md` — nothing here
assumes the engine choice has been made.

## Governing principle

Visual quality is a **product of systems**, not of any single one. The directive
states this and it is correct: geometry × materials × lighting × shadows ×
atmospherics × particles × weather × animation × camera × post × interaction.

The corollary matters more than the principle: **a project that spends its whole
budget on one axis looks worse than one that spends evenly.** High-poly models
under flat lighting look like a tech demo. Modest geometry under committed
lighting, correct materials and real atmosphere looks like a game.

Kingston is on our side here. An overcast British town is a **low-dynamic-range,
high-texture-detail** environment — soft ambient light, wet surfaces, dense small
detail. That is far cheaper to render convincingly than bright sun with hard
shadows and long sightlines. Our setting and our budget agree with each other.

## Frame budget — the number everything answers to

Target **60 fps → 16.6 ms**. Reserve 2 ms for OS and jitter. **14.6 ms of work.**

| System | Budget | Notes |
|---|---|---|
| Geometry / depth prepass | 2.0 ms | Opaque, front-to-back, instanced |
| Lighting (opaque shading) | 3.0 ms | Clustered forward; cost scales with local lights |
| Shadows | 1.5 ms | Cascades + a strict local-shadow cap |
| Atmospherics (fog) | 1.5 ms | Half-res froxel or analytic fallback |
| Transparent + particles | 2.0 ms | Half-res off-screen buffer |
| Post-processing chain | 1.5 ms | Grade, bloom, wetness composite |
| Gameplay + animation + physics (CPU) | 3.0 ms | Must overlap GPU, not serialise |
| **Headroom** | **0.1 ms** | Deliberately near zero — see below |

**A budget with no headroom is the point.** Every new effect must take its
milliseconds from something already in the table. "It only costs 0.4 ms" is how a
frame budget dies. New effects arrive with a named donor.

Report **frame time and variance**, never an average — per
`.claude/rules/game-stack.md`. A game averaging 60 fps that drops a frame at every
spawn is not a 60 fps game.

## Pipeline shape

**Clustered forward rendering.** Not deferred.

| | Clustered forward *(chosen)* | Deferred |
|---|---|---|
| Bandwidth | Low — critical on mobile TBDR | High G-buffer cost |
| MSAA | Cheap | Painful |
| Transparency | Native | Needs a forward pass anyway |
| Material variety | Good | Constrained by G-buffer layout |
| Many local lights | Good (clustered) | Excellent |

Mobile GPUs are tile-based deferred renderers; a full G-buffer fights the
hardware's own bandwidth strategy. Clustered forward gives us the many small
emissive light sources Kingston needs — shopfronts, headlights, phone screens,
chip-shop fluorescents — without paying deferred's bandwidth tax.

## Frame graph

```
 1  Culling            frustum → occlusion → distance; produce draw lists
 2  Shadow passes      1 directional cascade set + capped local shadows
 3  Depth prepass      opaque only; feeds SSAO, fog, soft particles, SSR
 4  Cluster assign     local lights → froxel clusters
 5  Opaque             clustered forward shading, instanced, front-to-back
 6  Screen-space       SSAO, SSR (wet ground only, half-res, HIGH+)
 7  Atmospherics       froxel fog integration, half-res, temporally reprojected
 8  Transparent        sorted back-to-front, half-res particle buffer
 9  Composite          upsample particles + fog with depth-aware filter
10  Post chain         exposure → bloom → grade → vignette → wetness overlay
11  UI                 native resolution, never in the post chain
```

**Steps 6 and 7 are the entire visual identity.** Wet ground reflections and
atmosphere are what make a Kingston street read as Kingston at night. They get
protected budget; geometry detail gets cut before they do.

## Resolution strategy

| Pass | Resolution | Reason |
|---|---|---|
| Opaque, depth | Native (or dynamic-scaled) | Silhouette and text clarity |
| SSAO, SSR | Half | Low-frequency; nobody sees the difference |
| Fog froxels | Quarter-res volume, temporally reprojected | Low-frequency; ND hit the same wall on PS4 |
| Particles, transparents | Half, depth-aware upsample | Overdraw is the #1 mobile particle killer |
| Post chain | Native | Grade and bloom must be clean |
| UI | Native, outside post | Never grade the HUD |

**Dynamic resolution scaling** on the opaque pass, driven by a rolling frame-time
average: scale 100 % → 70 % before dropping any *system*. Players notice a missing
reflection far more than a 15 % resolution change.

## Thermal reality — the constraint desktop advice ignores

A phone sustaining 60 fps will throttle. Plan for it rather than being surprised:

- **Three-stage thermal response.** Nominal → Fair → Serious, read from the OS
  thermal state. Each stage drops a defined step of the quality ladder
  (`08-mobile-performance-strategy.md`), never a sudden cliff.
- **Target 80 % of budget, not 100 %.** A frame that exactly fits at room
  temperature does not fit after twenty minutes of play.
- **Measure on a cold device *and* after 30 minutes.** The second number is the
  real one. Benchmarks taken in the first two minutes are fiction.

## Colour pipeline

Linear space throughout; sRGB only at the final write. HDR intermediate where the
target supports it.

- **Physically-grounded exposure** with a manual bias per lighting mood, so a
  sodium-lit street and an overcast noon both land where art direction wants them.
- **ACES-style tonemap**, then the Kingston grade (`03-lighting-architecture.md`).
- **Grading is a LUT per mood**, blended over time. Never a full-screen colour
  multiply — that flattens and desaturates, which is exactly the "screen filter"
  failure the directive rules out.

**Readability constraint, binding:** the grade is applied *before* UI, and never
reduces gameplay-critical contrast below the accessibility floor in
`.claude/rules/hig-for-games.md`. A screenshot that looks cinematic but hides an
NPC in shadow is a bug, not a look.

## Quality tiers

Four tiers, defined once and referenced by every system. Full ladder in
`08-mobile-performance-strategy.md`.

| | LOW | MEDIUM | HIGH | ULTRA |
|---|---|---|---|---|
| Device class | 5-year-old | 3-year-old | current | current Pro |
| Render scale | 0.6–0.7 | 0.8 | 1.0 dynamic | 1.0 |
| Shadows | Directional only, 1 cascade | 2 cascades | 3 + capped local | 4 + local |
| Fog | Analytic height fog | Froxel ¼, no reprojection | Froxel ¼ + reprojection | Froxel ½ |
| SSR | Off (cubemap only) | Off (cubemap) | Half-res, ground only | Half-res + contact |
| Particles | 25 % budget | 50 % | 100 % | 100 % + GPU sim |
| Wetness | Albedo darken + cubemap | + normal ripples | + SSR | Full |

**Graceful degradation rule:** LOW must still read as *the same game* — same
palette, same mood, same silhouettes. It loses fidelity, never identity. A LOW
screenshot and an ULTRA screenshot should be recognisably the same street.

## Module structure

Extends `docs/design/12-technical-architecture.md`. The `KingstonCore` boundary
survives the engine change unchanged — **that is the main reason it was drawn
there.**

```
KingstonCore          pure, no engine types
  world state · weather state machine · time of day · mission runtime
  material state (wetness, snow depth) as DATA
  ↓ publishes immutable state snapshots
KingstonRender        engine-facing
  frame graph · material system · lighting · fog · VFX · post
KingstonAnim          locomotion state, IK targets, blend weights
KingstonWorld         streaming, LOD, culling, instancing, interaction
```

**`KingstonCore` owns weather and material state as plain data; the renderer only
*reads* it.** Consequences: the weather state machine, the drying curve, the snow
accumulation model and the seasonal transitions are all unit-testable as pure
functions with no GPU, no device, and no engine — which is exactly where
`.claude/rules/verification.md` says coverage belongs.

## Verification

Per `.claude/rules/verification.md`, a visual system is not done when it compiles:

1. Unit tests on the pure state models in `KingstonCore`.
2. GPU capture inspected for the pass actually doing what it claims.
3. Frame time **and variance** measured, cold and after 30 minutes.
4. Screenshots at all four tiers, compared side by side.
5. Checked in three lighting moods × three weather states — a system that only
   looks right at night in the rain is not finished.
6. Readability check against the accessibility floor.
