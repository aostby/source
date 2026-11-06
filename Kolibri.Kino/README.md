# Kolibri.Kino

Your movie and series library: find titles, link them to the files on your disks, keep watchlists, a wishlist and your
own ratings, and keep them in step with Plex. Kino is a rewrite of Kolibri.SilverScreen as a layered, async app on .NET 10,
with two front ends: a WinForms app for Windows and a Blazor web app that runs in Docker on the NAS.
Both use the same LiteDB library file as SilverScreen (`SilverScreen.db`, by default `C:\TEMP\SilverScreen\SilverScreen.db`).

The user guide is the WinForms help (`src/Kolibri.Kino.WinForms/Help/help.html`, F1 in the app). This file is for developers.

## Projects

```
Kolibri.Kino.WinForms      UI: Windows forms. Calls controllers, never data classes.
Kolibri.Kino.Blazor        UI: web pages (Blazor Server), same rules. Runs in a container on the NAS.
        │
Kolibri.Kino.Controllers   Use cases (search, scan, series, watchlists, wishlist, ratings …). Depends on Core abstractions only.
        │
Kolibri.Kino.Core          Interfaces, models and options. OMDbApiNet.Model.Item is the shared movie model.
        ▲
Kolibri.Kino.Data          LiteDB repositories and the OMDb, TMDb, Plex and SubDL clients. Implements Core interfaces.
```

The WinForms and Blazor projects also reference Data, but only to wire up dependency injection in `Program.cs`.

| Project | Target | |
|---|---|---|
| `src/Kolibri.Kino.Core` | `net10.0` | Abstractions (`Abstractions/I*.cs`), models, `KinoOptions`, rules (cleanup, subtitles, rating bands) |
| `src/Kolibri.Kino.Data` | `net10.0` | LiteDB, OMDbApiNet, TMDbLib, Plex (HTTP), SubDL, file system scanning |
| `src/Kolibri.Kino.Controllers` | `net10.0` | One folder per feature: Scanning, Cleanup, LocalMovies, Series, Watchlists, Wishlist, Ratings, Subtitles, Playback, Trending, Discover, Library, Lookup, Settings, Usage, Logging |
| `src/Kolibri.Kino.WinForms` | `net10.0-windows` | MDI app (`MainForm`) with WebView2 for web pages. Windows only |
| `src/Kolibri.Kino.Blazor` | `net10.0` | Blazor Web App, interactive server rendering |
| `src/Kolibri.Kino.KMDb` | `net10.0` | Web API: OMDb's API on the IMDb database (MariaDB), with Swagger. See [KMDb](#kmdb-omdbs-api-on-the-imdb-database) |
| `tests/Kolibri.Kino.Tests` | `net10.0` | xUnit. Uses temporary databases, never the real library |
| `tools/Kolibri.Kino.SilverScreenCopy` | `net10.0` | Copies the library (SilverScreen.db, .imgdb, .logdb) to MariaDB for KMDb. See [KMDb](#kmdb-omdbs-api-on-the-imdb-database) |
| `tools/Kolibri.Kino.ImageDbMigrator` | `net10.0-windows` | Converts an old image cache (see [Image cache](#image-cache-silverscreenimgdb)) |

All projects are in `Kolibri.Kino.slnx`.

## Rules for new and ported code

- A form or page gets its controller through dependency injection and only `await`s controller methods.
- Every I/O method is `async Task<...>` and takes a `CancellationToken`. No `.Result` or `.Wait()`.
- LiteDB and HTTP code live in `Kolibri.Kino.Data`, behind an interface in `Kolibri.Kino.Core`.
- New features get a test in `tests/Kolibri.Kino.Tests`. Tests use temporary databases, never the real library.
- Kino works on stored documents, so fields it doesn't know (SilverScreen's) are kept when it saves.

## Building and running

You need the .NET 10 SDK.

**Windows:** everything builds and runs.

```
dotnet build Kolibri.Kino.slnx
dotnet test tests/Kolibri.Kino.Tests
dotnet run --project src/Kolibri.Kino.WinForms
dotnet run --project src/Kolibri.Kino.Blazor --launch-profile http     # http://localhost:5072
```

**Linux and macOS:** Core, Data, Controllers, Blazor and the tests build and run. WinForms and the ImageDbMigrator target
`net10.0-windows`, so they can't run. Build the projects one by one instead of the whole solution, or pass
`-p:EnableWindowsTargeting=true` to build (not run) the Windows projects too.

```
dotnet test tests/Kolibri.Kino.Tests
dotnet run --project src/Kolibri.Kino.Blazor --launch-profile http
```

`appsettings.Development.json` points the web app at `C:\TEMP\SilverScreen\SilverScreen.db`. On Linux, point it at a copy
of the library instead, for example with `Kino__LiteDbPath=~/kino/SilverScreen.db dotnet run …`, and set `Kino__SettingsUser`
to your Windows user name to use the settings saved on Windows (see [Settings user](#settings-user)).
Library file paths are Windows paths; `Kino:PathMappings` maps them to where the shares are mounted (see [Configuration](#configuration)).

**Docker image for the NAS:** see [Running on the UGREEN NAS](#running-on-the-ugreen-nas-docker). The scripts in `deploy/`
are PowerShell; on Linux run the `docker buildx` command at the top of `src/Kolibri.Kino.Blazor/Dockerfile` directly.

## Configuration

`appsettings.json`, section `Kino` (`KinoOptions`), holds only what belongs to this installation. In the container, set
the same keys as environment variables (`Kino__LiteDbPath`, `Kino__PathMappings__0__From` …).

| Key | Default | |
|---|---|---|
| `LiteDbPath` | WinForms: `C:\TEMP\SilverScreen\SilverScreen.db`, web: `/data/SilverScreen.db` | The library |
| `ImageDbPath` | empty | Image cache. Empty: next to `LiteDbPath` with the extension `.imgdb`, as in SilverScreen |
| `LogDbPath` | empty | Kino's log. Empty: next to `LiteDbPath` with the extension `.logdb` |
| `LogRetentionDays` | `30` | Log entries older than this are deleted |
| `SettingsUser` | empty | Whose `UserSettings` to use. Empty: the current user name |
| `Cleanup` | see [Cleaning up leftover files](#cleaning-up-leftover-files) | |
| `PathMappings` | empty | `From`/`To` pairs: a stored path prefix (`\\GREENLANTERN\Movies` or `//GREENLANTERN/Movies`) and where it is here (`/media/Movies`). Slashes and case don't matter |
| `BrowserHosts` | empty | `From`/`To` pairs: host names in links sent to the browser, e.g. `HTPC` → `htpc.local`, so phones can open Plex links |

Which levels are logged is set by `Logging:LogLevel:Default`.

### Which database the WinForms app opens

`DatabaseLocation` looks for it as SilverScreen does:

1. the file chosen in **Settings → LiteDB file**, remembered per user in `%LOCALAPPDATA%\Kolibri.Kino\usersettings.json`, while its folder exists;
2. else `Kino:LiteDbPath` in `appsettings.json` (embedded in the .exe as the default when the file is missing);
3. and if that database's user settings name another existing file (`LiteDBFilePath`), that one.

A folder without a database is fine: Kino asks, then makes a new, empty one there. A change applies after a restart.

## User settings

API keys (OMDb, TMDb, SubDL), subtitle languages, the Plex server and token, folders and the favorite watchlist are stored
in the database's `UserSettings` document (`_id` = Windows user name), shared with SilverScreen. Edit them in **Settings**
(both apps). **Test connections** tries the OMDb, TMDb and SubDL keys and the Plex token before you save. New values apply
at once.

OMDb and TMDb default to the public keys SilverScreen ships with, so a new database works at once. They are shared by
everyone using them (OMDb's 1,000 requests a day too), so both apps show a banner until you enter your own keys.
A key cleared in Settings falls back to the default. Settings left empty are removed rather than stored as empty, so
SilverScreen's own defaults apply to them.

## Features

| Feature | WinForms (menu) | Web | Controller |
|---|---|---|---|
| Search library and OMDb, import, details, similar movies, IMDb/TMDb links | Library → Search library and OMDb (`KinoForm`, `MovieDetailsForm`) | `/`, `/movie/{id}` | `MovieController`, `Library/*` |
| Recently added (by the files' dates) | | `/recent`, shelf on `/` | `RecentlyAddedController` |
| Local movies: scan, find movie for file, remove missing, clean up | Library → Local movies | | `MovieScanController`, `ManualLookupController`, `LocalMoviesController`, `CleanupController` |
| Local series: seasons and episodes, find new series, recently updated on Plex, missing episodes | Library → Local series | `/series/{id}` (episodes, missing, + wishlist) | `LocalSeriesController`, `NewSeriesController`, `RecentSeriesController` |
| Watchlists, mark watched (also on Plex), copy to / sync from Plex playlists | Library → Watchlists | `/watchlists/{list}` | `WatchlistController` |
| Wishlist (titles and episodes to get), CSV import/export | Library → Wishlist | `/wishlist` | `WishlistController` |
| My rating (half stars, sent to Plex), set missing IMDb rating, export/import | details window | movie page | `RatingController` |
| Subtitles: status, download from SubDL | details window | as tracks in the player | `SubtitleController`, `PlaybackSubtitles` |
| Play | opens the file | `/play/{id}`: the file in the browser, or the title on Plex | `PlaybackController` |
| Trending on TMDb this week | What's Up → Trending | `/trending` | `TrendingController` |
| Discover by season (Christmas, Halloween …) | What's Up → Discover | `/discover` | `DiscoverController` |
| Settings | File → Settings | `/settings` | `SettingsController` |
| API usage report | Help → API usage report | | `ApiUsageController` |
| Log | Help → Log | | `LogController` |

Every list in the WinForms app can be written to a CSV file that opens in Excel (**Write to Excel…**, Ctrl+S).

Not ported from SilverScreen: printing, WhatsApp and e-mail sharing, importing a friend's list from Excel, and the rating chart.

### Database collections added by Kino

Besides SilverScreen's collections (`Item`, `FileItem`, `WatchListItem`, `UserSettings`, `KolibriSeason`, `Episode` …),
Kino adds its own: `KinoSeason` and `KinoSeriesFetch` (cached episode lists), `KinoWishlist`, `KinoRating`,
`KinoTrending`, `KinoDiscover`, `KinoTmdbImdb` (TMDb → IMDb ids), `KinoFileDate`, `KinoIgnoredFolder` and `KinoApiUsage`.
The log is in its own file (`.logdb`, collection `KinoLog`). SilverScreen ignores them.

## Scanning a folder (SilverScreen's "Oppdater")

`MovieScanController` matches each video file to a movie, trying the cheapest source first:
1. an IMDb id in the file or folder name (`{imdb-tt…}`);
2. the local library, by title and year;
3. Plex, by title or original title and year (free; used when a Plex token is configured);
4. TMDb search (only a clear match: same title, year within one);
5. OMDb, by title and year.

If Plex or TMDb fails (server offline, key rejected), the scan skips it for the rest of the run and says so in the report.
For files the scan can't match, use **Find movie for file…**, or **Find movie…** in the scan report.
It searches your library, Plex and OMDb, and you pick the right movie. Titles and years come from both the file name
and the folder name. Series episodes are skipped.

If OMDb's daily limit is reached, the scan stops cleanly and keeps what it already linked. The next "Scan new files"
continues where it stopped.

Differences from SilverScreen, on purpose:
- An existing link to a file that still exists is never overwritten. The second file is reported as a duplicate.
- "Remove missing…" is a separate step with a confirmation, and it keeps the movie details.
- Cleaning up leftover files is configurable and asks first by default (see below).
- Plex never matches on only the first word of a title, as SilverScreen's fallback did.

## Cleaning up leftover files

After a scan, leftover files in the scanned movie folders can be deleted. **Clean up folder…** does the same for the whole folder.
They are deleted permanently, not moved to the Recycle Bin. Video files are never deleted. Settings are under `Kino:Cleanup`:

| Key | Default | |
|---|---|---|
| `AfterScan` | `Ask` | `Ask` lists the files and asks first; `Always` deletes without asking, like SilverScreen; `Never` skips the cleanup |
| `DeleteEmptyFolders` | `true` | Remove folders that are empty afterwards |
| `FilePatterns` | SilverScreen's list | `.nfo` matches the extension. `rus.srt` matches `Movie.rus.srt`, `Movie-rus.srt` and `Movie.rus.HI.srt`, but not `Walrus.srt` |

SilverScreen matched with a plain "ends with", so `ice.srt` (Icelandic) also deleted `Alice.srt`, and `tel.srt` deleted `Hotel.srt`.

## Plex

Plex is used when the Plex server name and token are set in Settings. Kino connects to `http://<server>:32400`.
It's used to match files in scans, to mark titles watched, to send your ratings, for "Recently updated" series, and
to copy watchlists to playlists and back. When the WinForms app starts, Plex playlists that aren't Kino watchlists yet
become watchlists of the same name.

## Series

A series is an `Item` with type `series`, linked to its **folder** (FileItem / TomatoUrl), as in SilverScreen.

- Episode lists come from the database first: Kino's own (`KinoSeason`), else what SilverScreen stored (`KolibriSeason` + `Episode`).
  Only a series neither has is fetched online, once (TMDb: the show and its seasons; without a TMDb key, OMDb one request per season).
  A series no one knows is asked about again only after 30 days. **Refresh episodes** fetches it again.
  SilverScreen requested every episode from OMDb separately; Kino never does.
- Files in the series folder are matched to episodes by name: `S01E02`, `S01E01E02`, `S01E01-E03`, `1x02`, and
  `E06` / `Episode 6` with the season taken from the folder (`Season 2`, `S02`, `Show 02`).
- **Find new series…** looks for folders nothing in the library is linked to. It matches only when that's safe
  (an IMDb tag in the folder name, a series in the library with the same title and year, or TMDb/OMDb by title and year)
  and asks about the rest. Nothing on disk is changed, except removing an IMDb tag when you ask.
- **Missing episodes…** lists aired episodes you have no file for, ongoing series first.

## Image cache (SilverScreen.imgdb)

Kino and SilverScreen (`ImageCacheDB` in Kolibri.net) share this layout:
collection `Image`, `_id` = key string (IMDb id, poster URL or name), `Data` = JPEG/PNG bytes.

The old layout (`ImageBase`, `_id = key.GetHashCode()`, base64 BMP) got a new key every time the app
started on .NET Core. Every lookup missed and the image was stored again. Convert an old file with:

```
dotnet run --project tools/Kolibri.Kino.ImageDbMigrator -c Release -- C:\TEMP\SilverScreen\SilverScreen.imgdb
```

It reads the source read-only and writes `SilverScreen.migrated.imgdb` next to it. Close both apps, then
rename the old file to `.bak` and the migrated file to `SilverScreen.imgdb`.

## Web app (Kolibri.Kino.Blazor)

A Blazor Web App (interactive server rendering): the pages run on the server and call the same controllers as
the WinForms windows, so there's no separate API. The browser only shows HTML and sends clicks over a SignalR connection.

| Page | |
|---|---|
| Library (`/`) | Search the library or OMDb, filter by type, genre, person and watched, sort, import. Recently added shelf. The search is in the address, so Back works |
| Recently added (`/recent`) | Library items in the order their files arrived |
| Movie / series (`/movie/{imdbId}`, `/series/{imdbId}`) | Details, import/update, watchlist, wishlist, mark watched, my rating, similar movies; for a series its episodes and what's missing |
| Play (`/play/{imdbId}`) | The library's file in the browser with its subtitles, or the title on Plex. For a series, its episodes |
| Trending (`/trending`), Discover (`/discover`) | As What's Up in WinForms |
| Watchlists (`/watchlists/{list}`) | Filter, mark watched, remove (optionally from Plex), new list, CSV export, copy to Plex |
| Wishlist (`/wishlist`) | Search, add, remove, CSV download (imported in WinForms with **Import CSV…**) |
| Settings (`/settings`) | Including Test connections, and export/import of ratings |

Other endpoints (`KinoEndpoints.cs`): `/poster/{imdbId}` (from the image cache, or downloaded once and stored there),
`/stream/{imdbId}` and `/subtitle/{imdbId}` (only files the library links to), `/export/watchlist`, `/export/wishlist`,
`/export/ratings`, and the Trending and Discover images.

At start, two background services warm up: `RecentlyAddedWarmUp` reads the files' dates (minutes the first time on a NAS
with cold disks) and `WatchedWarmUp` loads Plex's movie list for the Watched filter.

Not in the web app: Local movies, scanning, cleanup, Find new series, the Missing episodes report, the API usage report and the log.

### Settings user

Settings are stored per Windows user name (`_id` of the `UserSettings` document). In a container the user is
`app`, so set `Kino:SettingsUser` (environment variable `Kino__SettingsUser`) to your Windows user name to use
the keys, Plex token and favorite watchlist you saved on Windows. Empty means the current user.

### Ratings on two machines

The NAS and the PC each have their own copy of the library, and they aren't kept in step. Export your ratings on one
(Settings) and import them on the other: a newer rating wins, and the added or changed ones are sent to Plex.

### Running on the UGREEN NAS (Docker)

The DH4300 Plus has an ARM CPU, so the image is built for `linux/arm64`. The SDK cross-compiles on your PC,
so there's no emulation. Everything Kino uses is plain .NET (LiteDB, OMDbApiNet, TMDbLib), so it runs on Linux as is.

1. On the NAS: install **Docker** from the App Center, enable SSH (Control Panel → Terminal), and create the folder
   `docker/kolibri-kino/data`.
2. On the PC (Docker Desktop): `.\deploy\build-nas-image.ps1` → `deploy\out\kolibri-kino.tar`.
3. Copy `kolibri-kino.tar`, `deploy/compose.yaml` and `deploy/.env.example` (renamed `.env` and filled in) to
   `docker/kolibri-kino`. Copy `SilverScreen.db` and `SilverScreen.imgdb` to `docker/kolibri-kino/data`.
4. Over SSH: `cd /volume1/docker/kolibri-kino && docker load -i kolibri-kino.tar && docker compose up -d`
5. Open `http://<nas-ip>:8085`. Logs: `docker compose logs -f kino`.

To update, rebuild the image, copy it again, and run step 4 again. The data folder is kept.

**Only one machine may use a library file at a time.** LiteDB's shared mode coordinates programs on one computer,
not across the network. Don't point SilverScreen or Kino for Windows at the NAS's `SilverScreen.db` over SMB while the
container runs. Use a copy on each side, or stop the container first.

The app has no login. Keep it on your home network, or reach it from outside through a VPN such as Tailscale, not an open port.
Plex is reached as `http://<Plex server name>:32400` from inside the container. If the NAS can't resolve that
name, use the Plex server's IP address in Settings.

## Library storage: LiteDB file or MariaDB through KMDb

WinForms and the web app keep the library in MariaDB through Kolibri.Kino.KMDb when they can, and in the LiteDB file
when they can't. The choice is made in Data, under the repositories, so controllers and pages don't change.

- **The rule:** KMDb is used when **KMDb URL** (Settings → Uranus) and the **Uranus token** (KMDb's key) are set,
  `/ping` answers and the key is accepted (`GET /silverscreen`). Otherwise the LiteDB file. Checked at start (WinForms
  waits up to 8 s), after a settings change and every minute (`KmdbConnection.RunAsync`; the web app hosts it as
  `KinoStorage`). In Docker, `Kino__KmdbUrl` (compose: `http://kmdb:8080`) is used instead of the setting, since the
  container can't look up greenlantern by name.
- **Settings and the log always stay in the LiteDB files** (the KMDb URL itself is a setting). The image cache follows
  the library (`SwitchingImageStore`): through KMDb an IMDb id finds the best poster (Kino, Kodi, TMDb, Plex), and images
  from KMDb are kept in `SilverScreen.imgdb` too.
- **How:** every repository gets its collections from `KinoDatabase.Collection(name)`: a `SwitchingDocumentCollection`
  over `LiteDocumentCollection` (the file) and `KmdbDocumentCollection` (HTTP to `/silverscreen`). Documents travel as
  plain JSON (`BsonJson`: dates as ISO UTC text, turned back into dates on reading) and are mapped with the same
  `BsonMapper` as the file, so models read the same from both.
- **While KMDb is in use** reads come from KMDb (whole-collection reads kept 30 s) and writes go to KMDb **and** the
  LiteDB file, which so stays a current fallback. Every ten minutes what changed in KMDb (also from the other machine)
  is copied into the file (`GET …?since=`); the first time only notes the time, so an older MariaDB copy never
  overwrites newer data in the file.
- **While it isn't** (or the moment a call fails) the LiteDB file is used and each change is noted in
  `KinoPendingSync` (in the same file). When KMDb answers again they are sent: the document as it is in the file, or its
  removal. Without a KMDb URL nothing is noted.
- **Shown:** WinForms' status bar (right: "Library: MariaDB (KMDb)" / "Library: LiteDB file", details in the tooltip), the
  web app's Settings (box *Library*, with *Check KMDb now*), and *Test connections* (first line *Library*, which says
  MariaDB through KMDb is the better choice when it is available).
- **KMDb's side:** `PUT /silverscreen/{collection}` takes many documents at once (file dates come in thousands), a
  collection KMDb doesn't have yet is made on the first write, and images Kino sends are stored with their title and
  type. The copy tool leaves `KinoPendingSync` and `KinoSyncState` in the file.
- **Details page colours:** `/backdrop/{id}` serves the title's backdrop (else landscape art) from KMDb; `kino-art.js`
  takes two colours from it (else from the poster) for a soft gradient behind the details, and shows the backdrop
  itself blurred and faded (20 %, 13 % in light mode), so the text stays easy to read.

## KMDb: OMDb's API on the IMDb database

`src/Kolibri.Kino.KMDb` is a small web API that answers like http://www.omdbapi.com/, from IMDb's data sets
(https://datasets.imdbws.com/) loaded into MariaDB on HTPC. Its answers are OMDbApiNet's own models (`Item`, `Episode`,
`Season`, `SearchList`) written with OMDb's JSON names, so code that reads OMDb reads KMDb as it is; only the address
changes. It has no OMDb day limit. It only reads the database.

| Request | Answer |
|---|---|
| `/?i=tt0133093` | `Item` (an episode id gives an `Episode` with `Season`, `Episode` and `seriesID`) |
| `/?t=The Matrix&y=1999&type=movie` | `Item`: the exact title (or a Norwegian, Swedish or Danish title) with the most votes |
| `/?s=matrix&type=series&y=…&page=2` | `SearchList`, 10 per page, most votes first |
| `/?i=tt0903747&Season=1` | `Season` with its episodes and their ratings |
| `/?i=tt0903747&Season=5&Episode=14` (also with `t=`) | `Episode` |

The same lookups have their own routes for the Swagger page (`/swagger`): `/title/{id}`, `/find?t=`, `/search?s=`,
`/series/{id}/season/{n}` and `/series/{id}/season/{n}/episode/{e}`. Errors are OMDb's: `{"Response":"False","Error":"Movie not found!"}`.

- **Key:** every request needs the key, as OMDb's `apikey=` or the `X-Api-Key` header (Swagger: *Authorize*). It is the
  same key as the Uranus web service (`kino_config.php`, `UranusApiKey` in the settings). `/swagger` and `/health` are open.
- **Search** matches the start of a title, also after The/A/An ("matrix" finds The Matrix), and Nordic alternative
  titles ("Flåklypa" finds The Pinchcliffe Grand Prix). `LIKE '%word%'` would read all 12.8 million titles (≈18 s on HTPC);
  a start match takes well under a second. Without `type=` single episodes are left out. Over 50,000 hits ("the") is
  "Too many results.", as at OMDb.
- **Not in IMDb's data sets**: Plot, Poster, Rated, Released, Language, Country, Awards, Metascore. They come from
  the SilverScreen library when it has the title (see below), else `N/A`. A series has no Director (as at OMDb). Actors come from `title_principals`, which on HTPC holds the cast for the
  library's titles only (`kolibri_titles`); other titles have Actors `N/A`.
- **Database:** the HTPC tables follow the files (`title_basics`, `title_ratings`, `title_crew`, `title_episode`,
  `title_akas`, `title_principals`, `name_basics`, keyed by `tconst`/`nconst`). The user needs only SELECT on `imdb`.
- **Settings:** `ConnectionStrings__Imdb` (or `IMDB_CONNECTION`) and `Kmdb__ApiKey` (or `KMDB_API_KEY`). Locally, keep
  them in user secrets, never in appsettings:
  `dotnet user-secrets set "ConnectionStrings:Imdb" "…" --project src/Kolibri.Kino.KMDb` (and `"Kmdb:ApiKey"`), then
  `dotnet run --project src/Kolibri.Kino.KMDb` → http://localhost:5086/swagger.
- **On the NAS** it is the `kmdb` service in `deploy/compose.yaml` (port 8086). `.\deploy\deploy-to-nas.ps1` builds and
  copies both images (`-App kmdb` only KMDb) and adds `KMDB_PORT` and `KMDB_API_KEY` (copied from `KINO_WEB_API_KEY`) to
  the NAS's `.env`. `IMDB_CONNECTION` must be filled in there by hand, in single quotes. The container reaches HTPC's
  MariaDB by IP address. Swagger: http://greenlantern.local:8086/swagger.
- Data from IMDb is for personal, non-commercial use (https://developer.imdb.com/non-commercial-datasets/).

### Monitoring: /ping, /health, /metrics and Grafana

All three answer without the key.

| Endpoint | Answers |
|---|---|
| `/ping` (GET/HEAD) | `{"status":"ok","version":…,"uptimeSeconds":…}`. Never touches the databases: "the service runs". |
| `/health` | Both databases checked now, as JSON with status, time and error per check. **Healthy** 200; **Degraded** 200 when the SilverScreen library is down (OMDb still works); **Unhealthy** 503 when the IMDb database is down. |
| `/metrics` | Prometheus/OpenMetrics text from OpenTelemetry, for Prometheus or Grafana Alloy to scrape. |

The metrics in `/metrics`:
- KMDb's own: `kmdb_health_status{check="imdb|silverscreen"}` (1 healthy, 0.5 degraded, 0 unhealthy; the health checks
  run every 30 s in the background), `kmdb_health_check_duration_seconds`, `kmdb_omdb_requests_total{kind,result}`,
  `kmdb_library_enriched_total{kind}`, `kmdb_sync_documents_total{collection,changed}`, `kmdb_library_writes_total`,
  `kmdb_images_served_total{found}`, `kmdb_library_rows{collection}`, `kmdb_library_copy_age_seconds{collection}`,
  `kmdb_uptime_seconds`.
- ASP.NET Core: `http_server_request_duration_seconds` per route and status, Kestrel connections.
- MariaDB connections (MySqlConnector): `db_client_connections_*{pool_name="imdb|silverscreen"}`.
- .NET runtime: memory, GC, CPU, thread pool.

With `OTEL_EXPORTER_OTLP_ENDPOINT` set, metrics and traces (requests and SQL commands) also go out over OTLP, for
example to Grafana Alloy, Tempo or Grafana Cloud.

**Grafana:** `deploy/monitoring/grafana/dashboards/kmdb.json` is a ready dashboard: status, API, OMDb lookups, library
and sync, MariaDB connections and runtime. Import it in any Grafana with a Prometheus that scrapes
`http://greenlantern:8086/metrics`, or start the bundled ones: set `COMPOSE_PROFILES=monitoring` and
`GRAFANA_ADMIN_PASSWORD` in the NAS's `.env`, then deploy. That adds Prometheus (port 9090, 30 days of data) and
Grafana (http://greenlantern.local:3000) with the data source and the dashboard already set up (folder *Kolibri*).
The deploy script copies `deploy/monitoring` to the NAS.

### The SilverScreen library in MariaDB

The library is copied to the MariaDB database `silverscreen` on HTPC, one table per LiteDB collection with the same
name (`Item`, `Episode`, `FileItem`, `WatchListItem`, `KinoRating`, `KinoSeason`, `Image`, `Log` …). Each field is a
column; nested data (`Episodes`, `Cast`, `Genres`) is JSON text, images are LONGBLOB, dates are UTC. The plan is to move
Kino from LiteDB to this, keeping LiteDB as the fallback when MariaDB can't be reached.

- **Copy:** `dotnet run --project tools/Kolibri.Kino.SilverScreenCopy` copies `C:\TEMP\SilverScreen` (through a copy in
  `E:\DEV\tmp\silverscreen-copy`) with the user's `DefaultConnection`, database `silverscreen`. Each table is built as
  `<name>__new` and swapped in with one RENAME. It skips a table where KMDb has written rows since the last copy;
  `--replace` overwrites those too. `--only Item,Episode` copies some collections only. The whole library takes about a minute.
- **Images by title and type:** `Image` has, besides LiteDB's `_id`/`Added`/`Data`, the columns `ImdbId`, `ImageType`
  (poster, backdrop, still, profile, avatar, landscape, thumb, clearlogo, clearart, banner, discart, keyart, characterart, icon),
  `Season`/`Episode` (stills), `TmdbId` (people), `Source` (kino, kodi, tmdb, plex, omdb, web), `SourceUrl`, `Width`, `Height`.
  - `… -- images classify` fills them for Kino's cache from its keys (`tt…` poster, `tt…_S01E02` still, `tmdb-person-…`
    profile) and by matching cached addresses to the library (Movie, TvShow and its seasons, KinoSeason, Credits and
    SilverScreen's base64 credits/reviews, Item, Episode, WatchListItem, KinoWishlist, KinoSimilar, KinoDiscover/Trending;
    TMDb addresses by file name). Images that belong to nothing in the library are removed from MariaDB (not from LiteDB).
    A copy that brings a new Image table runs it by itself.
  - `… -- images kodi [--kodi \\HTPC\Shared\Kodi_Cache]` adds the art of Kodi library exports (videodb.xml with
    movies/Title_Year-poster/fanart/landscape/thumb/clearlogo.jpg) for the titles in Item, matched by title and year to
    the export's IMDb id; the newest export wins, Kodi's fanart is a backdrop. 2026-11-06: 5,050 images (643 MB). Kodi's
    other art (banner, clearart, discart, keyart, shows' art) is only addresses in videodb.xml, not files.
- - **Rights:** `asoes` needs `ALL` on `silverscreen` (the copy creates tables; KMDb adds columns for new fields).
- **Art (KMDb, no key):** `/silverscreen/art/{imdbId}` lists a title's images (type, source, size, address);
  `/silverscreen/art/{imdbId}/{type}` (`?season=&episode=` for stills) serves the best one, preferring kino, kodi, tmdb,
  plex, omdb, web. `/silverscreen/poster/{id}` uses the same lookup (an episode key `tt…_S01E02` gives its still).
- **Reading (KMDb):** `GET /silverscreen` (collections, rows, copy time), `/silverscreen/{collection}?page=&pageSize=&field=&value=&since=`,
  `/silverscreen/{collection}/{id}`. Images: `/silverscreen/poster/{imdbId}` and `/silverscreen/image/{id}`, with no key. A poster missing from the cache is fetched from the library's TMDb poster (Movie/TvShow, image.tmdb.org, no key) or another public address it has (never Plex) and kept in the cache; last from Plex when KMDb has the Plex token (`KMDB_PLEX_TOKEN` in `.env`, `Kmdb:PlexToken` in user secrets locally; sent to Plex in the `X-Plex-Token` header and resized by Plex to 500×750, never handed out)
  so they work in `<img src>`. `UserSettings` is never served, and `X-Plex-Token` is removed from every address.
- **OMDb answers** are filled in from `Item` and `Episode` where IMDb lacks a value, and season episodes get their
  release dates from `KolibriSeason`. IMDb's own values (title, year, rating, votes, crew) stay, since they are current.
  The poster becomes `/silverscreen/poster/{id}` when the image cache has one; Plex poster addresses are never handed out.
  A title IMDb's files don't have yet is answered from `Item`. Many library items came from Plex, with dates as
  `1999-03-31` and Plex ids in Metascore; dates are turned into OMDb's `31 Mar 1999` and Metascores outside 0–100 are left out.
- **Writing, for syncing WinForms and the web app** (key required):
  - `PUT /silverscreen/{collection}/{id}` replaces a document: a change the user made. Fields it leaves out become empty;
    binary fields are kept. A field the table lacks becomes a new column.
  - `POST /silverscreen/{collection}/sync` takes up to 1,000 documents and merges each with the stored one. What one side
    lacks (null, "", "N/A", an empty list) is filled from the other. When both have `Updated`, `Fetched`, `Checked` or
    `Added`, the newer one's values win; otherwise stored values stay. It returns the merged documents for the client
    to store in its own LiteDB, so both sides end up the same.
  - `GET …?since=<UTC time>` lists what changed (KMDb keeps `_modified` per row; copied rows count from the copy time).
  - `DELETE /silverscreen/{collection}/{id}`, and `PUT /silverscreen/image/{id}` (or `…/{collection}/{id}/{field}`) with
    the image bytes as the body.
  - Deletes are not remembered, so `since=` doesn't list them; a client that was offline doesn't learn about them.
