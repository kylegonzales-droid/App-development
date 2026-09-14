---
description: Full pre-ship gate — lint, build, test, run, and audit before calling anything done
allowed-tools: Bash, Read, Grep, Glob, Task
---

# Preflight

Run before claiming a feature is complete. Compiling is not completion.

Work through every gate. Do not stop at the first pass, and do not skip a gate
silently — a skipped gate is a reported result.

1. **Scope** — `git status` and `git diff`. Confirm nothing unrelated was
   modified. Unexpected files are a finding.
2. **Lint** — run SwiftLint if configured (`swiftlint --strict`). See the
   `swiftlint` skill.
3. **Build** — `/ios-build`. Zero errors, and account for every new warning.
4. **Test** — `/ios-test`. Report passed, failed and skipped counts.
5. **Run** — `/sim-run` on the flow that changed. Look at the result.
6. **Audit** — dispatch the agents the change warrants:
   - UI, HUD or menu work → `swiftui-auditor`
   - async, actor or threading work → `concurrency-auditor`
   - per-frame, physics, asset or launch work → `game-performance-auditor`
   - new modules or boundary changes → `architecture-auditor`
7. **Accessibility** — verify the non-negotiables in
   `.claude/rules/hig-for-games.md`: 44×44 pt targets, VoiceOver labels and
   traits, Dynamic Type, Reduce Motion with the game still playable, contrast,
   colour never the sole channel, safe areas.

Finish with a verdict against the definition of done in
`.claude/rules/verification.md`: which criteria are met, which are not, and what
remains. If any gate could not run, name it and say why.

An honest partial pass is the goal. Do not round up to "done".
