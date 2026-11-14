<#
.SYNOPSIS
    Bygger Kolibri.Etiketter som én selvstendig exe (win-x64) og kopierer den til release-mappen.

.EXAMPLE
    .\deploy.ps1                       # til \\Greenlantern\apps\RELEASE\Kolibri.Etiketter
    .\deploy.ps1 -Target .\publish     # lokalt, for testing
#>
param(
    [string]$Target = '\\Greenlantern\apps\RELEASE\Kolibri.Etiketter'
)

$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'Kolibri.Etiketter\Kolibri.Etiketter.csproj'
$staging = Join-Path $PSScriptRoot 'publish\_staging'

if (Test-Path $staging) { Remove-Item $staging -Recurse -Force }

dotnet publish $project -c Release -r win-x64 --self-contained `
    -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -p:DebugType=none -o $staging
if ($LASTEXITCODE -ne 0) { throw "dotnet publish feilet ($LASTEXITCODE)" }

if (Get-Process Kolibri.Etiketter -ErrorAction SilentlyContinue | Where-Object { $_.Path -like "$Target*" }) {
    throw "Kolibri.Etiketter kjører fra $Target. Lukk programmet og prøv igjen."
}

New-Item -ItemType Directory -Force -Path $Target | Out-Null
# Bare filene fra bygget kopieres; andre filer i mappen røres ikke.
Copy-Item (Join-Path $staging '*') $Target -Force
Remove-Item $staging -Recurse -Force

$exe = Join-Path $Target 'Kolibri.Etiketter.exe'
Write-Host "Ferdig: $exe ($((Get-Item $exe).LastWriteTime))"
