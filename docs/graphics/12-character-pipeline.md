# Character pipeline — "characters need to be good render"

Engine: Unity 6 + URP (`../decisions/0001-engine-unity-urp.md`).

## What actually makes a character look good

Not polygon count. On mobile, past roughly 30k triangles a character stops looking
better and starts costing more. Perceived quality comes, in this order:

1. **Silhouette** — readable in one glance, at thumbnail size, from behind.
2. **Eyes** — the single highest quality-per-cost element on a face. Dead eyes
   ruin an otherwise excellent character; good eyes rescue a modest one.
3. **Skin shading** — skin that reads as skin, not as painted plastic.
4. **Hair** — the usual failure point. Card-based hair done well beats strand
   hair done badly, and strand hair is not affordable here.
5. **Cloth normal and detail** — seams, weave, wear. Cheap, enormously effective.
6. **Animation** — a modest model that moves beautifully outperforms a detailed
   one that moves stiffly. See `06-animation-architecture.md`.
7. **Lighting and contact** — a character that is not grounded by contact shadow
   and reflection looks pasted on, whatever the mesh.

Budget the effort in that order. It is close to the inverse of where most projects
spend it.

## Tiers and budgets

Matches the three NPC tiers in `../design/04-npc-system.md`.

| Tier | Who | Tris LOD0 | Textures | Bones | Notes |
|---|---|---|---|---|---|
| **Hero** | Remi | 40–55k | 2K albedo/normal/ORM, 1K face detail | 80–95 | Blendshapes for face, full IK |
| **Named** | Yvonne, Mikey, Sanaa, Bev, Dami, Col | 22–30k | 2K set | 60–70 | Reduced blendshapes |
| **Reactive** | Shop staff, police, riders | 9–14k | 1K set | 40–50 | No blendshapes, look-at IK only |
| **Ambient** | Crowd | 2.5–5k | Shared 1K atlas, 4–6 variants | 22–28 | GPU instanced, no IK |

**LOD chains** on every tier: LOD0 → LOD1 (50 %) → LOD2 (25 %) → LOD3 (impostor,
ambient only). Bone counts halve at LOD2 via a reduced skeleton.

**On-screen ceiling:** 1 hero + 2 named + 8 reactive + ~90 ambient, inside the
3.0 ms CPU animation slice in `01-graphics-and-rendering-architecture.md`.

## Shading

URP's `Lit` shader is not enough for faces. Four custom Shader Graph materials:

### Skin
URP has no subsurface scattering. The mobile-affordable substitute:
- **Wrapped diffuse** — `NdotL` remapped through a wrap term so light bleeds past
  the terminator instead of stopping hard.
- **Thickness map** driving a warm transmission tint at ears, nostrils, fingers.
- **Dual specular** — one broad low-intensity lobe, one tight sharp lobe. This is
  what stops skin reading as plastic.
- **Micro-detail normal** tiled for pores, faded by distance.
- No screen-space SSS pass. Not affordable, and not needed at our camera distance.

### Eyes
The highest-value shader in the game.
- **Parallax-offset cornea** over a flat iris plane, so the iris has real depth.
- **Limbal darkening** ring.
- **A fixed specular catchlight** that stays present even when no light happens to
  be in the right place — always, in every game that looks good.
- Wet eyelid contact shadow.

### Hair
- **Card-based**, alpha-tested for the inner mass, one sorted alpha-blended pass
  for the outer flyaway layer. Never a single fully-transparent pass — that is the
  sorting artefact everyone recognises.
- Anisotropic highlight shifted along the strand direction.
- **Remi's beanie removes most of this problem for the hero**, which was a
  deliberate character-design choice in `../design/03-protagonist.md` and is worth
  keeping for exactly this reason.

### Cloth
- Standard PBR plus a **detail normal** for weave, and a **wear mask** in an unused
  channel driving edge abrasion.
- **Weather-reactive**: the same wetness value from
  `02-material-architecture.md` darkens fabric and raises its specular, so
  characters get wet in the rain from the same system that wets the road.
  This is the single detail that will sell rain more than any particle.

## Where the characters come from

Stated plainly, because it determines the schedule:

**I cannot produce production character art.** Not the mesh, not the textures, not
the rig, not the animation. That is a specialist human discipline and no engine
choice changes it.

**I can build** every shader above, the LOD and import pipeline, the animation
state machine, inertialisation, IK, the crowd instancing system, the
weather-reactive cloth response, the validation tooling and the performance gates.

Three realistic routes for the art itself, best first:

| Route | Cost | Quality ceiling | Notes |
|---|---|---|---|
| **Licensed base mesh + custom art direction** | Low–medium | High | Buy a well-topologised, rigged base human; re-sculpt, re-texture, re-clothe to our designs. **Recommended** — it skips the hardest and least differentiating work. |
| **MetaHuman** | Free under $1M/yr | Very high | Epic changed the licence in June 2025: MetaHumans are non-engine products, usable in commercial Unity games with no revenue share. Real integration cost — rig, groom and shaders are Unreal-shaped and need reworking for URP, and mobile perf work is substantial. Evaluate; do not assume. |
| **Character creation tool** | Low | Medium–high | Fast and consistent; check the licence permits commercial game distribution, and expect a recognisable "look" unless heavily art-directed over. |
| **Bespoke from scratch** | High | Highest | Only worth it for Remi, and only once the slice has proved the game. |

**Evaluation and licence checklists: [`13-character-sourcing.md`](13-character-sourcing.md).**
Run them before any asset enters the repository.

## Import and validation

Enforced by an editor-time validator so bad assets cannot land silently:

- Scale 1.0, Y-up, metres. Feet at origin.
- Humanoid rig with a valid avatar; bone naming to one convention.
- Mesh compression off for hero, medium for ambient.
- **Read/Write disabled** — doubles memory when left on, and is the most common
  mobile memory mistake.
- Textures ASTC, streaming on, correct sRGB flags (albedo sRGB; normal and ORM linear).
- LOD chain present, with the tier's triangle budget enforced as a build error.
- Blendshape count inside tier budget.

## Verification

Per `../../.claude/rules/verification.md`, a character is not done because it imports:

1. **Turntable** in all six Kingston grades (`03-lighting-architecture.md`) —
   especially NIGHT and STORM, where most characters fall apart.
2. **Silhouette check** at 64 px. If you cannot tell who it is, the design failed,
   not the render.
3. **Wet pass** — confirm cloth and skin respond to the shared wetness value.
4. **Locomotion pass** — no foot skate, no terminator artefacts on the face during
   head turns.
5. **Crowd pass** — 90 ambient instances inside the animation budget, no visible
   marching in step.
6. **Memory** — per-character texture and animation footprint tracked in CI
   against the tier budget.
