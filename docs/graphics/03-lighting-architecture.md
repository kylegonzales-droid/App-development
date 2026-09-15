# Lighting architecture

## The Kingston insight

Britain is **overcast most of the time**. That is not a limitation — it is the
single biggest performance gift this setting gives us.

Overcast means: soft ambient dominance, weak directional contribution, short
sightlines, low dynamic range, no hard shadow edges to alias. **Ambient lighting
quality matters more than direct lighting quality**, which is cheap to get right
and expensive to get wrong. (Naughty Dog reached the same conclusion for the same
reason — a mostly-overcast world made ambient the crucial component of their
lighting system, under a hard PS4 budget at 30 fps.)

Our night look is the mirror image: **dominated by many small local sources** —
shopfronts, sodium lamps, LED lamps, headlights, bus interiors, phone screens.
That is exactly what clustered forward rendering is good at
(`01-graphics-and-rendering-architecture.md`).

## Light types and budget

| Type | Count | Shadows | Cost |
|---|---|---|---|
| Directional (sun / moon) | 1 | Cascades: 1 LOW → 4 ULTRA | Fixed |
| Sky / ambient probe | 1 | — | Fixed, cheap |
| Local probes (baked) | ~1 per 15 m | — | Baked, near-free |
| Local dynamic | ≤ 24 visible, clustered | ≤ 4 cast shadows | Scales with overlap |
| Emissive materials | Unlimited | Never | Near-free |
| Light-cookie sources | ≤ 8 | — | Cheap, high value |

**Emissives carry the night look, not shadow-casting lights.** A shopfront is an
emissive plane plus one non-shadowing local light plus a reflection in the wet
ground. Three cheap things reading as one expensive thing — that is the whole
technique.

## Lighting moods

A **mood** is a complete authored parameter set, not a brightness value. The
directive is explicit and correct: *do not simply increase brightness.*

```
LightingMood {
  sunDirection, sunColour, sunIntensity
  skyGradient (zenith, horizon, ground)
  ambientIntensity, ambientColourBalance
  fogDensity, fogColour, fogHeightFalloff
  exposureBias
  localLightTint, localLightIntensityScale
  gradeLUT
  shadowStrength, shadowTint
}
```

Twelve authored moods, per the directive: morning · midday · afternoon · golden
hour · sunset · evening · night · overcast · rain · storm · winter · summer.

Moods **blend continuously** over the time-of-day curve and cross-blend with
weather (`04-weather-architecture.md`). Weather moods multiply time moods rather
than replacing them — "rain" is a modifier on whatever hour it is, not a fixed
look. Blending is interpolation over the parameter set, done in `KingstonCore` as
pure arithmetic, so mood transitions are unit-testable.

## Kingston colour language — original

Derived from the Phase 1 colour script (`docs/design/09-visual-direction.md`) and
extended to the six grades the directive asks for. **This is our own palette,
built from what Kingston actually looks like, not borrowed grading from any
existing game.**

The organising idea is specific to this place and this decade: **the tension
between dying sodium street lighting (warm amber) and new LED street lighting
(cold cyan-white).** Britain is mid-replacement. Every street has both. It is
free visual interest, it is true, and it maps onto the game's own theme of old
Kingston versus new.

| Grade | Key | Shadow | Accent | Character |
|---|---|---|---|---|
| **SUNNY KINGSTON** | Pale warm `#F0E6D2` | Cool blue `#4A5A72` | Foliage green | Rare, cherished, slightly overexposed |
| **WET KINGSTON** | Flat silver `#C6CED6` | Green-grey `#485259` | Brake red, hi-vis | Low contrast, high specular, the default |
| **NIGHT KINGSTON** | Sodium `#F2A03D` + LED `#BFE6F0` | Blue-black `#101822` | Police blue | Two-source tension. **The signature.** |
| **WINTER KINGSTON** | Blue-white `#DDE8F0` | Deep indigo `#28334A` | Warm window glow | Desaturated, long shadows, breath |
| **AUTUMN KINGSTON** | Low amber `#E0A860` | Damp brown-grey `#4A423A` | Leaf ochre, red | Warm key against cold sky |
| **STORM KINGSTON** | Bruised grey `#8A94A0` | Near-black `#1A1F26` | Lightning white | Highest contrast, lowest visibility |

**Grading is a LUT per grade, blended over time.** Never a full-screen colour
multiply — that desaturates and flattens, which is the "weather as screen filter"
failure the directive rules out.

### Readability floor — binding

Cinematic grading must never hide gameplay. Enforced, not aspirational:

- Gameplay-critical elements (interactables, NPCs, objectives, hazards) hold a
  minimum luminance separation from their background **in every grade**.
- STORM and NIGHT get a **rim-light / contact-shadow pass** on characters so
  silhouettes stay readable against dark ground.
- Verified by the `swiftui-auditor` accessibility pass and by screenshots in all
  six grades. A screenshot that looks beautiful and loses the NPC is a bug.

## Shadows

- **Cascaded shadow maps** for the directional light. Cascade count by tier.
- **Tight cascade fitting** to the camera's actual view — the single biggest
  shadow quality win available, and it costs nothing.
- **Local shadows are a scarce resource**: hard cap of 4, allocated by a priority
  score (distance × screen size × narrative importance). Everything else uses
  contact shadows and AO.
- **Contact shadows** (short screen-space ray from depth) ground characters and
  props far more convincingly than a low-res shadow map, at a fraction of the
  cost. HIGH+.
- **Overcast simplification:** when the directional contribution is weak — most of
  the time in Kingston — cascades drop to one and the budget moves to ambient and
  AO, automatically. The weather drives the shadow budget.

## Ambient and indirect

Where the overcast look actually lives.

- **Baked irradiance probes** on a ~15 m grid, denser in interiors and under
  cover. Baked per season, interpolated.
- **Sky occlusion** baked per-vertex — drives ambient *and* rain wetting *and*
  snow accumulation. **One bake, three systems.** This is the kind of sharing that
  makes a budget work.
- **SSAO** at half res for contact darkening.
- **Reflection probes** per zone for cubemap fallback where SSR is off (LOW/MEDIUM).
- No real-time GI. Not affordable, and not needed under overcast.

## Interiors

Selected interiors only (`docs/design/09-visual-direction.md`). Entered seamlessly
with a lighting-mood cross-blend rather than a load screen: interior probe set
fades in, exterior directional fades down, fog density drops, exposure adapts.
The transition *is* the effect.

## Implementation

**Package A (Unity URP)** — Forward+ clustered path. Moods as ScriptableObjects
driving a global lighting buffer. LUT grading via a custom post-process Renderer
Feature. Baked probes via Adaptive Probe Volumes.

**Package B (Metal)** — Cluster assignment compute pass; lights in a single
structured buffer; moods as a uniform block interpolated on CPU in `KingstonCore`;
LUT applied in the final composite shader.

## Verification

1. Unit-test mood blending arithmetic and the time-of-day curve — pure functions.
2. Capture the same street in all twelve moods × six grades; review as a contact
   sheet. Inconsistency shows up instantly at that scale.
3. Confirm the readability floor holds in every grade, with the accessibility
   settings on and off.
4. Profile local-light overlap in the worst case — the Charter at night with
   every shopfront lit — and confirm the cluster cost stays inside its 3.0 ms.
