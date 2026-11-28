# Kolibri Kino – storage overview

Where Kolibri Kino (WinForms on the PC, Blazor in Docker on the NAS GREENLANTERN) keeps its data.
Written 2026-11-28 from the README, the code and a look at the NAS and KMDb.

## 1. The library: two back ends behind one interface

### LiteDB file – `SilverScreen.db`
- Shared with the old Kolibri.SilverScreen app (same format). Kino keeps fields it doesn't know when it saves.
- Default `C:\TEMP\SilverScreen\SilverScreen.db`; the real path is the `LiteDBFilePath` setting (in `UserSettings`).
- **JUGGERNAUT:** the desktop app's own copy.
- **NAS:** `/volume1/docker/kolibri-kino/data/` (`\\GREENLANTERN\docker\kolibri-kino\data`), used by the web container.
- Files next to it:
  - `SilverScreen.imgdb` – image cache
  - `SilverScreen.logdb` – log
  - `*-log.db` – LiteDB's own journal
- Only one machine may open a file at a time (LiteDB's shared mode doesn't work across the network).

### MariaDB through KMDb (`src/Kolibri.Kino.KMDb`)
- MariaDB runs on **HTPC**. KMDb is a web API at `http://greenlantern:8086` (`http://kmdb:8080` inside Docker), Swagger at `/swagger`.
- Library documents: `/silverscreen/{collection}` and `/silverscreen/{collection}/{id}` – GET, PUT, DELETE, `?since=` for
  changes, plus `/sync` and image routes.
- Key: `X-Api-Key` header or `apikey=`. Same as the Uranus token (`UranusApiKey`, `kino_config.php`), and
  `KMDB_API_KEY` in the NAS `.env`.
- **When KMDb is used:** KMDb URL and Uranus token are set, `/ping` answers and the key is accepted. Checked at start,
  after a settings change and every minute. In Docker `Kino__KmdbUrl` overrides the setting.
- **While KMDb is in use:**
  - reads come from KMDb (whole-collection reads cached 30 s)
  - writes go to KMDb **and** the LiteDB file
  - every 10 minutes, changes in KMDb are copied into the file
- **When KMDb is down:** the file is used and changes are queued in `KinoPendingSync` (sync state in `KinoSyncState`);
  they are sent when KMDb answers again.
- **Shown:** WinForms status bar ("Library: MariaDB (KMDb)" / "Library: LiteDB file", details in the tooltip) and the
  web app's Settings → Library box.
- **Code:** `KinoDatabase.Collection(name)` → `SwitchingDocumentCollection` over `LiteDocumentCollection` and
  `KmdbDocumentCollection` (`src/Kolibri.Kino.Data/Storage/`).

## 2. Library collections

| Collection | Holds | Key |
|---|---|---|
| `Item` | Movie and series details (OMDb `Item` model) | IMDb id |
| `FileItem` | The file or series folder linked to each title (`FullName`, Windows UNC path) | IMDb id |
| `Episode` | SilverScreen's episodes | |
| `KolibriSeason` | SilverScreen's seasons (OMDb format) | `tt…_N` |
| `KinoSeason` | Kino's seasons and episodes (from TMDb or OMDb, with `Fetched`) | `tt…_N` |
| `KinoSeriesFetch` | When a series was last fetched, whether it was found, TmdbId | IMDb id |
| `KinoIgnoredFolder` | Series folders you chose to ignore | |
| `KinoFileDate` | File dates | |
| `WatchListItem` | Watchlists | |
| `KinoWishlist` | Wishlist | |
| `KinoRating` | Your ratings | |
| `KinoSimilar` | Cached similar titles | |
| `KinoTrending`, `KinoDiscover` | TMDb trending and discover caches | |
| `KinoTmdbImdb` | TMDb id → IMDb id (`movie:{id}` / `tv:{id}`) | |
| `KinoApiUsage` | API usage counts | |
| `UserSettings` | Settings – **always in the LiteDB file** | |
| `Log` | Log – **always in LiteDB** (`.logdb`) | |
| `KinoPendingSync`, `KinoSyncState` | KMDb sync bookkeeping – file only | |

"In library" on a details page only means the title is in `Item`. Episodes on a series page need a `FileItem` link to
the series folder.

## 3. Image cache
- `SilverScreen.imgdb` (`LiteDbImageStore`, `SwitchingImageStore`).
- Keys: IMDb id; `tt…_S01E02` for an episode; `tmdb-person-{id}`; `tmdb-movie-{id}` / `tmdb-tv-{id}` for titles
  without an IMDb id; or an address.
- While KMDb is in use, images come from KMDb (Kino, Kodi, TMDb or Plex) and are also kept in `.imgdb`.
- `tools/Kolibri.Kino.ImageDbMigrator` converts the old cache format.

## 4. The IMDb database (MariaDB on HTPC)
- Database `imdb`: `title_basics`, `title_ratings`, `title_crew`, `title_episode`, `title_akas`, `title_principals`,
  `name_basics`, `kolibri_titles`.
- KMDb only reads it and answers like OMDb. Connection: `ConnectionStrings__Imdb` / `IMDB_CONNECTION` (NAS `.env`, in
  single quotes).
- The library tables in MariaDB are a separate pool (`silverscreen`). `tools/Kolibri.Kino.SilverScreenCopy` copies
  `.db`, `.imgdb` and `.logdb` into them.

## 5. Media files (read-only)
- Shares: `\\GREENLANTERN\Movies`, `\\GREENLANTERN\Multimedia`, `\\GREENLANTERN\Series`.
- In the container: `/media/Movies`, `/media/Multimedia`, `/media/Series`. `Kino__PathMappings` in `compose.yaml`
  translates the stored Windows paths.
- "Find new series" only looks at folders **directly** under the series root; tag them `{imdb-tt…}` to link
  automatically.

## 6. Deployment and config
- NAS folder `/volume1/docker/kolibri-kino`: `compose.yaml`, `.env` (`KINO_WEB_API_KEY`, `KMDB_API_KEY`, `KMDB_PORT`,
  `IMDB_CONNECTION`), `data/`, `monitoring/` (Prometheus and Grafana in Docker volumes).
- `deploy\deploy-to-nas.ps1 -WithDatabases` stages the database files in `data-new\` and swaps them in after the
  container has stopped.
- On the PC, KMDb's secrets go in user secrets, never in `appsettings`.

## 7. Outside the library
- **Uranus:** Kino's web service at `problembar.net/uranus/apps/KolibriKino/` (`kino_config.php`).
- **APIs:** OMDb, TMDb, SubDL, Plex (`http://<server>:32400` with a token).

## Open issues (2026-11-28)
- **Daredevil: Born Again (tt18923754)** shows no episodes on the web: there's no `FileItem` link in KMDb. The folders
  are nested (`\\GREENLANTERN\Series\Daredevil\Daredevil.Born.Again {imdb-tt18923754}` and
  `…\Daredevil\Daredevil {imdb-tt3322312}`), so "Find new series" doesn't see them.
- **Not yet confirmed:** whether JUGGERNAUT's desktop app writes to KMDb at all. Check its status bar.
- **On hold:** titles with only a TMDb id (e.g. Surviving Helmand (2021), `{tmdb-897631}`) aren't shown, since
  everything is keyed on the IMDb id.
