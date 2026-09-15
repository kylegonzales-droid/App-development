# World-interaction architecture

## Governing principle

**Prioritise the interactions that produce the greatest sense of a living world
per unit of cost.** Not every prop needs physics. A door that opens properly is
worth more than fifty barrels that can be knocked over.

## The interaction tiers

| Tier | What | Cost | Examples |
|---|---|---|---|
| **Responsive** | Reacts to proximity or passage, no state | Near-free | Foliage bend, litter scatter, puddle ripple, pigeon flush |
| **Stateful** | Has state, remembered, saved | Low | Doors, shutters, lights, bins, gates |
| **Physical** | Simulated rigid body | Medium | Knocked bins, loose crates, thrown objects |
| **Systemic** | Writes to world state, affects gameplay | Varies | Vehicles, NPCs, breakables that matter |

Most of the perceived life comes from **Responsive** — the cheapest tier. That is
the lesson to take: spend on breadth of cheap reaction, not depth of expensive
simulation.

## Deformation and accumulation buffers

The technique behind footprints in snow, tyre tracks, puddle ripples and grass
bending is the same one, and it is worth building **once** as shared
infrastructure:

```
A camera-local render target, top-down, covering ~40 m around the player.
Things that deform the world write into it.
Materials sample it and respond.
It scrolls with the player; edges fade out.
```

| Buffer | Channels | Written by | Read by |
|---|---|---|---|
| **Deformation** | height offset | Feet, wheels, falling objects | Snow, mud, sand materials |
| **Ripple** | normal xy + age | Feet, wheels, rain impacts | Puddle and water materials |
| **Flatten** | bend direction + strength | Anything moving through | Grass, foliage |

One system, three uses, three systems' worth of apparent sophistication.
**Persistence is bounded by the buffer**, which is honest: tracks fade as you walk
away. That is a well-understood trade and players do not notice it.

**Cost:** one small render target (typically 256–512²) plus a handful of writes
per frame. Cheap. It is on the ULTRA/HIGH path first, MEDIUM for ripples only.

## Stateful world objects

```
InteractableState {
  id, kind
  state        closed | open | locked | broken | on | off
  lastChanged
  persists     bool   → written to save
}
```

Owned by `KingstonCore` — so door state, shutter state and light state are all
pure data, saved with the game, and testable without a renderer. The world object
in the scene is a *view* of that state.

This connects directly to the mission system: a mission consequence that closes a
shutter (`docs/design/06-mission-system.md`) writes the same state that the player
can see and the renderer draws.

## Priority list for the slice

Ordered by believability-per-cost. Build down this list, stop when the budget runs
out.

1. **Doors and shop entrances** — the baseline. A world where doors do not work is
   a diorama.
2. **Puddle ripples from movement** — directly ties the player to the weather.
3. **Foliage bend on passage** — near-free, instantly noticed.
4. **Bins** — knockable, and very British. High character per unit cost.
5. **Shutters** — narrative state, gameplay use, visible world change.
6. **Street lights** — on/off with time of day, breakable for darkness.
7. **Litter and leaves** reacting to wind and passage.
8. **Vehicle doors, boots, wipers, headlights.**
9. **Benches, railings, bollards** — collision and vault targets.
10. **Breakable glass** — shop fronts, car windows.

Deferred: full prop physics everywhere, destructible architecture, fluid
simulation.

## NPC reaction to the world

Already specified in `docs/design/04-npc-system.md` — the district alarm model.
Weather extends it: NPCs shelter, hurry, bunch in doorways, deploy umbrellas, and
thin out in heavy rain. These are behaviour-table entries keyed on
`WeatherState`, not new systems.

## Implementation

**Package A (Unity URP)** — accumulation buffers as `RenderTexture`s driven by a
custom Renderer Feature; interactables as components reading `KingstonCore` state.

**Package B (Metal)** — explicit render targets in the frame graph; write passes
before the opaque pass; materials sample in the shared surface function.

## Verification

1. Unit-test `InteractableState` transitions and save round-trip. No renderer.
2. Confirm buffer scroll has no seam or popping at the edges when the player moves
   fast in a vehicle — this is the classic failure of camera-local buffers.
3. Confirm tracks persist for the buffer's spatial extent and fade cleanly beyond.
4. Verify door/shutter state survives save, reload and zone transition.
