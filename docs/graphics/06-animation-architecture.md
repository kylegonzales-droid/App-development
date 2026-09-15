# Animation architecture

## Governing principle

The directive is right: **animation quality over polygon count**, and *the player
must never feel like a rigid model moving between canned animations*.

For a third-person game, **responsiveness beats fidelity**. A beautifully animated
character who takes 200 ms to respond to the thumb feels worse than a simpler one
that responds in 60 ms. Where the two conflict, responsiveness wins — and on a
touchscreen, where there is no controller tactility to compensate, it wins by
more than it does on console.

## Layered system

```
Layer 0   Locomotion        full body — the state machine below
Layer 1   Additive lean     procedural, from velocity and turn rate
Layer 2   Upper body        carrying, phone, aiming — masked to spine+
Layer 3   Look-at           procedural head/eye tracking, IK
Layer 4   Foot IK           ground adaptation, slope, stairs
Layer 5   Hand IK           doors, handrails, vehicle wheel, contact points
Layer 6   Reactions         additive hit, flinch, stumble
Layer 7   Weather additive  shiver, hunch, hood-up, umbrella
```

**Layer 7 is cheap and enormously valuable.** A character who hunches and pulls
their hood up in rain does more for world believability than a large increase in
mesh density, and it costs an additive pose plus a blend weight.

## Locomotion

```
        ┌──────────────────────────────────────┐
        ▼                                      │
    ┌───────┐  ┌──────┐  ┌─────┐  ┌──────┐  ┌──────────┐
    │ Idle  │─►│ Walk │─►│ Jog │─►│ Run  │─►│ Sprint   │
    └───┬───┘  └──┬───┘  └──┬──┘  └──┬───┘  └────┬─────┘
        │         │         │        │           │
        └─────────┴────┬────┴────────┴───────────┘
                       ▼
        ┌──────────────────────────────────┐
        │ Start · Stop · TurnInPlace ·     │
        │ PlantAndTurn · Vault · Slide ·   │
        │ VehicleEnter · VehicleExit       │
        └──────────────────────────────────┘
```

Driven by a **2D blend space** (speed × turn rate), with transition animations for
starts, stops and sharp turns. Stops and plant-turns are where "responsive" is won
or lost — they are the animations worth the most authoring effort.

## Motion matching — principles, scoped honestly

Full motion matching needs a large captured database and a per-frame nearest-pose
search. On a phone, the memory cost of the database is usually the blocker before
the search cost is.

**Adopt the principles, not the implementation:**

| Principle | How we apply it |
|---|---|
| Match the *trajectory*, not just the state | Blend space driven by predicted future trajectory, not only current velocity |
| Pose continuity across transitions | Inertialisation blending — always, everywhere |
| No hard state snapping | Every transition inertialised; no instant pose swaps |
| Foot phase matters | Phase-matched blending so feet do not skate through transitions |

**Inertialisation is the single highest-value technique here.** It blends from the
*current pose* (including velocity) into the target animation over 100–250 ms,
costs almost nothing, needs no database, and removes the large majority of the
"canned animation" feel. If only one item from this document is implemented, it
should be this one.

Full motion matching is **deferred past the slice** and re-evaluated with real
memory numbers.

## IK

| IK | Purpose | Tier |
|---|---|---|
| **Foot IK** | Ground adaptation, kerbs, stairs, slopes — Kingston is full of kerbs | MEDIUM+ |
| **Look-at** | Head and eyes track points of interest; huge life-per-cost | LOW |
| **Hand IK** | Doors, railings, steering wheel, carried objects | HIGH |
| Full-body IK | Not planned | — |

Two-bone IK with a pole vector; solved on CPU, cheap. **Foot IK plus look-at is
most of the perceived quality** for a fraction of the cost of the rest.

## Responsiveness budget — measured, not felt

| Input | Max latency to visible response |
|---|---|
| Movement start | **80 ms** |
| Direction change | **100 ms** |
| Stop | 150 ms (may settle longer) |
| Context action | **100 ms** |
| Melee strike | **80 ms** |
| Dodge | **60 ms** |

**These are tested, not eyeballed** — high-speed capture, or frame-stepped
recording, counting frames from touch to first visible motion. A regression here
is a bug of the same severity as a crash, because it degrades every second of play.

Technique: start the response *immediately* on the additive and procedural layers
while the base locomotion blend catches up. The character leans into the turn on
frame two even though the full turn animation takes 300 ms.

## NPC animation — cheap and varied

Per `docs/design/04-npc-system.md`, NPCs come in three tiers:

| Tier | Animation |
|---|---|
| **Ambient** (80–150) | Shared walk cycles, GPU-instanced, varied by playback offset, speed and prop. No IK. Crowd-rate update. |
| **Reactive** (15–30) | Full locomotion blend, look-at IK, foot IK at MEDIUM+ |
| **Persistent** (0–6) | Everything, including hand IK and authored performance |

**Variation without cost:** a small number of cycles × random phase offset ×
speed variation × prop (bag, buggy, umbrella, dog, phone) reads as a genuinely
varied crowd. Phase offset alone removes the marching-in-step tell.

## Animation compression and memory

Curve fitting with per-track error tolerances; tight tolerance on hands and face,
loose on distant NPC tracks. **Animation memory is usually the real constraint on
mobile, before CPU** — budget it explicitly per character tier and track it in CI.

## Weather and vehicle integration

Weather drives layer 7 (hunch, shiver, hood, umbrella) and surface response (slip
on ice, careful footing on snow). Vehicle entry and exit are full authored
transitions with hand IK to the door and wheel — the single most commonly botched
animation in this genre, and one where getting it right signals quality
immediately.

## Implementation

**Package A (Unity URP)** — Animator layers with avatar masks; Animation Rigging
for IK; custom inertialisation on top of `Playables`; GPU instancing for ambient
crowd.

**Package B (Metal)** — custom skeletal system: pose buffers, blend tree
evaluation in `KingstonAnim`, GPU skinning in compute, instanced crowd skinning.

## Verification

1. Unit-test blend weights, state transitions and inertialisation maths in
   `KingstonAnim` — pure functions, no renderer.
2. **Measure latency for every input in the table.** Frame-stepped capture. This
   is a gate, not a nice-to-have.
3. Foot-skate check: capture foot contact points against ground movement.
4. Crowd review: 100 ambient NPCs — confirm no visible marching in step.
5. Animation memory tracked per build in CI, with a budget that fails the build.
