param(
    [string]$AssetsRoot = (Resolve-Path "$PSScriptRoot\..\..\..\..").Path
)

$ErrorActionPreference = "Stop"

Write-Host "== SilicaII Architecture Checks =="
Write-Host "Assets root: $AssetsRoot"

$patterns = @(
    "ControladorLluvia",
    "CicloDiaNoche",
    "PropulsorBarSegments",
    "VitalityBarSegments",
    "NotificacionData",
    "Resources\.LoadAll",
    "FindObjectOfType",
    "FindFirstObjectByType",
    "GameObject\.Find",
    "AudioManager\.Instance",
    "GameManager\.Instance",
    "GameplayEvents\.",
    "QuestEvents\."
)

foreach ($pattern in $patterns) {
    Write-Host "`n-- Pattern: $pattern"
    rg -n $pattern "$AssetsRoot\Project\Scripts\Systems" -g "*.cs"
}

Write-Host "`nChecks completed. Review intentional legacy usages and migrate them phase by phase."
