# Open decisions — blocking Phase 2

Phase 1 deliberately did not decide these. Each changes work downstream, and
each is the user's call.

## 1. Engine and camera — **blocking everything**

> **Reopened and widened by the graphics directive.**
> Full analysis: `../graphics/00-engine-decision.md`.

Phase 1 recommended high-angle top-down 2D on SpriteKit. The graphics directive
then specified a realistic 3D feature set — screen-space effects, PBR materials
with wet/dry state transitions, volumetric fog, snow accumulation with
footprints, motion matching, LOD, quality tiers. **SpriteKit is 2D and cannot do
any of it.** The two cannot both stand.

Four coherent packages, in `../graphics/00-engine-decision.md`:

- **A — Unity URP**, visual target as written. Recommended if the realistic 3D
  target is non-negotiable.
- **B — Metal custom renderer**, target reduced to stylised realism, seasons and
  snow cut from the slice. Recommended if Swift-native is the real constraint.
- **C — RealityKit.** Cannot do the specified weather/material systems. Not
  recommended for this target.
- **D — SpriteKit 2D** as Phase 1 specified. Most likely to ship; requires
  abandoning the realistic-3D target.

Note: **SceneKit is eliminated** — Apple soft-deprecated it at WWDC25
(critical-bug-only maintenance, no new features).

**Nothing should be built until this is recorded.**

## 2. Art resourcing — **the project's real constraint**

A believable Kingston at premium quality is thousands of authored assets, and art
is on the critical path through every milestone. Claude can build the engine,
systems, tools and tests; Claude cannot produce production game art.

Options: hire or contract an artist · narrow the art scope dramatically (fewer
zones, more repetition, a more abstract style) · licence a coherent asset base
and art-direct on top · accept a longer timeline.

**This decides whether the slice is achievable, and it should be settled before
Phase 3.**

## 3. Protagonist gender

Written as non-binary (they/them). Genuinely uncommon in the genre and it suits
the character. Nothing structural depends on it. Options and trade-offs in
`03-protagonist.md`.

## 4. Title

Recommended `MANOR`, store subtitle `MANOR: Kingston`. It is a common English
word, so App Store discoverability is a real concern. Alternates in
`00-game-vision.md`.

## 5. iOS deployment target

Set at scaffold. Recommend the lowest target that supports Swift 6 strict
concurrency comfortably and covers the intended device base — decided with real
device-share data. The binding constraint is running well on a five-year-old
iPhone, not the API level.

## 6. Scope honesty

An open-world crime game is among the most expensive genres in the industry. The
vertical slice alone is roughly **4–6 months for one engineer and one artist**
full time. The design is structured so that a *small, excellent, finished* game —
two or three zones, one strong story, twenty missions — is reachable, and that is
a far better outcome than an unfinished large one.

Worth agreeing explicitly: **is the goal to ship a focused game, or to build
toward a large one over years?** The answer changes what gets built in Phase 2.
