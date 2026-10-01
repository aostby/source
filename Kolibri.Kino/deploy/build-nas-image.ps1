# Builds the Kolibri Kino image for the UGREEN NAS (linux/arm64) with Docker Desktop and saves it as
# deploy\out\kolibri-kino.tar, to copy to the NAS and load there. No registry needed.
#
#   .\deploy\build-nas-image.ps1
#   .\deploy\build-nas-image.ps1 -Platform linux/amd64   # for an Intel/AMD machine instead

param(
    [string]$Platform = 'linux/arm64',
    [string]$Tag = 'kolibri-kino:latest'
)

$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$out = Join-Path $PSScriptRoot 'out'
New-Item -ItemType Directory -Force $out | Out-Null
$tar = Join-Path $out 'kolibri-kino.tar'

docker buildx build --platform $Platform -f "$root\src\Kolibri.Kino.Blazor\Dockerfile" -t $Tag -o "type=docker,dest=$tar" $root
if ($LASTEXITCODE -ne 0) { throw "docker buildx failed ($LASTEXITCODE)" }

Write-Host ""
Write-Host "Saved $tar ($([math]::Round((Get-Item $tar).Length / 1MB)) MB). Next:"
Write-Host "  1. Copy it to the NAS folder docker\kolibri-kino (e.g. \\<nas>\docker\kolibri-kino)."
Write-Host "  2. On the NAS (SSH):  cd /volume1/docker/kolibri-kino && docker load -i kolibri-kino.tar && docker compose up -d"
