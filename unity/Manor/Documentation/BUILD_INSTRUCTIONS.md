# Build instructions — Manor Kingston

How to open and run the prototype from a clean machine.

## Read this first

**No part of this project has been compiled or run.** It was authored in a Linux
cloud container with no Unity, no .NET SDK and no GUI — the network policy there
blocks Microsoft's .NET distribution host. Expect compile errors on first open
and budget time to fix them. The numeric models are validated separately
(`../../../tools/model-validation/`), but that validates arithmetic, not C#.

## Requirements

| | |
|---|---|
| Unity | **Unity 6 (6000.x LTS)** via Unity Hub |
| Modules | **iOS Build Support** (tick at install — awkward to add later) |
| IDE | Visual Studio Community, Rider, or VS Code with the C# extension |
| Git | Any recent version |
| Disk | ~15 GB for Unity plus the project |

Everything above is free. **iOS builds additionally require macOS with Xcode** —
Apple's requirement, not a project one. Windows is fine for all development,
testing and play.

## Open it

1. Clone the repository.
2. Unity Hub → **Add → Add project from disk** → select `unity/Manor`
   (the folder containing `Assets/`, **not** the repository root).
3. Open it. Two prompts are expected:
   - **Version mismatch.** `ProjectVersion.txt` says `6000.0.0f1`, written without
     an Editor available. Accept the upgrade to your installed 6 LTS.
   - **Package resolution.** Unity pulls the packages in `Packages/manifest.json`.
     A few minutes on first open.
4. **Commit what Unity generates:** the updated `ProjectVersion.txt`,
   `Packages/packages-lock.json`, and **every `.meta` file**. Meta files carry the
   GUIDs that wire assets together — losing them breaks references for everyone.

## Build the playable scene

The scene is **generated, not stored.** There is no `.unity` file in source
control, deliberately: a procedural blockout can be rebuilt from scratch at any
time and there is no binary scene to corrupt or merge.

**Menu → Manor → Build Kingston Slice Scene**

That creates and saves `Assets/Manor/Scenes/KingstonSlice.unity` with the
streets, buildings, river, bridge, park, lighting, weather, NPCs, mission zones
and the player. Then press **Play**.

Safe to re-run at any time; it replaces the scene.

## Controls

| Input | Action |
|---|---|
| `W A S D` / left stick | Move |
| `Shift` | Run |
| Mouse / right stick | Look |
| `E` / gamepad west | Interact |
| `Esc` | Pause |

## Running the tests

**Window → General → Test Runner → EditMode → Run All.** 46 NUnit tests covering
the clock, surface wetness, weather simulation and mission runner.

These test `Manor.Core`, which is compiled with `noEngineReferences: true` and so
needs no play mode, no scene and no device.

## Validating the numeric models without Unity

```sh
python3 tools/model-validation/validate_core_model.py
```

32 checks, no dependencies. A pass means the model is right, **not** that the C#
compiles.

## Project layout

```
unity/Manor/
  Assets/Manor/
    Core/          Manor.Core    pure C#, noEngineReferences
    Core.Tests/    Manor.Core.Tests
    Game/          Manor.Game    MonoBehaviours
    Editor/        Manor.Editor  scene generator, character validator
    Generated/     created by the generator — safe to delete and rebuild
    Scenes/        generated scene output
  Documentation/
  Packages/
  ProjectSettings/
```

## Preparing for iOS

Already true of the project: URP, mobile-shaped quality tiers, no desktop-only
features.

Still required, on a Mac:
1. **File → Build Settings → iOS → Switch Platform.**
2. Set bundle identifier, signing team, and a minimum iOS version.
3. Build to an Xcode project, then build and sign from Xcode.
4. An Apple Developer account is needed for device deployment and the App Store.

Do not attempt to bypass Apple's toolchain requirement. There is no supported way
to produce a signed iOS build from Windows.
