param(
    [ValidateSet("editmode", "playmode")]
    [string]$TestPlatform = "editmode"
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
$unity = powershell -ExecutionPolicy Bypass -File (Join-Path $PSScriptRoot "find-unity-editor.ps1")
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

if (-not $unity -or -not (Test-Path $unity)) {
    Write-Error "Unity Editor executable was not found at the path returned by find-unity-editor.ps1."
    exit 1
}

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
