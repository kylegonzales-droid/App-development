# Material architecture

The directive's central demand: **weather is not a screen filter.** Materials must
carry state and change with the world. This document is how.

## The core idea — surface state is data, not a shader parameter

A surface's response to weather is described by **four scalars owned by
`KingstonCore`**, not by the renderer:

```
wetness     0…1    how much water the surface holds
snowDepth   0…1    accumulated snow, normalised to the surface's max
grime       0…1    authored base soiling, static
temperature °C     drives melting, freezing, evaporation, breath
```

These live in pure Swift, evolve by pure functions, and are unit-testable with no
GPU. The renderer samples them and shades accordingly. **That separation is what
makes the whole weather system testable** — the drying curve is arithmetic, not a
thing you verify by squinting at a screenshot.

## How wetness changes a surface — physically grounded

Wetting a surface does four real things. Reproduce those four, get it right
everywhere, for free:

| Effect | Implementation |
|---|---|
| **Darkens albedo** | Water reduces diffuse scattering. `albedo *= lerp(1, porosityDarkening, wetness)` — porous brick darkens hard (~0.45), glazed tile barely (~0.9) |
| **Smooths microsurface** | Water fills roughness. `roughness = lerp(roughness, 0.05, wetness * fillFactor)` |
| **Raises reflectance** | Water film has its own Fresnel. Blend toward F0 ≈ 0.02 as a coherent layer |
| **Flattens normals** | Water pools into micro-detail. `normal = lerp(normal, flat, wetness * 0.6)` |

**Porosity is a per-material constant** — one authored number that makes brick,
tarmac, painted metal and glass respond correctly and differently from the same
global wetness value. This is the highest-leverage parameter in the whole system.

## Material library

Each is a template: base PBR set + porosity + wetness response + snow affinity.

| Material | Porosity | Notes |
|---|---|---|
| Asphalt | 0.75 | Darkens strongly; the hero wet surface |
| Worn asphalt / patched | 0.80 | Puddles collect in the repairs |
| Concrete | 0.70 | Car park decks, kerbs |
| Paving slab / flagstone | 0.65 | Pavements; joints hold water |
| Brick | 0.85 | Darkens most; the British signature |
| Render / pebbledash | 0.70 | Estate walls |
| Glass | 0.02 | Droplets, not wetness |
| Bare metal | 0.10 | Railings, shutters |
| Painted metal | 0.15 | Lamp posts, bollards, bins |
| Wood | 0.60 | Benches, fencing, boat hulls |
| Plastic | 0.10 | Wheelie bins, signage |
| Grass | 0.55 | Darkens, clumps |
| Soil / mud | 0.90 | Saturates, then pools |
| Standing water | — | Its own shader |
| Snow | — | Its own shader |
| Ice | — | Its own shader |

## The state cycle the directive specifies

```
DRY ──rain starts──► WETTING ──rain continues──► SATURATED ──► PUDDLES FORM
                                                                     │
DRY ◄──evaporation── DRYING ◄──rain stops───────────────────────────┘
      (slow, uneven)
```

**Wetting is fast, drying is slow and uneven.** That asymmetry is what sells it.

```
wetting:  wetness += rainIntensity * absorptionRate * dt          // seconds
drying:   wetness -= evaporationRate(temp, wind, exposure) * dt   // many minutes
```

**Drying must be non-uniform or it looks like a fade.** Three cheap modifiers:

1. **Exposure** — a per-surface authored value. Sheltered ground under the railway
   arch stays wet long after the open Loop has dried.
2. **Drainage / height field** — low points hold water; camber sheds it. Roads
   dry at the crown first and the gutter last, which is exactly what real roads do
   and nobody ever implements.
3. **Blue-noise mask** — breaks the remaining uniformity into patches.

## Puddles

Puddles are **authored placement + dynamic depth**, not simulation.

- Artists paint a **puddle mask** into the terrain/road material during
  environment authoring. Placement is art-directed — under the arch, in the
  kerbside dip, in the pothole outside Verrall's.
- Depth is driven at runtime by accumulated rainfall, clamped by the mask.
- Below a depth threshold: a wet patch. Above: a reflective surface with its own
  normal ripples and Fresnel.
- **Ripples** from rain impact (procedural, driven by rain intensity) and from
  interaction (character and vehicle, written into a local ripple render target —
  see `07-world-interaction-architecture.md`).

Fully dynamic puddle simulation is explicitly rejected: expensive, hard to
art-direct, and no more convincing than a painted mask with dynamic depth.

## Snow accumulation

Same pattern — **state as data, shading as response.**

```
snowDepth += snowfallRate * exposureToSky * (1 - slope) * dt
snowDepth -= meltRate(temperature, groundHeat, footTraffic) * dt
```

- **Slope-aware** — accumulates on horizontal, not vertical. A single dot product
  against world up, weighted by a per-material snow affinity.
- **Sky-exposure aware** — baked per-vertex or sampled from a low-res sky
  occlusion texture, so it does not snow under the railway bridge.
- **Displacement at HIGH+**, parallax offset at MEDIUM, albedo blend at LOW.
- **Deformation** (footprints, tyre tracks) writes into a camera-local
  accumulation render target. See `07-world-interaction-architecture.md`.
- **Melting** produces `wetness`, then slush — the systems chain, which is what
  makes it feel like a world rather than a toggle.

**Scoping honesty:** snow is the most expensive item in this document and Kingston
snow is rare and short-lived. It is scheduled **after** the vertical slice
(`10-vertical-slice-graphics-plan.md`). The architecture reserves its place; the
implementation waits.

## Authoring

- **Master material per family**, instances for variation. Never one-off shaders.
- **Texture atlasing by zone**, matching the streaming chunks in
  `docs/design/12-technical-architecture.md`.
- **Channel-packed maps**: ORM (occlusion / roughness / metallic) in one texture.
  Bandwidth is the mobile constraint, not shader maths.
- **Shader complexity cap** — a hard node/instruction budget per material family,
  enforced at review. Mobile guidance in circulation puts graph node counts near
  100 for mobile; treat that as the ceiling, not the target.
- **Every material ships with all four weather responses authored** before it is
  accepted. A material that only looks right dry is not finished.

## Implementation

**Package A (Unity URP)** — Shader Graph master materials with a custom
`WeatherSurface` sub-graph consuming a global weather buffer. Snow and ripple
accumulation as `RenderTexture`s driven by a custom Renderer Feature. Puddle masks
in a vertex-colour or mask-texture channel.

**Package B (Metal)** — a shared surface-shading function with the four wetness
operations, `#include`-d by every material shader. Weather state in a single
uniform buffer bound once per frame. Accumulation targets as explicit textures in
the frame graph.

## Verification

1. Unit-test the state model: wetting rate, drying curve, exposure weighting,
   melt, saturation clamping. **No GPU required, and this is where the real
   coverage lives.**
2. Visual matrix: every material family × dry / wetting / saturated / drying /
   snow, captured at all four quality tiers.
3. The drying test: soak a street, stop the rain, timelapse it. **If it fades
   uniformly, the system is wrong** and the exposure/drainage modifiers are not
   working.
