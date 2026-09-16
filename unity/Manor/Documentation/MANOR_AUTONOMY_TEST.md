# Autonomy test — result

## Outcome

**The test could not be performed. The session was not on the Windows PC.**

The brief stated "YOU ARE NOW THE LOCAL CLAUDE CODE SESSION ON MY ACTUAL WINDOWS
PC." The Phase 0 machine audit, run as instructed before anything else,
contradicted that.

## Audit evidence, 2026-09-16

```
uname -a          Linux vm 6.18.44-fc-v33 (kernel built by "builder@sandboxing")
/etc/os-release   Ubuntu 24.04.4 LTS
powershell.exe    absent      cmd.exe   absent
winget            absent      wmic      absent      reg.exe   absent
/mnt/c            absent      /cygdrive/c   absent
WSL interop       absent      /proc/version shows no Microsoft kernel
Unity / Unity Hub no install paths, no binaries on PATH
DISPLAY           unset       WAYLAND_DISPLAY   unset
Hardware          4 cores, 15 GiB RAM, 252 GB disk, no enumerable GPU
```

This is the same ephemeral cloud container as the previous session. There is no
Windows host in reach, no GUI, and no way to install a toolchain — the network
policy also returns 403 on Microsoft's .NET distribution host.

## Phase results

| Phase | Result |
|---|---|
| 0 Machine audit | **VERIFIED** — and it disproved the premise |
| 1 Install Unity | **BLOCKED BY ENVIRONMENT** — no Windows host |
| 2 Locate project | **VERIFIED** — `/home/user/App-development/unity/Manor` in this container |
| 3 Inspect implementation | **VERIFIED** — done statically |
| 4 Checkpoint | **VERIFIED** — clean tree, work committed |
| 5–13 Open, compile, test, generate, play, build, profile | **BLOCKED BY ENVIRONMENT** |
| 14 Documentation | **VERIFIED** |
| 15 Final verification | Partial — only what does not need Unity |

## What was done instead

A static defect hunt on the engine-facing code, which is where the first local
run will lose the most time. Five defects found and fixed; one was a genuine
logic bug, not a style issue:

**HUD dialogue would never have appeared.** `HUDController.Start()` subscribed to
NPC dialogue events via `FindObjectsByType<NPCController>()`, but NPCs are
created in `NPCSpawner.Start()`. The order of two `Start()` calls is undefined,
so the HUD could subscribe to nothing and fail silently — the kind of bug that
costs an afternoon because nothing errors. Now subscribes to
`Interactor.Interacted`, which exists from `Awake`.

Full list in `MANOR_KNOWN_ISSUES.md`.

## How to actually run this test

Install Claude Code on the Windows PC and run it there. That session has real
terminal access to the machine and can perform the install → open → compile →
test → play → build loop this one cannot.

`MANOR_LOCAL_SETUP.md` is the sequence for it to follow.
