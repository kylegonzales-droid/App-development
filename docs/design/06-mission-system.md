# Mission system

**Requirement:** support dozens of missions without hard-coding each one.

**Answer:** missions are **data** — a graph of beats interpreted by a runtime that
lives in `KingstonCore` and is fully unit-testable without a scene, a view, or a
device.

## Structure

```
Mission
 └── Beat[]                      a node in the graph
      ├── entry conditions       WorldState predicates
      ├── objective              one of a small composable vocabulary
      ├── complications[]        events that may fire while the beat runs
      ├── outcomes[]             success / failure / partial / player choice
      └── transitions            outcome → next beat (or end)
```

A mission is a small file. The runtime does not know what "the warehouse job" is —
it knows how to run a `Deliver` objective with a `PoliceSweep` complication.

## Objective vocabulary

Deliberately small. New missions come from *recombination*, not new code.

| Objective | Parameters |
|---|---|
| `GoTo` | location, radius, time limit? |
| `Deliver` | item, from, to, condition (intact / unseen / before time) |
| `Collect` | item(s), count, locations |
| `Talk` | character, dialogue node |
| `Follow` | target, min/max distance, without being seen? |
| `Evade` | heat threshold, duration or safehouse |
| `Protect` | target, duration, damage threshold |
| `Survive` | duration, wave table |
| `Race` | checkpoint route, time |
| `Avoid` | zone(s) or NPC type, for the beat's duration |
| `Wait` | until time-of-day or WorldState flag |

Eleven verbs cover every mission category in the brief. A "chase mission" is
`Follow` + `Evade`. An "investigation" is `GoTo` + `Talk` + `Collect`. A
"negotiation" is `Talk` with branching outcomes and a `Protect` fallback.

## Complications — the anti-"go here, shoot everyone" rule

Every beat may declare complications. These are what stop a mission being a
waypoint queue. A complication fires on a trigger (timer, proximity, WorldState,
random weighted) and **changes the problem mid-beat**:

- A police patrol enters the route.
- The rival faction is already there.
- The contact does not show; someone else does.
- A shutter that was open is closed — reroute.
- It starts raining; grip and sightlines change.
- A phone call arrives that changes the objective's meaning.
- A civilian sees you and is deciding whether to call it in.

**Authoring rule:** a beat with no complication and no choice is a travel
sequence, and a mission may contain at most one.

## Structure of a well-formed mission

Enforced by an authoring checklist, not by code:

1. **Setup** — why now, why you, what's at stake. Delivered in transit, not in a
   cutscene where possible.
2. **Objective** — clear, and expressible in one line on the HUD.
3. **Complication** — at least one, ideally emergent rather than scripted.
4. **Decision** — at least one real choice with no obviously correct answer.
5. **Consequence** — writes to `WorldState`; visible in the world afterwards.
6. **Resolution** — pay, standing change, and a line that lands.

## Choice and consequence

Choices write to `WorldState` (flat key-value, saved). Consequences are read by:
spawn tables, dialogue pools, shop shutters, corner occupancy, price modifiers,
which characters will meet you, and which missions unlock.

**Faction standing is two independent scalars**, `standing.quayside` and
`standing.fiveStar`, each −100…+100 — not one slider. Missions can raise both,
lower both, or trade. Holding both high is the most profitable and least stable
state in the game, and the story is built to squeeze it.

Gates are expressed as predicates: `standing.quayside >= 40 && !flag.toyinTruthKnown`.

## Mission categories at ship

Story (critical path) · Faction (either side, mutually exclusive branches) ·
Character (Mikey, Bev, Denise, Dami — relationship arcs) · Delivery · Chase ·
Escape · Investigation · Protection · Collection · Route challenge (timed, the
"street racing" analogue, using real Kingston geography) · Stealth ·
Negotiation · Opportunistic street events.

## Why this lives in `KingstonCore`

The mission runtime imports nothing from SpriteKit or SwiftUI. That means a whole
mission graph can be executed in a unit test — every branch, every predicate,
every consequence — in milliseconds, with no simulator. Per
`.claude/rules/verification.md`, this is where the tests carry their weight.
