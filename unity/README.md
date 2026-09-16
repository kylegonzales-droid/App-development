# Unity project

`Manor/` is the game. Unity 6 (6000.x LTS) + Universal Render Pipeline, iOS/Metal.

Engine rationale: [`../docs/decisions/0001-engine-unity-urp.md`](../docs/decisions/0001-engine-unity-urp.md).

## Before you open it

Two pinned values were written **without a Unity Editor available** and will need
confirming on first open:

- `ProjectSettings/ProjectVersion.txt` says `6000.0.0f1`. Unity will offer to
  upgrade it to whatever 6 LTS you have installed. Accept, then **commit the
  resulting version** so the team is pinned to one Editor.
- `Packages/manifest.json` pins plausible package versions. Unity resolves these
  against your Editor on first open; commit the resolved `packages-lock.json`.

Neither has been validated by an Editor. Everything else in `Assets/` is
hand-authored source.

## Layout

```
Manor/
  Assets/Manor/
    Core/          Manor.Core      pure C#, noEngineReferences: true
    Core.Tests/    Manor.Core.Tests  NUnit, runs in the Unity Test Runner
```

## The boundary that matters

`Manor.Core.asmdef` sets **`noEngineReferences: true`**. That is not a style
preference — it means the compiler physically cannot resolve `UnityEngine`
inside `Core/`. Game rules, weather, surface state, time and (later) the mission
runtime therefore cannot accidentally reach into the engine, and every one of them
is testable without entering play mode.

This is the same layering rule the project has had since Phase 1
(`../docs/design/12-technical-architecture.md`); Unity just lets the compiler
enforce it.

**Do not add references to `Manor.Core.asmdef`.**

## What is implemented

| Area | State |
|---|---|
| `Core/Time` | Game clock (1 real minute = 1 game hour), day parts |
| `Core/Surfaces` | Material profiles, wetting and drying model, per-patch exposure and drainage |
| `Core/Weather` | Seeded Markov weather chain, seasonal bias, smooth transitions, scripted override |
| `Core.Tests` | 33 NUnit tests across the three areas |
| Everything else | Not started |

There is **no scene, no renderer, no character, no shader** yet. This is the
engine-free foundation those are built on; see
[`../docs/graphics/11-first-prototype.md`](../docs/graphics/11-first-prototype.md)
for what gets built next.

## Running the tests

Open the project, then **Window → General → Test Runner → EditMode → Run All**.

The tests have **not been run** — this container has no Unity and no .NET SDK
(the network policy blocks Microsoft's distribution host). The *arithmetic* they
assert has been validated independently; see
[`../tools/model-validation/`](../tools/model-validation/).
