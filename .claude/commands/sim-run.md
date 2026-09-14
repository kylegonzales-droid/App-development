---
description: Build, install, and launch the game on the iOS Simulator, then capture a screenshot
allowed-tools: Bash, Read, Grep, Glob
---

# Run on Simulator

Exercise the app the way a player would, and produce visual evidence.

Use the `ios-simulator` skill for `simctl` detail.

1. `xcrun simctl list devices available` — pick a booted device or boot one
   with `xcrun simctl boot <udid>`.
2. Build for that destination (see `/ios-build`).
3. Install and launch:

   ```
   xcrun simctl install <udid> <path/to/App.app>
   xcrun simctl launch --console-pty <udid> <bundle-id>
   ```
4. Drive the specific flow named in $ARGUMENTS, or the main play loop.
5. Capture evidence: `xcrun simctl io <udid> screenshot shot.png`, or
   `recordVideo` for motion and transitions.
6. **Look at the screenshot.** Describe what is actually on screen — layout,
   legibility, clipping, safe-area violations, whether it matches intent.
   A diff is not visual validation.

Watch the console for runtime warnings, constraint breakage, and dropped frames.

If the Simulator is unavailable, say the change is unverified at runtime rather
than implying it was exercised.
