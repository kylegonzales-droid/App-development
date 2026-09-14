---
description: Run unit and UI tests on the iOS Simulator and report results honestly
allowed-tools: Bash, Read, Grep, Glob
---

# Test

$ARGUMENTS may name a specific test, suite, or target. Without arguments, run
the full suite.

1. Verify `xcodebuild` exists. If not, stop and say tests cannot run without a
   macOS host with Xcode.
2. Run:

   ```
   xcodebuild -workspace <ws> -scheme <scheme> \
     -destination 'platform=iOS Simulator,name=iPhone 17' test
   ```

   Narrow with `-only-testing:<Target>/<Suite>/<test>` when arguments were given.

Report the real counts: passed, failed, skipped. Never describe a suite as
passing when tests were skipped — name the skipped tests and why.

For each failure, determine whether the test or the code is wrong before
changing either. Never skip, disable, or quarantine a failing test to reach
green; a failing test is information.

If coverage of the changed logic is missing, say so — that is a finding, not an
afterthought.
