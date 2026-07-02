# Feather Detective Prototype Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a playable 15-minute Unity URP narrative detective prototype where a small bird explores a park, investigates 12 feathers, infers bird-specific memory rules, and completes a nest deduction board.

**Architecture:** The repo root is the Unity project root. Core deduction logic lives in small C# classes with EditMode tests; MonoBehaviours are thin adapters for player movement, interaction, memory playback, and UI. A Unity Editor scene builder creates the low-poly park, perches, feather assets, and prototype scene so the visual slice is reproducible from source.

**Tech Stack:** Unity URP, C# MonoBehaviour scripts, ScriptableObject data, Unity Test Framework, legacy Unity input, generated low-poly primitive meshes, Git/GitHub.

---

## Current Context

The repository currently contains `README.md`, the approved design spec, and a clean `main` branch tracking `origin/main` at `https://github.com/Jkkkeen/HackathonGame.git`.

Unity Editor was not found in `PATH` or the standard Unity Hub install directory on this machine during planning. The implementation therefore creates all source-controlled Unity project files directly and includes scripts under `tools/` that find Unity when it is installed. Unity-generated folders such as `Library/`, `Temp/`, and `Logs/` stay out of git.

## File Structure

Create or modify these files:

- `.gitignore` - Unity-specific ignored folders and build outputs.
- `.gitattributes` - text normalization and Unity binary asset handling.
- `Packages/manifest.json` - Unity package dependencies including URP and Test Framework.
- `Assets/FeatherDetective/Scripts/FeatherDetective.Runtime.asmdef` - runtime assembly.
- `Assets/FeatherDetective/Tests/EditMode/FeatherDetective.EditModeTests.asmdef` - EditMode tests.
- `Assets/FeatherDetective/Scripts/Data/BirdSpecies.cs` - bird species enum.
- `Assets/FeatherDetective/Scripts/Data/ClueRole.cs` - main mystery vs atmosphere clue enum.
- `Assets/FeatherDetective/Scripts/Data/DeductionSlotId.cs` - nest board slot enum.
- `Assets/FeatherDetective/Scripts/Data/DeductionAnswer.cs` - serializable slot/feather answer pair.
- `Assets/FeatherDetective/Scripts/Data/FeatherDefinition.cs` - ScriptableObject feather data.
- `Assets/FeatherDetective/Scripts/Data/DeductionSolution.cs` - ScriptableObject solution data.
- `Assets/FeatherDetective/Scripts/State/InvestigationState.cs` - collected feathers and board placements.
- `Assets/FeatherDetective/Scripts/Runtime/InvestigationRuntime.cs` - scene adapter for state, inventory, memories, and board.
- `Assets/FeatherDetective/Scripts/Interaction/IInspectable.cs` - inspectable target interface.
- `Assets/FeatherDetective/Scripts/Interaction/FeatherPickup.cs` - feather scene object.
- `Assets/FeatherDetective/Scripts/Movement/PerchNode.cs` - authored perch.
- `Assets/FeatherDetective/Scripts/Movement/PerchSelector.cs` - pure perch selection utility.
- `Assets/FeatherDetective/Scripts/Movement/BirdPlayerController.cs` - walk, hop, inspect, and linked-perch glide.
- `Assets/FeatherDetective/Scripts/Camera/FixedThirdPersonCamera.cs` - calm fixed follow camera.
- `Assets/FeatherDetective/Scripts/Memory/IMemoryEffect.cs` - shared memory effect contract.
- `Assets/FeatherDetective/Scripts/Memory/MemoryContext.cs` - references used by memory presentations.
- `Assets/FeatherDetective/Scripts/Memory/MemoryPlaybackController.cs` - species-to-effect dispatcher.
- `Assets/FeatherDetective/Scripts/Memory/CrowMemoryEffect.cs` - dark screen and spatial audio memory.
- `Assets/FeatherDetective/Scripts/Memory/MagpieMemoryEffect.cs` - grayscale scene with selected color hints.
- `Assets/FeatherDetective/Scripts/Memory/PigeonMemoryEffect.cs` - animated white route lines.
- `Assets/FeatherDetective/Scripts/Memory/WoodpeckerMemoryEffect.cs` - brief first-person rhythm view.
- `Assets/FeatherDetective/Scripts/Memory/SparrowMemoryEffect.cs` - ambient tension shift.
- `Assets/FeatherDetective/Scripts/World/ColorMemoryTarget.cs` - renderer color hint metadata.
- `Assets/FeatherDetective/Scripts/World/RouteLineRenderer.cs` - LineRenderer route animation.
- `Assets/FeatherDetective/Scripts/World/FirstPersonMemoryRig.cs` - rhythm camera motion.
- `Assets/FeatherDetective/Scripts/World/AtmosphereController.cs` - wind, insects, and flock ambience.
- `Assets/FeatherDetective/Scripts/Deduction/DeductionBoard.cs` - board slot placement and solved check.
- `Assets/FeatherDetective/Scripts/Deduction/DeductionSlot.cs` - individual board slot.
- `Assets/FeatherDetective/Scripts/Deduction/EndingController.cs` - quiet ending trigger.
- `Assets/FeatherDetective/Scripts/UI/InteractionPrompt.cs` - small prompt text.
- `Assets/FeatherDetective/Scripts/UI/FeatherInventoryUI.cs` - collected feather list.
- `Assets/FeatherDetective/Scripts/UI/DeductionBoardUI.cs` - nest board controls.
- `Assets/FeatherDetective/Scripts/UI/EndingMessageUI.cs` - final text.
- `Assets/FeatherDetective/Editor/PrototypeSceneBuilder.cs` - reproducible park, feather, and scene generator.
- `Assets/FeatherDetective/Editor/FeatherDetective.Editor.asmdef` - editor assembly for scene generation tools.
- `Assets/FeatherDetective/Tests/EditMode/FeatherDefinitionTests.cs` - feather data tests.
- `Assets/FeatherDetective/Tests/EditMode/InvestigationStateTests.cs` - collection and deduction state tests.
- `Assets/FeatherDetective/Tests/EditMode/PerchSelectorTests.cs` - linked-perch selection tests.
- `Assets/FeatherDetective/Tests/EditMode/MemoryPlaybackControllerTests.cs` - memory dispatch tests.
- `Assets/FeatherDetective/Tests/EditMode/PrototypeSceneBuilderTests.cs` - generated scene content tests.
- `tools/find-unity-editor.ps1` - local Unity Editor lookup helper.
- `tools/run-unity-tests.ps1` - Unity Test Runner wrapper.

## Test Commands

Use this command for EditMode tests after Unity Editor is installed:

```powershell
powershell -ExecutionPolicy Bypass -File tools/run-unity-tests.ps1 -TestPlatform editmode
```

Expected after all tasks: process exits `0`, creates `TestResults/EditMode.xml`, and reports all EditMode tests passing.

Use this command for PlayMode smoke tests after the generated scene exists:

```powershell
powershell -ExecutionPolicy Bypass -File tools/run-unity-tests.ps1 -TestPlatform playmode
```

Expected after all tasks: process exits `0`, creates `TestResults/PlayMode.xml`, and reports no script compilation errors.

---

### Task 1: Unity Project Skeleton And Tooling

**Files:**
- Create: `.gitignore`
- Create: `.gitattributes`
- Create: `Packages/manifest.json`
- Create: `Assets/FeatherDetective/Scripts/FeatherDetective.Runtime.asmdef`
- Create: `Assets/FeatherDetective/Tests/EditMode/FeatherDetective.EditModeTests.asmdef`
- Create: `tools/find-unity-editor.ps1`
- Create: `tools/run-unity-tests.ps1`

- [ ] **Step 1: Create Unity ignore rules**

Create `.gitignore` with:

```gitignore
[Ll]ibrary/
[Tt]emp/
[Oo]bj/
[Bb]uild/
[Bb]uilds/
[Ll]ogs/
[Uu]ser[Ss]ettings/
[Mm]emoryCaptures/
Recordings/

*.csproj
*.sln
*.suo
*.tmp
*.user
*.userprefs
*.pidb
*.booproj
*.svd
*.pdb
*.mdb
*.opendb
*.VC.db

.vs/
.vscode/
.idea/
TestResults/
```

- [ ] **Step 2: Create git attributes**

Create `.gitattributes` with:

```gitattributes
* text=auto
*.unity merge=unityyamlmerge eol=lf
*.prefab merge=unityyamlmerge eol=lf
*.asset merge=unityyamlmerge eol=lf
*.mat merge=unityyamlmerge eol=lf
*.anim merge=unityyamlmerge eol=lf
*.controller merge=unityyamlmerge eol=lf
*.png binary
*.jpg binary
*.jpeg binary
*.wav binary
*.mp3 binary
*.fbx binary
```

- [ ] **Step 3: Create package manifest**

Create `Packages/manifest.json` with:

```json
{
  "dependencies": {
    "com.unity.render-pipelines.universal": "17.0.3",
    "com.unity.test-framework": "1.4.5",
    "com.unity.textmeshpro": "3.0.9",
    "com.unity.timeline": "1.8.7",
    "com.unity.ugui": "2.0.0",
    "com.unity.modules.ai": "1.0.0",
    "com.unity.modules.androidjni": "1.0.0",
    "com.unity.modules.animation": "1.0.0",
    "com.unity.modules.assetbundle": "1.0.0",
    "com.unity.modules.audio": "1.0.0",
    "com.unity.modules.cloth": "1.0.0",
    "com.unity.modules.director": "1.0.0",
    "com.unity.modules.imageconversion": "1.0.0",
    "com.unity.modules.imgui": "1.0.0",
    "com.unity.modules.jsonserialize": "1.0.0",
    "com.unity.modules.particlesystem": "1.0.0",
    "com.unity.modules.physics": "1.0.0",
    "com.unity.modules.physics2d": "1.0.0",
    "com.unity.modules.screencapture": "1.0.0",
    "com.unity.modules.terrain": "1.0.0",
    "com.unity.modules.terrainphysics": "1.0.0",
    "com.unity.modules.tilemap": "1.0.0",
    "com.unity.modules.ui": "1.0.0",
    "com.unity.modules.uielements": "1.0.0",
    "com.unity.modules.umbra": "1.0.0",
    "com.unity.modules.unityanalytics": "1.0.0",
    "com.unity.modules.unitywebrequest": "1.0.0",
    "com.unity.modules.unitywebrequestassetbundle": "1.0.0",
    "com.unity.modules.unitywebrequestaudio": "1.0.0",
    "com.unity.modules.unitywebrequesttexture": "1.0.0",
    "com.unity.modules.unitywebrequestwww": "1.0.0",
    "com.unity.modules.vehicles": "1.0.0",
    "com.unity.modules.video": "1.0.0",
    "com.unity.modules.vr": "1.0.0",
    "com.unity.modules.wind": "1.0.0",
    "com.unity.modules.xr": "1.0.0"
  }
}
```

- [ ] **Step 4: Create assembly definitions**

Create `Assets/FeatherDetective/Scripts/FeatherDetective.Runtime.asmdef` with:

```json
{
  "name": "FeatherDetective.Runtime",
  "rootNamespace": "FeatherDetective",
  "references": [],
  "includePlatforms": [],
  "excludePlatforms": [],
  "allowUnsafeCode": false,
  "overrideReferences": false,
  "precompiledReferences": [],
  "autoReferenced": true,
  "defineConstraints": [],
  "versionDefines": [],
  "noEngineReferences": false
}
```

Create `Assets/FeatherDetective/Tests/EditMode/FeatherDetective.EditModeTests.asmdef` with:

```json
{
  "name": "FeatherDetective.EditModeTests",
  "rootNamespace": "FeatherDetective.Tests",
  "references": [
    "FeatherDetective.Runtime"
  ],
  "includePlatforms": [
    "Editor"
  ],
  "excludePlatforms": [],
  "allowUnsafeCode": false,
  "overrideReferences": true,
  "precompiledReferences": [
    "nunit.framework.dll"
  ],
  "autoReferenced": false,
  "defineConstraints": [
    "UNITY_INCLUDE_TESTS"
  ],
  "versionDefines": [],
  "noEngineReferences": false
}
```

- [ ] **Step 5: Create Unity lookup helper**

Create `tools/find-unity-editor.ps1` with:

```powershell
$ErrorActionPreference = "Stop"

$candidates = @()
if ($env:UNITY_EDITOR) {
    $candidates += $env:UNITY_EDITOR
}

$hubRoot = "C:\Program Files\Unity\Hub\Editor"
if (Test-Path $hubRoot) {
    $candidates += Get-ChildItem -Path $hubRoot -Directory |
        Sort-Object Name -Descending |
        ForEach-Object { Join-Path $_.FullName "Editor\Unity.exe" }
}

$pathUnity = Get-Command Unity -ErrorAction SilentlyContinue
if ($pathUnity) {
    $candidates += $pathUnity.Source
}

$editor = $candidates | Where-Object { $_ -and (Test-Path $_) } | Select-Object -First 1
if (-not $editor) {
    Write-Error "Unity Editor executable was not found. Install Unity with URP support or set UNITY_EDITOR to the full Unity.exe path."
    exit 1
}

Write-Output $editor
```

- [ ] **Step 6: Create Unity test wrapper**

Create `tools/run-unity-tests.ps1` with:

```powershell
param(
    [ValidateSet("editmode", "playmode")]
    [string]$TestPlatform = "editmode"
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
$unity = powershell -ExecutionPolicy Bypass -File (Join-Path $PSScriptRoot "find-unity-editor.ps1")
$resultsDir = Join-Path $repoRoot "TestResults"
New-Item -ItemType Directory -Force -Path $resultsDir | Out-Null

$resultFile = Join-Path $resultsDir ("{0}.xml" -f ($(if ($TestPlatform -eq "editmode") { "EditMode" } else { "PlayMode" })))

& $unity `
    -batchmode `
    -quit `
    -projectPath $repoRoot `
    -runTests `
    -testPlatform $TestPlatform `
    -testResults $resultFile

exit $LASTEXITCODE
```

- [ ] **Step 7: Verify helper behavior**

Run:

```powershell
powershell -ExecutionPolicy Bypass -File tools/find-unity-editor.ps1
```

Expected on a machine without Unity Editor: FAIL with `Unity Editor executable was not found`.

Expected on a machine with Unity Editor: PASS and print the full `Unity.exe` path.

- [ ] **Step 8: Commit and push**

```powershell
git add .gitignore .gitattributes Packages/manifest.json Assets/FeatherDetective/Scripts/FeatherDetective.Runtime.asmdef Assets/FeatherDetective/Tests/EditMode/FeatherDetective.EditModeTests.asmdef tools/find-unity-editor.ps1 tools/run-unity-tests.ps1
git commit -m "chore: add unity project skeleton"
git push
```

---

### Task 2: Feather Data Model

**Files:**
- Create: `Assets/FeatherDetective/Tests/EditMode/FeatherDefinitionTests.cs`
- Create: `Assets/FeatherDetective/Scripts/Data/BirdSpecies.cs`
- Create: `Assets/FeatherDetective/Scripts/Data/ClueRole.cs`
- Create: `Assets/FeatherDetective/Scripts/Data/DeductionSlotId.cs`
- Create: `Assets/FeatherDetective/Scripts/Data/DeductionAnswer.cs`
- Create: `Assets/FeatherDetective/Scripts/Data/FeatherDefinition.cs`
- Create: `Assets/FeatherDetective/Scripts/Data/DeductionSolution.cs`

- [ ] **Step 1: Write failing feather definition tests**

Create `Assets/FeatherDetective/Tests/EditMode/FeatherDefinitionTests.cs` with:

```csharp
using NUnit.Framework;
using UnityEngine;

namespace FeatherDetective.Tests
{
    public sealed class FeatherDefinitionTests
    {
        [Test]
        public void FeatherDefinitionStoresSensoryIdentityAndCompatibleSlots()
        {
            var feather = ScriptableObject.CreateInstance<FeatherDefinition>();
            feather.ConfigureForTests(
                "crow-bench",
                "Feather by the feeding bench",
                BirdSpecies.Crow,
                ClueRole.MainMystery,
                new[] { DeductionSlotId.LastFeedingDay, DeductionSlotId.WhatChanged },
                new[] { new Color(0.1f, 0.2f, 0.3f) },
                new[] { new Vector3(0f, 0f, 0f), new Vector3(1f, 0f, 2f) },
                "Hospital siren, wheels, and a soft worried voice."
            );

            Assert.That(feather.Id, Is.EqualTo("crow-bench"));
            Assert.That(feather.DisplayName, Is.EqualTo("Feather by the feeding bench"));
            Assert.That(feather.Species, Is.EqualTo(BirdSpecies.Crow));
            Assert.That(feather.Role, Is.EqualTo(ClueRole.MainMystery));
            Assert.That(feather.CanPlaceInSlot(DeductionSlotId.LastFeedingDay), Is.True);
            Assert.That(feather.CanPlaceInSlot(DeductionSlotId.WhereHeWent), Is.False);
            Assert.That(feather.HighlightColors, Has.Length.EqualTo(1));
            Assert.That(feather.RoutePoints, Has.Length.EqualTo(2));
            Assert.That(feather.MemorySummary, Does.Contain("Hospital siren"));
        }

        [Test]
        public void DeductionSolutionIsSatisfiedOnlyByExactRequiredFeatherIds()
        {
            var crow = CreateFeather("crow-bench", BirdSpecies.Crow, DeductionSlotId.LastFeedingDay);
            var pigeon = CreateFeather("pigeon-park-edge", BirdSpecies.Pigeon, DeductionSlotId.WhereHeWent);
            var solution = ScriptableObject.CreateInstance<DeductionSolution>();
            solution.ConfigureForTests(new[]
            {
                new DeductionAnswer(DeductionSlotId.LastFeedingDay, crow),
                new DeductionAnswer(DeductionSlotId.WhereHeWent, pigeon)
            });

            Assert.That(solution.RequiredSlots, Has.Length.EqualTo(2));
            Assert.That(solution.IsRequiredAnswer(DeductionSlotId.LastFeedingDay, crow), Is.True);
            Assert.That(solution.IsRequiredAnswer(DeductionSlotId.WhereHeWent, crow), Is.False);
        }

        private static FeatherDefinition CreateFeather(string id, BirdSpecies species, DeductionSlotId slot)
        {
            var feather = ScriptableObject.CreateInstance<FeatherDefinition>();
            feather.ConfigureForTests(id, id, species, ClueRole.MainMystery, new[] { slot }, new Color[0], new Vector3[0], id);
            return feather;
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run:

```powershell
powershell -ExecutionPolicy Bypass -File tools/run-unity-tests.ps1 -TestPlatform editmode
```

Expected with Unity Editor installed: FAIL with compile errors for missing `FeatherDefinition`, `BirdSpecies`, `ClueRole`, `DeductionSlotId`, `DeductionAnswer`, and `DeductionSolution`.

- [ ] **Step 3: Add data model implementation**

Create `Assets/FeatherDetective/Scripts/Data/BirdSpecies.cs` with:

```csharp
namespace FeatherDetective
{
    public enum BirdSpecies
    {
        Crow,
        Magpie,
        Pigeon,
        Woodpecker,
        Sparrow
    }
}
```

Create `Assets/FeatherDetective/Scripts/Data/ClueRole.cs` with:

```csharp
namespace FeatherDetective
{
    public enum ClueRole
    {
        MainMystery,
        Atmosphere
    }
}
```

Create `Assets/FeatherDetective/Scripts/Data/DeductionSlotId.cs` with:

```csharp
namespace FeatherDetective
{
    public enum DeductionSlotId
    {
        LastFeedingDay,
        WhatChanged,
        WhereHeWent,
        WhyBirdsWaited,
        WhatEvidenceMeans
    }
}
```

Create `Assets/FeatherDetective/Scripts/Data/DeductionAnswer.cs` with:

```csharp
using System;
using UnityEngine;

namespace FeatherDetective
{
    [Serializable]
    public struct DeductionAnswer
    {
        [SerializeField] private DeductionSlotId slotId;
        [SerializeField] private FeatherDefinition feather;

        public DeductionAnswer(DeductionSlotId slotId, FeatherDefinition feather)
        {
            this.slotId = slotId;
            this.feather = feather;
        }

        public DeductionSlotId SlotId => slotId;
        public FeatherDefinition Feather => feather;
    }
}
```

Create `Assets/FeatherDetective/Scripts/Data/FeatherDefinition.cs` with:

```csharp
using System;
using UnityEngine;

namespace FeatherDetective
{
    [CreateAssetMenu(menuName = "Feather Detective/Feather Definition")]
    public sealed class FeatherDefinition : ScriptableObject
    {
        [SerializeField] private string id = string.Empty;
        [SerializeField] private string displayName = string.Empty;
        [SerializeField] private BirdSpecies species;
        [SerializeField] private ClueRole role;
        [SerializeField] private DeductionSlotId[] compatibleSlots = Array.Empty<DeductionSlotId>();
        [SerializeField] private Color[] highlightColors = Array.Empty<Color>();
        [SerializeField] private Vector3[] routePoints = Array.Empty<Vector3>();
        [SerializeField] private string memorySummary = string.Empty;

        public string Id => id;
        public string DisplayName => displayName;
        public BirdSpecies Species => species;
        public ClueRole Role => role;
        public DeductionSlotId[] CompatibleSlots => compatibleSlots;
        public Color[] HighlightColors => highlightColors;
        public Vector3[] RoutePoints => routePoints;
        public string MemorySummary => memorySummary;

        public bool CanPlaceInSlot(DeductionSlotId slotId)
        {
            return Array.IndexOf(compatibleSlots, slotId) >= 0;
        }

        public void ConfigureForTests(
            string newId,
            string newDisplayName,
            BirdSpecies newSpecies,
            ClueRole newRole,
            DeductionSlotId[] newCompatibleSlots,
            Color[] newHighlightColors,
            Vector3[] newRoutePoints,
            string newMemorySummary)
        {
            id = newId;
            displayName = newDisplayName;
            species = newSpecies;
            role = newRole;
            compatibleSlots = newCompatibleSlots ?? Array.Empty<DeductionSlotId>();
            highlightColors = newHighlightColors ?? Array.Empty<Color>();
            routePoints = newRoutePoints ?? Array.Empty<Vector3>();
            memorySummary = newMemorySummary;
        }
    }
}
```

Create `Assets/FeatherDetective/Scripts/Data/DeductionSolution.cs` with:

```csharp
using System;
using UnityEngine;

namespace FeatherDetective
{
    [CreateAssetMenu(menuName = "Feather Detective/Deduction Solution")]
    public sealed class DeductionSolution : ScriptableObject
    {
        [SerializeField] private DeductionAnswer[] requiredAnswers = Array.Empty<DeductionAnswer>();

        public DeductionAnswer[] RequiredSlots => requiredAnswers;

        public bool IsRequiredAnswer(DeductionSlotId slotId, FeatherDefinition feather)
        {
            if (feather == null)
            {
                return false;
            }

            foreach (var answer in requiredAnswers)
            {
                if (answer.SlotId == slotId && answer.Feather != null && answer.Feather.Id == feather.Id)
                {
                    return true;
                }
            }

            return false;
        }

        public void ConfigureForTests(DeductionAnswer[] answers)
        {
            requiredAnswers = answers ?? Array.Empty<DeductionAnswer>();
        }
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run:

```powershell
powershell -ExecutionPolicy Bypass -File tools/run-unity-tests.ps1 -TestPlatform editmode
```

Expected: PASS for `FeatherDefinitionTests`.

- [ ] **Step 5: Commit and push**

```powershell
git add Assets/FeatherDetective/Scripts/Data Assets/FeatherDetective/Tests/EditMode/FeatherDefinitionTests.cs
git commit -m "feat: add feather data model"
git push
```

---

### Task 3: Investigation State

**Files:**
- Create: `Assets/FeatherDetective/Tests/EditMode/InvestigationStateTests.cs`
- Create: `Assets/FeatherDetective/Scripts/State/InvestigationState.cs`

- [ ] **Step 1: Write failing investigation state tests**

Create `Assets/FeatherDetective/Tests/EditMode/InvestigationStateTests.cs` with:

```csharp
using NUnit.Framework;
using UnityEngine;

namespace FeatherDetective.Tests
{
    public sealed class InvestigationStateTests
    {
        [Test]
        public void CollectStoresEachFeatherOnlyOnce()
        {
            var state = new InvestigationState();
            var feather = CreateFeather("crow-bench", BirdSpecies.Crow, DeductionSlotId.LastFeedingDay);

            Assert.That(state.Collect(feather), Is.True);
            Assert.That(state.Collect(feather), Is.False);
            Assert.That(state.HasCollected(feather), Is.True);
            Assert.That(state.CollectedCount, Is.EqualTo(1));
        }

        [Test]
        public void TryPlaceRequiresCollectedCompatibleFeather()
        {
            var state = new InvestigationState();
            var feather = CreateFeather("pigeon-route", BirdSpecies.Pigeon, DeductionSlotId.WhereHeWent);

            Assert.That(state.TryPlace(feather, DeductionSlotId.WhereHeWent), Is.False);

            state.Collect(feather);

            Assert.That(state.TryPlace(feather, DeductionSlotId.LastFeedingDay), Is.False);
            Assert.That(state.TryPlace(feather, DeductionSlotId.WhereHeWent), Is.True);
            Assert.That(state.GetPlacedFeatherId(DeductionSlotId.WhereHeWent), Is.EqualTo("pigeon-route"));
        }

        [Test]
        public void IsSolvedRequiresEverySolutionSlotToMatch()
        {
            var crow = CreateFeather("crow-bench", BirdSpecies.Crow, DeductionSlotId.LastFeedingDay);
            var pigeon = CreateFeather("pigeon-edge", BirdSpecies.Pigeon, DeductionSlotId.WhereHeWent);
            var solution = ScriptableObject.CreateInstance<DeductionSolution>();
            solution.ConfigureForTests(new[]
            {
                new DeductionAnswer(DeductionSlotId.LastFeedingDay, crow),
                new DeductionAnswer(DeductionSlotId.WhereHeWent, pigeon)
            });

            var state = new InvestigationState();
            state.Collect(crow);
            state.Collect(pigeon);
            state.TryPlace(crow, DeductionSlotId.LastFeedingDay);

            Assert.That(state.IsSolved(solution), Is.False);

            state.TryPlace(pigeon, DeductionSlotId.WhereHeWent);

            Assert.That(state.IsSolved(solution), Is.True);
        }

        private static FeatherDefinition CreateFeather(string id, BirdSpecies species, DeductionSlotId slot)
        {
            var feather = ScriptableObject.CreateInstance<FeatherDefinition>();
            feather.ConfigureForTests(id, id, species, ClueRole.MainMystery, new[] { slot }, new Color[0], new Vector3[0], id);
            return feather;
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run:

```powershell
powershell -ExecutionPolicy Bypass -File tools/run-unity-tests.ps1 -TestPlatform editmode
```

Expected with Unity Editor installed: FAIL with compile errors for missing `InvestigationState`.

- [ ] **Step 3: Add investigation state**

Create `Assets/FeatherDetective/Scripts/State/InvestigationState.cs` with:

```csharp
using System.Collections.Generic;

namespace FeatherDetective
{
    public sealed class InvestigationState
    {
        private readonly HashSet<string> collectedIds = new HashSet<string>();
        private readonly Dictionary<DeductionSlotId, string> placements = new Dictionary<DeductionSlotId, string>();

        public int CollectedCount => collectedIds.Count;
        public IReadOnlyDictionary<DeductionSlotId, string> Placements => placements;

        public bool Collect(FeatherDefinition feather)
        {
            if (feather == null || string.IsNullOrWhiteSpace(feather.Id))
            {
                return false;
            }

            return collectedIds.Add(feather.Id);
        }

        public bool HasCollected(FeatherDefinition feather)
        {
            return feather != null && collectedIds.Contains(feather.Id);
        }

        public bool TryPlace(FeatherDefinition feather, DeductionSlotId slotId)
        {
            if (!HasCollected(feather) || !feather.CanPlaceInSlot(slotId))
            {
                return false;
            }

            placements[slotId] = feather.Id;
            return true;
        }

        public string GetPlacedFeatherId(DeductionSlotId slotId)
        {
            return placements.TryGetValue(slotId, out var featherId) ? featherId : string.Empty;
        }

        public bool IsSolved(DeductionSolution solution)
        {
            if (solution == null || solution.RequiredSlots.Length == 0)
            {
                return false;
            }

            foreach (var answer in solution.RequiredSlots)
            {
                if (answer.Feather == null)
                {
                    return false;
                }

                if (!placements.TryGetValue(answer.SlotId, out var placedId) || placedId != answer.Feather.Id)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run:

```powershell
powershell -ExecutionPolicy Bypass -File tools/run-unity-tests.ps1 -TestPlatform editmode
```

Expected: PASS for `InvestigationStateTests` and existing feather tests.

- [ ] **Step 5: Commit and push**

```powershell
git add Assets/FeatherDetective/Scripts/State/InvestigationState.cs Assets/FeatherDetective/Tests/EditMode/InvestigationStateTests.cs
git commit -m "feat: track feather investigation state"
git push
```

---

### Task 4: Perch Selection And Bird Movement

**Files:**
- Create: `Assets/FeatherDetective/Tests/EditMode/PerchSelectorTests.cs`
- Create: `Assets/FeatherDetective/Scripts/Interaction/IInspectable.cs`
- Create: `Assets/FeatherDetective/Scripts/UI/InteractionPrompt.cs`
- Create: `Assets/FeatherDetective/Scripts/Movement/PerchNode.cs`
- Create: `Assets/FeatherDetective/Scripts/Movement/PerchSelector.cs`
- Create: `Assets/FeatherDetective/Scripts/Movement/BirdPlayerController.cs`
- Create: `Assets/FeatherDetective/Scripts/Camera/FixedThirdPersonCamera.cs`

- [ ] **Step 1: Write failing perch selection tests**

Create `Assets/FeatherDetective/Tests/EditMode/PerchSelectorTests.cs` with:

```csharp
using NUnit.Framework;
using UnityEngine;

namespace FeatherDetective.Tests
{
    public sealed class PerchSelectorTests
    {
        [Test]
        public void SelectBestReachablePerchPrefersForwardLinkedPerch()
        {
            var originObject = new GameObject("origin");
            var forwardObject = new GameObject("forward");
            var sideObject = new GameObject("side");

            var origin = originObject.AddComponent<PerchNode>();
            var forward = forwardObject.AddComponent<PerchNode>();
            var side = sideObject.AddComponent<PerchNode>();

            originObject.transform.position = Vector3.zero;
            forwardObject.transform.position = new Vector3(0f, 0f, 4f);
            sideObject.transform.position = new Vector3(4f, 0f, 0f);
            origin.ConfigureForTests(new[] { forward, side });

            var selected = PerchSelector.SelectBestReachable(origin, Vector3.zero, Vector3.forward, 6f, 0.25f);

            Assert.That(selected, Is.EqualTo(forward));

            Object.DestroyImmediate(originObject);
            Object.DestroyImmediate(forwardObject);
            Object.DestroyImmediate(sideObject);
        }

        [Test]
        public void SelectBestReachablePerchReturnsNullWhenNoLinkedPerchMatchesDistance()
        {
            var originObject = new GameObject("origin");
            var farObject = new GameObject("far");

            var origin = originObject.AddComponent<PerchNode>();
            var far = farObject.AddComponent<PerchNode>();

            farObject.transform.position = new Vector3(0f, 0f, 20f);
            origin.ConfigureForTests(new[] { far });

            var selected = PerchSelector.SelectBestReachable(origin, Vector3.zero, Vector3.forward, 6f, 0.25f);

            Assert.That(selected, Is.Null);

            Object.DestroyImmediate(originObject);
            Object.DestroyImmediate(farObject);
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run:

```powershell
powershell -ExecutionPolicy Bypass -File tools/run-unity-tests.ps1 -TestPlatform editmode
```

Expected with Unity Editor installed: FAIL with compile errors for missing `PerchNode` and `PerchSelector`.

- [ ] **Step 3: Add perch and movement code**

Create `Assets/FeatherDetective/Scripts/Interaction/IInspectable.cs` with:

```csharp
namespace FeatherDetective
{
    public interface IInspectable
    {
        string PromptText { get; }
        void Inspect();
    }
}
```

Create `Assets/FeatherDetective/Scripts/UI/InteractionPrompt.cs` with:

```csharp
using UnityEngine;
using UnityEngine.UI;

namespace FeatherDetective
{
    public sealed class InteractionPrompt : MonoBehaviour
    {
        [SerializeField] private Text label;

        public void ConfigureForBuilder(Text newLabel)
        {
            label = newLabel;
            Hide();
        }

        public void Show(string text)
        {
            if (label == null)
            {
                return;
            }

            label.text = text;
            label.enabled = true;
        }

        public void Hide()
        {
            if (label != null)
            {
                label.enabled = false;
            }
        }
    }
}
```

Create `Assets/FeatherDetective/Scripts/Movement/PerchNode.cs` with:

```csharp
using System.Collections.Generic;
using UnityEngine;

namespace FeatherDetective
{
    public sealed class PerchNode : MonoBehaviour
    {
        [SerializeField] private Transform landingPoint;
        [SerializeField] private List<PerchNode> linkedPerches = new List<PerchNode>();

        public Vector3 LandingPosition => landingPoint != null ? landingPoint.position : transform.position;
        public IReadOnlyList<PerchNode> LinkedPerches => linkedPerches;

        public void ConfigureForTests(IEnumerable<PerchNode> links)
        {
            linkedPerches.Clear();
            linkedPerches.AddRange(links);
        }
    }
}
```

Create `Assets/FeatherDetective/Scripts/Movement/PerchSelector.cs` with:

```csharp
using System.Collections.Generic;
using UnityEngine;

namespace FeatherDetective
{
    public static class PerchSelector
    {
        public static PerchNode SelectBestReachable(PerchNode current, Vector3 birdPosition, Vector3 forward, float maxDistance, float minForwardDot)
        {
            if (current == null)
            {
                return null;
            }

            var bestScore = float.NegativeInfinity;
            PerchNode best = null;

            foreach (var perch in current.LinkedPerches)
            {
                if (perch == null)
                {
                    continue;
                }

                var offset = perch.LandingPosition - birdPosition;
                var distance = offset.magnitude;
                if (distance > maxDistance || distance <= 0.01f)
                {
                    continue;
                }

                var direction = offset.normalized;
                var dot = Vector3.Dot(forward.normalized, direction);
                if (dot < minForwardDot)
                {
                    continue;
                }

                var score = dot * 2f - distance * 0.05f;
                if (score > bestScore)
                {
                    bestScore = score;
                    best = perch;
                }
            }

            return best;
        }
    }
}
```

Create `Assets/FeatherDetective/Scripts/Movement/BirdPlayerController.cs` with:

```csharp
using System.Collections;
using UnityEngine;

namespace FeatherDetective
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class BirdPlayerController : MonoBehaviour
    {
        [SerializeField] private float walkSpeed = 3.5f;
        [SerializeField] private float hopSpeed = 4.8f;
        [SerializeField] private float gravity = -14f;
        [SerializeField] private float glideDuration = 0.8f;
        [SerializeField] private float glideRange = 7f;
        [SerializeField] private Transform visualRoot;
        [SerializeField] private InteractionPrompt prompt;

        private CharacterController controller;
        private Vector3 velocity;
        private PerchNode currentPerch;
        private IInspectable focusedInspectable;
        private bool gliding;

        public void ConfigureForBuilder(InteractionPrompt newPrompt, PerchNode startPerch)
        {
            prompt = newPrompt;
            currentPerch = startPerch;
            if (startPerch != null)
            {
                transform.position = startPerch.LandingPosition;
            }
        }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (gliding)
            {
                return;
            }

            var input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
            if (input.sqrMagnitude > 1f)
            {
                input.Normalize();
            }

            var move = input * walkSpeed;
            controller.Move(move * Time.deltaTime);

            if (input.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(input, Vector3.up);
            }

            if (controller.isGrounded && velocity.y < 0f)
            {
                velocity.y = -1f;
            }

            if (controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                velocity.y = hopSpeed;
            }

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.E) && focusedInspectable != null)
            {
                focusedInspectable.Inspect();
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                var target = PerchSelector.SelectBestReachable(currentPerch, transform.position, transform.forward, glideRange, 0.1f);
                if (target != null)
                {
                    StartCoroutine(GlideTo(target));
                }
            }
        }

        private IEnumerator GlideTo(PerchNode target)
        {
            gliding = true;
            var start = transform.position;
            var end = target.LandingPosition;
            var elapsed = 0f;

            while (elapsed < glideDuration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / glideDuration);
                var arc = Mathf.Sin(t * Mathf.PI) * 0.8f;
                transform.position = Vector3.Lerp(start, end, t) + Vector3.up * arc;
                yield return null;
            }

            transform.position = end;
            currentPerch = target;
            velocity = Vector3.zero;
            gliding = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PerchNode perch))
            {
                currentPerch = perch;
            }

            if (other.TryGetComponent(out IInspectable inspectable))
            {
                focusedInspectable = inspectable;
                prompt?.Show(inspectable.PromptText);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out IInspectable inspectable) && ReferenceEquals(inspectable, focusedInspectable))
            {
                focusedInspectable = null;
                prompt?.Hide();
            }
        }
    }
}
```

Create `Assets/FeatherDetective/Scripts/Camera/FixedThirdPersonCamera.cs` with:

```csharp
using UnityEngine;

namespace FeatherDetective
{
    public sealed class FixedThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 8f, -9f);
        [SerializeField] private float followSharpness = 8f;

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            var desired = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desired, 1f - Mathf.Exp(-followSharpness * Time.deltaTime));
            transform.rotation = Quaternion.Euler(48f, 0f, 0f);
        }
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run:

```powershell
powershell -ExecutionPolicy Bypass -File tools/run-unity-tests.ps1 -TestPlatform editmode
```

Expected: PASS for `PerchSelectorTests` and previous EditMode tests.

- [ ] **Step 5: Commit and push**

```powershell
git add Assets/FeatherDetective/Scripts/Movement Assets/FeatherDetective/Scripts/Camera Assets/FeatherDetective/Scripts/Interaction/IInspectable.cs Assets/FeatherDetective/Scripts/UI/InteractionPrompt.cs Assets/FeatherDetective/Tests/EditMode/PerchSelectorTests.cs
git commit -m "feat: add bird movement and perch selection"
git push
```

---

### Task 5: Memory Playback System

**Files:**
- Create: `Assets/FeatherDetective/Tests/EditMode/MemoryPlaybackControllerTests.cs`
- Create: `Assets/FeatherDetective/Scripts/Memory/IMemoryEffect.cs`
- Create: `Assets/FeatherDetective/Scripts/Memory/MemoryContext.cs`
- Create: `Assets/FeatherDetective/Scripts/Memory/MemoryPlaybackController.cs`
- Create: `Assets/FeatherDetective/Scripts/Memory/CrowMemoryEffect.cs`
- Create: `Assets/FeatherDetective/Scripts/Memory/MagpieMemoryEffect.cs`
- Create: `Assets/FeatherDetective/Scripts/Memory/PigeonMemoryEffect.cs`
- Create: `Assets/FeatherDetective/Scripts/Memory/WoodpeckerMemoryEffect.cs`
- Create: `Assets/FeatherDetective/Scripts/Memory/SparrowMemoryEffect.cs`
- Create: `Assets/FeatherDetective/Scripts/World/ColorMemoryTarget.cs`
- Create: `Assets/FeatherDetective/Scripts/World/RouteLineRenderer.cs`
- Create: `Assets/FeatherDetective/Scripts/World/FirstPersonMemoryRig.cs`
- Create: `Assets/FeatherDetective/Scripts/World/AtmosphereController.cs`
- Create: `Assets/FeatherDetective/Scripts/World/ProceduralAudio.cs`

- [ ] **Step 1: Write failing memory dispatch tests**

Create `Assets/FeatherDetective/Tests/EditMode/MemoryPlaybackControllerTests.cs` with:

```csharp
using System.Collections;
using NUnit.Framework;
using UnityEngine;

namespace FeatherDetective.Tests
{
    public sealed class MemoryPlaybackControllerTests
    {
        [Test]
        public void SelectEffectReturnsEffectMatchingFeatherSpecies()
        {
            var crow = new FakeMemoryEffect(BirdSpecies.Crow);
            var magpie = new FakeMemoryEffect(BirdSpecies.Magpie);

            var selected = MemoryPlaybackController.SelectEffectForTests(new IMemoryEffect[] { crow, magpie }, BirdSpecies.Magpie);

            Assert.That(selected, Is.EqualTo(magpie));
        }

        [Test]
        public void SelectEffectReturnsNullWhenSpeciesIsMissing()
        {
            var crow = new FakeMemoryEffect(BirdSpecies.Crow);

            var selected = MemoryPlaybackController.SelectEffectForTests(new IMemoryEffect[] { crow }, BirdSpecies.Sparrow);

            Assert.That(selected, Is.Null);
        }

        private sealed class FakeMemoryEffect : IMemoryEffect
        {
            public FakeMemoryEffect(BirdSpecies species)
            {
                Species = species;
            }

            public BirdSpecies Species { get; }

            public IEnumerator Play(FeatherDefinition feather, MemoryContext context)
            {
                yield break;
            }
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run:

```powershell
powershell -ExecutionPolicy Bypass -File tools/run-unity-tests.ps1 -TestPlatform editmode
```

Expected with Unity Editor installed: FAIL with compile errors for missing `IMemoryEffect`, `MemoryContext`, and `MemoryPlaybackController`.

- [ ] **Step 3: Add memory system implementation**

Create `Assets/FeatherDetective/Scripts/Memory/IMemoryEffect.cs` with:

```csharp
using System.Collections;

namespace FeatherDetective
{
    public interface IMemoryEffect
    {
        BirdSpecies Species { get; }
        IEnumerator Play(FeatherDefinition feather, MemoryContext context);
    }
}
```

Create `Assets/FeatherDetective/Scripts/Memory/MemoryContext.cs` with:

```csharp
using UnityEngine;
using UnityEngine.UI;

namespace FeatherDetective
{
    public sealed class MemoryContext : MonoBehaviour
    {
        [SerializeField] private CanvasGroup darkOverlay;
        [SerializeField] private AudioSource memoryAudioSource;
        [SerializeField] private ColorMemoryTarget[] colorTargets;
        [SerializeField] private RouteLineRenderer routeLineRenderer;
        [SerializeField] private FirstPersonMemoryRig firstPersonRig;
        [SerializeField] private AtmosphereController atmosphereController;

        public CanvasGroup DarkOverlay => darkOverlay;
        public AudioSource MemoryAudioSource => memoryAudioSource;
        public ColorMemoryTarget[] ColorTargets => colorTargets;
        public RouteLineRenderer RouteLineRenderer => routeLineRenderer;
        public FirstPersonMemoryRig FirstPersonRig => firstPersonRig;
        public AtmosphereController AtmosphereController => atmosphereController;

        public void ConfigureForBuilder(CanvasGroup overlay, AudioSource audioSource, ColorMemoryTarget[] targets, RouteLineRenderer routes, FirstPersonMemoryRig rig, AtmosphereController atmosphere)
        {
            darkOverlay = overlay;
            memoryAudioSource = audioSource;
            colorTargets = targets;
            routeLineRenderer = routes;
            firstPersonRig = rig;
            atmosphereController = atmosphere;
        }
    }
}
```

Create `Assets/FeatherDetective/Scripts/Memory/MemoryPlaybackController.cs` with:

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FeatherDetective
{
    public sealed class MemoryPlaybackController : MonoBehaviour
    {
        [SerializeField] private MemoryContext context;
        private readonly List<IMemoryEffect> effects = new List<IMemoryEffect>();
        private bool playing;

        public void ConfigureForBuilder(MemoryContext newContext)
        {
            context = newContext;
        }

        private void Awake()
        {
            effects.Clear();
            foreach (var behaviour in GetComponents<MonoBehaviour>())
            {
                if (behaviour is IMemoryEffect effect)
                {
                    effects.Add(effect);
                }
            }
        }

        public void PlayMemory(FeatherDefinition feather)
        {
            if (playing || feather == null)
            {
                return;
            }

            var effect = SelectEffect(effects, feather.Species);
            if (effect != null)
            {
                StartCoroutine(PlayRoutine(effect, feather));
            }
        }

        private IEnumerator PlayRoutine(IMemoryEffect effect, FeatherDefinition feather)
        {
            playing = true;
            yield return effect.Play(feather, context);
            playing = false;
        }

        private static IMemoryEffect SelectEffect(IEnumerable<IMemoryEffect> candidates, BirdSpecies species)
        {
            foreach (var effect in candidates)
            {
                if (effect != null && effect.Species == species)
                {
                    return effect;
                }
            }

            return null;
        }

        public static IMemoryEffect SelectEffectForTests(IEnumerable<IMemoryEffect> candidates, BirdSpecies species)
        {
            return SelectEffect(candidates, species);
        }
    }
}
```

Create `Assets/FeatherDetective/Scripts/World/ColorMemoryTarget.cs` with:

```csharp
using UnityEngine;

namespace FeatherDetective
{
    [RequireComponent(typeof(Renderer))]
    public sealed class ColorMemoryTarget : MonoBehaviour
    {
        [SerializeField] private Color memoryColor = Color.white;
        private Renderer cachedRenderer;
        private Color originalColor;

        public Color MemoryColor => memoryColor;

        private void Awake()
        {
            cachedRenderer = GetComponent<Renderer>();
            originalColor = cachedRenderer.material.color;
        }

        public void ApplyMagpieState(Color[] importantColors)
        {
            var important = false;
            foreach (var color in importantColors)
            {
                if (Vector4.Distance(color, memoryColor) < 0.08f)
                {
                    important = true;
                    break;
                }
            }

            var gray = originalColor.grayscale;
            var grayscaleColor = new Color(gray, gray, gray, originalColor.a);
            cachedRenderer.material.color = important ? originalColor : Color.Lerp(grayscaleColor, originalColor, 0.12f);
        }

        public void Restore()
        {
            cachedRenderer.material.color = originalColor;
        }
    }
}
```

Create `Assets/FeatherDetective/Scripts/World/RouteLineRenderer.cs` with:

```csharp
using System.Collections;
using UnityEngine;

namespace FeatherDetective
{
    [RequireComponent(typeof(LineRenderer))]
    public sealed class RouteLineRenderer : MonoBehaviour
    {
        private LineRenderer lineRenderer;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.enabled = false;
        }

        public IEnumerator AnimateRoute(Vector3[] points, float duration)
        {
            if (points == null || points.Length < 2)
            {
                yield break;
            }

            lineRenderer.enabled = true;
            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);

            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                lineRenderer.widthMultiplier = Mathf.Lerp(0.02f, 0.14f, t);
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);
            lineRenderer.enabled = false;
        }
    }
}
```

Create `Assets/FeatherDetective/Scripts/World/FirstPersonMemoryRig.cs` with:

```csharp
using System.Collections;
using UnityEngine;

namespace FeatherDetective
{
    public sealed class FirstPersonMemoryRig : MonoBehaviour
    {
        [SerializeField] private Camera memoryCamera;
        [SerializeField] private AudioSource impactAudioSource;

        public void ConfigureForBuilder(Camera newMemoryCamera, AudioSource newImpactAudioSource)
        {
            memoryCamera = newMemoryCamera;
            impactAudioSource = newImpactAudioSource;
            if (memoryCamera != null)
            {
                memoryCamera.enabled = false;
            }
        }

        public IEnumerator PlayRhythm(float duration)
        {
            if (memoryCamera != null)
            {
                memoryCamera.enabled = true;
            }

            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var pulse = Mathf.Abs(Mathf.Sin(elapsed * 11f));
                transform.localPosition = new Vector3(0f, pulse * 0.08f, 0f);
                if (impactAudioSource != null && pulse > 0.98f && !impactAudioSource.isPlaying)
                {
                    impactAudioSource.Play();
                }

                yield return null;
            }

            transform.localPosition = Vector3.zero;
            if (memoryCamera != null)
            {
                memoryCamera.enabled = false;
            }
        }
    }
}
```

Create `Assets/FeatherDetective/Scripts/World/ProceduralAudio.cs` with:

```csharp
using UnityEngine;

namespace FeatherDetective
{
    public static class ProceduralAudio
    {
        public static AudioClip CreateTone(string name, float frequency, float duration, float volume)
        {
            const int sampleRate = 44100;
            var sampleCount = Mathf.CeilToInt(sampleRate * duration);
            var samples = new float[sampleCount];

            for (var i = 0; i < sampleCount; i++)
            {
                var t = i / (float)sampleRate;
                samples[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * volume;
            }

            var clip = AudioClip.Create(name, sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        public static AudioClip CreateSoftNoise(string name, float duration, float volume)
        {
            const int sampleRate = 44100;
            var sampleCount = Mathf.CeilToInt(sampleRate * duration);
            var samples = new float[sampleCount];
            var seed = 17;

            for (var i = 0; i < sampleCount; i++)
            {
                seed = seed * 1103515245 + 12345;
                var normalized = ((seed / 65536) % 32768) / 32768f;
                samples[i] = (normalized * 2f - 1f) * volume;
            }

            var clip = AudioClip.Create(name, sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}
```

Create `Assets/FeatherDetective/Scripts/World/AtmosphereController.cs` with:

```csharp
using System.Collections;
using UnityEngine;

namespace FeatherDetective
{
    public sealed class AtmosphereController : MonoBehaviour
    {
        [SerializeField] private AudioSource windSource;
        [SerializeField] private AudioSource insectSource;
        [SerializeField] private Transform flockRoot;

        public void ConfigureForBuilder(AudioSource newWindSource, AudioSource newInsectSource, Transform newFlockRoot)
        {
            windSource = newWindSource;
            insectSource = newInsectSource;
            flockRoot = newFlockRoot;
            if (windSource != null)
            {
                windSource.clip = ProceduralAudio.CreateSoftNoise("SoftWind", 1f, 0.05f);
                windSource.loop = true;
                windSource.Play();
            }

            if (insectSource != null)
            {
                insectSource.clip = ProceduralAudio.CreateTone("SoftInsects", 880f, 0.25f, 0.02f);
                insectSource.loop = true;
                insectSource.Play();
            }
        }

        public IEnumerator ShiftSparrowAtmosphere(float duration)
        {
            var startWind = windSource != null ? windSource.volume : 0f;
            var startInsects = insectSource != null ? insectSource.volume : 0f;
            var elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Sin(Mathf.Clamp01(elapsed / duration) * Mathf.PI);

                if (windSource != null)
                {
                    windSource.volume = Mathf.Lerp(startWind, 0.85f, t);
                }

                if (insectSource != null)
                {
                    insectSource.volume = Mathf.Lerp(startInsects, 0.1f, t);
                }

                if (flockRoot != null)
                {
                    flockRoot.localScale = Vector3.one * Mathf.Lerp(1f, 1.18f, t);
                }

                yield return null;
            }

            if (windSource != null)
            {
                windSource.volume = startWind;
            }

            if (insectSource != null)
            {
                insectSource.volume = startInsects;
            }

            if (flockRoot != null)
            {
                flockRoot.localScale = Vector3.one;
            }
        }
    }
}
```

Create the five memory effect files with this pattern:

```csharp
using System.Collections;
using UnityEngine;

namespace FeatherDetective
{
    public sealed class CrowMemoryEffect : MonoBehaviour, IMemoryEffect
    {
        [SerializeField] private float duration = 3f;
        public BirdSpecies Species => BirdSpecies.Crow;

        public IEnumerator Play(FeatherDefinition feather, MemoryContext context)
        {
            if (context?.DarkOverlay != null)
            {
                context.DarkOverlay.alpha = 1f;
            }

            if (context?.MemoryAudioSource != null)
            {
                if (context.MemoryAudioSource.clip == null)
                {
                    context.MemoryAudioSource.clip = ProceduralAudio.CreateTone("CrowMemorySirenTone", 420f, duration, 0.08f);
                }

                context.MemoryAudioSource.panStereo = -0.35f;
                context.MemoryAudioSource.Play();
            }

            yield return new WaitForSeconds(duration);

            if (context?.DarkOverlay != null)
            {
                context.DarkOverlay.alpha = 0f;
            }
        }
    }
}
```

`MagpieMemoryEffect`:

```csharp
using System.Collections;
using UnityEngine;

namespace FeatherDetective
{
    public sealed class MagpieMemoryEffect : MonoBehaviour, IMemoryEffect
    {
        [SerializeField] private float duration = 3f;
        public BirdSpecies Species => BirdSpecies.Magpie;

        public IEnumerator Play(FeatherDefinition feather, MemoryContext context)
        {
            if (context?.ColorTargets != null)
            {
                foreach (var target in context.ColorTargets)
                {
                    target?.ApplyMagpieState(feather.HighlightColors);
                }
            }

            yield return new WaitForSeconds(duration);

            if (context?.ColorTargets != null)
            {
                foreach (var target in context.ColorTargets)
                {
                    target?.Restore();
                }
            }
        }
    }
}
```

`PigeonMemoryEffect`:

```csharp
using System.Collections;
using UnityEngine;

namespace FeatherDetective
{
    public sealed class PigeonMemoryEffect : MonoBehaviour, IMemoryEffect
    {
        [SerializeField] private float duration = 3f;
        public BirdSpecies Species => BirdSpecies.Pigeon;

        public IEnumerator Play(FeatherDefinition feather, MemoryContext context)
        {
            if (context?.RouteLineRenderer != null)
            {
                yield return context.RouteLineRenderer.AnimateRoute(feather.RoutePoints, duration);
            }
        }
    }
}
```

`WoodpeckerMemoryEffect`:

```csharp
using System.Collections;
using UnityEngine;

namespace FeatherDetective
{
    public sealed class WoodpeckerMemoryEffect : MonoBehaviour, IMemoryEffect
    {
        [SerializeField] private float duration = 2.5f;
        public BirdSpecies Species => BirdSpecies.Woodpecker;

        public IEnumerator Play(FeatherDefinition feather, MemoryContext context)
        {
            if (context?.FirstPersonRig != null)
            {
                yield return context.FirstPersonRig.PlayRhythm(duration);
            }
        }
    }
}
```

`SparrowMemoryEffect`:

```csharp
using System.Collections;
using UnityEngine;

namespace FeatherDetective
{
    public sealed class SparrowMemoryEffect : MonoBehaviour, IMemoryEffect
    {
        [SerializeField] private float duration = 4f;
        public BirdSpecies Species => BirdSpecies.Sparrow;

        public IEnumerator Play(FeatherDefinition feather, MemoryContext context)
        {
            if (context?.AtmosphereController != null)
            {
                yield return context.AtmosphereController.ShiftSparrowAtmosphere(duration);
            }
        }
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run:

```powershell
powershell -ExecutionPolicy Bypass -File tools/run-unity-tests.ps1 -TestPlatform editmode
```

Expected: PASS for `MemoryPlaybackControllerTests` and previous EditMode tests.

- [ ] **Step 5: Commit and push**

```powershell
git add Assets/FeatherDetective/Scripts/Memory Assets/FeatherDetective/Scripts/World Assets/FeatherDetective/Tests/EditMode/MemoryPlaybackControllerTests.cs
git commit -m "feat: add bird memory playback effects"
git push
```

---

### Task 6: Runtime Interaction And Deduction Board

**Files:**
- Create: `Assets/FeatherDetective/Tests/EditMode/DeductionBoardTests.cs`
- Create: `Assets/FeatherDetective/Scripts/Interaction/FeatherPickup.cs`
- Create: `Assets/FeatherDetective/Scripts/Runtime/InvestigationRuntime.cs`
- Create: `Assets/FeatherDetective/Scripts/Deduction/DeductionBoard.cs`
- Create: `Assets/FeatherDetective/Scripts/Deduction/DeductionSlot.cs`
- Create: `Assets/FeatherDetective/Scripts/Deduction/EndingController.cs`
- Create: `Assets/FeatherDetective/Scripts/UI/FeatherInventoryUI.cs`
- Create: `Assets/FeatherDetective/Scripts/UI/EndingMessageUI.cs`

- [ ] **Step 1: Extend investigation tests for solved board behavior**

Update `Assets/FeatherDetective/Tests/EditMode/InvestigationStateTests.cs` with this additional test:

```csharp
[Test]
public void IsSolvedRejectsAtmosphereFeatherPlacedIntoMainMysterySlot()
{
    var atmosphere = CreateFeather("sparrow-flowers", BirdSpecies.Sparrow, DeductionSlotId.WhatChanged);
    atmosphere.ConfigureForTests("sparrow-flowers", "sparrow-flowers", BirdSpecies.Sparrow, ClueRole.Atmosphere, new[] { DeductionSlotId.WhatChanged }, new Color[0], new Vector3[0], "quiet flowers");

    var required = CreateFeather("sparrow-trees", BirdSpecies.Sparrow, DeductionSlotId.WhatChanged);
    var solution = ScriptableObject.CreateInstance<DeductionSolution>();
    solution.ConfigureForTests(new[] { new DeductionAnswer(DeductionSlotId.WhatChanged, required) });

    var state = new InvestigationState();
    state.Collect(atmosphere);
    state.TryPlace(atmosphere, DeductionSlotId.WhatChanged);

    Assert.That(state.IsSolved(solution), Is.False);
}
```

Create `Assets/FeatherDetective/Tests/EditMode/DeductionBoardTests.cs` with:

```csharp
using NUnit.Framework;
using UnityEngine;

namespace FeatherDetective.Tests
{
    public sealed class DeductionBoardTests
    {
        [Test]
        public void InspectPlacesSelectedFeatherIntoCompatibleSlotAndTriggersEndingWhenSolved()
        {
            var feather = CreateFeather("pigeon-edge", BirdSpecies.Pigeon, DeductionSlotId.WhatEvidenceMeans);
            var solution = ScriptableObject.CreateInstance<DeductionSolution>();
            solution.ConfigureForTests(new[] { new DeductionAnswer(DeductionSlotId.WhatEvidenceMeans, feather) });

            var runtimeObject = new GameObject("runtime");
            var boardObject = new GameObject("board");
            var endingObject = new GameObject("ending");
            var slotObject = new GameObject("slot");

            var runtime = runtimeObject.AddComponent<InvestigationRuntime>();
            var board = boardObject.AddComponent<DeductionBoard>();
            var ending = endingObject.AddComponent<EndingController>();
            var slot = slotObject.AddComponent<DeductionSlot>();
            slot.ConfigureForBuilder(DeductionSlotId.WhatEvidenceMeans);
            board.ConfigureForBuilder(runtime, solution, ending, new[] { slot });

            runtime.CollectFeather(feather);
            board.Inspect();

            Assert.That(runtime.State.GetPlacedFeatherId(DeductionSlotId.WhatEvidenceMeans), Is.EqualTo("pigeon-edge"));
            Assert.That(ending.HasPlayed, Is.True);

            Object.DestroyImmediate(runtimeObject);
            Object.DestroyImmediate(boardObject);
            Object.DestroyImmediate(endingObject);
            Object.DestroyImmediate(slotObject);
        }

        private static FeatherDefinition CreateFeather(string id, BirdSpecies species, DeductionSlotId slot)
        {
            var feather = ScriptableObject.CreateInstance<FeatherDefinition>();
            feather.ConfigureForTests(id, id, species, ClueRole.MainMystery, new[] { slot }, new Color[0], new Vector3[0], id);
            return feather;
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail for missing runtime adapters**

Run:

```powershell
powershell -ExecutionPolicy Bypass -File tools/run-unity-tests.ps1 -TestPlatform editmode
```

Expected with Unity Editor installed: FAIL with compile errors for missing `InvestigationRuntime`, `DeductionBoard`, `EndingController`, and `DeductionSlot`.

- [ ] **Step 3: Add interaction and runtime adapters**

Create `Assets/FeatherDetective/Scripts/Interaction/FeatherPickup.cs` with:

```csharp
using UnityEngine;

namespace FeatherDetective
{
    public sealed class FeatherPickup : MonoBehaviour, IInspectable
    {
        [SerializeField] private FeatherDefinition definition;
        [SerializeField] private InvestigationRuntime runtime;
        [SerializeField] private GameObject collectedVisual;

        public FeatherDefinition Definition => definition;
        public string PromptText => "Inspect";

        public void ConfigureForBuilder(FeatherDefinition newDefinition, InvestigationRuntime newRuntime)
        {
            definition = newDefinition;
            runtime = newRuntime;
        }

        public void Inspect()
        {
            if (definition == null || runtime == null)
            {
                return;
            }

            if (runtime.CollectFeather(definition))
            {
                if (collectedVisual != null)
                {
                    collectedVisual.SetActive(false);
                }
            }
        }
    }
}
```

Create `Assets/FeatherDetective/Scripts/Runtime/InvestigationRuntime.cs` with:

```csharp
using UnityEngine;

namespace FeatherDetective
{
    public sealed class InvestigationRuntime : MonoBehaviour
    {
        [SerializeField] private MemoryPlaybackController memoryPlayback;
        [SerializeField] private FeatherInventoryUI inventoryUI;
        private readonly List<FeatherDefinition> collectedFeathers = new List<FeatherDefinition>();

        public InvestigationState State { get; } = new InvestigationState();
        public FeatherDefinition SelectedFeather { get; private set; }
        public IReadOnlyList<FeatherDefinition> CollectedFeathers => collectedFeathers;

        public void ConfigureForBuilder(MemoryPlaybackController newMemoryPlayback, FeatherInventoryUI newInventoryUI)
        {
            memoryPlayback = newMemoryPlayback;
            inventoryUI = newInventoryUI;
        }

        public bool CollectFeather(FeatherDefinition feather)
        {
            var collected = State.Collect(feather);
            SelectedFeather = feather;
            if (collected)
            {
                collectedFeathers.Add(feather);
            }

            inventoryUI?.Refresh(this);
            memoryPlayback?.PlayMemory(feather);
            return collected;
        }

        public bool TryPlaceSelected(DeductionSlotId slotId)
        {
            if (SelectedFeather == null)
            {
                return false;
            }

            return State.TryPlace(SelectedFeather, slotId);
        }

        public void SelectNextCollectedFeather()
        {
            if (collectedFeathers.Count == 0)
            {
                SelectedFeather = null;
                inventoryUI?.Refresh(this);
                return;
            }

            var currentIndex = collectedFeathers.IndexOf(SelectedFeather);
            var nextIndex = (currentIndex + 1 + collectedFeathers.Count) % collectedFeathers.Count;
            SelectedFeather = collectedFeathers[nextIndex];
            inventoryUI?.Refresh(this);
        }
    }
}
```

Create `Assets/FeatherDetective/Scripts/UI/FeatherInventoryUI.cs` with:

```csharp
using UnityEngine;
using UnityEngine.UI;

namespace FeatherDetective
{
    public sealed class FeatherInventoryUI : MonoBehaviour
    {
        [SerializeField] private Text label;

        public void ConfigureForBuilder(Text newLabel)
        {
            label = newLabel;
        }

        public void Refresh(InvestigationRuntime runtime)
        {
            if (label == null || runtime == null)
            {
                return;
            }

            var selected = runtime.SelectedFeather != null ? runtime.SelectedFeather.DisplayName : "None";
            label.text = $"Feathers {runtime.State.CollectedCount}/12\nSelected: {selected}";
        }
    }
}
```

Create `Assets/FeatherDetective/Scripts/UI/EndingMessageUI.cs` with:

```csharp
using UnityEngine;
using UnityEngine.UI;

namespace FeatherDetective
{
    public sealed class EndingMessageUI : MonoBehaviour
    {
        [SerializeField] private Text label;
        [SerializeField] private CanvasGroup group;

        public void ConfigureForBuilder(Text newLabel, CanvasGroup newGroup)
        {
            label = newLabel;
            group = newGroup;
            if (group != null)
            {
                group.alpha = 0f;
            }
        }

        public void Show(string message)
        {
            if (label != null)
            {
                label.text = message;
            }

            if (group != null)
            {
                group.alpha = 1f;
            }
        }
    }
}
```

Create `Assets/FeatherDetective/Scripts/Deduction/DeductionSlot.cs` with:

```csharp
using UnityEngine;

namespace FeatherDetective
{
    public sealed class DeductionSlot : MonoBehaviour
    {
        [SerializeField] private DeductionSlotId slotId;
        public DeductionSlotId SlotId => slotId;

        public void ConfigureForBuilder(DeductionSlotId newSlotId)
        {
            slotId = newSlotId;
        }
    }
}
```

Create `Assets/FeatherDetective/Scripts/Deduction/DeductionBoard.cs` with:

```csharp
using UnityEngine;

namespace FeatherDetective
{
    public sealed class DeductionBoard : MonoBehaviour, IInspectable
    {
        [SerializeField] private InvestigationRuntime runtime;
        [SerializeField] private DeductionSolution solution;
        [SerializeField] private EndingController endingController;
        [SerializeField] private DeductionSlot[] slots;

        public string PromptText => "Place";

        public void ConfigureForBuilder(InvestigationRuntime newRuntime, DeductionSolution newSolution, EndingController newEndingController, DeductionSlot[] newSlots)
        {
            runtime = newRuntime;
            solution = newSolution;
            endingController = newEndingController;
            slots = newSlots;
        }

        public void Inspect()
        {
            if (runtime?.SelectedFeather == null || slots == null)
            {
                return;
            }

            foreach (var slot in slots)
            {
                if (slot != null && runtime.SelectedFeather.CanPlaceInSlot(slot.SlotId))
                {
                    Place(slot.SlotId);
                    return;
                }
            }
        }

        public bool Place(DeductionSlotId slotId)
        {
            if (runtime == null)
            {
                return false;
            }

            var placed = runtime.TryPlaceSelected(slotId);
            if (placed && runtime.State.IsSolved(solution))
            {
                endingController?.PlayEnding();
            }

            return placed;
        }
    }
}
```

Create `Assets/FeatherDetective/Scripts/Deduction/EndingController.cs` with:

```csharp
using UnityEngine;

namespace FeatherDetective
{
    public sealed class EndingController : MonoBehaviour
    {
        [SerializeField] private EndingMessageUI endingMessage;
        [SerializeField] private Light sun;
        [SerializeField] private Color resolvedSkyColor = new Color(0.76f, 0.86f, 0.95f);

        public bool HasPlayed { get; private set; }

        public void ConfigureForBuilder(EndingMessageUI newEndingMessage, Light newSun)
        {
            endingMessage = newEndingMessage;
            sun = newSun;
        }

        public void PlayEnding()
        {
            if (HasPlayed)
            {
                return;
            }

            HasPlayed = true;
            RenderSettings.ambientLight = resolvedSkyColor;
            if (sun != null)
            {
                sun.intensity = 1.15f;
            }

            endingMessage?.Show("He stayed at the hospital with his wife. The park was waiting with him.");
        }
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run:

```powershell
powershell -ExecutionPolicy Bypass -File tools/run-unity-tests.ps1 -TestPlatform editmode
```

Expected: PASS for all EditMode tests.

- [ ] **Step 5: Commit and push**

```powershell
git add Assets/FeatherDetective/Scripts/Interaction/FeatherPickup.cs Assets/FeatherDetective/Scripts/Runtime Assets/FeatherDetective/Scripts/Deduction Assets/FeatherDetective/Scripts/UI/FeatherInventoryUI.cs Assets/FeatherDetective/Scripts/UI/EndingMessageUI.cs Assets/FeatherDetective/Tests/EditMode/InvestigationStateTests.cs Assets/FeatherDetective/Tests/EditMode/DeductionBoardTests.cs
git commit -m "feat: connect feather interaction and deduction board"
git push
```

---

### Task 7: Nest Board UI Hint

**Files:**
- Create: `Assets/FeatherDetective/Scripts/UI/DeductionBoardUI.cs`

- [ ] **Step 1: Add nest board UI script**

Create `Assets/FeatherDetective/Scripts/UI/DeductionBoardUI.cs` with:

```csharp
using UnityEngine;
using UnityEngine.UI;

namespace FeatherDetective
{
    public sealed class DeductionBoardUI : MonoBehaviour
    {
        [SerializeField] private Text label;

        public void ConfigureForBuilder(Text newLabel)
        {
            label = newLabel;
        }

        public void ShowBoardHint()
        {
            if (label != null)
            {
                label.text = "Nest board";
            }
        }
    }
}
```

- [ ] **Step 2: Run tests to verify compile**

Run:

```powershell
powershell -ExecutionPolicy Bypass -File tools/run-unity-tests.ps1 -TestPlatform editmode
```

Expected: PASS for all EditMode tests and no UI compile errors.

- [ ] **Step 3: Commit and push**

```powershell
git add Assets/FeatherDetective/Scripts/UI/DeductionBoardUI.cs
git commit -m "feat: add nest board ui hint"
git push
```

---

### Task 8: Prototype Scene Builder

**Files:**
- Create: `Assets/FeatherDetective/Editor/FeatherDetective.Editor.asmdef`
- Modify: `Assets/FeatherDetective/Tests/EditMode/FeatherDetective.EditModeTests.asmdef`
- Create: `Assets/FeatherDetective/Tests/EditMode/PrototypeSceneBuilderTests.cs`
- Create: `Assets/FeatherDetective/Editor/PrototypeSceneBuilder.cs`

- [ ] **Step 1: Write failing scene builder tests**

Create `Assets/FeatherDetective/Editor/FeatherDetective.Editor.asmdef` with:

```json
{
  "name": "FeatherDetective.Editor",
  "rootNamespace": "FeatherDetective",
  "references": [
    "FeatherDetective.Runtime"
  ],
  "includePlatforms": [
    "Editor"
  ],
  "excludePlatforms": [],
  "allowUnsafeCode": false,
  "overrideReferences": false,
  "precompiledReferences": [],
  "autoReferenced": false,
  "defineConstraints": [],
  "versionDefines": [],
  "noEngineReferences": false
}
```

Update `Assets/FeatherDetective/Tests/EditMode/FeatherDetective.EditModeTests.asmdef` so `references` includes the editor assembly:

```json
"references": [
  "FeatherDetective.Runtime",
  "FeatherDetective.Editor"
]
```

Create `Assets/FeatherDetective/Tests/EditMode/PrototypeSceneBuilderTests.cs` with:

```csharp
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FeatherDetective.Tests
{
    public sealed class PrototypeSceneBuilderTests
    {
        [Test]
        public void BuildSceneCreatesTwelveFeathersAndAtLeastEightPerches()
        {
            PrototypeSceneBuilder.BuildInOpenSceneForTests();

            Assert.That(Object.FindObjectsOfType<FeatherPickup>().Length, Is.EqualTo(12));
            Assert.That(Object.FindObjectsOfType<PerchNode>().Length, Is.GreaterThanOrEqualTo(8));
            Assert.That(Object.FindObjectOfType<BirdPlayerController>(), Is.Not.Null);
            Assert.That(Object.FindObjectOfType<DeductionBoard>(), Is.Not.Null);

            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run:

```powershell
powershell -ExecutionPolicy Bypass -File tools/run-unity-tests.ps1 -TestPlatform editmode
```

Expected with Unity Editor installed: FAIL with compile errors for missing `PrototypeSceneBuilder`.

- [ ] **Step 3: Add scene builder**

Create `Assets/FeatherDetective/Editor/PrototypeSceneBuilder.cs` with these public entry points and generated content:

```csharp
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FeatherDetective
{
    public static class PrototypeSceneBuilder
    {
        private const string ScenePath = "Assets/FeatherDetective/Scenes/ParkPrototype.unity";
        private const string DataPath = "Assets/FeatherDetective/Data/Feathers";

        [MenuItem("Feather Detective/Build Prototype Scene")]
        public static void BuildPrototypeScene()
        {
            BuildInOpenSceneForTests();
            Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), ScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void BuildInOpenSceneForTests()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            Directory.CreateDirectory(DataPath);

            var materials = CreateMaterials();
            var feathers = CreateFeatherDefinitions();
            var solution = CreateSolution(feathers);

            CreateLighting();
            var colorTargets = CreateParkGeometry(materials);
            var runtime = CreateRuntime(materials, colorTargets);
            var perches = CreatePerches();
            CreatePlayer(perches[0], materials);
            CreateFeathers(feathers, runtime, materials);
            CreateNestBoard(runtime, solution, materials);
            CreateCamera();
        }

        private static Material[] CreateMaterials()
        {
            return new[]
            {
                Material("Grass", new Color(0.34f, 0.68f, 0.36f)),
                Material("Path", new Color(0.68f, 0.62f, 0.53f)),
                Material("Wood", new Color(0.55f, 0.35f, 0.22f)),
                Material("Leaf", new Color(0.20f, 0.55f, 0.28f)),
                Material("Feather", new Color(0.95f, 0.88f, 0.66f)),
                Material("HospitalBlue", new Color(0.36f, 0.62f, 0.95f))
            };
        }

        private static Material Material(string name, Color color)
        {
            var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material.name = name;
            material.color = color;
            return material;
        }

        private static FeatherDefinition[] CreateFeatherDefinitions()
        {
            return new[]
            {
                Feather("crow-bench", "Bench sound feather", BirdSpecies.Crow, ClueRole.MainMystery, new[] { DeductionSlotId.LastFeedingDay }, "Hospital siren, wheels, and a soft worried voice.", new Vector3[0], Color.yellow),
                Feather("magpie-bench", "Bench color feather", BirdSpecies.Magpie, ClueRole.MainMystery, new[] { DeductionSlotId.WhatChanged }, "Red bracelet, blue thermos, yellow seed bag.", new Vector3[0], Color.blue),
                Feather("pigeon-path", "Path route feather", BirdSpecies.Pigeon, ClueRole.MainMystery, new[] { DeductionSlotId.WhereHeWent }, "Route away from the park.", new[] { new Vector3(-5f, 0.04f, -2f), new Vector3(0f, 0.04f, 1f), new Vector3(7f, 0.04f, 5f) }, Color.white),
                Feather("woodpecker-sign", "Sign rhythm feather", BirdSpecies.Woodpecker, ClueRole.MainMystery, new[] { DeductionSlotId.WhatChanged }, "Cane taps and hurried packing rhythm.", new Vector3[0], Color.white),
                Feather("sparrow-trees", "Tree atmosphere feather", BirdSpecies.Sparrow, ClueRole.MainMystery, new[] { DeductionSlotId.WhyBirdsWaited }, "Wind rises while the flock waits.", new Vector3[0], Color.white),
                Feather("crow-fountain", "Fountain sound feather", BirdSpecies.Crow, ClueRole.MainMystery, new[] { DeductionSlotId.WhatEvidenceMeans }, "Phone ringing near fountain water.", new Vector3[0], Color.white),
                Feather("magpie-notice", "Notice color feather", BirdSpecies.Magpie, ClueRole.MainMystery, new[] { DeductionSlotId.WhereHeWent }, "Blue-white hospital sign color.", new Vector3[0], Color.blue),
                Feather("pigeon-edge", "Park edge route feather", BirdSpecies.Pigeon, ClueRole.MainMystery, new[] { DeductionSlotId.WhatEvidenceMeans }, "Repeated trips between home and hospital direction.", new[] { new Vector3(7f, 0.04f, 5f), new Vector3(4f, 0.04f, -4f), new Vector3(7f, 0.04f, 5f) }, Color.white),
                Feather("sparrow-flowers", "Flower atmosphere feather", BirdSpecies.Sparrow, ClueRole.Atmosphere, new[] { DeductionSlotId.WhatChanged }, "The park quiets after feeding stops.", new Vector3[0], Color.white),
                Feather("woodpecker-tree", "Tree rhythm feather", BirdSpecies.Woodpecker, ClueRole.Atmosphere, new[] { DeductionSlotId.WhatChanged }, "Ordinary maintenance knocks.", new Vector3[0], Color.white),
                Feather("magpie-roof", "Roof color feather", BirdSpecies.Magpie, ClueRole.Atmosphere, new[] { DeductionSlotId.WhatEvidenceMeans }, "Bright picnic colors on a roof edge.", new Vector3[0], Color.red),
                Feather("crow-water", "Water sound feather", BirdSpecies.Crow, ClueRole.Atmosphere, new[] { DeductionSlotId.LastFeedingDay }, "Water, shoes, and children laughing.", new Vector3[0], Color.white)
            };
        }

        private static FeatherDefinition Feather(string id, string displayName, BirdSpecies species, ClueRole role, DeductionSlotId[] slots, string summary, Vector3[] route, Color color)
        {
            var assetPath = $"{DataPath}/{id}.asset";
            var feather = AssetDatabase.LoadAssetAtPath<FeatherDefinition>(assetPath);
            if (feather == null)
            {
                feather = ScriptableObject.CreateInstance<FeatherDefinition>();
                AssetDatabase.CreateAsset(feather, assetPath);
            }

            feather.ConfigureForTests(id, displayName, species, role, slots, new[] { color }, route, summary);
            EditorUtility.SetDirty(feather);
            return feather;
        }

        private static DeductionSolution CreateSolution(FeatherDefinition[] feathers)
        {
            var solution = ScriptableObject.CreateInstance<DeductionSolution>();
            solution.ConfigureForTests(new[]
            {
                new DeductionAnswer(DeductionSlotId.LastFeedingDay, feathers[0]),
                new DeductionAnswer(DeductionSlotId.WhatChanged, feathers[1]),
                new DeductionAnswer(DeductionSlotId.WhereHeWent, feathers[6]),
                new DeductionAnswer(DeductionSlotId.WhyBirdsWaited, feathers[4]),
                new DeductionAnswer(DeductionSlotId.WhatEvidenceMeans, feathers[7])
            });
            return solution;
        }

        private static InvestigationRuntime CreateRuntime(Material[] materials, ColorMemoryTarget[] colorTargets)
        {
            var systems = new GameObject("GameSystems");

            var canvas = CreateCanvas();
            var overlay = CreateOverlay(canvas.transform);
            var promptText = CreateText("PromptText", canvas.transform, "Inspect", new Vector2(0f, -220f), 24, TextAnchor.MiddleCenter);
            var inventoryText = CreateText("InventoryText", canvas.transform, "Feathers 0/12", new Vector2(-470f, 250f), 20, TextAnchor.UpperLeft);
            var boardText = CreateText("BoardText", canvas.transform, "Nest board", new Vector2(0f, 250f), 20, TextAnchor.UpperCenter);
            var endingGroup = CreateEndingGroup(canvas.transform, out var endingText);

            var prompt = systems.AddComponent<InteractionPrompt>();
            prompt.ConfigureForBuilder(promptText);

            var inventory = systems.AddComponent<FeatherInventoryUI>();
            inventory.ConfigureForBuilder(inventoryText);

            var boardUi = systems.AddComponent<DeductionBoardUI>();
            boardUi.ConfigureForBuilder(boardText);

            var endingUi = systems.AddComponent<EndingMessageUI>();
            endingUi.ConfigureForBuilder(endingText, endingGroup);

            var memoryAudio = systems.AddComponent<AudioSource>();
            memoryAudio.playOnAwake = false;
            memoryAudio.spatialBlend = 0.75f;

            var routeObject = new GameObject("PigeonRouteLine");
            var routeLine = routeObject.AddComponent<LineRenderer>();
            routeLine.material = materials[4];
            routeLine.widthMultiplier = 0.08f;
            routeLine.useWorldSpace = true;
            var routeRenderer = routeObject.AddComponent<RouteLineRenderer>();

            var rhythmObject = new GameObject("WoodpeckerMemoryRig");
            var rhythmCameraObject = new GameObject("WoodpeckerMemoryCamera");
            rhythmCameraObject.transform.SetParent(rhythmObject.transform);
            var rhythmCamera = rhythmCameraObject.AddComponent<Camera>();
            rhythmCamera.enabled = false;
            var rhythmAudio = rhythmObject.AddComponent<AudioSource>();
            rhythmAudio.playOnAwake = false;
            var firstPersonRig = rhythmObject.AddComponent<FirstPersonMemoryRig>();
            firstPersonRig.ConfigureForBuilder(rhythmCamera, rhythmAudio);

            var atmosphereObject = new GameObject("Atmosphere");
            var windSource = atmosphereObject.AddComponent<AudioSource>();
            var insectSource = atmosphereObject.AddComponent<AudioSource>();
            windSource.loop = true;
            insectSource.loop = true;
            windSource.volume = 0.2f;
            insectSource.volume = 0.35f;
            var flockRoot = new GameObject("FlockSilhouettes").transform;
            var atmosphere = atmosphereObject.AddComponent<AtmosphereController>();
            atmosphere.ConfigureForBuilder(windSource, insectSource, flockRoot);

            var context = systems.AddComponent<MemoryContext>();
            context.ConfigureForBuilder(overlay, memoryAudio, colorTargets, routeRenderer, firstPersonRig, atmosphere);

            var playback = systems.AddComponent<MemoryPlaybackController>();
            playback.ConfigureForBuilder(context);
            systems.AddComponent<CrowMemoryEffect>();
            systems.AddComponent<MagpieMemoryEffect>();
            systems.AddComponent<PigeonMemoryEffect>();
            systems.AddComponent<WoodpeckerMemoryEffect>();
            systems.AddComponent<SparrowMemoryEffect>();

            var runtime = systems.AddComponent<InvestigationRuntime>();
            runtime.ConfigureForBuilder(playback, inventory);
            return runtime;
        }

        private static Canvas CreateCanvas()
        {
            var canvasObject = new GameObject("PrototypeCanvas");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private static CanvasGroup CreateOverlay(Transform parent)
        {
            var overlayObject = new GameObject("MemoryDarkOverlay");
            overlayObject.transform.SetParent(parent, false);
            var image = overlayObject.AddComponent<Image>();
            image.color = Color.black;
            var rect = image.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var group = overlayObject.AddComponent<CanvasGroup>();
            group.alpha = 0f;
            group.blocksRaycasts = false;
            return group;
        }

        private static CanvasGroup CreateEndingGroup(Transform parent, out Text endingText)
        {
            var groupObject = new GameObject("EndingMessage");
            groupObject.transform.SetParent(parent, false);
            var group = groupObject.AddComponent<CanvasGroup>();
            group.alpha = 0f;
            endingText = CreateText("EndingText", groupObject.transform, string.Empty, new Vector2(0f, 0f), 26, TextAnchor.MiddleCenter);
            endingText.color = new Color(0.12f, 0.16f, 0.18f);
            return group;
        }

        private static Text CreateText(string name, Transform parent, string value, Vector2 anchoredPosition, int size, TextAnchor anchor)
        {
            var textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);
            var text = textObject.AddComponent<Text>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = size;
            text.alignment = anchor;
            text.color = new Color(0.08f, 0.1f, 0.12f);
            text.rectTransform.sizeDelta = new Vector2(520f, 120f);
            text.rectTransform.anchoredPosition = anchoredPosition;
            return text;
        }

        private static void CreateLighting()
        {
            RenderSettings.ambientLight = new Color(0.62f, 0.72f, 0.76f);
            var sunObject = new GameObject("Sun");
            sunObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            var sun = sunObject.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.intensity = 1f;
            sun.shadows = LightShadows.Soft;
        }

        private static ColorMemoryTarget[] CreateParkGeometry(Material[] materials)
        {
            var targets = new List<ColorMemoryTarget>();
            AddPrimitive("Grass", PrimitiveType.Cube, new Vector3(0f, -0.05f, 0f), new Vector3(18f, 0.1f, 14f), materials[0]);
            AddPrimitive("CurvedPath", PrimitiveType.Cube, new Vector3(0f, 0f, 0f), new Vector3(15f, 0.08f, 2.2f), materials[1]);
            AddPrimitive("FeedingBench", PrimitiveType.Cube, new Vector3(-4f, 0.45f, -2f), new Vector3(2.6f, 0.35f, 0.7f), materials[2]);
            AddPrimitive("Fountain", PrimitiveType.Cylinder, new Vector3(2.5f, 0.35f, -1.5f), new Vector3(1.4f, 0.35f, 1.4f), materials[5]);
            AddPrimitive("LowRoof", PrimitiveType.Cube, new Vector3(6f, 1.8f, -4.5f), new Vector3(3.5f, 0.25f, 2f), materials[2]);
            var sign = AddPrimitive("HospitalDirectionSign", PrimitiveType.Cube, new Vector3(5.5f, 1.2f, 3.5f), new Vector3(1.4f, 0.8f, 0.08f), materials[5]);
            targets.Add(sign.AddComponent<ColorMemoryTarget>());
            var bag = AddPrimitive("YellowSeedBag", PrimitiveType.Cube, new Vector3(-3.3f, 0.8f, -1.5f), new Vector3(0.45f, 0.45f, 0.25f), materials[4]);
            targets.Add(bag.AddComponent<ColorMemoryTarget>());

            for (var i = 0; i < 5; i++)
            {
                var x = -7f + i * 3.5f;
                AddPrimitive($"TreeTrunk_{i}", PrimitiveType.Cylinder, new Vector3(x, 0.65f, 4.5f), new Vector3(0.35f, 1.3f, 0.35f), materials[2]);
                AddPrimitive($"TreeCrown_{i}", PrimitiveType.Sphere, new Vector3(x, 1.8f, 4.5f), new Vector3(1.4f, 1.1f, 1.4f), materials[3]);
            }

            return targets.ToArray();
        }

        private static PerchNode[] CreatePerches()
        {
            var perches = new[]
            {
                CreatePerch("Perch_Bench", new Vector3(-4f, 0.9f, -2f)),
                CreatePerch("Perch_TreeA", new Vector3(-7f, 2.35f, 4.5f)),
                CreatePerch("Perch_TreeB", new Vector3(-3.5f, 2.35f, 4.5f)),
                CreatePerch("Perch_Sign", new Vector3(5.5f, 1.75f, 3.5f)),
                CreatePerch("Perch_Roof", new Vector3(6f, 2.05f, -4.5f)),
                CreatePerch("Perch_Fountain", new Vector3(2.5f, 0.9f, -1.5f)),
                CreatePerch("Perch_Nest", new Vector3(0f, 1.2f, -5.5f)),
                CreatePerch("Perch_PathPost", new Vector3(0f, 1.1f, 2.5f))
            };

            perches[0].ConfigureForTests(new[] { perches[1], perches[5], perches[7] });
            perches[1].ConfigureForTests(new[] { perches[0], perches[2], perches[6] });
            perches[2].ConfigureForTests(new[] { perches[1], perches[3], perches[7] });
            perches[3].ConfigureForTests(new[] { perches[2], perches[4] });
            perches[4].ConfigureForTests(new[] { perches[3], perches[5] });
            perches[5].ConfigureForTests(new[] { perches[0], perches[4], perches[6] });
            perches[6].ConfigureForTests(new[] { perches[1], perches[5], perches[7] });
            perches[7].ConfigureForTests(new[] { perches[0], perches[2], perches[6] });
            return perches;
        }

        private static PerchNode CreatePerch(string name, Vector3 position)
        {
            var perchObject = new GameObject(name);
            perchObject.transform.position = position;
            var collider = perchObject.AddComponent<SphereCollider>();
            collider.radius = 0.45f;
            collider.isTrigger = true;
            return perchObject.AddComponent<PerchNode>();
        }

        private static void CreatePlayer(PerchNode start, Material[] materials)
        {
            var player = new GameObject("PlayerBird");
            player.AddComponent<CharacterController>().height = 0.7f;
            var body = AddPrimitive("BirdBody", PrimitiveType.Sphere, Vector3.zero, new Vector3(0.45f, 0.32f, 0.55f), materials[4]);
            body.transform.SetParent(player.transform, false);
            var controller = player.AddComponent<BirdPlayerController>();
            controller.ConfigureForBuilder(Object.FindObjectOfType<InteractionPrompt>(), start);
        }

        private static void CreateFeathers(FeatherDefinition[] feathers, InvestigationRuntime runtime, Material[] materials)
        {
            var positions = new[]
            {
                new Vector3(-4.8f, 0.2f, -1.6f), new Vector3(-3.5f, 0.2f, -1.4f), new Vector3(-1f, 0.2f, 0.9f),
                new Vector3(4.6f, 0.2f, 3.2f), new Vector3(-6.5f, 0.2f, 4.2f), new Vector3(2.5f, 0.2f, -2.5f),
                new Vector3(5.8f, 0.2f, 3.9f), new Vector3(7f, 0.2f, 5f), new Vector3(-2f, 0.2f, 5.5f),
                new Vector3(-7f, 1.3f, 4.2f), new Vector3(6f, 2.2f, -4.3f), new Vector3(1.8f, 0.2f, -1.1f)
            };

            for (var i = 0; i < feathers.Length; i++)
            {
                var featherObject = AddPrimitive($"Feather_{feathers[i].Id}", PrimitiveType.Capsule, positions[i], new Vector3(0.12f, 0.28f, 0.12f), materials[4]);
                featherObject.transform.rotation = Quaternion.Euler(65f, i * 23f, 20f);
                var collider = featherObject.GetComponent<Collider>();
                collider.isTrigger = true;
                var pickup = featherObject.AddComponent<FeatherPickup>();
                pickup.ConfigureForBuilder(feathers[i], runtime);
            }
        }

        private static void CreateNestBoard(InvestigationRuntime runtime, DeductionSolution solution, Material[] materials)
        {
            var boardObject = AddPrimitive("NestBoard", PrimitiveType.Cube, new Vector3(0f, 0.75f, -5.5f), new Vector3(2.8f, 0.2f, 1.6f), materials[2]);
            boardObject.GetComponent<Collider>().isTrigger = true;
            var slots = new DeductionSlot[5];
            var ids = new[]
            {
                DeductionSlotId.LastFeedingDay,
                DeductionSlotId.WhatChanged,
                DeductionSlotId.WhereHeWent,
                DeductionSlotId.WhyBirdsWaited,
                DeductionSlotId.WhatEvidenceMeans
            };

            for (var i = 0; i < ids.Length; i++)
            {
                var slotObject = AddPrimitive($"DeductionSlot_{ids[i]}", PrimitiveType.Cube, new Vector3(-1.1f + i * 0.55f, 0.95f, -5.5f), new Vector3(0.35f, 0.08f, 0.5f), materials[4]);
                slotObject.transform.SetParent(boardObject.transform, true);
                var slot = slotObject.AddComponent<DeductionSlot>();
                slot.ConfigureForBuilder(ids[i]);
                slots[i] = slot;
            }

            var endingObject = new GameObject("EndingController");
            var ending = endingObject.AddComponent<EndingController>();
            ending.ConfigureForBuilder(Object.FindObjectOfType<EndingMessageUI>(), GameObject.Find("Sun").GetComponent<Light>());

            var board = boardObject.AddComponent<DeductionBoard>();
            board.ConfigureForBuilder(runtime, solution, ending, slots);
        }

        private static void CreateCamera()
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.68f, 0.82f, 0.94f);
            var follow = cameraObject.AddComponent<FixedThirdPersonCamera>();
            follow.SetTarget(GameObject.Find("PlayerBird").transform);
            cameraObject.transform.position = new Vector3(0f, 8f, -9f);
            cameraObject.transform.rotation = Quaternion.Euler(48f, 0f, 0f);
        }

        private static GameObject AddPrimitive(string name, PrimitiveType type, Vector3 position, Vector3 scale, Material material)
        {
            var gameObject = GameObject.CreatePrimitive(type);
            gameObject.name = name;
            gameObject.transform.position = position;
            gameObject.transform.localScale = scale;
            if (material != null)
            {
                gameObject.GetComponent<Renderer>().sharedMaterial = material;
            }

            return gameObject;
        }
    }
}
```

- [ ] **Step 4: Run tests to verify scene builder passes**

Run:

```powershell
powershell -ExecutionPolicy Bypass -File tools/run-unity-tests.ps1 -TestPlatform editmode
```

Expected: PASS for `PrototypeSceneBuilderTests` and previous EditMode tests.

- [ ] **Step 5: Generate the scene**

Open Unity, then choose menu item:

```text
Feather Detective > Build Prototype Scene
```

Expected: `Assets/FeatherDetective/Scenes/ParkPrototype.unity` is created. The scene contains `PlayerBird`, `GameSystems`, `NestBoard`, 12 feather pickups, at least 8 perches, park geometry, lighting, UI canvas, and main camera.

- [ ] **Step 6: Commit and push**

```powershell
git add Assets/FeatherDetective/Editor Assets/FeatherDetective/Tests/EditMode/FeatherDetective.EditModeTests.asmdef Assets/FeatherDetective/Tests/EditMode/PrototypeSceneBuilderTests.cs Assets/FeatherDetective/Data Assets/FeatherDetective/Scenes
git commit -m "feat: generate low-poly park prototype scene"
git push
```

---

### Task 9: Playable Vertical Slice Verification

**Files:**
- Modify: `README.md`

- [ ] **Step 1: Update README with play instructions**

Replace `README.md` with:

```markdown
# HackathonGame

Small Unity narrative detective prototype about a bird investigating feathers in a city park.

## Open

Open this folder as a Unity project. The project uses URP and expects Unity with Universal Render Pipeline package support.

## Build Scene

In Unity, choose:

`Feather Detective > Build Prototype Scene`

Open:

`Assets/FeatherDetective/Scenes/ParkPrototype.unity`

## Controls

- WASD or arrow keys: walk
- Space: hop
- G: glide to a linked perch
- E: inspect a feather or place a selected feather at the nest board

## Tests

```powershell
powershell -ExecutionPolicy Bypass -File tools/run-unity-tests.ps1 -TestPlatform editmode
```

## Prototype Goal

Find 12 feathers, compare five species-specific memory presentations, and complete the nest deduction board. The ending reveals the old man stayed at the hospital with his wife.
```

- [ ] **Step 2: Run EditMode tests**

Run:

```powershell
powershell -ExecutionPolicy Bypass -File tools/run-unity-tests.ps1 -TestPlatform editmode
```

Expected: PASS for all EditMode tests.

- [ ] **Step 3: Run PlayMode smoke tests**

Run:

```powershell
powershell -ExecutionPolicy Bypass -File tools/run-unity-tests.ps1 -TestPlatform playmode
```

Expected: PASS with no compile errors. If no PlayMode tests exist yet, Unity still exits successfully after compilation.

- [ ] **Step 4: Manual playthrough checklist**

Open `Assets/FeatherDetective/Scenes/ParkPrototype.unity` in Unity and enter Play Mode. Verify:

- The bird starts at a visible perch in the park.
- WASD moves the bird and Space hops.
- G glides to a linked perch.
- E inspects nearby feathers.
- Inspecting Crow feathers darkens the screen and plays memory audio.
- Inspecting Magpie feathers desaturates non-clue colors.
- Inspecting Pigeon feathers animates white route lines.
- Inspecting Woodpecker feathers enables the first-person rhythm view.
- Inspecting Sparrow feathers changes wind, insects, and flock ambience.
- The inventory count reaches `Feathers 12/12`.
- The nest board accepts main mystery feathers.
- The solved board triggers the hospital-with-wife ending text.

- [ ] **Step 5: Commit and push**

```powershell
git add README.md
git commit -m "docs: add prototype play instructions"
git push
```

---

### Task 10: Final Repository Review

**Files:**
- No planned file changes.

- [ ] **Step 1: Check working tree**

Run:

```powershell
git status --short --branch
```

Expected:

```text
## main...origin/main
```

- [ ] **Step 2: Check latest commits**

Run:

```powershell
git log --oneline --decorate -8
```

Expected: latest commits include:

```text
docs: add prototype play instructions
feat: generate low-poly park prototype scene
feat: add nest board ui hint
feat: connect feather interaction and deduction board
feat: add bird memory playback effects
feat: add bird movement and perch selection
feat: track feather investigation state
feat: add feather data model
```

- [ ] **Step 3: Push any remaining commits**

Run:

```powershell
git push
```

Expected:

```text
Everything up-to-date
```

- [ ] **Step 4: Final acceptance statement**

Report these verification results to the user:

- Whether Unity Editor was available locally.
- Whether EditMode tests passed.
- Whether PlayMode smoke tests passed.
- Whether the generated scene was opened in Play Mode.
- The latest pushed commit hash on `origin/main`.
