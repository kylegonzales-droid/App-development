# Visual direction

> **Style and camera sections are contingent on the engine decision.**
> This document specifies *hand-authored 2D* per the Phase 1 recommendation. The
> graphics directive later specified realistic 3D. See
> `../graphics/00-engine-decision.md`.
>
> **What survives either decision:** the colour script, the six Kingston grades,
> the sodium-versus-LED lighting motif, the lighting-as-storytelling stance, the
> wet-tarmac signature, the animation principles and the anti-generic checklist.
> Those are carried forward and extended in `../graphics/03-lighting-architecture.md`
> and `../graphics/09-kingston-environment-pipeline.md`.

## The one-line brief

**A British market town at night, seen from above, after rain.**

If a screenshot does not immediately read as *Britain*, *night*, and *wet*, it is
off-brief. Wet tarmac reflecting shop signage is the signature image of this game.

## Style

**Hand-authored, high-contrast, graphic 2D** — not pixel art, not vector-flat,
not an attempt at photorealism. Think illustrated rather than rendered: confident
shapes, limited palettes, light doing the storytelling.

Why this is the right call for this project:
- It is achievable at quality by a small team, which photorealism is not.
- It ages well. Stylised games from a decade ago still look intentional.
- It performs. A 2D scene at 60 fps on a five-year-old iPhone is realistic.
- It is distinctive. The failure mode to avoid is the generic asset-pack look,
  and the defence against it is a *specific, committed* art direction.

## The camera

**High-angle top-down, roughly a 55–65° pitch feel**, achieved with authored 2D
art rather than a 3D camera. The player sees the character from above and behind
— enough to read the street ahead, enough to see the character's silhouette.

*(This is the "third-person" of this game. See `12-technical-architecture.md` —
it is the decision that everything else rests on.)*

| Behaviour | Spec |
|---|---|
| **Follow** | Smooth damped follow, ~120 ms lag. Never rigid. |
| **Look-ahead** | Offsets in the direction of travel, scaled by speed — up to ~25 % of screen height at vehicle speed. Shows you where you're going, not where you've been. |
| **Zoom** | Pulls out with speed. Tight on foot in alleys, wide in a car on the Loop. |
| **Peek** | Drag anywhere on the right to look; springs back on release. |
| **Cinematic** | Story beats take control: slow push-in, held wide shots, a locked frame on a doorway. Used sparingly so it lands. |
| **Reduce Motion** | No shake, no punch, no roll. Cuts replace moves. |

**Interiors** — selected buildings only (The Late Plate, Verrall's, the lock-up,
the storage yard office). Entered seamlessly with a camera pull-in and a lighting
change, not a loading screen.

## Colour direction

A **colour script by zone × day-part**, authored as palette pairs. This is the
single highest-leverage art decision — it is what makes the world feel authored
rather than assembled.

### Day-part palettes

| Day-part | Key light | Shadow | Accent | Feel |
|---|---|---|---|---|
| **Early** | Pale grey-blue `#B9C4CE` | Cool slate `#3E4852` | Delivery orange | Empty, cold, honest |
| **Commute** | Flat white overcast `#D7DCE0` | Neutral grey `#5A6067` | Bus red, hi-vis | Crowded, low contrast |
| **Trade** | Soft daylight `#E4E2DA` | Warm grey `#6B675E` | Market canvas, brick | Busy, ordinary |
| **Turnover** | Low gold `#E8B472` | Long blue `#3A4A63` | Brake red | Warm, transitional |
| **Night** | Sodium amber `#F2A03D` + LED cyan `#BFE6F0` | Deep blue-black `#101822` | Police blue, neon | **The signature look** |
| **Dead** | Sparse cyan `#9FC6D4` | Near-black `#080C12` | Single lit window | Isolated, tense |

**The night palette is the game's identity.** The tension between the old sodium
lamps (warm amber, disappearing from real Britain) and new LED street lighting
(cold cyan-white) is both visually gorgeous and thematically exact — old Kingston
versus new, Quayside versus Five Star, in the street lighting.

### Zone tints

Charter — warm stone and brick. Reach — green-grey, moss, water.
Crossing — concrete, sodium, oil. Loop — tarmac, white lines, red lights.
Greenside — brick, painted render, washing. Arrivals — steel, glass, tile.
Back of House — rust, corrugation, hand-painted signs. Hogsmill — chlorophyll,
mud, unlit dark.

### Faction colour

**Quayside** — bottle green, oxidised copper, brass, oxblood. Nothing new.
**Five Star** — hi-vis yellow-green, matte black, reflective silver, screen-glow blue.
**Remi** — deliberately neutral: black, charcoal, one warm orange strap.

Faction identity is **never colour alone** — it is colour + mark (swan / rating
star) + silhouette (donkey jacket and van / thermal bag and helmet).

## Lighting

Light is the primary storytelling tool and the main source of production value.

- **Sources, not ambience.** Street lamps, shopfronts, headlights, phone screens,
  bus interiors, chip-shop fluorescents, a single lit flat window.
- **Wet ground.** After rain, every source gets a vertical reflection — cheap in
  2D (mirrored, blurred, tinted, scrolled with a noise mask) and transformative.
- **Volumetrics** sparingly: sodium haze, river fog, breath in cold, extractor
  steam behind the chicken shop.
- **Light as navigation.** Lit routes feel safe and are watched. Dark routes are
  safe from watching and unsafe otherwise. This is a mechanic.

## Animation direction

**Grounded, weighted, unheroic.** Remi moves like a real tired 24-year-old, not
an action figure.

- Idles are character: hands in pockets, checking the phone, shifting weight,
  looking down the street. Never a T-pose loop.
- Transitions matter more than poses — the turn, the stumble, the vault without
  ceremony.
- NPC animation is **cheap and varied**: a small number of walk cycles combined
  with varied speed, gait offset, and prop (bag, buggy, dog, phone).
- **Secondary motion sells everything**: jacket, bag strap, hair, aerials,
  hanging signs, litter, puddle ripples. Cheap in 2D, enormous return.
- Every animation has a **Reduce Motion** variant or is exempt because it is
  gameplay-essential.

## Weather

| State | Visual | Mechanical |
|---|---|---|
| **Clear** | Rare, bright, long shadows | Best sightlines both ways |
| **Overcast** | Default. Flat light, low contrast | Neutral |
| **Drizzle** | Faint specular, damp ground | Slightly fewer NPCs |
| **Rain** | Full reflections, streaks, run-off, umbrellas | Grip down, NPC density down, police sightlines down |
| **River fog** | Heavy falloff from the Thames | Sightlines cut both ways; the best cover in the game |

Fog is both random and scripted for key story beats.

## Anti-generic checklist

Before any environment art is accepted:

- [ ] Could this street be anywhere? If yes, it fails.
- [ ] Is there at least one thing only a British person would notice? (Wheelie
      bin placement, a Pelican crossing, a yellow grit bin, a satellite dish
      cluster, a bay window, a bus stop flag, roadworks that have been there
      for months.)
- [ ] Does the lighting come from named sources in the scene?
- [ ] Does the silhouette read at 400 pt wide?
- [ ] Is the palette drawn from the zone × day-part script, or invented ad hoc?
- [ ] Is there any real brand, logo, or copyrighted design in frame? (Must be no.)
