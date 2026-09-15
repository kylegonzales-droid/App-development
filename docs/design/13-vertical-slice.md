# Vertical slice plan

**Goal:** one complete, polished, genuinely fun five-minute experience that proves
the game is worth building — not a broad, shallow demo of sixteen half-systems.

## Sequencing principle: feel before scope

The order below is deliberate. **Movement feel is validated before any content is
built**, because if the core movement is not enjoyable on a phone, nothing built
on top of it will be, and every week spent on content before that is at risk.

## Milestones

### VS0 — Feel *(the go / no-go gate)*
**One street block. Nothing else.**

- Project scaffold, four packages, import-boundary check in CI
- Player sprite, high-angle camera with look-ahead and damped follow
- Floating-stick movement, run, vault
- One block of the Charter, art-directed to final quality — not placeholder
- Day and night lighting for that one block, with wet-ground reflections

**Exit criteria — all must pass:**
- 60 fps sustained on the oldest target device, measured with variance
- Someone who is not the developer plays it **one-handed, standing, on a bus**,
  and says walking around is enjoyable with nothing to do
- The block reads as *Britain, night, wet* in a screenshot with no UI

**If VS0 fails, stop and fix the feel. Do not proceed to VS1.**

### VS1 — World
- Chunk streaming with hysteresis; one atlas per zone
- Zones A (Charter) and C (Crossing) playable end to end
- Full day/night cycle; overcast, rain, fog
- Interiors: The Late Plate
- Minimap

### VS2 — Life
- NPC tiers: ambient crowd with flow fields, reactive individuals, persistent characters
- Day-part density tables for A and C
- District alarm model with all four bands
- Ambient barks; reactive exchanges
- Dami as the first persistent NPC, fully voiced

### VS3 — Drive
- Vehicle model: courier moped + hatchback + Quayside van
- Auto-accelerate, arc steering, handbrake, assist toggle
- Zone D (the Loop), one-way system, **wrong-way heat mechanic**
- Vehicle heat and the respray at Verrall's (Zone G stub)
- Core Haptics adopted here

### VS4 — Heat
- Police response tiers 0–3
- Evasion: line of sight, crowds, culverts, towpath, respray
- Car park chase space in Zone C fully built
- Basic melee: tap / swipe / hold

### VS5 — Story
- Mission runtime in `KingstonCore`, data-driven, fully unit-tested
- **One complete mission** with setup → objective → complication → decision →
  consequence → resolution
- Both factions present and visually distinct; Yvonne and Mikey appear
- Two independent standing scalars; reward and progression
- Save/load, versioned Codable
- Mission complete / failed screens

## The one mission — "Last Orders"

A single mission that exercises every system and states the game's thesis.

> Dami's shop has a delivery due at half eleven. Quayside's van is blocking the
> service bay again, and Col wants Remi to move it *and* pick something up while
> it's open. Five Star's Mikey is outside on the bike, and he has worked out what
> the pickup is. Both of them want Remi to do a favour that burns the other.

- **Setup** — delivered in transit, over the phone, no cutscene.
- **Objective** — get the van moved and the delivery in before half eleven.
- **Complications** — the yard is not empty; it starts raining; a patrol comes
  round the Loop; Mikey will not leave.
- **Decision** — tell Col, tell Mikey, tell both, or tell neither. No option is
  free, none is obviously right.
- **Consequence** — writes `WorldState`; both standings move independently; the
  corner outside The Late Plate is occupied differently afterwards, visibly.
- **Resolution** — you get paid. Dami asks who that was. You say a bloke about a van.

Every listed vertical-slice requirement is exercised: map, player, movement,
camera, NPCs, interaction, both factions, combat (avoidable), a vehicle, police,
minimap, mission marker, a complete mission, reward, progression.

## Explicitly out of scope for the slice

The phone app, Zones B/E/F/G/H beyond stubs, firearms, the full faction roster,
GameKit, purchases, multiple missions, interiors beyond The Late Plate, weather
beyond three states, the social feed, VO beyond Dami and two barks.

## Effort — honest

| Milestone | Engineering | Art & audio |
|---|---|---|
| VS0 | 2–3 weeks | 2–4 weeks *(one block at final quality)* |
| VS1 | 3–5 weeks | 6–10 weeks |
| VS2 | 3–4 weeks | 3–5 weeks |
| VS3 | 3–4 weeks | 2–4 weeks |
| VS4 | 2–3 weeks | 1–2 weeks |
| VS5 | 3–4 weeks | 2–3 weeks + VO |

**Roughly 4–6 months for one full-time engineer and one full-time artist**, with
the art on the critical path throughout — and that is for a *slice*, not a game.

**The dominant risk is art, not code.** A believable Kingston at premium quality
is thousands of authored assets. Claude can write the engine, the systems, the
tests and the tools; Claude cannot produce production game art. Securing an
artist — or radically narrowing the art scope — is the highest-priority decision
after the engine confirmation.
