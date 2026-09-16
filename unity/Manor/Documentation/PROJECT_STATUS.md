# Project status — Manor Kingston

Updated 2026-09-16.

## The honest headline

**Nothing here has been compiled, run, or played.**

The autonomy brief asked for a session operating a Windows PC: inspect the
machine, install Unity, open the Editor, run the game, read runtime errors and
iterate. That session is running in an **ephemeral Ubuntu 24.04 container in
Anthropic's cloud**, attached to the GitHub repository. Verified:

- No Windows host reachable — no `/mnt/c`, no host filesystem mount.
- No GUI — `DISPLAY` unset, no window control or screenshot tooling.
- No Unity, no .NET SDK, no PowerShell.
- The network policy returns **403 on Microsoft's .NET distribution host**, so a
  toolchain cannot be installed either.

So the install → build → run → debug loop could not be performed, and no report
claiming otherwise would be true. What was done instead: author the complete
prototype so a machine that *does* have Unity can open it and start that loop
immediately.

**To actually run the autonomy test, use Claude Code installed locally on the
Windows PC** — it has real terminal access to that machine.

## Tools

| Tool | Status |
|---|---|
| Unity | **Not installed.** Required: Unity 6 (6000.x LTS) + iOS Build Support |
| .NET SDK | **Not installable here** (network policy) |
| Git | Present, used |
| Python 3 | Present, used for model validation |
| Node 22 | Present, unused by the game |

## Architecture

```
Manor.Core     pure C#, noEngineReferences:true — the compiler prevents it
               importing UnityEngine
  Time/        GameClock (1 real minute = 1 game hour), DayPart
  Surfaces/    material profiles, wetting/drying model, per-patch exposure & drainage
  Weather/     seeded Markov chain, seasonal bias, smooth transitions
  Missions/    beat graph, objective vocabulary, runner
  World/       WorldState flags and faction standing

Manor.Game     MonoBehaviours
  Player/      CharacterController locomotion, camera-relative, run
  Camera/      damped follow, velocity look-ahead, wall collision
  Interaction/ IInteractable, single best-candidate context action
  NPC/         archetype ScriptableObject, wander/idle/talk, spawner
  Missions/    zone triggers, NPC-to-mission bridge
  Systems/     GameDirector, time-of-day, weather driver, wetness, quality tiers
  UI/          IMGUI HUD: objective, prompt, clock, weather, dialogue, pause

Manor.Editor   editor-only
               Kingston scene generator, NPC prefab builder, character validator
```

## Implemented

| System | State |
|---|---|
| Game clock and day parts | Complete, tested |
| Weather simulation | Complete, tested — 9 conditions, 4 seasons, scripted override |
| Surface wetting and drying | Complete, tested — non-uniform drying by exposure and drainage |
| Mission graph and runner | Complete, tested — data-driven, "The First Day" authored |
| World state and faction standing | Complete, tested — two independent scalars |
| Third-person player controller | Written, **uncompiled** |
| Third-person camera | Written, **uncompiled** |
| Interaction system | Written, **uncompiled** |
| NPC wander/idle/dialogue + spawner | Written, **uncompiled** |
| Weather → particles, fog, wetness | Written, **uncompiled** |
| Time of day → sun, ambient | Written, **uncompiled** |
| Quality tiers (Low/Medium/High) | Written, **uncompiled** |
| HUD and pause | Written, **uncompiled** |
| Kingston blockout generator | Written, **uncompiled** |
| Character import validator | Written, **uncompiled** |

## The environment

Generated procedurally, not hand-placed, so it can be rebuilt at any time and
there is no binary scene file in source control.

High street with pavements, kerbs, centre dashes, **double yellow lines** and a
zebra crossing · four side streets · the Thames with an embankment wall and
**Kings Bridge** on piers · ~24 buildings in varied brick tones with shopfront
glazing and fascias · a park with trees · lamp posts alternating **sodium and
LED** with real point lights · bins, bollards, a bus stop.

Geography follows `docs/design/01-world-and-map.md`: river west, pedestrianised
core, high street running north–south, park to the south-east.

## Not implemented

Seasons beyond the data model · snow accumulation · custom wet-surface Shader
Graph (the prototype darkens and glosses URP Lit via MaterialPropertyBlock
instead, so no shader can fail to compile) · character models and animation ·
audio · minimap · save/load · LOD chains · occlusion culling · baked lighting ·
object pooling for NPCs.

## Known risks

1. **Uncompiled C#.** ~2,400 lines no compiler has seen. Most likely failures:
   URP API surface differences across 6000.x, `SerializedObject` property names
   in the generator, and Input System namespace availability.
2. **Unvalidated version pins.** `ProjectVersion.txt` and `manifest.json` package
   versions were written without an Editor. Unity resolves both on first open.
3. **Placeholder player and NPCs** are capsules. Real characters need
   `docs/graphics/13-character-sourcing.md`.
4. **No performance data.** No device, no profiler, no frame times. Every
   performance claim in the docs is a target, not a measurement.

## Next steps

1. Open in Unity 6, fix compile errors, commit the resolved version and lock file.
2. Run the Test Runner; report results.
3. **Manor → Build Kingston Slice Scene**, press Play, walk the mission.
4. The feel gate (`docs/graphics/10-vertical-slice-graphics-plan.md`): 60 fps
   sustained, input latency inside budget, and someone who is not the developer
   enjoys simply moving around. **Before any art.**
5. Then: character sourcing, wet-surface Shader Graph, audio, LODs.
