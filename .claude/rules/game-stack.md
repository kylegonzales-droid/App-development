# Game stack and architecture rules

## Engine posture

The declared target is a 2D mobile game: **SwiftUI for app chrome, SpriteKit for
the gameplay surface**, bridged with `SpriteView`. This is a recommendation
pending Phase 1 confirmation, not a ratified decision.

Do not change engine direction (to Metal, SceneKit, RealityKit, Unity, or
anything else) without an explicit decision from the user recorded in the repo.
If a requirement appears that the current engine genuinely cannot meet, raise it
as a decision to be made — do not quietly start building against a second engine.

## Layering

Keep these separable, whichever engine is confirmed:

- **Game logic** — rules, state machines, progression, scoring. Plain Swift
  types with no import of SpriteKit, SwiftUI, or UIKit. This layer is where unit
  tests carry their weight, so it must be testable without a scene or a view.
- **Presentation** — `SKScene`/`SKNode` subclasses, SwiftUI views, animation,
  particles, audio triggers. Reads from game logic; does not own the rules.
- **Platform services** — persistence, Game Center, purchases, analytics. Behind
  narrow protocols so gameplay does not depend on the vendor.

A value that both the simulation and the renderer need belongs in game logic, not
duplicated on a node.

## The frame budget is a correctness constraint

A premium mobile game holds 60 fps (120 where ProMotion is available). That means
a ~16.6 ms budget, ~8.3 ms at 120 Hz.

In per-frame code paths (`update(_:)`, `didSimulatePhysics`, contact delegates,
custom shaders):

- No allocation in the steady state. Pool nodes, particles, and buffers.
- No `String` formatting, no `NSLog`/`print`, no JSON, no disk I/O.
- No forced texture loads. Preload atlases at scene setup.
- No synchronous work that scales with entity count without a measured bound.

`.claude/skills/swift-memory-performance` covers `InlineArray` and `Span` for
zero-overhead hot paths. Treat a frame-time regression as a bug, not a polish item.

## Adopting a framework

Before adding any framework or third-party dependency, answer in writing:
1. What present requirement needs it?
2. What was considered instead, including doing nothing?
3. What does it cost — binary size, launch time, permissions, review risk?
4. How is it tested?

Unanswered means not adopted. This applies to SwiftData, StoreKit 2, Core
Haptics, AVFoundation, Metal, GameplayKit, CloudKit, Firebase and every
equivalent. Skills for several of these are installed; an installed skill is
reference material, not permission to adopt.

## Original work only

Study leading studios for engineering discipline and polish. Never copy
characters, art, audio, branding, copy, or proprietary implementation from Riot
Games or any other studio. Assets in this repository must be original or
properly licensed, with the licence recorded.
