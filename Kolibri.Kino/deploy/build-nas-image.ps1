# Builds an image for the UGREEN NAS (linux/arm64) with Docker Desktop and saves it as deploy\out\<name>.tar, to
# copy to the NAS and load there. No registry needed.
#
#   .\deploy\build-nas-image.ps1                         # Kino (the web app): deploy\out\kolibri-kino.tar
#   .\deploy\build-nas-image.ps1 -App kmdb               # Kolibri.Kino.KMDb (OMDb API on IMDb data): kolibri-kmdb.tar
#   .\deploy\build-nas-image.ps1 -Platform linux/amd64   # for an Intel/AMD machine instead

param(
    [ValidateSet('kino', 'kmdb')]
    [string]$App = 'kino',
    [string]$Platform = 'linux/arm64',
    # Its own BuildKit container. Docker Desktop's built-in builder (desktop-linux) has skipped the RUN steps
    # (restore/publish "done" in 2 s with no output, then COPY --from=build fails: "/app": not found).
    [string]$Builder = 'kino-nas'
)

$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$out = Join-Path $PSScriptRoot 'out'
New-Item -ItemType Directory -Force $out | Out-Null
$name = "kolibri-$App"
$tar = Join-Path $out "$name.tar"
$dockerfile = if ($App -eq 'kmdb') { "$root\src\Kolibri.Kino.KMDb\Dockerfile" } else { "$root\src\Kolibri.Kino.Blazor\Dockerfile" }
$tag = "$($name):latest"

docker buildx inspect $Builder *> $null
if ($LASTEXITCODE -ne 0) {
    docker buildx create --name $Builder --driver docker-container | Out-Null
    if ($LASTEXITCODE -ne 0) { throw "docker buildx create $Builder failed ($LASTEXITCODE)" }
}

docker buildx build --builder $Builder --platform $Platform -f $dockerfile -t $tag -o "type=docker,dest=$tar" $root
if ($LASTEXITCODE -ne 0) { throw "docker buildx failed ($LASTEXITCODE)" }

Write-Host ""
Write-Host "Saved $tar ($([math]::Round((Get-Item $tar).Length / 1MB)) MB). Next:"
Write-Host "  1. Copy it to the NAS folder docker\kolibri-kino (e.g. \\<nas>\docker\kolibri-kino)."
Write-Host "  2. On the NAS (SSH):  cd /volume1/docker/kolibri-kino && docker load -i $name.tar && docker compose up -d"
