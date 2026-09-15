# Mobile performance strategy

## The rule this document exists to enforce

**Never assume console or desktop resources.** Every high-end effect ships with a
mobile strategy or it does not ship.

Two facts worth keeping in view, from the reference material behind this
directive: *The Last of Us Part II* targeted **30 fps on PlayStation 4**, and the
team still had to fight for volumetric fog resolution inside that budget. A phone
sustaining 60 fps under thermal load has substantially less headroom than a PS4 at
30. We are not aiming at that budget; we are aiming at a fraction of it, twice as
often.

## Device tiers

| Tier | Class | Render scale | Target |
|---|---|---|---|
| **LOW** | ~5-year-old iPhone | 0.6–0.7 | 30 fps locked |
| **MEDIUM** | ~3-year-old | 0.8 | 60 fps |
| **HIGH** | Current | 1.0 dynamic | 60 fps |
| **ULTRA** | Current Pro | 1.0 | 60 fps, 120 where ProMotion allows |

**A locked 30 on LOW is a legitimate, honest target.** A stuttering 45 is worse
than a stable 30 in every way that a player can perceive.

## The quality ladder

Tier selection is automatic from device class, overridable in Settings. Under
thermal pressure the game **walks down this ladder one rung at a time**, in this
order, and walks back up when pressure clears:

```
 1  Render scale        1.0 → 0.85 → 0.7
 2  Particle budget     100% → 60% → 30%
 3  SSR                 half-res → cubemap only → off
 4  Local shadow count  4 → 2 → 0 (contact shadows only)
 5  Fog                 froxel+reproj → froxel → analytic height fog
 6  Shadow cascades     4 → 3 → 2 → 1
 7  Decal budget        256 → 128 → 64
 8  NPC ambient count   150 → 100 → 60
 9  Draw distance       full → 0.8 → 0.6
10  Foliage density     100% → 60% → 30%
```

**Order matters and is deliberate.** Render scale first because it is the largest
win for the least perceptual damage. NPC count and draw distance last because they
change what the *game* is, not just how it looks.

**Never crossed:** the palette, the lighting moods, the wet-material response, and
the readability floor. LOW must look like the same game
(`01-graphics-and-rendering-architecture.md`).

## Thermal management

- Read the OS thermal state; respond at **Nominal → Fair → Serious**.
- **Target 80 % of frame budget**, not 100 %. A frame that exactly fits when cold
  does not fit at minute thirty.
- **Hysteresis on every transition** so quality does not oscillate visibly.
- Cap frame rate when the device is already comfortable — running at 90 fps to
  then throttle to 40 is worse than holding 60 throughout.

## Memory

| Budget | Target |
|---|---|
| Textures | Largest single line — atlas per zone, streamed, ASTC compressed |
| Meshes | LOD chains, shared across instances |
| Animation | **Often the surprise constraint** — budgeted per character tier, tracked in CI |
| Audio | Streamed music, pooled one-shots |
| Render targets | Explicitly budgeted; aliased where lifetimes permit |

Hard rules, from `.claude/rules/game-stack.md`:
- **Zero steady-state allocation in the frame loop.** Pools everywhere.
- **No synchronous asset loads during play.** Everything async, pre-warmed at zone
  entry.
- **Texture streaming by zone**, matching the chunk grid in
  `docs/design/12-technical-architecture.md`.

## Draw-call and bandwidth strategy

Mobile GPUs are tile-based deferred renderers. The consequences:

- **Bandwidth is the scarce resource, not raw arithmetic.** Prefer more maths over
  more texture fetches. Channel-pack aggressively.
- **Never read back a render target mid-frame** unless unavoidable — it breaks
  tiling and costs far more than it appears to.
- **GPU instancing** for everything repeated: railings, lamp posts, bollards,
  bins, windows, parked cars, crowd NPCs.
- **Static batching** per chunk for immovable geometry.
- **Atlas by zone** so a street is a handful of draw calls, not hundreds.
- **Aggressive culling**: frustum → occlusion (portals and occluder geometry work
  well in a dense street grid) → distance → screen size.

Kingston helps again: a dense town with short sightlines and buildings that
occlude each other is the best possible case for occlusion culling. Long open
sightlines are the enemy, and there are almost none.

## Measurement discipline

Per `.claude/rules/verification.md`:

1. **Frame time and variance, never an average.**
2. **Cold and after 30 minutes.** The second number is the real one.
3. **On the oldest supported device**, not the newest.
4. Instruments for CPU and memory; GPU capture for pass-level cost; ETTrace for
   launch and focused flows; MetricKit for field telemetry.
5. **CI performance gate**: a scripted flythrough of the prototype street,
   measured per build, failing on regression beyond a threshold.

The `game-performance-auditor` agent reviews per-frame paths for allocation,
string formatting, texture loads and unbounded scaling.

## Budget ownership

Every system in these documents has a declared millisecond budget
(`01-graphics-and-rendering-architecture.md`). **A new effect must name the system
it is taking milliseconds from.** There is no spare budget, by design.
