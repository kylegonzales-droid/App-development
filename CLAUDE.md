# App-development — iOS Mobile Game

## Current state of this repository — read this first

**This repository contains no application code.** As of the Phase 0 environment
setup it held zero commits: no Xcode project, no Swift sources, no assets, no
tests, no build scripts. Everything under `.claude/` is development tooling, not
product code.

Do not describe, summarise, or reason about "the existing game" as if it exists.
It does not yet. Until real sources land, every statement about architecture,
deployment target, Swift version, or dependencies is a *proposal*, not a fact
about this codebase.

## Target stack (declared, not yet implemented)

| Aspect | Status |
|---|---|
| Language | Swift 6 with strict concurrency — **to be set in Phase 1** |
| App shell / UI chrome | SwiftUI *(if the stack stays Swift-native)* |
| Gameplay surface | **UNDECIDED — blocking.** See `docs/graphics/00-engine-decision.md` |
| Testing | Swift Testing (unit) + XCUITest (flows) |
| Deployment target | **Undecided.** Pick the lowest target the feature set truly needs. |
| Persistence, audio, haptics, Game Center, monetisation | **Not adopted.** See "Adding frameworks". |

**The engine is an open, blocking decision.** Phase 1 recommended high-angle 2D
on SpriteKit; a later graphics directive specified a realistic 3D feature set that
SpriteKit cannot deliver. Four packages and a recommendation are in
`docs/graphics/00-engine-decision.md`. Do not scaffold, and do not pick an engine
implicitly by starting to write code, until this is recorded.

## Non-negotiable operating principles

1. **Inspect before modifying.** Read the actual files. Never assume structure.
2. **Preserve existing functionality.** No drive-by rewrites of working code.
3. **Prefer reusable systems** over one-off copies of the same logic.
4. **Avoid unnecessary dependencies.** See "Adding frameworks" below.
5. **Avoid speculative architecture.** Build for the requirement in front of you.
6. **Build incrementally.** Small, reviewable, independently verifiable changes.
7. **Test every meaningful change.** A change without a test is unfinished.
8. **Use the iOS Simulator when available** to exercise real behaviour.
9. **Validate visual UI changes** by looking at them, not by reading the diff.
10. **Profile performance when appropriate** rather than guessing at it.
11. **Compiling is not completion.** Never report a feature done because it built.

These are expanded in `.claude/rules/`, which is imported below and is binding.

@.claude/rules/engineering-principles.md
@.claude/rules/game-stack.md
@.claude/rules/hig-for-games.md
@.claude/rules/verification.md

## Adding frameworks

Adopt a framework only when a real, present requirement needs it — never because
it is idiomatic, available, or listed as a possibility. Frameworks explicitly
*not* adopted, pending justification: SwiftData, StoreKit 2, Core Haptics,
AVFoundation, Core Animation (beyond what SwiftUI/SpriteKit use internally),
Metal, GameplayKit, CloudKit, and any third-party backend.

When adopting one, record in the same change: what requires it, what was
considered instead, and how it is tested.

## Tooling reality check

The Claude Code session that set this project up ran on **Linux**, where no Swift
toolchain, Xcode, or Simulator exists. Build, test, and Simulator verification
require a macOS host with Xcode. If those tools are absent, say so plainly and
mark the work unverified — never imply a build or test result you did not obtain.

## Skills

30 skills are installed project-local under `.claude/skills/` and load
automatically. Provenance and licensing: `.claude/SKILLS-ATTRIBUTION.md`.
