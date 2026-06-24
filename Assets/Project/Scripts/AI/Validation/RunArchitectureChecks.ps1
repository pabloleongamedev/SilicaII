param(
    [string]$AssetsRoot = (Resolve-Path "$PSScriptRoot\..\..\..\..").Path
)

$ErrorActionPreference = "Stop"

Write-Host "== SilicaII Architecture Checks =="
Write-Host "Assets root: $AssetsRoot"

$scriptsRoot = Join-Path $AssetsRoot "Project\Scripts\Systems"

$checks = @(
    @{ Name = "Legacy Spanish type names"; Pattern = "ControladorLluvia|CicloDiaNoche|PropulsorBarSegments|VitalityBarSegments|NotificacionData" },
    @{ Name = "Hidden global lookups"; Pattern = "Resources\.LoadAll|FindObjectOfType|FindFirstObjectByType|GameObject\.Find" },
    @{ Name = "Legacy singleton access"; Pattern = "GameManager\.Instance|GameSettings\.Instance|AudioManager\.Instance" },
    @{ Name = "New singleton declarations"; Pattern = "public\s+static\s+.*\s+Instance\b|static\s+.*\s+Instance\s*\{\s*get;" },
    @{ Name = "Legacy static event buses"; Pattern = "GameplayEvents\.|QuestEvents\.|InventoryEvents\.|CraftingEvents\.|NotificationEvents\.|GameStateEvents\.|UIStateEvents\.|ScannerEvents\.|WeatherEvents\.|InputActivityEvents\." },
    @{ Name = "Mutable global delivery route"; Pattern = "activeBox" },
    @{ Name = "UI-owned pause state"; Pattern = "Time\.timeScale" }
)

$allowedTimeScaleFiles = @(
    "GamePauseService.cs"
)

$hasViolations = $false

foreach ($check in $checks) {
    Write-Host "`n-- $($check.Name): $($check.Pattern)"

    $matches = Get-ChildItem -Path $scriptsRoot -Recurse -Filter "*.cs" |
        Select-String -Pattern $check.Pattern |
        Where-Object {
            $_.Line -and $_.Line -notmatch "^\s*(//|/\*|\*)"
        }

    if ($check.Name -eq "UI-owned pause state") {
        $matches = $matches | Where-Object {
            $file = Split-Path $_.Path -Leaf
            $allowedTimeScaleFiles -notcontains (Split-Path $file -Leaf)
        }
    }

    if ($matches) {
        $hasViolations = $true
        $matches | ForEach-Object { Write-Host "$($_.Path):$($_.LineNumber):$($_.Line.Trim())" }
    }
    else {
        Write-Host "OK"
    }
}

if ($hasViolations) {
    Write-Error "Architecture checks found forbidden or suspicious patterns."
}

Write-Host "`nChecks completed. No forbidden architecture patterns found."
