param(
    [string]$ProjectRoot = (Resolve-Path "$PSScriptRoot\..\..\..\..\..").Path,
    [switch]$ShowUnityLogErrors,
    [string]$UnityLogPath = "$env:LOCALAPPDATA\Unity\Editor\Editor.log"
)

$ErrorActionPreference = "Stop"

$assetsRoot = Join-Path $ProjectRoot "Assets"
$systemsProject = Join-Path $ProjectRoot "SilicaII.Systems.csproj"
$architectureChecks = Join-Path $assetsRoot "Project\Scripts\AI\Validation\RunArchitectureChecks.ps1"

$failedSteps = New-Object System.Collections.Generic.List[string]

function Invoke-HealthStep {
    param(
        [string]$Name,
        [scriptblock]$Action
    )

    Write-Host ""
    Write-Host "== $Name =="

    try {
        & $Action
        Write-Host "OK: $Name"
    }
    catch {
        $failedSteps.Add($Name)
        Write-Host "FAILED: $Name"
        Write-Host $_.Exception.Message
    }
}

function Assert-FileExists {
    param([string]$Path)

    if (-not (Test-Path -LiteralPath $Path)) {
        throw "Missing required file: $Path"
    }
}

Write-Host "== SilicaII Project Health Check =="
Write-Host "Project root: $ProjectRoot"
Write-Host "Assets root: $assetsRoot"

Invoke-HealthStep "Required documentation exists" {
    $requiredDocs = @(
        "Assets\AI\PROJECT_CONTEXT.md",
        "Assets\AI\Documentacion\Arquitectura_Tecnica_SilicaII.docx",
        "Assets\AI\Documentacion\Manual_Integracion_SilicaII.docx",
        "Assets\AI\Documentacion\Base\Requerimientos_Sistemas_SilicaII.docx",
        "Assets\AI\Documentacion\Base\Instructivo_Construccion_Sistemas_SilicaII.docx"
    )

    foreach ($doc in $requiredDocs) {
        Assert-FileExists (Join-Path $ProjectRoot $doc)
    }
}

Invoke-HealthStep "Dotnet build SilicaII.Systems" {
    Assert-FileExists $systemsProject
    dotnet build $systemsProject --nologo
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet build failed with exit code $LASTEXITCODE"
    }
}

Invoke-HealthStep "Architecture rules" {
    Assert-FileExists $architectureChecks
    powershell -ExecutionPolicy Bypass -File $architectureChecks -AssetsRoot $assetsRoot
    if ($LASTEXITCODE -ne 0) {
        throw "Architecture checks failed with exit code $LASTEXITCODE"
    }
}

Invoke-HealthStep "Missing Script scan" {
    $unityAssets = @(
        (Join-Path $assetsRoot "Project\Scenes"),
        (Join-Path $assetsRoot "Project\Prefabs")
    )

    $files = foreach ($path in $unityAssets) {
        if (Test-Path -LiteralPath $path) {
            Get-ChildItem -Path $path -Recurse -Include "*.unity", "*.prefab"
        }
    }

    $missingScripts = $files | Select-String -Pattern "m_Script:\s+\{fileID:\s+0\}"

    if ($missingScripts) {
        $missingScripts | ForEach-Object {
            Write-Host "$($_.Path):$($_.LineNumber):$($_.Line.Trim())"
        }

        throw "Missing Script references found."
    }
}

Invoke-HealthStep "Git working tree summary" {
    git -C $ProjectRoot status --short
}

if ($ShowUnityLogErrors) {
    Invoke-HealthStep "Unity Editor log recent errors" {
        Assert-FileExists $UnityLogPath

        $matches = Select-String -Path $UnityLogPath -Pattern "error CS|Exception|NullReferenceException|MissingReferenceException|Missing Script|ScriptCompilation" |
            Select-Object -Last 80

        if ($matches) {
            $matches | ForEach-Object { Write-Host "$($_.LineNumber):$($_.Line.Trim())" }
        }
        else {
            Write-Host "No recent error-like lines found in Unity Editor log."
        }
    }
}
else {
    Write-Host ""
    Write-Host "Unity log check skipped. Use -ShowUnityLogErrors to print recent error-like lines."
    Write-Host "Default Unity log path: $UnityLogPath"
}

Write-Host ""

if ($failedSteps.Count -gt 0) {
    Write-Host "Health check completed with failures:"
    $failedSteps | ForEach-Object { Write-Host "- $_" }
    exit 1
}

Write-Host "Health check completed successfully."
