# Character sourcing — evaluation and licence checklist

A working checklist for acquiring the base meshes Remi and the named cast are
built from. Use it every time, on every asset. Pipeline context:
`12-character-pipeline.md`.

## What you are actually buying

You are **not** buying "a character". You are buying **a deformation-ready
topology and a rig** — the two things that are hardest to make, least visible in
a marketing render, and most expensive to fix later.

Everything a storefront screenshot shows you — the sculpt, the textures, the
clothes, the hair — you will replace with Remi's design anyway. **Judge an asset
on its wireframe and its skeleton, never on its beauty shot.**

A gorgeous mesh with bad topology is worthless. A plain mesh with excellent
topology is the foundation of every character in the game.

## The routes, ranked

| Route | Indicative cost | Ceiling | Verdict |
|---|---|---|---|
| **Licensed rigged base human** | £30–£250 per base | High | **Recommended.** Buy one good male and one good female base, build the whole cast from them. |
| **MetaHuman** | Free under $1M/yr revenue | Very high | **Now genuinely viable** — since June 2025 MetaHumans are non-engine products, usable in Unity with no Epic revenue share. Real integration cost: the rig, groom and shaders are Unreal-shaped and need reworking for URP, and mobile LOD/perf work is substantial. Evaluate, do not assume. |
| **Character creation tool** | £0–£500 | Medium–high | Fast and consistent. Expect a recognisable house look unless heavily art-directed over. Check commercial game distribution explicitly. |
| **Mixamo** | Free | Low (characters) / useful (rigging) | Characters are not hero quality. The **auto-rigger and animation library are genuinely useful for prototyping**, and the licence permits commercial game use. Cannot redistribute raw assets — fine for a shipped game, not for an asset pack. |
| **Bespoke freelance** | £1,500–£6,000 per hero | Highest | Only for Remi, and only after the slice proves the game. |

Costs are indicative and move; confirm at purchase.

## Topology acceptance checklist

Ask the seller for wireframe and skeleton screenshots before buying. If they will
not provide them, that is your answer.

**Mesh**
- [ ] **Quad-dominant.** Triangles only where hidden. **No n-gons anywhere.**
- [ ] **Edge loops encircling every deformation zone** — shoulder, elbow, wrist,
      hip, knee, ankle. This is the single most important criterion; without it
      joints collapse and pinch when animated and no amount of skin weighting saves it.
- [ ] **Concentric loops around eyes and mouth.** Non-negotiable for any face
      that will emote.
- [ ] Even polygon density. No wasted density on flat planes, no starved areas at
      joints.
- [ ] Clean symmetry with a straight centre line.
- [ ] **Separate sub-meshes** for body, clothing, hair, eyes, teeth — not one
      welded blob. You cannot swap clothing on a welded mesh.
- [ ] Triangle count at or below the tier budget in `12-character-pipeline.md`,
      or a supplied LOD chain that reaches it.

**UVs and textures**
- [ ] No unintended overlapping UVs. Mirrored islands are fine if deliberate.
- [ ] Consistent texel density across the body.
- [ ] Seams hidden in armpits, inner arms, under clothing lines.
- [ ] **No baked lighting or ambient occlusion burned into albedo.** This is a
      common and fatal defect — it fights every lighting mood in
      `03-lighting-architecture.md`.

**Rig**
- [ ] **Maps cleanly to Unity Humanoid.** Test the avatar configuration before
      committing; a rig that will not map costs you Mecanim, retargeting and the
      entire animation library.
- [ ] Bone count within tier budget.
- [ ] **Maximum 4 skin influences per vertex** — the mobile limit.
- [ ] No stray or zero weights.
- [ ] Sensible joint orientation and a clean hierarchy; no junk nulls.
- [ ] Blendshapes present if the character needs to emote.

**Scene hygiene**
- [ ] Real-world scale — an adult is ~1.7–1.85 m.
- [ ] Feet at origin, Y-up, facing +Z.
- [ ] **A-pose preferred** over T-pose; shoulders deform better from A-pose.
- [ ] Transforms frozen, history cleared, no leftover construction geometry.

## Licence checklist

Run every item. One failure kills the asset.

- [ ] **Commercial use in a distributed video game** is explicitly permitted.
- [ ] **Modification and derivative works** permitted — you will re-sculpt it.
- [ ] Permitted on **mobile / iOS App Store** distribution specifically.
- [ ] Licence scope understood: **per-seat, per-project or per-title?**
- [ ] Attribution requirements, if any, are ones you can actually satisfy.
- [ ] **Not "editorial use only".** This kills it instantly and is common on
      stock marketplaces.
- [ ] **Likeness release.** If the mesh is a scan of a real person, confirm the
      model release covers use in an interactive commercial product. Frequently
      missed and genuinely dangerous.
- [ ] **Provenance of AI-generated content** — if the asset was AI-generated, its
      copyright status may be unclear. Treat with caution.
- [ ] Revenue thresholds or seat requirements above a certain turnover
      (MetaHuman: Unreal seats required above $1M/yr).

**On the day you buy, save:**
1. The invoice.
2. A **PDF or screenshot of the full licence text as it read that day.**
3. The product page.
4. The seller's name and the asset version.

Marketplace terms change and delisted products take their licence pages with
them. If you ever have to prove what you were granted, the page you did not save
is the one you will need. Record it in a `PROVENANCE.md` beside the asset.

## Evaluation procedure

Do this before the asset enters the repository.

1. **Buy one candidate**, not five. Evaluate properly before scaling.
2. Import to Unity. Configure as **Humanoid** and check the avatar maps with no
   errors and no manual bone fixing.
3. **Retarget a Mixamo walk cycle onto it.** Free, fast, and it exposes rig
   problems immediately.
4. Inspect deformation at the extremes: arm fully raised, elbow fully bent, knee
   fully bent, hip at full stride. **Look for collapse, pinching and candy-wrap
   twisting at the wrist and shoulder.** This is where bad topology shows.
5. Check silhouette at 64 px.
6. Render a turntable in **NIGHT and STORM** grades
   (`03-lighting-architecture.md`). Characters that look fine in neutral studio
   light routinely fall apart in sodium and rain.
7. Profile: draw calls, skinned mesh cost, texture and animation memory against
   the tier budget.
8. Only then commit it, with its `PROVENANCE.md`.

## What Claude can and cannot do here

**Cannot:** produce the mesh, textures, rig or animation. No engine choice changes
this.

**Can:** build every shader in `12-character-pipeline.md`, the import validator
that enforces this checklist automatically at editor time, the LOD pipeline, the
animation state machine, inertialisation, IK, crowd instancing, the
weather-reactive cloth response, and the CI memory gates.

**Sequence:** get one base mesh through the procedure above, then Claude builds
the validator so every subsequent asset is checked automatically instead of by eye.
