# Vertical-slice graphics plan

Follows the directive's Phase A–R order, folded into the Phase 1 slice milestones
(`docs/design/13-vertical-slice.md`). **Each phase ends with a visual result on a
device, not a system that is "ready".**

## The sequencing rule

> Do not build huge systems without first proving the visual result.

So: **prototype the look before building the architecture behind it.** Every phase
below produces something you can hold in your hand and judge. Anything that does
not is re-planned.

## Phases

| # | Phase | Builds | Visual proof at the end |
|---|---|---|---|
| **A** | Core rendering | Frame graph, clustered forward, colour pipeline, tier system | One lit street, correct exposure, 60 fps |
| **B** | Kingston blockout | One street grey-boxed to correct scale, streaming stub | The street plays; proportions are right |
| **C** | Player character | Mesh, rig, locomotion blend space, inertialisation | Character moves and it feels good |
| **D** | Camera + movement | Third-person camera, look-ahead, collision, touch controls | **The feel gate** — see below |
| **G** | Lighting | Moods, probes, emissives, sky-occlusion bake | Same street, day → dusk → night |
| **J** | Wet materials | The four wetness operations, porosity, drying curve | Street soaks and dries convincingly |
| **I** | Rain | Layers 1–9, camera-locked volume | **Rain on a wet Kingston street at night** |
| **H** | Weather framework | State machine, transitions, fan-out to NPC and vehicle | Weather changes itself, and the town responds |
| **E** | NPC system | Three tiers, crowd instancing, weather behaviour | A street with people who react to rain |
| **M** | VFX framework | Pools, budgets, tiers, contracts | Splashes, steam, spray |
| **F** | Vehicle prototype | One vehicle, handling, spray, wipers, lights | Driving the wet Loop at night |
| **N** | Interaction | Doors, ripples, foliage, bins; accumulation buffers | The world reacts to being moved through |
| **O** | Mission system | Already designed and pure — `docs/design/06-mission-system.md` | One mission runs |
| **P** | UI / HUD | Per `docs/design/08-ui-system.md` | Readable in all grades |
| **Q** | Optimisation | Ladder, thermal response, CI perf gate | 30 min sustained on LOW device |
| **R** | Slice polish | Grade pass, audio, haptics, accessibility | Shippable slice |
| **K/L** | Snow, seasons | **Deferred past the slice** | — |

**Phases K and L are deliberately cut from the slice.** Kingston snow is rare and
brief; seasons multiply the art cost of every asset. The architecture reserves
their place (`02`, `04`); the implementation waits until the slice has proved the
game is worth building.

## The feel gate (end of Phase D)

Unchanged from `docs/design/13-vertical-slice.md`, and it still governs:

- 60 fps sustained on the oldest target device, measured with variance, after 30
  minutes.
- Input latency inside the table in `06-animation-architecture.md`.
- Someone who is not the developer plays it **one-handed, standing, on a bus**, and
  enjoys simply moving around with nothing to do.

**If Phase D fails, stop.** Everything after it is built on that foundation, and
no amount of rain and reflection rescues a character that feels bad to move.

## Graphics acceptance criteria for the slice

- Holds tier targets on all four device classes, cold and after 30 minutes.
- Reads correctly in **all six colour grades** and in dry / wet / foggy.
- LOW and ULTRA are recognisably the same street.
- Readability floor holds in every grade with accessibility settings on and off.
- Zero steady-state allocation in the frame loop, verified in Instruments.
- CI performance gate green: scripted flythrough, no regression beyond threshold.
