# Builds a one-click PC-Spacesaver.exe in the project root.
$ErrorActionPreference = "Stop"
$root = $PSScriptRoot
$project = Join-Path $root "src\Spacesaver\Spacesaver.csproj"
$exe = Join-Path $root "PC-Spacesaver.exe"

Write-Host "Publishing single-file self-contained exe..." -ForegroundColor Cyan
dotnet publish $project -p:PublishProfile=root-singlefile

if (-not (Test-Path $exe)) {
    Write-Error "Publish failed - exe not found at $exe"
}

$sizeMb = [math]::Round((Get-Item $exe).Length / 1MB, 1)
Write-Host ""
Write-Host "Ready: $exe ($sizeMb MB)" -ForegroundColor Green
Write-Host "Double-click to run. No install required."
