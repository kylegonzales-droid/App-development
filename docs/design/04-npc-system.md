# NPC system

**Principle:** do not attempt simulated human intelligence. Build three cheap,
distinct tiers and spend the budget where the player is looking.

## Three tiers

| Tier | Count on screen | Cost/frame | Has | Example |
|---|---|---|---|---|
| **Ambient** | 80–150 | ~nothing | Position, walk cycle, flow-field steering, flee flag | Shoppers, commuters, crowd |
| **Reactive** | 15–30 | Small | State machine, senses, can be talked to, schedule | Shop worker, PCSO, rider, drinker |
| **Persistent** | 0–6 | Full | Name, memory, relationship, save-tracked state | Yvonne, Mikey, Bev, Dami |

**Ambient NPCs are a crowd system, not characters.** They follow flow fields per
street, batch into one draw call per atlas, tick on a shared timer (not every
frame), and have exactly one behaviour switch: *flee or don't*. They never talk,
never fight, never remember. Promotion to Reactive happens when the player looks
at one long enough to matter — swap the instance, keep the position.

**Persistent NPCs** are the only ones whose state is written to the save.

## Archetypes

Data, not subclasses. Each archetype is a record: sprite set, walk speed, day-part
density per zone, alarm threshold, dialogue pool, reaction table.

**Civilians:** commuter, student, shopper, parent-with-buggy, older resident,
tourist, jogger, dog walker, drinker, night-bus rider, street cleaner.
**Workers:** shop worker, market trader, barista, chicken-shop staff, mechanic,
delivery rider (neutral), taxi driver, bus driver, security guard, doorman.
**Authority:** PCSO (presence only), response officer, patrol car pair, traffic
warden, station staff.
**Faction:** Quayside yard hand, Quayside driver, Five Star rider, Five Star
driver, plus named lieutenants.

British specificity matters more than variety: the buggy, the trolley, the
Tuesday lunch club, the man who is clearly waiting for someone who is late.

## Schedules

Coarse and cheap. Each archetype has a **day-part density table** per zone —
"how many of me exist in Zone F between 08:00 and 10:00" — plus a small set of
anchor points (a stall, a doorway, a bus stop). NPCs are not persistently
simulated off-screen; they are **spawned to match the expected population** when
the player arrives and despawned behind them.

The illusion is in the *distribution*, not in individual continuity. A commuter
tide at Arrivals at 08:30 and six people at 01:00 sells the world far better than
tracking one person's whole day.

Persistent NPCs are the exception: they have real locations and the player can go
and find them.

## Reaction model

Per-NPC omniscience is expensive and reads badly. Instead:

**Per-district `alarm` scalar, 0…1.** Player actions add to it; it decays. NPCs
read the district alarm plus their own line of sight.

```
0.0–0.2  Normal      Ambient behaviour. Player is furniture.
0.2–0.4  Noticed     Heads turn. Phones come out. Some cross the road.
0.4–0.7  Alarmed     Ambient NPCs flee along flow fields. Shutters come down.
0.7–1.0  Emergency   Everyone gone. 999 calls placed. Police response spawns.
```

This gives the brief's required behaviours cheaply:

| Player does | Result |
|---|---|
| Walks past | Nothing. Alarm unchanged. |
| Starts a fight | Local alarm spike; witnesses in LOS react individually; crowd flees outward |
| Causes chaos | District alarm → Emergency; civilians clear; police spawn on the Loop |
| Enters restricted area | Security (Reactive) challenges; alarm rises only if ignored |
| Completes a mission | `WorldState` flag → changed spawn tables, dialogue, shutters, corner occupancy |

Witnessing is **line of sight + distance**, evaluated only for Reactive NPCs, on
a staggered tick — never all of them in one frame.

## Conversation

Not a dialogue tree for every NPC. Three levels:
- **Ambient barks** — one-line, pooled per archetype and day-part, no interaction.
- **Reactive exchanges** — short, 2–4 line, context-selected from a pool, can
  yield information or a micro-job.
- **Persistent scenes** — authored, branching, consequence-bearing.

Barks carry most of the cultural texture and cost almost nothing. This is where
the British-ness lives: the weather, the buses, the football, the roadworks, the
price of everything.

## World state

A flat key-value store (`WorldState`) of flags and counters, owned by
`KingstonCore`, written to the save. Missions read and write it; spawn tables,
dialogue pools, shop shutters and corner occupancy all query it. This is the
mechanism by which "the world changes after a mission" without bespoke code per
mission.

## Explicitly deferred

Full daily routines per individual, persistent off-screen simulation, NPC-to-NPC
relationships, economy simulation, NPC memory of the player beyond Persistent
tier. The tiers above are designed so these can be added later without rework.
