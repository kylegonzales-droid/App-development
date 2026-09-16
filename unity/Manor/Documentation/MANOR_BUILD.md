# Build — Manor Kingston

Build and test commands. **None have been executed**; see `MANOR_KNOWN_ISSUES.md`.

Opening the project for the first time is covered in `MANOR_LOCAL_SETUP.md`;
this file is the repeatable commands once it opens.

Set these once per shell:

```powershell
$UNITY   = "C:\Program Files\Unity\Hub\Editor\<VERSION>\Editor\Unity.exe"
$PROJECT = "<REPO>\unity\Manor"
```

Discover `<VERSION>` with `Get-ChildItem "C:\Program Files\Unity\Hub\Editor"`.

## Tests

```powershell
& $UNITY -runTests -batchmode -projectPath $PROJECT `
         -testPlatform EditMode `
         -testResults "$PROJECT\..\..\TestResults.xml" -logFile -
```

Exit code 0 means all passed. `TestResults.xml` is NUnit format; failures carry
the message and stack trace.

## Generate the scene from the command line

```powershell
& $UNITY -batchmode -quit -projectPath $PROJECT `
         -executeMethod Manor.Editor.KingstonSceneBuilder.Build -logFile -
```

`KingstonSceneBuilder.Build` is public and static, so it is directly callable.

## Windows development build

There is **no scripted build pipeline yet**. Either use
**File → Build Settings → Windows → Build**, or add
`Assets/Manor/Editor/BuildPipeline.cs` exposing a static method and call it the
same way as above. The second is worth doing as soon as the project compiles,
so builds are one command and CI-able.

## Validating the numeric models without Unity

```sh
python3 tools/model-validation/validate_core_model.py
```

32 checks, no dependencies, exits non-zero on failure. Validates the weather
chain, the wetting/drying curve and the mission graph. **A pass means the model
is right, not that the C# compiles.**

## Reading logs

Unity's Editor log on Windows:

```
%LOCALAPPDATA%\Unity\Editor\Editor.log
```

`-logFile -` in the commands above streams to stdout instead, which is what you
want for any automated run.
