# Verification — what "done" means here

## Compiling is not completion

A successful build proves the code parses and type-checks. It proves nothing
about whether the feature works, looks right, performs, or is reachable by the
player. Never report a feature complete on the strength of a build.

## Definition of done

A change is done when all of the following are true and you can say which command
produced each result:

1. It compiles in the Unity Editor with no new warnings.
2. Unit tests covering the changed logic exist and pass in the Test Runner.
3. It has been exercised at runtime — the actual path a player takes, in the
   Simulator or on device.
4. Visual and motion changes have been *looked at*, not inferred from the diff.
5. Per-frame code paths have been measured if they were touched.
6. Accessibility obligations in `hig-for-games.md` still hold.
7. Nothing unrelated was modified.

If a criterion cannot be met, say which one and why. An honest "built and unit
tested; not yet run in the Simulator" is worth more than an unqualified "done".

## Testing

Unit tests belong on game logic — rules, state machines, weather, surfaces,
scoring, progression — which is why `Manor.Core` is compiled with
`noEngineReferences: true`. Test behaviour and boundaries, not implementation
detail; a test that breaks on every refactor is a liability.

Use the TDD skills (`tdd-feature`, `tdd-bug-fix`, `tdd-refactor-guard`) for
workflow and NUnit via the Unity Test Runner for the API. Every bug fix starts
with a failing test that reproduces the bug, so it cannot regress silently.

Never skip, disable, or quarantine a failing test to get to green. A failing test
is information.

## Running it

Play mode in the Editor is the fast loop; **a device build is the truth**. Editor
frame times are not device frame times and must never be reported as such.

`.claude/skills/ios-simulator` still covers `xcrun simctl` for installing and
driving iOS builds, capturing screenshots and video, and log streaming. Note that
GPU performance in the Simulator is not representative — profile on hardware.

## Performance

Measure before optimising and after. `debugging-instruments` for general CPU,
memory and hangs; `ios-ettrace-performance` for launch and focused flows;
`ios-memgraph-analysis` for leaks and heap growth; `swiftui-performance` for view
update storms; `metrickit` for field telemetry.

Report frame time and its variance, not just an average — a game that averages
60 fps while dropping frames at every spawn is not a 60 fps game.

## When the toolchain is missing

Compiling and testing need the Unity Editor; an iOS build needs macOS with Unity
and Xcode. The development container has **none of these**, and no .NET SDK
either — so C# written there is **unverified by any compiler**, and must be
described that way.

What can still be verified without them: the pure numeric models, via
`tools/model-validation/validate_core_model.py`. A pass there means the model is
right, not that the code compiles.

Do the work that does not need the toolchain, state plainly which checks did not
run, and never simulate, guess at, or describe output you did not get.
