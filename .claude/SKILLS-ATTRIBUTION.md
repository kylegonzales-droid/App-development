# Installed skills — provenance and licensing

30 skills are vendored into `.claude/skills/`. They are third-party material,
not code authored for this project. This file exists to satisfy the notice
requirements of their licences; do not delete it.

Installed during Phase 0 environment setup on 2026-09-14.

---

## dpearson2699/swift-ios-skills — 21 skills

- Source: https://github.com/dpearson2699/swift-ios-skills
- Commit: `8d90fd1` (v3.9.1)
- Licence: **PolyForm Perimeter 1.0.0** — full text in
  `.claude/licenses/swift-ios-skills-LICENSE.txt`

> Required Notice: Copyright (c) 2025 dpearson2699 (https://github.com/dpearson2699)

PolyForm Perimeter permits use, modification and distribution for any purpose
**except providing a product that competes with the software**. Developing a
game does not compete with a Claude Code skills collection, so this use is
permitted. The constraint to respect: do not repackage or redistribute these
skills as a competing skills collection. The licence text and the Required
Notice above must travel with any copy — which is why both are committed here.

Per-skill `evals/` directories were dropped; they are the upstream project's own
test fixtures and serve no purpose here.

| Skill | Purpose |
|---|---|
| `spritekit` | 2D game engine — scenes, sprites, actions, physics, particles, tile maps |
| `gamekit` | Game Center — auth, leaderboards, achievements, matchmaking |
| `swift-language` | Modern Swift idioms — expressions, typed throws, result builders, generics |
| `swift-concurrency` | Strict concurrency, Sendable, actor isolation, SE-0466 |
| `swift-architecture` | MV/MVVM/TCA/Clean, module boundaries, migration |
| `swift-api-design-guidelines` | Apple naming and API design conventions |
| `swiftui-patterns` | `@Observable` ownership, state wiring, view decomposition, previews |
| `swiftui-layout-components` | Stacks, grids, lists, scroll views, forms, controls |
| `swiftui-navigation` | NavigationStack/SplitView, sheets, tabs, deep linking |
| `swiftui-animation` | Springs, transitions, PhaseAnimator, KeyframeAnimator, SF Symbol effects |
| `swiftui-gestures` | Tap, drag, magnify, rotate, composition, conflict resolution |
| `swiftui-performance` | View update storms, identity churn, Observation scope |
| `swift-testing` | Swift Testing — `@Test`, `#expect`, traits, XCTest migration |
| `ios-accessibility` | VoiceOver, Dynamic Type, Switch Control, traits, audit technique |
| `ios-simulator` | `xcrun simctl` — lifecycle, install, launch, screenshots, logs |
| `debugging-instruments` | LLDB, Memory Graph Debugger, Instruments; crash and hang triage |
| `ios-memgraph-analysis` | `.memgraph` capture, leak and heap-growth ownership paths |
| `ios-ettrace-performance` | ETTrace launch and flow profiling, dSYM matching |
| `metrickit` | Production performance telemetry, hang and crash reports |
| `swiftlint` | Lint configuration, rule selection, CI integration |
| `swiftdata` | Persistence — `@Model`, `@Query`, ModelContainer *(reference only; not adopted)* |

---

## auleostudio/claude-code-skill-apple-platform-development — 8 skills

- Source: https://github.com/auleostudio/claude-code-skill-apple-platform-development
- Commit: `4069301`
- Licence: **MIT**, declared in the repository README. Copyright Au Léo Studio
  (https://auleostudio.com).

Note: the upstream repository declares MIT in its README but **ships no
`LICENSE` file**, so there is no canonical licence text to copy. MIT permits
this use; if that ambiguity matters for a commercial release, ask upstream to
add the file.

Flattened from nested upstream paths into single-level skill directories so
Claude Code discovers them.

| Skill | Upstream path | Purpose |
|---|---|---|
| `ui-review` | `ios/ui-review` | SwiftUI review workflow — HIG, fonts, Dynamic Type, accessibility |
| `swift-memory-performance` | `swift/memory` | `InlineArray` and `Span` for zero-overhead hot paths |
| `tdd-feature` | `testing/tdd-feature` | Red-green-refactor for new features |
| `tdd-bug-fix` | `testing/tdd-bug-fix` | Reproduce a bug as a failing test before fixing |
| `tdd-refactor-guard` | `testing/tdd-refactor-guard` | Coverage check before refactoring |
| `characterization-test-generator` | `testing/characterization-test-generator` | Pin current behaviour before changing it |
| `test-data-factory` | `testing/test-data-factory` | Fixture factories and builders |
| `snapshot-test-setup` | `testing/snapshot-test-setup` | Visual regression testing *(proposes a dependency — not adopted)* |

`swift-memory-performance` was renamed from upstream `memory`; its frontmatter
`name:` was updated to match, which is the only content edit made to any
vendored skill.

---

## ebuntario/apple-hig — 1 skill

- Source: https://github.com/ebuntario/apple-hig
- Commit: `c434b91` (v1.0.0)
- Licence: **MIT**, Copyright (c) 2026 Ethan Buntario — full text in
  `.claude/licenses/apple-hig-LICENSE.txt`

`apple-hig` carries the full HIG reference set: foundations, components,
patterns, inputs, platforms and technologies — including `game-center.md` and
`game-controllers.md`. Installed as a single skill with `SKILL.md`, `references/`
and `assets/`.

The upstream `setup` script and `bin/` were not copied: the script only chmods
its own uninstaller, validates frontmatter, and optionally appends an import to
the *global* `~/.claude/CLAUDE.md`. None of that applies to a project-local
install, and the global edit is explicitly unwanted here.

**Known upstream defect:** 12 markdown cross-references point to 6 files that do
not exist in v1.0.0 (`foundations/writing.md`, `foundations/privacy.md`,
`foundations/inclusion.md`, `foundations/spatial-layout.md`,
`technologies/voiceover.md`, `technologies/mac-catalyst.md`). Verified missing
upstream, so this is not an installation error. Impact is minor — `SKILL.md` and
every other reference resolve.

Read `.claude/rules/hig-for-games.md` before applying this skill. It is a game:
HIG governs system-facing surfaces and the accessibility non-negotiables, not
the art direction of the playfield.

---

## Updating

These are vendored copies pinned to the commits above, not submodules. To update
one, re-copy from upstream at a newer commit, re-check licence terms, and update
the commit SHA here.
