# First visual prototype — "One Wet Street"

**The single deliverable that answers the question this whole directive is
really asking:** can this project produce something that makes a person look at a
phone and say *"this is a premium UK mobile game set in Kingston"*?

Build this **before** committing to the full production plan. It is the cheapest
possible test of the most expensive assumption.

## What it is

**One street corner in Kingston. Night. Raining. Playable.**

Roughly **80 metres of street**, fully art-directed, with a character you can walk
around it, one NPC, and rain. Nothing else. No missions, no combat, no vehicles,
no map, no menus.

## Why this street

The **corner outside The Late Plate**, where the Crossing exits onto the Loop
(`docs/design/01-world-and-map.md`). It is the single most narratively important
location in the game, so the art is not throwaway — it is the first real asset.

It also contains, naturally, every environmental element worth testing:

- A **pedestrianised edge** and a **road** — two surface types, two lighting
  conditions
- A **railway arch** — sheltered ground that stays dry, proving exposure-driven
  wetness
- A **shopfront** (The Late Plate) — emissive light, the warmest thing in frame
- **Both lamp types** — sodium and LED, the game's signature colour tension
- A **kerb, a gully and a camber** — proving drainage-driven drying
- **Brick and tarmac** — the two highest-porosity materials, where wetness reads hardest

## Required contents

| Element | Spec |
|---|---|
| **Kingston street** | 80 m corner, kit-of-parts architecture, full street furniture, UK road markings, both lamp types, fictional signage |
| **Player** | Remi, rigged, locomotion blend space, inertialisation, foot IK, look-at, weather hunch layer |
| **Rain** | Layers 1–9 of `04-weather-architecture.md` |
| **Wet road** | Full four-operation wetness, porosity per material, puddles under the arch and in the gully, SSR at HIGH |
| **Lighting** | NIGHT KINGSTON mood; sodium + LED + shopfront emissive + one passing headlight |
| **Fog / atmosphere** | Froxel fog with light scattering; sodium haloing in the mist |
| **NPC** | One Reactive-tier pedestrian: walks, look-at, umbrella up, hurries, shelters under the arch |
| **Interaction** | The Late Plate's door opens; puddles ripple underfoot; a wheelie bin can be knocked; foliage bends |
| **Camera** | Third-person, look-ahead, collision, touch control |

## Explicitly excluded

Missions, combat, vehicles, the map, menus, save, more than one NPC, more than one
street, seasons, snow, day cycle beyond night, GameKit, audio beyond a basic
ambience bed.

## Success criteria

**Visual** — a still frame at night in the rain is genuinely striking, reads
unmistakably as a British town, and does not read as an asset-pack game.

**Feel** — one-handed, standing on a bus, walking around the corner is enjoyable
with nothing to do. Input latency inside the table in `06-animation-architecture.md`.

**Technical** — 60 fps on MEDIUM and HIGH, 30 locked on LOW, **sustained 30
minutes**, with variance reported. Zero steady-state allocation.

**Systemic** — stop the rain and the street **dries unevenly**: the arch stays wet,
the gully stays wet, the crown of the road dries first. If it fades uniformly, the
architecture in `02-material-architecture.md` is not working and the prototype has
failed its most important test.

**The one-sentence test** — someone unfamiliar with the project watches thirty
seconds of it and says, unprompted, that it looks like a real British game.

## Build order

```
1  Blockout the corner, grey, correct scale       ◄── playtest before art
2  Camera + character + touch controls            ◄── FEEL GATE. Stop if bad.
3  Base materials, dry, night lighting
4  Wetness system + drying curve                  ◄── unit-tested in KingstonCore
5  Rain layers 1–9
6  Fog + light scattering
7  Puddles + SSR
8  One NPC + umbrella + shelter behaviour
9  Interactions: door, ripples, bin, foliage
10 Quality tiers + thermal response
11 Grade pass + polish
```

Steps 1–2 are the gate. **Do not art-pass a street that does not play well.**

## Estimated effort

| | Engineering | Art |
|---|---|---|
| Package A (Unity URP) | 4–7 weeks | 4–8 weeks |
| Package B (Metal) | **6–9 months** (renderer first) | 4–8 weeks |

The gap between those two rows is the engine decision made concrete. It is the
clearest argument in these documents for resolving `00-engine-decision.md` before
anything else.

## What this prototype buys

- **The engine decision, validated by evidence** rather than argument.
- **A real art cost per metre of street**, which converts the schedule from guess
  to estimate — the single most valuable number this project can obtain.
- **The feel gate passed or failed early**, cheaply.
- **A vertical proof that all the systems chain**: weather → material → lighting →
  VFX → NPC → interaction, which is the thesis of the whole architecture.
- Something to show an artist, a collaborator, or an investor.
