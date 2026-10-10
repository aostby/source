# Builds Kolibri Kino for the UGREEN NAS and puts everything in its docker folder over the network share, ready
# to start. The container is not touched until the last step, which runs on the NAS:
#
#   .\deploy\deploy-to-nas.ps1                    # build the image, copy it and compose.yaml to the NAS
#   .\deploy\deploy-to-nas.ps1 -WithDatabases     # also stage the library from C:\TEMP\SilverScreen (replaces the NAS copy)
#   .\deploy\deploy-to-nas.ps1 -RunOnNas          # then also run the NAS step over SSH (asks for your NAS password)
#   .\deploy\deploy-to-nas.ps1 -SkipBuild         # copy the image built last time
#
# The databases go to data-new\ first and are swapped in on the NAS after the container has stopped: copying
# over a database Kino has open could damage it.

param(
    [string]$Nas = 'GREENLANTERN',
    [string]$NasUser = $env:USERNAME,
    [string]$Share = '\\GREENLANTERN\docker\kolibri-kino',
    [string]$NasFolder = '/volume1/docker/kolibri-kino',
    [string]$LibraryFolder = 'C:\TEMP\SilverScreen',
    [switch]$WithDatabases,
    [switch]$RunOnNas,
    [switch]$SkipBuild
)

$ErrorActionPreference = 'Stop'
$tar = Join-Path $PSScriptRoot 'out\kolibri-kino.tar'

function Step([string]$text) { Write-Host "`n== $text" -ForegroundColor Cyan }

if (-not (Test-Path $Share)) { throw "Can't reach $Share. Is the NAS on and the docker share available?" }

if (-not $SkipBuild) {
    Step 'Docker Desktop'
    docker version --format '{{.Server.Version}}' 2>$null | Out-Null
    if ($LASTEXITCODE -ne 0) {
        $exe = @('E:\DEV\Docker\Docker\Docker Desktop.exe', "$env:ProgramFiles\Docker\Docker\Docker Desktop.exe") | Where-Object { Test-Path $_ } | Select-Object -First 1
        if (-not $exe) { throw 'Docker Desktop is not running and was not found. Start it and run this again.' }
        Write-Host "Starting $exe …"
        Start-Process $exe
        for ($i = 0; $i -lt 60; $i++) {
            Start-Sleep -Seconds 5
            docker version --format '{{.Server.Version}}' 2>$null | Out-Null
            if ($LASTEXITCODE -eq 0) { break }
        }
        if ($LASTEXITCODE -ne 0) { throw 'Docker Desktop did not start within 5 minutes.' }
    }

    Step 'Building the image (linux/arm64)'
    & (Join-Path $PSScriptRoot 'build-nas-image.ps1')
}
if (-not (Test-Path $tar)) { throw "No image at $tar. Run without -SkipBuild." }

Step "Copying the image and compose.yaml to $Share"
Copy-Item $tar (Join-Path $Share 'kolibri-kino.tar') -Force
Copy-Item (Join-Path $PSScriptRoot 'compose.yaml') (Join-Path $Share 'compose.yaml') -Force
if ((Get-FileHash $tar).Hash -ne (Get-FileHash (Join-Path $Share 'kolibri-kino.tar')).Hash) { throw 'The image on the NAS differs from the one built. Run again.' }
Write-Host 'Image copied and checked.'

# Settings added since the .env was written, with their defaults from .env.example.
$envFile = Join-Path $Share '.env'
if (Test-Path $envFile) {
    $current = Get-Content $envFile -Raw
    if ($current -notmatch '(?m)^PLEX_BROWSER_HOST=') {
        Add-Content $envFile "`n# Plex links in the browser use this name; phones can't look up HTPC but find htpc.local.`nPLEX_BROWSER_HOST=htpc.local"
        Write-Host 'Added PLEX_BROWSER_HOST=htpc.local to .env.'
    }
} else {
    Write-Warning "$envFile is missing: copy deploy\.env.example there and fill it in before starting."
}

$swap = ''
if ($WithDatabases) {
    Step "Staging the library from $LibraryFolder"
    if (Get-Process Kolibri.Kino.WinForms -ErrorAction SilentlyContinue) {
        Write-Warning 'Kino for Windows is running. Its last changes may not be written yet; close it for a clean copy.'
    }
    $staging = Join-Path $Share 'data-new'
    New-Item -ItemType Directory -Force $staging | Out-Null
    foreach ($name in 'SilverScreen.db', 'SilverScreen.imgdb', 'SilverScreen.logdb') {
        $source = Join-Path $LibraryFolder $name
        if (-not (Test-Path $source)) { Write-Warning "$source is missing; skipped."; continue }
        Copy-Item $source (Join-Path $staging $name) -Force
        Write-Host "  $name ($([math]::Round((Get-Item $source).Length / 1MB)) MB)"
    }
    # Skipped when data-new is gone (the command was run before), so running it twice still starts Kino.
    $swap = ' && if [ -d data-new ]; then sudo rm -f data/*-log.db && sudo cp data-new/SilverScreen.* data/ && sudo rm -rf data-new; fi'
}

$command = "cd $NasFolder && sudo docker compose down$swap && sudo docker load -i kolibri-kino.tar && sudo docker compose up -d"

if ($RunOnNas) {
    Step "Starting the new version on $Nas (SSH as $NasUser)"
    ssh -t "$NasUser@$Nas" $command
    if ($LASTEXITCODE -ne 0) { throw "The NAS step failed ($LASTEXITCODE). Run it yourself over SSH: $command" }
    Write-Host "`nDone: http://$($Nas.ToLower()).local:8085" -ForegroundColor Green
} else {
    Step 'Last step: paste this into PowerShell (it runs the command on the NAS over SSH)'
    Write-Host "ssh -t $NasUser@$($Nas.ToLower()) `"$command`"" -ForegroundColor Yellow
    Write-Host "`nThen open http://$($Nas.ToLower()).local:8085"
}
