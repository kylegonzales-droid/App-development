# Engineering principles

Binding rules for all work in this repository. Imported by `CLAUDE.md`.

## Inspect before modifying
Read the files you are about to change, plus their call sites and tests. If you
cannot name what currently exists, you are not ready to change it. When a request
assumes something that is not in the repository, say so rather than inventing it.

## Preserve what works
Do not refactor, rename, reformat, or restructure code that the task did not ask
you to touch. If you spot a real problem outside your scope, report it and leave
it. Deleting working code requires an explicit instruction.

## Reusable systems over repetition
The second occurrence of a pattern is a signal, the third is an obligation. Build
shared systems for things a game accumulates fast: entity spawning, state
transitions, tuning constants, audio cues, HUD components, save/load.

Balance this against speculative generality — extract when the duplication is real,
not when you predict it.

## No speculative architecture
Do not add protocols, layers, dependency-injection containers, event buses, or
plugin points for requirements that do not exist. A concrete type used once is
better than an abstraction used once. Generalise when the second caller arrives.

## Incremental change
Each change should be independently reviewable and independently verifiable.
Prefer a sequence of small, working commits to one large one. Never mix a
refactor with a behaviour change in the same commit.

## Honest reporting
- If a build failed, quote the failure.
- If a test was skipped, say it was skipped and why.
- If you could not verify something, mark it unverified.
- If you made an assumption, state it where the user will see it.

Distinguish a **pre-existing failure** from a **failure your change introduced**,
and prove the distinction (for example by checking the same command on the base
commit) rather than asserting it.

## Scope discipline
Deliver what was asked. If part of the scope is blocked, finish everything else
and state explicitly what you left out and why. Narrowing the work is the user's
call, not yours.
