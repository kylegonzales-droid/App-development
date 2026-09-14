---
description: Build the app for the iOS Simulator and triage any failure
allowed-tools: Bash, Read, Grep, Glob
---

# Build

Build for the iOS Simulator and report the real result.

1. Locate the project: prefer `*.xcworkspace` over `*.xcodeproj`. If neither
   exists, stop and say the project has not been scaffolded yet — do not invent
   a build command.
2. Confirm the toolchain: `xcodebuild -version`. If it is missing, stop and say
   a macOS host with Xcode is required. Never report a build result you did
   not obtain.
3. List schemes with `xcodebuild -list`, then build:

   ```
   xcodebuild -workspace <ws> -scheme <scheme> \
     -destination 'platform=iOS Simulator,name=iPhone 17' \
     -configuration Debug build
   ```

   Substitute a simulator that `xcrun simctl list devices available` actually
   reports. Use `-project` instead of `-workspace` when there is no workspace.

For a failure: quote the actual compiler output, identify the root cause, and
distinguish a pre-existing failure from one introduced by recent changes —
verify that distinction against the base commit rather than asserting it.

Report new warnings as well as errors. Do not silence a warning to make output
clean.
