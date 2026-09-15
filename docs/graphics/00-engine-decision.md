# Engine decision — read before anything else

**This directive changes the project's technical premise, and the change must be
stated plainly before any architecture is written.**

## What the directive asks for

Screen-space effects. GPU-driven particles. Volumetric-looking fog. Custom
post-processing. PBR materials with wet/dry state transitions. Snow accumulation
with footprints and tyre tracks. Deformable surfaces. Clustered or deferred
lighting with local lights and emissives. LOD, culling, instancing, batching.
Motion matching and IK. LOW/MEDIUM/HIGH/ULTRA quality tiers. A third-person
camera over a realistic open world.

**That is the feature list of a modern 3D real-time renderer.** Every item on it
assumes polygons, a programmable render graph, and the ability to write to and
sample custom render targets.

## What the project currently is

Phase 0 installed a **SpriteKit** skill environment. Phase 1 recommended
**high-angle top-down 2D on SpriteKit**, and recorded the engine as the single
blocking open decision (`docs/design/15-open-decisions.md`).

**SpriteKit is a 2D engine. It cannot do any of the above.** Not slowly — at all.
There is no depth buffer to sample, no G-buffer, no material model, no render
graph, no skeletal animation system, no LOD. This directive and the Phase 1
technical plan cannot both stand.

So the decision has to be made now, and it is yours.

## The options, honestly

| | SpriteKit | SceneKit | RealityKit | Metal (custom) | Unity URP |
|---|---|---|---|---|---|
| 3D PBR materials | ✗ | ✓ | ✓ | build it | ✓ |
| Custom render passes / post chain | ✗ | limited | **✗** | ✓ | ✓ |
| Screen-space wetness, SSR | ✗ | ✗ | ✗ | ✓ | ✓ |
| Snow accumulation buffers, footprints | ✗ | ✗ | **✗** | ✓ | ✓ |
| Froxel / volumetric-approx fog | ✗ | ✗ | ✗ | ✓ | ✓ |
| GPU particles | ✗ | limited | limited | ✓ | ✓ |
| Motion matching / IK | ✗ | limited | some IK | build it | ✓ |
| LOD, culling, instancing, quality tiers | n/a | partial | partial | build it | ✓ built-in |
| Open-world streaming | build | build | build | build | ✓ Addressables |
| Swift-native | ✓ | ✓ | ✓ | ✓ | ✗ |
| Phase 0 skills apply | ✓ | partly | partly | partly | shell only |
| **Status** | wrong tool | **soft-deprecated WWDC25** | Apple's path, low ceiling | you are writing an engine | industry default |

**SceneKit is out.** Apple soft-deprecated it at WWDC25 — critical-bug-only
maintenance, no new features, migration guidance pointing at RealityKit. Starting
a multi-year project on it would be malpractice.

**RealityKit is the subtle trap.** It is Apple's forward path and it renders
beautifully, but it is a high-level scene framework aimed at AR and spatial
content. It does not expose the custom render passes and post-processing chain
that this directive's weather and material systems are *built out of*. Wetness
maps, puddle ripple targets, snow accumulation buffers and screen-space rain all
require writing to and sampling your own render targets. You cannot build the
specified weather engine in RealityKit. It would be a fine choice for a
different, simpler visual target.

**Metal is honest but enormous.** It can do every item on the list, because you
implement every item on the list. Realistically that is **two to four
engineer-years of renderer work before there is a game** — render graph, material
system, lighting, shadows, particles, animation, streaming, tooling. Claude can
write a great deal of that code. It cannot compress the calendar, and the result
would be a bespoke engine one person maintains forever.

**Unity URP does ~80 % of this on day one**, has a mature iOS/Metal mobile path,
built-in LOD, culling, instancing, quality tiers, Addressables streaming, and a
real animation stack. The weather engine below becomes weeks of work rather than
years. Cost: it is not Swift-native, and most of Phase 0's *engine* skills stop
applying — though the iOS platform skills (simulator, Instruments, MetricKit,
accessibility, HIG, App Store) all still matter for shipping.

## The scope reality — this matters more than the engine

The reference points in this directive are SIGGRAPH 2020 talks on *The Last of Us
Part II*. Worth holding two facts about them:

- That game targeted **30 fps on PlayStation 4**, and the team still had to fight
  for volumetric fog resolution within its budget. A phone GPU sustaining 60 fps
  has **far less** thermal and power headroom than a PS4 has at 30.
- It credited roughly two thousand people, with a multi-year schedule and a
  bespoke engine and toolchain built over four previous titles.

"Use the discipline, not the technology" is exactly the right instinct, and it is
the framing this whole document set adopts. But discipline does not reduce the
asset count. **The binding constraint on this project is not rendering technique —
it is content volume**: every metre of a believable Kingston is authored geometry,
materials, props, signage and lighting. That was already the top risk in
`docs/design/14-roadmap.md`, and moving to realistic 3D multiplies it by a large
factor, because 3D assets cost several times what 2D ones do.

I can architect, implement, optimise and test all of the systems below. I cannot
produce production 3D art, and no engine choice changes that.

## Three coherent packages — pick one, do not mix

**Package A — Unity URP, visual target as written.**
The only realistic path to the directive's stated feature set. Accept: not
Swift-native, Phase 0's engine skills mostly retired, and an art budget measured
in thousands of authored 3D assets. *Recommended if the visual target is
non-negotiable.*

**Package B — Metal custom renderer, visual target reduced.**
Stays Swift-native, keeps Phase 0. Target becomes **stylised realism** rather than
realism. Slice ships rain + wet materials + fog only; seasons, snow accumulation
and motion matching are deferred past the slice or cut. Accept: you are building
an engine, and first playable is much further out. *Recommended if Swift-native is
the real constraint.*

**Package C — RealityKit.**
Apple's forward path, least custom code, lowest ceiling. The specified weather and
material systems are **not implementable**. Only viable with a substantially
simpler visual target. *Not recommended for this directive.*

**Package D — Phase 1 as written (SpriteKit 2D).**
Still a genuinely good game and by far the most likely to ship. Requires
abandoning this directive's realistic-3D target. *Recommended if shipping is the
priority.*

## My recommendation

**If the realistic 3D target stands: Package A (Unity URP).** It is the only
option where the described systems are achievable by a small team in a sane
timeframe.

**If staying Swift-native matters more than the visual target: Package B**, with
the target explicitly revised down to stylised realism and seasons cut from the
slice.

**I would push back on doing neither** — keeping a realistic 3D target on a
Swift-native stack is the combination that does not close, and it is the one this
directive currently implies.

## How the rest of these documents are written

Every architecture that follows is **engine-agnostic at the system level** — state
machines, data models, budgets, quality tiers, LOD policy, pooling strategy,
authoring pipeline. That design is real work and it survives the engine decision
intact.

Where implementation genuinely differs, the document says so in an
**Implementation** block naming Unity URP (Package A) and Metal (Package B)
separately. Nothing below assumes the decision has been made.

**Do not begin implementation until this decision is recorded.**
