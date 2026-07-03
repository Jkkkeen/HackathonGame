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
$logsDir = Join-Path $repoRoot "Logs"
New-Item -ItemType Directory -Force -Path $logsDir | Out-Null
$logFile = Join-Path $logsDir ("{0}.log" -f ($(if ($TestPlatform -eq "editmode") { "EditMode" } else { "PlayMode" })))

$arguments = @(
    "-batchmode",
    "-nographics",
    "-projectPath",
    $repoRoot,
    "-logFile",
    $logFile,
    "-runTests",
    "-testPlatform",
    $TestPlatform,
    "-testResults",
    $resultFile
)

$process = Start-Process -FilePath $unity -ArgumentList $arguments -PassThru
$startedAt = Get-Date
$timeout = [TimeSpan]::FromMinutes(30)
$testRunResult = $null
while (-not $process.HasExited) {
    if (Test-Path $resultFile) {
        try {
            $testResults = [xml](Get-Content -Raw -LiteralPath $resultFile)
            $testRunResult = $testResults."test-run".result
        }
        catch {
            $testRunResult = $null
        }

        if ($testRunResult) {
            Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
            break
        }
    }

    if (((Get-Date) - $startedAt) -gt $timeout) {
        Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
        Write-Error "Unity test run timed out after $($timeout.TotalMinutes) minutes. See log: $logFile"
        exit 1
    }

    Start-Sleep -Seconds 2
    $process.Refresh()
}

if (-not (Test-Path $resultFile)) {
    Write-Error "Unity exited with code $($process.ExitCode), but did not create test results at $resultFile. See log: $logFile"
    exit 1
}

$testResults = [xml](Get-Content -Raw -LiteralPath $resultFile)
$testRun = $testResults."test-run"
if ($testRun.result -like "Passed*") {
    exit 0
}

exit 2
