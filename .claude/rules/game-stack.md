# Game stack and architecture rules

## Engine posture

**Unity 6 + URP. The game is 3D, third-person, iOS via Metal.**
Decided in `docs/decisions/0001-engine-unity-urp.md`.

This is not open for drift. Do not reintroduce SpriteKit, SwiftUI or a 2D camera
for gameplay. If a requirement appears that Unity genuinely cannot meet, raise it
as a decision to be made — do not quietly start building against something else.

## Layering

- **Game logic** — rules, state machines, progression, scoring, weather and
  surface state. Lives in `Manor.Core`, whose assembly definition sets
  `noEngineReferences: true`, so the compiler physically prevents it importing
  `UnityEngine`. This is where unit tests carry their weight.
- **Presentation** — MonoBehaviours, renderers, shaders, VFX, animation, camera.
  Reads from game logic; never owns the rules.
- **Platform services** — persistence, Game Center, purchases, analytics. Behind
  narrow interfaces so gameplay does not depend on a vendor.

A value both the simulation and the renderer need belongs in `Manor.Core`, not
duplicated on a component.

**Never add references to `Manor.Core.asmdef`.**

## The frame budget is a correctness constraint

A premium mobile game holds 60 fps (120 where ProMotion is available). That means
a ~16.6 ms budget, ~8.3 ms at 120 Hz.

In per-frame code paths (`Update`, `FixedUpdate`, `LateUpdate`, collision
callbacks, custom shaders):

- No allocation in the steady state. Pool nodes, particles, and buffers.
- No `String` formatting or concatenation, no `Debug.Log`, no JSON, no disk I/O.
- No forced texture loads. Preload atlases at scene setup.
- No synchronous work that scales with entity count without a measured bound.

Treat a frame-time regression as a bug, not a polish item. Budgets per system are
in `docs/graphics/01-graphics-and-rendering-architecture.md`; there is no spare
headroom by design, so a new effect must name the system it takes milliseconds from.

## Adopting a framework

Before adding any framework or third-party dependency, answer in writing:
1. What present requirement needs it?
2. What was considered instead, including doing nothing?
3. What does it cost — binary size, launch time, permissions, review risk?
4. How is it tested?

Unanswered means not adopted. This applies to Unity packages, Asset Store
purchases, analytics, ads, IAP, backends and every equivalent. An installed skill
or an available package is reference material, not permission to adopt.

Any third-party asset must have its licence checked for commercial iOS
distribution before it enters the repository, and the provenance recorded.

## Original work only

Study leading studios for engineering discipline and polish. Never copy
characters, art, audio, branding, copy, or proprietary implementation from Riot
Games or any other studio. Assets in this repository must be original or
properly licensed, with the licence recorded.
