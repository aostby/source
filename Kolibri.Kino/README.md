# Kolibri.Kino

A rewrite of Kolibri.SilverScreen as a layered, async WinForms app on .NET 10.
It uses the same LiteDB library file as SilverScreen (`C:\TEMP\SilverScreen\SilverScreen.db`).

## Layers

```
Kolibri.Kino.WinForms      UI: forms only. Calls controllers, never data classes.
Kolibri.Kino.Blazor        UI: web pages (Blazor Server), same rules. Runs in a container on the NAS.
        │
Kolibri.Kino.Controllers   Use cases (search, import, delete). Depends on Core abstractions only.
        │
Kolibri.Kino.Core          Interfaces + options. OMDbApiNet.Model.Item is the shared model.
        ▲
Kolibri.Kino.Data          LiteDB repository + OMDb client. Implements Core interfaces.
```

The WinForms and Blazor projects also reference Data, but only to wire up dependency injection in `Program.cs`.

## Rules for porting old code

- A form gets its controller through its constructor and only `await`s controller methods.
- Every I/O method is `async Task<...>` and takes a `CancellationToken`. No `.Result` or `.Wait()`.
- LiteDB and HTTP code live in `Kolibri.Kino.Data`, behind an interface in `Kolibri.Kino.Core`.
- New features get a test in `tests/Kolibri.Kino.Tests`. Tests use temporary databases, never the real library.

## Windows

`KinoForm` is the start window: it searches the library and OMDb and opens the other windows. It's named
`KinoForm` rather than `MainForm` to leave room for an MDI main window with menus later.

| Window | Port of SilverScreen's |
|---|---|
| Local movies | ShowLocalMoviesForm, plus Oppdater (scan), cleanup and "Find movie for file" (MovieForm's file lookup) |
| Local series | ShowLocalSeriesForm + DetailsFormSeries (one tab per season) |
| Watchlists | WatchlistForm |
| Settings | the "Innstillinger" property grid |

## Icon

`assets/kino.svg` is the "Movie" icon from https://www.iconpacks.net/free-icon/movie-850.html. It's free for
personal and commercial use, and no attribution is required. `assets/make-icon.cs` builds `assets/kino.ico` from it
in 9 sizes, from 16 to 256 px:

```
cd assets
dotnet run make-icon.cs
```

The `.ico` is the `.exe` icon (`ApplicationIcon`). It's also embedded and set on every window (`Controls/AppIcon.cs`).

## Configuration

`src/Kolibri.Kino.WinForms/appsettings.json`, section `Kino`, holds only what belongs to this installation:

| Key          | Default |
|--------------|---------|
| `LiteDbPath` | `C:\TEMP\SilverScreen\SilverScreen.db` |
| `ImageDbPath` | empty: the file next to `LiteDbPath` with the extension `.imgdb`, as in SilverScreen |
| `Cleanup` | see below |

## User settings (Settings window)

API keys (OMDb, TMDb, SubDL), the Plex server and token, folders and the favorite watchlist are stored in the
database's `UserSettings` document (`_id` = Windows user name), shared with SilverScreen. Edit them in
**Settings…**. **Test connections** tries the OMDb key, the TMDb key and the Plex token before you save.
New values apply at once; there's no need to restart.

When Kino saves, fields it doesn't know are kept. Settings left empty are removed rather than stored as empty,
so SilverScreen's own defaults apply to them.

## Scanning a folder (SilverScreen's "Oppdater")

`MovieScanController` matches each video file to a movie, trying the cheapest source first:
1. an IMDb id in the file or folder name (`{imdb-tt…}`);
2. the local library, by title and year;
3. Plex, by title or original title and year (free; used when a Plex token is configured);
4. TMDb search (only a clear match: same title, year within one);
5. OMDb, by title and year.

If Plex or TMDb fails (server offline, key rejected), the scan skips it for the rest of the run and says so in the report.
For files the scan can't match, use **Find movie for file…**, or **Find movie…** in the scan report.
It searches your library, Plex and OMDb, and you pick the right movie.

Titles and years come from both the file name and the folder name. Series episodes are skipped.

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
It loads all movie libraries once per scan, and again when the token changes (one request per library),
and keeps the movies that have an IMDb id.

## Series

A series is an `Item` with type `series`, linked to its **folder** (FileItem / TomatoUrl), as in SilverScreen.
When you select a series in **Local series**:

- Season and episode details are fetched **once** and cached in `KinoSeason`, so the next time is instant.
  **Refresh episodes** fetches them again.
  - With a TMDb key: the show is found by IMDb id, then one request per season. You get plots, air dates, scores and screenshots.
  - Without one: OMDb, one request per season (titles and ratings).
  - SilverScreen requested every episode from OMDb separately; Kino never does.
- If nothing can be fetched, Kino shows what SilverScreen stored (`KolibriSeason` + `Episode`), read-only.
- Files in the series folder are matched to episodes by name:
  - `S01E02`, `S01E01E02`, `S01E01-E03`, `1x02`;
  - `E06` / `Episode 6` with the season taken from the folder (`Season 2`, `S02`, `Show 02`).
- Episodes on disk are green and playable. Files for episodes the list doesn't have get their own rows.
  Files without such a name (extras) are counted in the status bar.

Not ported yet: adding new series from a folder (SilverScreen's OMDBSearchForSeriesForm).

## Watchlists

Stored in the `WatchListItem` collection with `_id` = IMDb id, as in SilverScreen, so a movie is on one watchlist
at a time. Adding it to another list moves it. An empty list is a placeholder entry without a movie.
Kino works on the stored documents, so fields it doesn't use are kept, such as SilverScreen's `Picture` bytes.

**Copy to Plex playlist** adds the list's movies to the Plex playlist of the same name and creates it if missing.
Movies already in the playlist are skipped. When you remove a movie, Kino asks whether to remove it from the
Plex playlist too.

Not ported yet: printing, the Excel-interop export (**Export CSV…** opens in Excel without Office being required),
WhatsApp and e-mail sharing, importing a friend's list from Excel, and the rating chart.

OMDb's free key allows 1,000 requests a day. If the limit is reached, the scan stops cleanly and keeps
what it already linked. The next "Scan new files" continues where it stopped.

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

| Page | Port of |
|---|---|
| Library (`/`) | KinoForm: search the library or OMDb, filter by type, sort, import. The search is in the address, so Back works |
| Movie (`/movie/{imdbId}`) | MovieDetailsForm and KinoForm's menu: details, import/update, remove, add to a watchlist, IMDb/TMDb links |
| Watchlists (`/watchlists/{list}`) | WatchlistsForm: filter, mark watched, remove (optionally from Plex), new list, CSV export, copy to Plex |
| Settings (`/settings`) | SettingsForm, including Test connections |

Posters are served from `/poster/{imdbId}`. They come from the image cache, or are downloaded once and stored there.
Not ported yet: Local movies, Local series, scanning and cleanup. The paths in the library are Windows paths
(`\\GREENLANTERN\Multimedia\…`), so these need a way to map them to the folders mounted in the container first.

Run it on Windows against the library in `appsettings.Development.json`:

```
dotnet run --project src/Kolibri.Kino.Blazor --launch-profile http
```

### Settings user

Settings are stored per Windows user name (`_id` of the `UserSettings` document). In a container the user is
`app`, so set `Kino:SettingsUser` (environment variable `Kino__SettingsUser`) to your Windows user name to use
the keys, Plex token and favorite watchlist you saved on Windows. Empty means the current user, as before.

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
