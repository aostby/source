using System.Xml.Linq;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Data;

/// <summary>
/// The Plex server from the user settings (XPlexServerName, XPlexToken): its movies, keyed by IMDb id, and its
/// video playlists. Port of Kolibri.net's PlexController (loading/lookup and the playlist methods).
/// </summary>
/// <remarks>
/// Movies are loaded on first use, on <see cref="RefreshAsync"/>, and again when the server or token changes in
/// Settings: one request for the section list and one per movie section. Movies without an IMDb guid are ignored,
/// as in SilverScreen. SilverScreen's title fallback (first word only) is not ported: it linked files to the wrong movie.
/// </remarks>
public sealed class PlexLibrary(HttpClient http, ISettingsStore settings) : IMediaServerLibrary, IMediaServerPlaylists, IMediaServerSeries
{
    private readonly SemaphoreSlim _loadLock = new(1, 1);
    private Loaded? _loaded;
    private (Connection Connection, string Identifier)? _machine;
    private (Connection Connection, IReadOnlyDictionary<string, string> ByImdbId)? _shows;

    public bool IsConfigured => CurrentConnection() is not null;

    public async Task RefreshAsync(CancellationToken ct = default)
    {
        var connection = CurrentConnection() ?? throw new InvalidOperationException("Plex is not configured.");
        await _loadLock.WaitAsync(ct).ConfigureAwait(false);
        try
        {
            _loaded = new Loaded(connection, await LoadAsync(connection, ct).ConfigureAwait(false));
        }
        finally
        {
            _loadLock.Release();
        }
    }

    public async Task<Item?> FindByImdbIdAsync(string imdbId, CancellationToken ct = default) =>
        (await MoviesAsync(ct).ConfigureAwait(false))?.GetValueOrDefault(imdbId)?.Item;

    public async Task<string?> FindPosterAsync(string imdbId, CancellationToken ct = default) =>
        await FindByImdbIdAsync(imdbId, ct).ConfigureAwait(false) is { } item ? PosterSized(item.Poster) : null;

    /// <summary>
    /// The thumb through Plex's photo transcoder at about 600×900: the thumb itself is the full original (often over
    /// 1 MB). Null for no poster.
    /// </summary>
    internal static string? PosterSized(string? thumbUrl)
    {
        if (!Uri.TryCreate(thumbUrl, UriKind.Absolute, out var thumb)) return null;
        var token = thumb.Query.TrimStart('?');
        return $"{thumb.GetLeftPart(UriPartial.Authority)}/photo/:/transcode?width=600&height=900&minSize=1&upscale=0" +
               $"&url={Uri.EscapeDataString(thumb.AbsolutePath)}{(token.Length > 0 ? "&" + token : "")}";
    }

    public async Task<Item?> FindMovieByTitleAsync(string title, int? year, CancellationToken ct = default)
    {
        if (await MoviesAsync(ct).ConfigureAwait(false) is not { } movies) return null;

        var wanted = title.Trim();
        var sameTitle = movies.Values
            .Where(m => string.Equals(m.Item.Title, wanted, StringComparison.OrdinalIgnoreCase)
                     || string.Equals(m.OriginalTitle, wanted, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (year is null) return sameTitle.Count == 1 ? sameTitle[0].Item : null;

        return (sameTitle.FirstOrDefault(m => m.Year == year) ?? sameTitle.FirstOrDefault(m => Math.Abs((m.Year ?? 0) - year.Value) == 1))?.Item;
    }

    public async Task<IReadOnlyList<string>> GetWatchedImdbIdsAsync(CancellationToken ct = default) =>
        (await MoviesAsync(ct).ConfigureAwait(false))?.Values.Where(m => m.Watched).Select(m => m.Item.ImdbId).ToList() ?? [];

    /// <summary>Plex's "mark as played" (/:/scrobble), for the user the token belongs to.</summary>
    public async Task<MarkWatchedOutcome> MarkWatchedAsync(string imdbId, CancellationToken ct = default)
    {
        var connection = CurrentConnection() ?? throw new InvalidOperationException("Plex is not configured.");
        if ((await MoviesAsync(ct).ConfigureAwait(false))?.GetValueOrDefault(imdbId) is not { } movie) return MarkWatchedOutcome.NotOnServer;
        if (movie.Watched) return MarkWatchedOutcome.AlreadyWatched;
        await SendAsync(connection, HttpMethod.Get,
            $"/:/scrobble?identifier=com.plexapp.plugins.library&key={Uri.EscapeDataString(movie.RatingKey)}", ct).ConfigureAwait(false);
        movie.Watched = true;
        return MarkWatchedOutcome.Marked;
    }

    /// <summary>Plex's "rate" (/:/rate, rating 0–10, -1 clears), for the user the token belongs to: private to that user.</summary>
    public async Task<bool> RateAsync(string imdbId, int? rating, CancellationToken ct = default)
    {
        if (rating is < 1 or > 10) throw new ArgumentOutOfRangeException(nameof(rating), rating, "A rating is from 1 to 10.");
        var connection = CurrentConnection() ?? throw new InvalidOperationException("Plex is not configured.");
        if (await RatingKeyAsync(connection, imdbId, ct).ConfigureAwait(false) is not { } ratingKey) return false;
        await SendAsync(connection, HttpMethod.Put,
            $"/:/rate?key={Uri.EscapeDataString(ratingKey)}&identifier=com.plexapp.plugins.library&rating={rating ?? -1}", ct).ConfigureAwait(false);
        return true;
    }

    /// <summary>The movie's ratingKey, else the show's (series), from the TV sections' show lists; null when Plex hasn't got it.</summary>
    private async Task<string?> RatingKeyAsync(Connection connection, string imdbId, CancellationToken ct) =>
        (await MoviesAsync(ct).ConfigureAwait(false))?.GetValueOrDefault(imdbId)?.RatingKey is { Length: > 0 } movieKey
            ? movieKey
            : (await ShowsAsync(connection, ct).ConfigureAwait(false)).GetValueOrDefault(imdbId) is { Length: > 0 } showKey ? showKey : null;

    /// <summary>The title's page in Plex Web on app.plex.tv (see <see cref="WebPage"/>).</summary>
    /// <remarks>A movie first; else a show (series), from the TV sections' show lists, loaded once per server.</remarks>
    public async Task<string?> GetWebPageAsync(string imdbId, CancellationToken ct = default)
    {
        if (CurrentConnection() is not { } connection) return null;
        if (await RatingKeyAsync(connection, imdbId, ct).ConfigureAwait(false) is not { } ratingKey) return null;

        var machine = await MachineIdentifierAsync(connection, ct).ConfigureAwait(false);
        return WebPage(machine, ratingKey);
    }

    /// <summary>/library/metadata/{showKey}/allLeaves: every episode with its season (parentIndex) and number (index).</summary>
    public async Task<IReadOnlyDictionary<(int Season, int Episode), string>> GetEpisodeWebPagesAsync(string seriesImdbId, CancellationToken ct = default)
    {
        if (CurrentConnection() is not { } connection) return new Dictionary<(int, int), string>();
        if ((await ShowsAsync(connection, ct).ConfigureAwait(false)).GetValueOrDefault(seriesImdbId) is not { } showKey) return new Dictionary<(int, int), string>();

        var leaves = await SendAsync(connection, HttpMethod.Get, $"/library/metadata/{Uri.EscapeDataString(showKey)}/allLeaves", ct).ConfigureAwait(false);
        var machine = await MachineIdentifierAsync(connection, ct).ConfigureAwait(false);
        return EpisodeKeysFrom(leaves).ToDictionary(e => e.Key, e => WebPage(machine, e.Value));
    }

    internal static IReadOnlyDictionary<(int Season, int Episode), string> EpisodeKeysFrom(XDocument? allLeaves)
    {
        var keys = new Dictionary<(int, int), string>();
        foreach (var video in allLeaves?.Descendants("Video") ?? [])
            if (int.TryParse((string?)video.Attribute("parentIndex"), out var season)
                && int.TryParse((string?)video.Attribute("index"), out var episode)
                && (string?)video.Attribute("ratingKey") is { Length: > 0 } ratingKey)
                keys.TryAdd((season, episode), ratingKey);
        return keys;
    }

    /// <summary>IMDb id → ratingKey of each show in the TV sections (/library/sections/{key}/all?type=2&amp;includeGuids=1).</summary>
    private async Task<IReadOnlyDictionary<string, string>> ShowsAsync(Connection connection, CancellationToken ct)
    {
        if (_shows is { } known && known.Connection == connection) return known.ByImdbId;

        var sections = await SendAsync(connection, HttpMethod.Get, "/library/sections", ct).ConfigureAwait(false);
        var shows = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var key in (sections?.Descendants("Directory") ?? []).Where(d => (string?)d.Attribute("type") == "show").Select(d => (string?)d.Attribute("key")).OfType<string>())
        {
            var list = await SendAsync(connection, HttpMethod.Get, $"/library/sections/{key}/all?type=2&includeGuids=1", ct).ConfigureAwait(false);
            foreach (var (imdbId, ratingKey) in ShowKeysFrom(list)) shows.TryAdd(imdbId, ratingKey);
        }
        _shows = (connection, shows);
        return shows;
    }

    internal static IEnumerable<(string ImdbId, string RatingKey)> ShowKeysFrom(XDocument? shows) =>
        (shows?.Descendants("Directory") ?? [])
            .Select(d => (ImdbId: ImdbIdOf(d), RatingKey: (string?)d.Attribute("ratingKey")))
            .Where(s => !string.IsNullOrEmpty(s.ImdbId) && !string.IsNullOrEmpty(s.RatingKey))
            .Select(s => (s.ImdbId!, s.RatingKey!));

    /// <summary>
    /// app.plex.tv/desktop/#!/server/{machineIdentifier}/details?key=/library/metadata/{ratingKey}. Plex Web on plex.tv, where
    /// the browser is usually signed in already; the server's own {server}:32400/web sends a browser that isn't through
    /// plex.tv's sign-in, which drops the page and lands on Plex's home page.
    /// </summary>
    internal static string WebPage(string machineIdentifier, string ratingKey) =>
        $"{PlexWeb}#!/server/{machineIdentifier}/details?key={Uri.EscapeDataString($"/library/metadata/{ratingKey}")}";

    /// <summary>Plex Web on plex.tv.</summary>
    private const string PlexWeb = "https://app.plex.tv/desktop/";

    public async Task<IReadOnlyList<Item>> SearchAsync(string text, CancellationToken ct = default)
    {
        if (await MoviesAsync(ct).ConfigureAwait(false) is not { } movies) return [];

        var wanted = text.Trim();
        return movies.Values
            .Where(m => m.Item.Title.Contains(wanted, StringComparison.OrdinalIgnoreCase)
                     || (m.OriginalTitle?.Contains(wanted, StringComparison.OrdinalIgnoreCase) ?? false))
            .Select(m => m.Item)
            .ToList();
    }

    #region Recently updated series

    /// <summary>Episodes are read newest first in pages of this many, until they are older than asked.</summary>
    private const int EpisodePage = 200;

    /// <summary>At most this many episodes per section, however long ago is asked for.</summary>
    private const int MaxEpisodes = 3000;

    /// <summary>
    /// Per TV section: the shows with their IMDb ids (one request), then the episodes newest first
    /// (/library/sections/{key}/all?type=4&amp;sort=addedAt:desc, paged) back to <paramref name="since"/>.
    /// </summary>
    public async Task<IReadOnlyList<RecentSeriesUpdate>> GetRecentlyUpdatedSeriesAsync(DateTime since, CancellationToken ct = default)
    {
        var connection = CurrentConnection() ?? throw new InvalidOperationException("Plex is not configured.");
        var sections = await SendAsync(connection, HttpMethod.Get, "/library/sections", ct).ConfigureAwait(false);
        var showSectionKeys = (sections?.Descendants("Directory") ?? [])
            .Where(d => (string?)d.Attribute("type") == "show")
            .Select(d => (string?)d.Attribute("key"))
            .OfType<string>();

        var updates = new List<RecentSeriesUpdate>();
        foreach (var key in showSectionKeys)
        {
            var shows = await SendAsync(connection, HttpMethod.Get, $"/library/sections/{key}/all?type=2&includeGuids=1", ct).ConfigureAwait(false);
            var pages = new List<XDocument>();
            for (var start = 0; start < MaxEpisodes; start += EpisodePage)
            {
                var page = await SendAsync(connection, HttpMethod.Get,
                    $"/library/sections/{key}/all?type=4&sort=addedAt:desc&X-Plex-Container-Start={start}&X-Plex-Container-Size={EpisodePage}",
                    ct).ConfigureAwait(false);
                if (page is null) break;
                pages.Add(page);
                var videos = page.Descendants("Video").ToList();
                if (videos.Count < EpisodePage || videos.Any(v => AddedAt(v) < since)) break;
            }
            updates.AddRange(RecentFrom(shows, pages, since));
        }
        return updates.OrderByDescending(u => u.LastAdded).ToList();
    }

    /// <summary>The episodes in <paramref name="episodePages"/> added since <paramref name="since"/>, grouped by show.</summary>
    internal static IReadOnlyList<RecentSeriesUpdate> RecentFrom(XDocument? shows, IEnumerable<XDocument> episodePages, DateTime since)
    {
        var imdbIds = (shows?.Descendants("Directory") ?? [])
            .GroupBy(d => (string?)d.Attribute("ratingKey") ?? "")
            .ToDictionary(g => g.Key, g => ImdbIdOf(g.First()));

        return episodePages
            .SelectMany(p => p.Descendants("Video"))
            .Where(v => AddedAt(v) >= since)
            .GroupBy(v => (string?)v.Attribute("grandparentRatingKey") ?? (string?)v.Attribute("grandparentTitle") ?? "")
            .Select(g => new RecentSeriesUpdate(
                imdbIds.GetValueOrDefault(g.Key),
                (string?)g.First().Attribute("grandparentTitle") ?? "?",
                g.Select(v => new RecentEpisode(
                        Int(v, "parentIndex"), Int(v, "index"), (string?)v.Attribute("title") ?? "", AddedAt(v)))
                    .OrderByDescending(e => e.Added)
                    .ToList()))
            .OrderByDescending(u => u.LastAdded)
            .ToList();

        static int Int(XElement e, string name) => int.TryParse((string?)e.Attribute(name), out var n) ? n : 0;
    }

    private static DateTime AddedAt(XElement video) =>
        long.TryParse((string?)video.Attribute("addedAt"), out var unix)
            ? DateTimeOffset.FromUnixTimeSeconds(unix).LocalDateTime
            : DateTime.MinValue;

    private static string? ImdbIdOf(XElement element) => element.Elements("Guid")
        .Select(g => (string?)g.Attribute("id"))
        .FirstOrDefault(id => id?.StartsWith("imdb://", StringComparison.Ordinal) == true)?["imdb://".Length..];

    #endregion

    #region Playlists

    public async Task<IReadOnlyList<string>> GetPlaylistNamesAsync(CancellationToken ct = default)
    {
        var connection = CurrentConnection() ?? throw new InvalidOperationException("Plex is not configured.");
        return (await PlaylistsAsync(connection, ct).ConfigureAwait(false)).Keys.Order(StringComparer.CurrentCultureIgnoreCase).ToList();
    }

    public async Task<PlaylistSyncResult> AddToPlaylistAsync(string playlistName, IReadOnlyList<string> imdbIds, CancellationToken ct = default)
    {
        var connection = CurrentConnection() ?? throw new InvalidOperationException("Plex is not configured.");
        var movies = await MoviesAsync(ct).ConfigureAwait(false) ?? new Dictionary<string, PlexMovie>();

        var notOnServer = imdbIds.Where(id => !movies.ContainsKey(id)).ToList();
        var wanted = imdbIds.Where(movies.ContainsKey).Select(id => movies[id].RatingKey).Distinct().ToList();

        var playlists = await PlaylistsAsync(connection, ct).ConfigureAwait(false);
        if (!playlists.TryGetValue(playlistName.Trim(), out var playlistKey))
        {
            if (wanted.Count == 0) return new PlaylistSyncResult(0, 0, notOnServer, Created: false);

            // Plex creates a playlist from a seed URI; it accepts a comma-separated list of items.
            var uri = await ItemsUriAsync(connection, wanted, ct).ConfigureAwait(false);
            await SendAsync(connection, HttpMethod.Post,
                $"/playlists?type=video&smart=0&title={Uri.EscapeDataString(playlistName)}&uri={Uri.EscapeDataString(uri)}", ct).ConfigureAwait(false);
            return new PlaylistSyncResult(wanted.Count, 0, notOnServer, Created: true);
        }

        var present = (await PlaylistItemsAsync(connection, playlistKey, ct).ConfigureAwait(false)).Select(i => i.RatingKey).ToHashSet();
        var toAdd = wanted.Where(rk => !present.Contains(rk)).ToList();
        if (toAdd.Count > 0)
        {
            var uri = await ItemsUriAsync(connection, toAdd, ct).ConfigureAwait(false);
            await SendAsync(connection, HttpMethod.Put, $"/playlists/{playlistKey}/items?uri={Uri.EscapeDataString(uri)}", ct).ConfigureAwait(false);
        }
        return new PlaylistSyncResult(toAdd.Count, wanted.Count - toAdd.Count, notOnServer, Created: false);
    }

    /// <summary>
    /// The playlist in Plex Web on plex.tv: app.plex.tv/desktop/#!/server/{machineIdentifier}/playlist?key=/playlists/{key}
    /// (as <see cref="WebPage"/>). Plex Web may ask to sign in the first time; the web window remembers it.
    /// </summary>
    public async Task<string?> GetPlaylistWebPageAsync(string playlistName, CancellationToken ct = default)
    {
        var connection = CurrentConnection() ?? throw new InvalidOperationException("Plex is not configured.");
        if (!(await PlaylistsAsync(connection, ct).ConfigureAwait(false)).TryGetValue(playlistName.Trim(), out var playlistKey)) return null;
        var identity = await SendAsync(connection, HttpMethod.Get, "/identity", ct).ConfigureAwait(false);
        var machine = (string?)identity?.Root?.Attribute("machineIdentifier")
            ?? throw new InvalidOperationException("Plex did not report its machine identifier.");
        return $"{PlexWeb}#!/server/{machine}/playlist?key={Uri.EscapeDataString($"/playlists/{playlistKey}")}";
    }

    /// <summary>
    /// /playlists/{key}/items?includeGuids=1: each title with its IMDb id and whether it's watched (viewCount &gt; 0;
    /// a title only started has lastViewedAt but no viewCount, so it isn't).
    /// </summary>
    public async Task<IReadOnlyList<PlaylistEntry>?> GetPlaylistEntriesAsync(string playlistName, CancellationToken ct = default)
    {
        var connection = CurrentConnection() ?? throw new InvalidOperationException("Plex is not configured.");
        if (!(await PlaylistsAsync(connection, ct).ConfigureAwait(false)).TryGetValue(playlistName.Trim(), out var playlistKey)) return null;
        var doc = await SendAsync(connection, HttpMethod.Get, $"/playlists/{playlistKey}/items?includeGuids=1", ct).ConfigureAwait(false);
        return EntriesFrom(doc);
    }

    internal static IReadOnlyList<PlaylistEntry> EntriesFrom(XDocument? playlistItems) =>
        (playlistItems?.Descendants("Video") ?? [])
            .Select(v => new PlaylistEntry(
                ImdbIdOf(v),
                (string?)v.Attribute("title") ?? "?",
                (string?)v.Attribute("year"),
                (string?)v.Attribute("type"),
                int.TryParse((string?)v.Attribute("viewCount"), out var views) && views > 0))
            .ToList();

    public async Task<bool> RemoveFromPlaylistAsync(string playlistName, string imdbId, CancellationToken ct = default)
    {
        var connection = CurrentConnection() ?? throw new InvalidOperationException("Plex is not configured.");
        if ((await MoviesAsync(ct).ConfigureAwait(false))?.GetValueOrDefault(imdbId) is not { } movie) return false;
        if (!(await PlaylistsAsync(connection, ct).ConfigureAwait(false)).TryGetValue(playlistName.Trim(), out var playlistKey)) return false;

        var entry = (await PlaylistItemsAsync(connection, playlistKey, ct).ConfigureAwait(false))
            .FirstOrDefault(i => i.RatingKey == movie.RatingKey);
        if (entry.PlaylistItemId is null) return false;

        await SendAsync(connection, HttpMethod.Delete, $"/playlists/{playlistKey}/items/{entry.PlaylistItemId}", ct).ConfigureAwait(false);
        return true;
    }

    private async Task<Dictionary<string, string>> PlaylistsAsync(Connection connection, CancellationToken ct)
    {
        var doc = await SendAsync(connection, HttpMethod.Get, "/playlists?playlistType=video", ct).ConfigureAwait(false);
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var p in doc?.Descendants("Playlist") ?? [])
            if ((string?)p.Attribute("title") is { } title && (string?)p.Attribute("ratingKey") is { } key)
                result.TryAdd(title.Trim(), key); // "KrigsFilms " on Plex is "KrigsFilms" in Kino
        return result;
    }

    private async Task<IReadOnlyList<(string RatingKey, string? PlaylistItemId)>> PlaylistItemsAsync(Connection connection, string playlistKey, CancellationToken ct)
    {
        var doc = await SendAsync(connection, HttpMethod.Get, $"/playlists/{playlistKey}/items", ct).ConfigureAwait(false);
        return (doc?.Descendants("Video") ?? [])
            .Select(v => ((string?)v.Attribute("ratingKey") ?? "", (string?)v.Attribute("playlistItemID")))
            .ToList();
    }

    private async Task<string> ItemsUriAsync(Connection connection, IEnumerable<string> ratingKeys, CancellationToken ct)
    {
        var identity = await SendAsync(connection, HttpMethod.Get, "/identity", ct).ConfigureAwait(false);
        var machine = (string?)identity?.Root?.Attribute("machineIdentifier")
            ?? throw new InvalidOperationException("Plex did not report its machine identifier.");
        return $"server://{machine}/com.plexapp.plugins.library/library/metadata/{string.Join(',', ratingKeys)}";
    }

    #endregion

    /// <summary>The server's machine identifier, asked once per server.</summary>
    private async Task<string> MachineIdentifierAsync(Connection connection, CancellationToken ct)
    {
        if (_machine is { } known && known.Connection == connection) return known.Identifier;

        var identity = await SendAsync(connection, HttpMethod.Get, "/identity", ct).ConfigureAwait(false);
        var machine = (string?)identity?.Root?.Attribute("machineIdentifier")
            ?? throw new InvalidOperationException("Plex did not report its machine identifier.");
        _machine = (connection, machine);
        return machine;
    }

    private Connection? CurrentConnection()
    {
        var current = settings.Current;
        return current.GetPlexBaseUrl() is { } url && !string.IsNullOrWhiteSpace(current.XPlexToken)
            ? new Connection(url, current.XPlexToken.Trim())
            : null;
    }

    /// <summary>The movie map for the current server/token; loaded on first use or when they changed.</summary>
    private async Task<IReadOnlyDictionary<string, PlexMovie>?> MoviesAsync(CancellationToken ct)
    {
        if (CurrentConnection() is not { } connection) return null;
        if (_loaded is not { } loaded || loaded.Connection != connection) await RefreshAsync(ct).ConfigureAwait(false);
        return _loaded?.Movies;
    }

    private async Task<IReadOnlyDictionary<string, PlexMovie>> LoadAsync(Connection connection, CancellationToken ct)
    {
        var sections = await SendAsync(connection, HttpMethod.Get, "/library/sections", ct).ConfigureAwait(false);
        var movieSectionKeys = (sections?.Descendants("Directory") ?? [])
            .Where(d => (string?)d.Attribute("type") == "movie")
            .Select(d => (string?)d.Attribute("key"))
            .OfType<string>();

        var movies = new Dictionary<string, PlexMovie>(StringComparer.OrdinalIgnoreCase);
        foreach (var key in movieSectionKeys)
        {
            var section = await SendAsync(connection, HttpMethod.Get, $"/library/sections/{key}/all?includeGuids=1", ct).ConfigureAwait(false);
            foreach (var video in section?.Descendants("Video") ?? [])
                if (ToMovie(video, connection) is { } movie)
                    movies[movie.Item.ImdbId] = movie;
        }
        return movies;
    }

    /// <summary>Sends a request with the token header; returns the XML answer, or null when there is none.</summary>
    private async Task<XDocument?> SendAsync(Connection connection, HttpMethod method, string pathAndQuery, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(method, connection.BaseUrl + pathAndQuery);
        request.Headers.Add("X-Plex-Token", connection.Token);
        request.Headers.Add("Accept", "application/xml");
        using var response = await http.SendAsync(request, ct).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
        return string.IsNullOrWhiteSpace(body) ? null : XDocument.Parse(body);
    }

    /// <summary>Same fields as SilverScreen's LoadSectionAsync, in OMDb's formats where they differ.</summary>
    private static PlexMovie? ToMovie(XElement video, Connection connection)
    {
        var imdbId = video.Elements("Guid")
            .Select(g => (string?)g.Attribute("id"))
            .FirstOrDefault(id => id?.StartsWith("imdb://", StringComparison.Ordinal) == true)?["imdb://".Length..];
        if (string.IsNullOrEmpty(imdbId)) return null;

        string Tags(string element) => string.Join(", ", video.Elements(element).Select(e => (string?)e.Attribute("tag")).OfType<string>());
        var thumb = (string?)video.Attribute("thumb");
        var minutes = long.TryParse((string?)video.Attribute("duration"), out var ms) ? ms / 60000 : 0;
        var year = int.TryParse((string?)video.Attribute("year"), out var y) ? y : (int?)null;
        var title = (string?)video.Attribute("title") ?? imdbId;

        var item = new Item
        {
            ImdbId = imdbId,
            Title = title,
            Year = year?.ToString() ?? "N/A",
            Released = (string?)video.Attribute("originallyAvailableAt") ?? "N/A",
            Plot = (string?)video.Attribute("summary") ?? "N/A",
            Rated = (string?)video.Attribute("contentRating") ?? "N/A",
            ImdbRating = (string?)video.Attribute("audienceRating") ?? "N/A",
            Runtime = minutes > 0 ? $"{minutes} min" : "N/A",
            Genre = Tags("Genre"),
            Director = Tags("Director"),
            Writer = Tags("Writer"),
            Actors = Tags("Role"),
            Country = Tags("Country"),
            Type = "movie",
            Website = $"https://www.imdb.com/title/{imdbId}",
            // As SilverScreen: the thumb URL carries the token, which is already stored in the same database.
            Poster = thumb is null ? "N/A" : $"{connection.BaseUrl}{thumb}?X-Plex-Token={connection.Token}",
            Response = "True",
        };
        return new PlexMovie(item, (string?)video.Attribute("originalTitle"), year, (string?)video.Attribute("ratingKey") ?? "")
        {
            Watched = int.TryParse((string?)video.Attribute("viewCount"), out var views) && views > 0,
        };
    }

    private sealed record Connection(string BaseUrl, string Token);

    private sealed record Loaded(Connection Connection, IReadOnlyDictionary<string, PlexMovie> Movies);

    private sealed record PlexMovie(Item Item, string? OriginalTitle, int? Year, string RatingKey)
    {
        /// <summary>Played to the end on Plex (viewCount &gt; 0), for the token's user.</summary>
        public bool Watched { get; set; }
    }
}
