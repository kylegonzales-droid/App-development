# Weather architecture

**Weather is world state, not a visual effect.** It lives in `KingstonCore` as
pure data, evolves by pure functions, and is consumed by rendering, audio, NPC
behaviour, vehicle handling and gameplay alike. Everything below is unit-testable
without a GPU.

## State model

```
WeatherState {
  season          spring | summer | autumn | winter
  condition       clear | cloudy | overcast | lightRain | heavyRain
                  | storm | fog | lightSnow | heavySnow
  transition      from, to, progress 0…1
  windVector      direction + speed
  temperature     °C
  precipitation   0…1 intensity
  cloudCover      0…1
  visibility      metres
}
```

Everything downstream derives from these. There is no separate "rain effect
enabled" flag anywhere in the codebase — if rain is falling, it is because
`precipitation > 0`, and every system reads that one number.

## Transitions

**Weather never snaps.** A Markov chain over conditions, weighted by season, with
a minimum dwell time and a transition duration of 30–180 seconds.

```
overcast ──0.4──► lightRain ──0.3──► heavyRain ──0.15──► storm
    ▲                  │                  │                │
    └──────0.5─────────┴──────0.4─────────┴───────0.6──────┘
```

Seasonal weighting: winter raises fog and snow and lowers clear; summer raises
clear and lowers precipitation. Kingston-specific tuning: **overcast is the
default state and should hold the majority of playtime.** A world that is
constantly dramatic is a world with no weather at all.

Scripted override for story beats — fog for a specific mission — with a clean
hand-back to the simulation afterwards.

## Fan-out — what weather actually drives

This is the directive's core demand and the reason weather is state, not a filter.

| Consumer | Response |
|---|---|
| **Materials** | `wetness`, `snowDepth`, `temperature` (`02-material-architecture.md`) |
| **Lighting** | Mood blend, fog density/colour, exposure, cloud shadow (`03`) |
| **VFX** | Precipitation layers, splashes, mist, leaves, breath (`05`) |
| **Audio** | Rain on surfaces, wind, thunder, muffled ambience |
| **NPC appearance** | Coats, hoods, **umbrellas**, scarves, bare arms in summer |
| **NPC behaviour** | Hurry, shelter under awnings, density drops, bunching at doorways |
| **NPC density** | Heavy rain cuts street population substantially |
| **Vehicles** | Grip, spray, wipers, headlights on, screen wetness |
| **Gameplay** | Police sightlines, sound masking, footprints in snow as a tracking mechanic |
| **Drainage** | Gutter flow, downpipe runoff, puddle growth |
| **Foliage** | Wind sway amplitude, leaf shed in autumn |

**Umbrellas, hurrying and doorway bunching are worth more than any shader.** They
are what makes rain feel like it is happening *to a town* rather than *on a
camera*, and they are cheap.

## Seasons

A season is an authored parameter set plus asset swaps, cross-faded over an
in-game week (not instantly).

| | Spring | Summer | Autumn | Winter |
|---|---|---|---|---|
| Daylight hours | 06–20 | 05–21 | 07–18 | 08–16 |
| Foliage | Fresh green, blossom | Full dark green | Ochre, red, thinning | Bare |
| Ground litter | — | — | **Leaves** (drifts, wind-driven) | Grit, slush |
| NPC clothing | Layers | Short sleeves | Coats | Heavy coats, hats, breath |
| Base temperature | 8–15 °C | 15–26 °C | 6–14 °C | −2–8 °C |
| Weather bias | Showers | Clear-biased | Wind + rain | Fog, frost, occasional snow |
| Grade | Neutral | SUNNY | AUTUMN | WINTER |

**Scoping honesty:** seasons are a **post-slice feature**. The vertical slice
ships **autumn/winter only** — which is the right choice regardless of budget,
because wet dark Kingston is the game's identity and the most striking thing we
can show. The architecture reserves the seats; the implementation waits.

## Rain — twelve layers, honestly costed

The directive's twelve layers, each with a real technique and a real budget. Not
all ship at once.

| # | Layer | Technique | Cost | Tier |
|---|---|---|---|---|
| 1 | Distant rain atmosphere | Fog density + visibility drop | ~free | LOW |
| 2 | Mid-distance particles | Camera-locked GPU particle volume | 0.3 ms | LOW |
| 3 | Near-camera streaks | Scrolling parallax planes, 2–3 layers | 0.15 ms | LOW |
| 4 | Surface impact | Spawned from a low-res ground raycast grid | 0.25 ms | MEDIUM |
| 5 | Puddle ripples | Procedural normal animation in the puddle shader | ~free | MEDIUM |
| 6 | Water accumulation | `wetness` state (`02`) | ~free | LOW |
| 7 | Wet material response | Albedo/roughness/normal/Fresnel (`02`) | ~free | LOW |
| 8 | Character + vehicle wetness | Same four ops, driven by sky exposure | ~free | MEDIUM |
| 9 | Glass droplets | Screen-space droplet shader on glass + windscreens | 0.1 ms | MEDIUM |
| 10 | Mist / volumetrics | Froxel fog (`01`, `03`) | shared | MEDIUM |
| 11 | Road reflections | SSR half-res on wet ground, cubemap fallback | 0.5 ms | HIGH |
| 12 | Drainage flow | Authored flow-map channel in gutters and downpipes | ~free | HIGH |

**Total rain cost at HIGH ≈ 1.3 ms of the 14.6 ms frame** — affordable *because*
seven of the twelve layers are near-free consequences of state that other systems
already own. That is the argument for the state-driven architecture in one line.

**Rain volume is camera-locked.** Never simulate rain across the map — only in a
box around the camera, which is standard practice and the difference between
affordable and impossible.

## Snow

Falling particles (wind-advected) · accumulation on horizontal surfaces ·
footprints and tyre tracks via a camera-local deformation target
(`07-world-interaction-architecture.md`) · melting into `wetness` → slush ·
colder grade · NPC clothing · snow on roofs and ledges via sky exposure.

**Post-slice.** Kingston snow is rare and brief; the fidelity-to-playtime ratio is
poor. Architecture reserved, implementation deferred.

## Fog and atmospherics

Kingston has a real river, and river fog is both authentic and the best cover
mechanic in the game (`docs/design/01-world-and-map.md`).

- **HIGH/ULTRA** — froxel volumetric fog, quarter-res volume, temporally
  reprojected. Light scattering from local sources: sodium lamps haloing in mist
  is the money shot of this setting.
- **MEDIUM** — froxel without reprojection.
- **LOW** — analytic exponential height fog, per-pixel, no volume. Still reads as
  atmosphere; loses only the light shafts.

**Local mist volumes** (hand-placed near the river, in the culverts, over the
Hogsmill) are authored boxes that raise local density — art-directed atmosphere at
near-zero cost.

**On volumetrics generally:** Naughty Dog hit the froxel-resolution wall on PS4 at
30 fps. We will hit it far harder. The directive's instruction — *the player should
perceive atmosphere before they perceive the technical compromise* — is exactly
right, and the practical expression of it is: **spend on fog colour, density
gradient and light scattering; do not spend on froxel resolution.** Nobody has
ever noticed quarter-res fog. Everybody notices flat fog.

## Implementation

**Package A (Unity URP)** — weather state pushed to a global constant buffer;
VFX Graph for precipitation; custom Renderer Feature for the froxel pass and SSR;
Timeline for scripted story weather.

**Package B (Metal)** — weather uniform buffer bound once per frame; compute
dispatch for froxel integration; particle simulation in compute with indirect
draw.

## Verification

1. **Unit tests on the state machine** — transition probabilities, dwell times,
   seasonal weighting, temperature curves, derived visibility. No GPU. This is the
   bulk of the coverage.
2. Soak test: run an accelerated in-game year and assert the condition
   distribution matches the intended Kingston climate. Catches a Markov chain that
   drifts into permanent storm.
3. Visual matrix: every condition × every season × three times of day.
4. Frame time and variance in the worst case — **storm at night in the Charter
   with every shopfront lit** — cold and after 30 minutes.
5. Confirm the fan-out: assert NPCs actually deploy umbrellas, density actually
   drops, grip actually changes. **A weather system that only changes pixels has
   failed its brief.**
