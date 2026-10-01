using System.Xml.Linq;
using Kolibri.Kino.Core.Abstractions;
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
public sealed class PlexLibrary(HttpClient http, ISettingsStore settings) : IMediaServerLibrary, IMediaServerPlaylists
{
    private readonly SemaphoreSlim _loadLock = new(1, 1);
    private Loaded? _loaded;

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
        if (!playlists.TryGetValue(playlistName, out var playlistKey))
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

    public async Task<bool> RemoveFromPlaylistAsync(string playlistName, string imdbId, CancellationToken ct = default)
    {
        var connection = CurrentConnection() ?? throw new InvalidOperationException("Plex is not configured.");
        if ((await MoviesAsync(ct).ConfigureAwait(false))?.GetValueOrDefault(imdbId) is not { } movie) return false;
        if (!(await PlaylistsAsync(connection, ct).ConfigureAwait(false)).TryGetValue(playlistName, out var playlistKey)) return false;

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
                result.TryAdd(title, key);
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

        var item = new Item
        {
            ImdbId = imdbId,
            Title = (string?)video.Attribute("title") ?? imdbId,
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
        return new PlexMovie(item, (string?)video.Attribute("originalTitle"), year, (string?)video.Attribute("ratingKey") ?? "");
    }

    private sealed record Connection(string BaseUrl, string Token);

    private sealed record Loaded(Connection Connection, IReadOnlyDictionary<string, PlexMovie> Movies);

    private sealed record PlexMovie(Item Item, string? OriginalTitle, int? Year, string RatingKey);
}
