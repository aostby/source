# Builds Kolibri Kino for the UGREEN NAS and puts everything in its docker folder over the network share, ready
# to start. The containers are not touched until the last step, which runs on the NAS:
#
#   .\deploy\deploy-to-nas.ps1                    # build both images (Kino and KMDb), copy them and compose.yaml to the NAS
#   .\deploy\deploy-to-nas.ps1 -App kino          # only Kino (the web app); -App kmdb: only Kolibri.Kino.KMDb
#   .\deploy\deploy-to-nas.ps1 -WithDatabases     # also stage the library from C:\TEMP\SilverScreen (replaces the NAS copy)
#   .\deploy\deploy-to-nas.ps1 -RunOnNas          # then also run the NAS step over SSH (asks for your NAS password)
#   .\deploy\deploy-to-nas.ps1 -SkipBuild         # copy the images built last time
#
# The databases go to data-new\ first and are swapped in on the NAS after the container has stopped: copying
# over a database Kino has open could damage it.

param(
    [ValidateSet('all', 'kino', 'kmdb')]
    [string]$App = 'all',
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
$apps = if ($App -eq 'all') { @('kino', 'kmdb') } else { @($App) }

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

    foreach ($a in $apps) {
        Step "Building the $a image (linux/arm64)"
        & (Join-Path $PSScriptRoot 'build-nas-image.ps1') -App $a
    }
}

Step "Copying the images and compose.yaml to $Share"
foreach ($a in $apps) {
    $tar = Join-Path $PSScriptRoot "out\kolibri-$a.tar"
    if (-not (Test-Path $tar)) { throw "No image at $tar. Run without -SkipBuild." }
    $target = Join-Path $Share "kolibri-$a.tar"
    Copy-Item $tar $target -Force
    if ((Get-FileHash $tar).Hash -ne (Get-FileHash $target).Hash) { throw "The $a image on the NAS differs from the one built. Run again." }
    Write-Host "kolibri-$a.tar copied and checked."
}
Copy-Item (Join-Path $PSScriptRoot 'compose.yaml') (Join-Path $Share 'compose.yaml') -Force
# Prometheus and Grafana settings and the KMDb dashboard (used by the optional "monitoring" profile).
Copy-Item (Join-Path $PSScriptRoot 'monitoring') $Share -Recurse -Force

# Settings added since the .env was written, with their defaults from .env.example. Values are never printed.
$envFile = Join-Path $Share '.env'
if (Test-Path $envFile) {
    $current = Get-Content $envFile -Raw
    if ($current -notmatch '(?m)^PLEX_BROWSER_HOST=') {
        Add-Content $envFile "`n# Plex links in the browser use this name; phones can't look up HTPC but find htpc.local.`nPLEX_BROWSER_HOST=htpc.local"
        Write-Host 'Added PLEX_BROWSER_HOST=htpc.local to .env.'
    }
    if ($current -notmatch '(?m)^KINO_WEB_API_KEY=') {
        Add-Content $envFile "`n# The key in kino_config.php on problembar.net; the wishlist and ratings are sent there. Empty turns it off.`nKINO_WEB_API_KEY="
        Write-Warning 'Added an empty KINO_WEB_API_KEY to .env: fill it in to send the wishlist and ratings to problembar.net.'
    }
    if ($current -notmatch '(?m)^KMDB_PORT=') {
        Add-Content $envFile "`n# Kolibri.Kino.KMDb (OMDb's API on the IMDb database): http://<nas>:8086/swagger`nKMDB_PORT=8086"
        Write-Host 'Added KMDB_PORT=8086 to .env.'
    }
    if ($current -notmatch '(?m)^KMDB_API_KEY=') {
        # The same key as the Uranus web service, so take it from KINO_WEB_API_KEY when that is filled in.
        $same = [regex]::Match($current, '(?m)^KINO_WEB_API_KEY=(.*)$').Groups[1].Value.Trim()
        Add-Content $envFile "`n# The key KMDb clients send (apikey= or X-Api-Key): the Uranus key, as KINO_WEB_API_KEY.`nKMDB_API_KEY=$same"
        if ($same) { Write-Host 'Added KMDB_API_KEY to .env (the same key as KINO_WEB_API_KEY).' }
        else { Write-Warning 'Added an empty KMDB_API_KEY to .env: fill in the Uranus key, or KMDb refuses every request.' }
    }
    if ($current -notmatch '(?m)^IMDB_CONNECTION=.') {
        if ($current -notmatch '(?m)^IMDB_CONNECTION=') {
            Add-Content $envFile "`n# The IMDb database KMDb reads (DefaultConnection in Kino's settings). In single quotes, so `$ in the password stays as it is.`nIMDB_CONNECTION="
        }
        Write-Warning 'IMDB_CONNECTION in .env is empty: KMDb starts only with the database connection filled in.'
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

$load = ($apps | ForEach-Object { "sudo docker load -i kolibri-$_.tar" }) -join ' && '
$command = "cd $NasFolder && sudo docker compose down$swap && $load && sudo docker compose up -d"

if ($RunOnNas) {
    Step "Starting the new version on $Nas (SSH as $NasUser)"
    ssh -t "$NasUser@$Nas" $command
    if ($LASTEXITCODE -ne 0) { throw "The NAS step failed ($LASTEXITCODE). Run it yourself over SSH: $command" }
    Write-Host "`nDone: http://$($Nas.ToLower()).local:8085 (KMDb: http://$($Nas.ToLower()).local:8086/swagger)" -ForegroundColor Green
} else {
    Step 'Last step: paste this into PowerShell (it runs the command on the NAS over SSH)'
    Write-Host "ssh -t $NasUser@$($Nas.ToLower()) `"$command`"" -ForegroundColor Yellow
    Write-Host "`nThen open http://$($Nas.ToLower()).local:8085 (KMDb: http://$($Nas.ToLower()).local:8086/swagger)"
}
