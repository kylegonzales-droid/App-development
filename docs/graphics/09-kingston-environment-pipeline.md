# Kingston environment pipeline

**The dominant cost of this project.** Not rendering — content. This document is
how a believable Kingston gets authored without an infinite budget.

## The rule

> Do not simply place generic American assets into Kingston.

This is the difference between a game set in Kingston and a game set in
Generic City with a Thames texture. It is enforced by the kit below and by the
review checklist at the end.

## Kit-of-parts

British streets are **highly repetitive with high small-scale variation** —
terraces of identical houses distinguished by doors, windows, bins, satellite
dishes, curtains and paint. That is the best possible case for a modular kit:
small authored set, enormous apparent variety.

### Architecture modules

| Module | Variants | Notes |
|---|---|---|
| Victorian terrace | 6 façades × 8 door/window swaps | The default residential fabric |
| Georgian townhouse | 4 | Church Street character |
| Post-war brick parade | 4 | Shops below, flats above |
| Deck-access estate block | 3 | Greenside |
| 1960s–80s commercial | 5 | The mall, offices |
| Neo-Tudor / mock historic | 3 | Market Place frontage |
| Railway arch | 3 | Back of House |
| Multi-storey car park deck | 2 | The Crossing |
| Riverside boatshed / warehouse | 3 | The Reach |

### Street furniture — where the Britishness lives

Individually trivial, collectively decisive. A street with correct furniture reads
as British even with mediocre buildings; the reverse is not true.

Lamp post (2 types — **one sodium, one LED**, per `03-lighting-architecture.md`) ·
wheelie bins (3 sizes, 4 colours) · litter bin · bollard (4 types) · pedestrian
guardrail · **Belisha beacon and zebra crossing** · **Pelican crossing pole with
tactile cone** · traffic light (UK sequence) · bus stop flag and shelter ·
**post box** · telephone cabinet · grit bin · road sign set (UK Transport
typeface shapes — original artwork) · **double yellow lines** · box junction ·
speed hump · drain cover and gully · manhole · utility cabinet · satellite dish ·
wall-mounted gas box · security shutter · A-board · hanging pub sign ·
scaffolding · roadworks barrier and cones · CCTV pole · parking meter ·
cycle stand · planter · phone mast.

### Signage

**All original artwork.** No real brand, logo, typeface licence or shopfront design
is reproduced. UK road signs follow the *shapes and colour conventions* of the
public standard (which is functional, and necessary for authenticity) with
original lettering.

Fictional business fascias per `docs/design/01-world-and-map.md` — The Late Plate,
Verrall's Motors, The Bentham Centre, Marsh Self-Store.

## Authoring pipeline

```
Reference       photo survey + the geography research already done
    ↓           (docs/research/kingston-geography.md)
Blockout        grey-box geometry, correct scale and sightlines, playable
    ↓
Playtest        ◄── gate: does it play well BEFORE it looks good?
    ↓
Kit pass        modular architecture placed
    ↓
Material pass   master materials + weather response authored
    ↓
Furniture pass  street furniture, the Britishness layer
    ↓
Lighting pass   moods, emissives, probes, sky occlusion bake
    ↓
Detail pass     decals, dirt, wear, damp, moss, litter
    ↓
Optimise        LOD chains, atlasing, instancing, occluders
    ↓
QA              visual matrix × weather × time × quality tier
```

**The blockout gate is the most important step and the one most often skipped.**
Geometry that plays badly is not rescued by materials. Playtest the grey box.

## Scale and proportion

Kingston's medieval core has **3–8 m building plot widths**
(`docs/research/kingston-geography.md`). That narrowness is the character of the
place and must survive into the game — the temptation to widen streets for camera
comfort will destroy exactly what makes it recognisable.

Resolve camera problems with camera solutions (`docs/design/09-visual-direction.md`)
— not by making Kingston look like a business park.

**Reference-check every module against real measurements:** UK door heights,
storey heights, kerb heights, lane widths, bollard spacing, road-marking
dimensions. Wrong proportions read as "foreign" even when nobody can say why.

## LOD policy

| Level | Distance | Content |
|---|---|---|
| LOD0 | < 15 m | Full detail, all decals, furniture |
| LOD1 | 15–40 m | Reduced mesh, merged small props |
| LOD2 | 40–100 m | Silhouette geometry, baked detail into texture |
| LOD3 | > 100 m | Impostor / billboard block |

Building interiors beyond the authored ones use **parallax-mapped interior
shaders** in windows — very cheap, and it removes the dead-eyed empty-window look
that instantly reads as a game.

## Streaming

Chunk grid per `docs/design/12-technical-architecture.md`, one texture atlas per
zone, one-chunk hysteresis so standing on a boundary cannot thrash. Async loading
only; nothing synchronous during play.

## What Claude can and cannot do here

Stated plainly, because it determines the plan:

**Claude can** build the streaming system, the LOD pipeline, the material system,
the procedural placement tools, the kit-assembly tooling, the validation scripts,
the performance gates, the blockout geometry generators, and every line of the
runtime.

**Claude cannot** produce production 3D art: modelled and UV'd architecture,
authored PBR textures, rigged and animated characters, or original signage
artwork.

**Consequence:** the environment pipeline is where the project's schedule is
actually decided. The options remain those in `docs/design/15-open-decisions.md` —
hire an artist, narrow the scope hard, licence a coherent base kit and art-direct
on top, or accept a long timeline. **A licensed modular British/European street
kit, art-directed and re-materialled to our colour language, is the most realistic
path for a small team** and is worth serious evaluation before any bespoke art is
commissioned.

## Review checklist — every street, before acceptance

From `docs/design/09-visual-direction.md`, extended:

- [ ] Could this street be anywhere? If yes, it fails.
- [ ] At least one detail only a British person would notice.
- [ ] Correct UK road markings, sign shapes, crossing types, kerb heights.
- [ ] Both lamp types present — sodium and LED.
- [ ] Bins, and they are the right bins.
- [ ] Lighting comes from named sources in the scene.
- [ ] Reads correctly in all six colour grades.
- [ ] Reads correctly dry, wet and foggy.
- [ ] Silhouette holds at LOD2.
- [ ] Inside draw-call and bandwidth budget.
- [ ] No real brand, logo, or copyrighted design anywhere in frame.
