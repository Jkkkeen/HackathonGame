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
