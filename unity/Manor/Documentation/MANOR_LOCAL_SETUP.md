# Local setup — run this on the Windows PC

The steps a local session (or you) runs to take the project from cloned to
playable. **None of this has been executed** — see `MANOR_KNOWN_ISSUES.md`.

## 1. Install Unity

```powershell
winget install --id Unity.UnityHub -e
```

Then in Unity Hub → **Installs → Install Editor → Unity 6.3 LTS** (or the newest
6000.x LTS offered), with these modules:

- **Windows Build Support (IL2CPP)** — for the Windows development build
- **Visual Studio** integration, unless you already have Rider or VS Code
- Skip iOS Build Support on Windows; it cannot produce an iOS build without macOS

Unity Hub requires a signed-in Unity account and licence acceptance. **That is a
manual step** — it cannot be automated and is the one place to expect a stop.

## 2. Open the project

```powershell
$hub = "C:\Program Files\Unity\Hub\Editor"
Get-ChildItem $hub                      # discover the installed version folder
$unity = "$hub\<VERSION>\Editor\Unity.exe"
& $unity -projectPath "<REPO>\unity\Manor"
```

Expect on first open: a version-upgrade prompt (accept) and package resolution
(a few minutes).

**Commit what Unity generates** — the updated `ProjectVersion.txt`,
`Packages/packages-lock.json`, and every `.meta` file. Meta files carry the GUIDs
that wire assets together.

## 3. Check the render pipeline

The repository contains **no URP pipeline asset**. Without one, materials fall
back to the built-in shader and the look is wrong.

Create → Rendering → URP Asset (with Universal Renderer), then assign it in
**Project Settings → Graphics → Scriptable Render Pipeline Settings** and in
**Project Settings → Quality** for each level.

## 4. Compile

Watch the Console. Expected failure classes are ranked in
`MANOR_KNOWN_ISSUES.md`. Fix, recompile, repeat — one fix at a time, not a batch
of speculative changes.

## 5. Run the tests

**Window → General → Test Runner → EditMode → Run All.** 39 tests.

Headless equivalent:

```powershell
& $unity -runTests -batchmode -projectPath "<REPO>\unity\Manor" `
         -testPlatform EditMode -testResults "<REPO>\TestResults.xml" -logFile -
```

## 6. Generate the world

**Menu → Manor → Build Kingston Slice Scene.**

Writes `Assets/Manor/Scenes/KingstonSlice.unity`. Safe to re-run; it replaces the
scene. Then press **Play**.

## 7. Controls

| Input | Action |
|---|---|
| `W A S D` / left stick | Move |
| `Shift` | Run |
| Mouse / right stick | Look |
| `E` / gamepad west | Interact |
| `Esc` | Pause |

## 8. Windows build

**File → Build Settings → Windows → Add Open Scenes → Build.**

Headless equivalent needs a small build script; there is none yet. Add one at
`Assets/Manor/Editor/BuildPipeline.cs` if a one-command build is wanted.

## What to send back

The Console output, the Test Runner counts, and a screenshot of Play Mode. With
those I can fix what actually broke rather than what I predict might.
