$Root = "Assets/Project/Scripts"

$Patterns = @(
    "FindObjectOfType",
    "GameObject.Find",
    "Resources.LoadAll",
    "SendMessage",
    "BroadcastMessage"
)

foreach ($Pattern in $Patterns) {
    Write-Host ""
    Write-Host "Checking pattern: $Pattern"
    Select-String -Path "$Root/**/*.cs" -Pattern $Pattern -ErrorAction SilentlyContinue
}
