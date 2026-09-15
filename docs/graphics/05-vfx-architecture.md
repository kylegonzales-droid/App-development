# VFX architecture

## Governing rule

The directive states it and it is the right filter: **never add an effect because
it looks impressive. Effects communicate world state.** An effect that does not
tell the player something true about the world is cut, however good it looks.

## Every effect declares a contract

No effect enters the project without all six. This is the gate, enforced at review.

```
Effect {
  lifecycle        spawn → simulate → fade → despawn → return to pool
  pooling          pre-warmed count; zero steady-state allocation
  qualityLevels    LOW / MEDIUM / HIGH / ULTRA behaviour, explicitly authored
  budget           declared ms and particle count at each tier
  fallback         what LOW does instead — never "nothing" for a state-carrying effect
  meaning          what world state this communicates
}
```

**`meaning` is a required field.** If it cannot be filled in, the effect does not
ship.

## Pooling — non-negotiable

Per `.claude/rules/game-stack.md`, steady-state allocation in the game loop is a
bug. Every emitter, particle buffer and decal comes from a pre-warmed pool sized
at load. Pool exhaustion **degrades** (oldest recycled, or spawn skipped by
priority) — it never allocates and never stalls.

Pools are sized from the worst realistic case, not the average: storm at night in
the Charter, with a chase in progress.

## Effect catalogue

| Effect | Meaning it carries | Tier floor |
|---|---|---|
| Rain layers 1–12 | Precipitation state | LOW (subset) |
| Snow fall / accumulation | Season, temperature | post-slice |
| Puddle ripples | Rain intensity, and something moved through it | MEDIUM |
| Splash (foot, wheel) | Water depth, your own speed | MEDIUM |
| Road spray behind vehicles | Surface wetness + speed | MEDIUM |
| Windscreen droplets + wipers | You are in a car, in rain | MEDIUM |
| Breath vapour | Cold — winter, night | HIGH |
| Steam (vents, extractor fans, drains) | A real place with real plumbing | MEDIUM |
| Exhaust | Engine running, cold morning | MEDIUM |
| Dust / grit | Dry surface disturbed | LOW |
| Leaves (drift, wind, kick-up) | Autumn, wind direction | post-slice |
| Litter movement | Wind direction and strength | LOW |
| Sparks | Impact, damage | MEDIUM |
| Smoke (vehicle damage) | Vehicle state | LOW |
| Impact effects (melee) | Contact registered | LOW |
| Glass break | Destruction state | MEDIUM |
| Light haze / halo in mist | Fog density + a light source | MEDIUM |
| UI particles | Feedback only — **strictly rationed** | LOW |

**LOW never gets "nothing" for a state-carrying effect.** If rain is the only
signal that the road is now slippery, LOW must still show rain — fewer particles,
simpler shading, same information. Degradation removes fidelity, never meaning.

## Budgets

| Tier | Particle budget | Active emitters | VFX frame cost |
|---|---|---|---|
| LOW | 1,500 | 24 | 0.5 ms |
| MEDIUM | 4,000 | 48 | 1.0 ms |
| HIGH | 10,000 | 96 | 2.0 ms |
| ULTRA | 20,000 (GPU sim) | 160 | 2.5 ms |

**Overdraw is the real cost on mobile, not particle count.** Controls:

- Particles render into a **half-resolution off-screen buffer**, composited with a
  depth-aware upsample (`01-graphics-and-rendering-architecture.md`).
- **Hard cap on per-pixel layers**; the worst offenders (large soft smoke sprites)
  get tight alpha bounds and geometry-trimmed quads rather than full quads.
- **Soft particles** against depth to avoid hard intersection lines.
- Large, near-camera, high-alpha effects are the ones to interrogate — a single
  full-screen smoke card can cost more than ten thousand small particles.

## GPU vs CPU simulation

| Simulation | Where | Why |
|---|---|---|
| Rain, snow, dust, leaves, litter | **GPU** | Thousands of independent particles, no gameplay read-back |
| Splashes, sparks, impacts | CPU spawn → GPU sim | Spawned by gameplay events |
| Anything gameplay reads back | **CPU** | GPU read-back stalls the pipeline — never do it per frame |

The principle behind GPU-driven effects is that particles spawn from *world
information* — surface type, normal, wetness — rather than from hand-placed
emitters, which is both cheaper and more systemic. That is the approach to adopt:
**rain impact particles should sample the surface they hit and produce the right
splash for tarmac, water, or grass automatically**, rather than an artist placing
emitters per surface.

## Decals

Decals carry a large share of environmental storytelling at low cost: puddle
edges, oil stains, tyre marks, kerb scuffs, drain staining, damp patches on brick.

- **Clustered decal projection**, batched by atlas, in the opaque pass.
- Budget: 64 visible at LOW → 256 at ULTRA.
- **Dynamic decals** (tyre marks, scuffs) come from a pool with an age-out.

## LOD and culling

- **Distance bands** per effect: full → reduced rate → billboard → cull.
- **Screen-size culling** — an emitter under a few pixels is skipped entirely.
- **Off-screen emitters do not simulate** unless a gameplay system needs their
  state, in which case the *state* advances and the *particles* do not.
- **Importance scaling** — effects near the player and near the objective keep
  budget; background effects are thinned first under pressure.

## Implementation

**Package A (Unity URP)** — VFX Graph for GPU-simulated systems; Shuriken for
CPU-spawned gameplay effects; custom Renderer Feature for the half-res particle
buffer and decal pass.

**Package B (Metal)** — compute-shader particle simulation with indirect draw;
explicit pool buffers; decals as a clustered projection pass in the frame graph.

## Verification

1. Unit-test pool behaviour: exhaustion degrades rather than allocates; recycling
   is correct; no leak over a long soak.
2. Assert **zero steady-state allocation** in the effect path under Instruments —
   this is a hard gate, per `.claude/rules/game-stack.md`.
3. Overdraw visualisation on the worst case; confirm the layer cap holds.
4. Each effect reviewed at all four tiers, in three weather states — and against
   its declared `meaning`.
5. Frame time and variance in a storm chase, cold and after 30 minutes.
