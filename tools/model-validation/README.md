# Core model validation

`validate_core_model.py` is a line-for-line Python port of the numeric models in
`unity/Manor/Assets/Manor/Core/`, asserting the same properties as the NUnit suite.

## Why it exists

The development container has **no Unity and no .NET SDK** — the network policy
blocks Microsoft's .NET distribution host — so the C# cannot be compiled or run
here. The arithmetic can be, and the arithmetic is where the real risk lives: a
drying curve that fades uniformly, or a weather chain that drifts into permanent
storm, are logic bugs a compiler would never catch.

**A pass here means the model is right. It does not mean the C# compiles.**
Run the NUnit suite in the Unity Editor for that.

## Running it

```sh
python3 tools/model-validation/validate_core_model.py
```

No dependencies. Exits non-zero on failure, so it can go in CI.

## Keeping it honest

If you change the C# model, change this too — or delete it. **A reference model
that has silently drifted from the code is worse than no reference model**, because
it produces confident green ticks about code that no longer exists.

The RNG differs from .NET's `System.Random`, so this checks *statistical*
properties (climate mix, snow never outside winter, transitions never snapping),
never exact sequences.
