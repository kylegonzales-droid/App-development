# Combat direction

## Stance

Combat exists, matters, and is **not** the point. The player fantasy is knowing
the town, not being the hardest person in it. Remi is a courier, not a soldier.

**Design constraints, binding:**
- **Stylised, not realistic.** Readable at a high-angle camera on a phone.
- **No gore.** Impact is communicated by motion, sound, camera and light — not
  by blood or injury detail.
- **Escape is always viable.** Every encounter has a route out. If a fight cannot
  be run from, it is a design bug.
- **Firearms are rare and permanent.** Not a weapon tier — a story threshold.
  Drawing one changes how the town treats you for the rest of the game.

## Melee

Three inputs, contextual, no combo strings to memorise:

| Input | Action |
|---|---|
| **Tap** | Strike. Auto-targets the nearest threat in a forward arc. |
| **Swipe** | Dodge / reposition in the swipe direction. Brief invulnerability. |
| **Hold** | Guard. Reduces damage, drains stamina, cannot move fast. |

Depth comes from **positioning and the environment**, not input complexity:
numbers, chokepoints, what's behind you, whether you can reach the alley. A
market stall between you and two people is a real tactic.

Stamina gates aggression. Out of stamina you can still move and dodge — never
locked into being hit.

## Environmental interaction

Shove someone into a bin store. Put a stall between you. Pull a shutter down.
Kick a bike into a path. Cheap to build, hugely characterful, and it rewards
knowing the space — which is the whole game.

## Firearms

Present in the world, held by few. The escalation rule:

```
flag.firearmDrawn          → police response tier permanently raised
flag.firearmDischarged     → both factions change posture; some characters
                             will no longer meet you; Dami's shop becomes a
                             target; certain endings close
```

This is a **narrative system implemented as a flag**, not a weapon-balance
problem. It is the single strongest lever for "nobody is clean."

## Police response

Heat is a per-district scalar, separate from NPC alarm, that escalates in tiers:

| Tier | Response |
|---|---|
| 0 | None |
| 1 | PCSO notices; a car is dispatched to *look* |
| 2 | Single response car, follows the Loop correctly |
| 3 | Two units, coordinating, one cuts across the ring |
| 4 | Containment — the bridge and the Loop's exits get attention |

**Evasion is geography.** Break line of sight, use the culverts and the towpath
where cars cannot follow, respray at Back of House, go quiet in a crowd at
Arrivals. Heat decays with time out of sight; it does not decay while visible.

The police drive the one-way system *correctly*, which is the joke and the
mechanic: going the wrong way is faster, raises heat, and is exactly what a
Kingston local would know to do.

## Deferred

Weapon variety and progression, cover systems, executions, injury states,
gunfights as routine content. The design above is deliberately shallow in combat
and deep in escape, and should stay that way unless play testing says otherwise.
