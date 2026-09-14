# Verification — what "done" means here

## Compiling is not completion

A successful build proves the code parses and type-checks. It proves nothing
about whether the feature works, looks right, performs, or is reachable by the
player. Never report a feature complete on the strength of a build.

## Definition of done

A change is done when all of the following are true and you can say which command
produced each result:

1. It builds for the iOS Simulator with no new warnings.
2. Unit tests covering the changed logic exist and pass.
3. It has been exercised at runtime — the actual path a player takes, in the
   Simulator or on device.
4. Visual and motion changes have been *looked at*, not inferred from the diff.
5. Per-frame code paths have been measured if they were touched.
6. Accessibility obligations in `hig-for-games.md` still hold.
7. Nothing unrelated was modified.

If a criterion cannot be met, say which one and why. An honest "built and unit
tested; not yet run in the Simulator" is worth more than an unqualified "done".

## Testing

Unit tests belong on game logic — rules, state machines, scoring, progression —
which is why that layer stays free of SpriteKit and SwiftUI imports. Test
behaviour and boundaries, not implementation detail; a test that breaks on every
refactor is a liability.

Use the TDD skills (`tdd-feature`, `tdd-bug-fix`, `tdd-refactor-guard`) for
workflow and `swift-testing` for the API. Every bug fix starts with a failing
test that reproduces the bug, so it cannot regress silently.

Never skip, disable, or quarantine a failing test to get to green. A failing test
is information.

## Simulator

`.claude/skills/ios-simulator` covers `xcrun simctl`: boot, install, launch,
screenshots, video, log streaming, deep links, permission state, status-bar
overrides. Use it to verify real behaviour and to capture evidence of visual
changes.

## Performance

Measure before optimising and after. `debugging-instruments` for general CPU,
memory and hangs; `ios-ettrace-performance` for launch and focused flows;
`ios-memgraph-analysis` for leaks and heap growth; `swiftui-performance` for view
update storms; `metrickit` for field telemetry.

Report frame time and its variance, not just an average — a game that averages
60 fps while dropping frames at every spawn is not a 60 fps game.

## When the toolchain is missing

Builds, tests and the Simulator need macOS with Xcode. On a machine without
them, do the work that does not require them, and state plainly that
verification did not happen. Do not simulate, guess at, or describe output you
did not get.
