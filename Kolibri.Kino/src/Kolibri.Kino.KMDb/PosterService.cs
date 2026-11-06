using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Kolibri.Kino.KMDb;

/// <summary>
/// Posters for /silverscreen/poster/{imdbId}. Kino's image cache holds only the posters Kino has shown, so a title
/// without one gets it from the library's other sources: the TMDb poster of Movie/TvShow (public, no key), another
/// public poster address the library has (Item, Episode, WatchListItem, KinoWishlist), and last Plex, when the service
/// has the Plex token (Kmdb:PlexToken). The token goes to Plex in the X-Plex-Token header only; addresses in answers
/// and logs never carry it. A fetched poster is kept in the cache under the IMDb id, as Kino does.
/// </summary>
public sealed class PosterService(
    ISilverScreenRepository library, ISilverScreenWriter writer, IHttpClientFactory http, ILogger<PosterService> log,
    KmdbMetrics? metrics = null, IOptions<KmdbOptions>? options = null, PlexPosterIndex? plex = null)
{
    public const string HttpClientName = "posters";

    /// <summary>TMDb's poster size: 500 px wide, as Kino uses.</summary>
    public const string TmdbImageBase = "https://image.tmdb.org/t/p/w500";

    private const int MaxBytes = 10 * 1024 * 1024;

    /// <summary>
    /// The poster for a title, or for an episode key (tt0903747_S01E02) its still: the best one in the cache (by IMDb id
    /// and type), else the image keyed by the id itself, else (posters only) fetched from the library's sources and
    /// cached with what it is.
    /// </summary>
    public async Task<SilverScreenImage?> GetAsync(string imdbId, CancellationToken ct = default)
    {
        var (title, type, season, episode) = ArtKey(imdbId);
        var art = await library.ArtAsync(title, type, season, episode, ct).ConfigureAwait(false)
                  ?? await library.ImageAsync(imdbId, ct).ConfigureAwait(false);
        if (art is not null || type != "poster") return art;

        foreach (var url in await SourcesAsync(imdbId, ct, waitForPlex: true).ConfigureAwait(false))
        {
            var data = await DownloadAsync(url, ct).ConfigureAwait(false);
            if (data is null) continue;
            try
            {
                // Keyed by the IMDb id, as Kino keys posters (nothing has that key: ImageAsync above found none).
                await writer.StoreImageAsync(imdbId, data, imdbId, "poster", SourceOf(url), url, ct: ct).ConfigureAwait(false);
            }
            catch (MySqlException ex)
            {
                log.LogWarning("Poster for {Id} not cached: {Message}", imdbId, ex.Message);
            }
            metrics?.PosterFetched(found: true);
            return new SilverScreenImage(data, DateTime.UtcNow);
        }
        metrics?.PosterFetched(found: false);
        return null;
    }

    /// <summary>
    /// What an image key in Kino's cache is: tt… a poster, tt…_S01E02 that episode's still, tmdb-person-123 a profile
    /// photo (by TMDb id); null for other keys (addresses, names).
    /// </summary>
    internal static (string? ImdbId, string Type, int? Season, int? Episode, int? TmdbId)? KinoKey(string key)
    {
        if (System.Text.RegularExpressions.Regex.Match(key, "^tmdb-person-([0-9]+)$") is { Success: true } p)
            return (null, "profile", null, null, int.Parse(p.Groups[1].Value));
        if (!key.StartsWith("tt", StringComparison.Ordinal) || !OmdbService.IsImdbId(key.Split('_')[0])) return null;
        var (imdb, type, season, episode) = ArtKey(key);
        return (imdb, type, season, episode, null);
    }

    /// <summary>"tt0903747_S01E02" → the series' still of that episode; any other id → its poster.</summary>
    internal static (string ImdbId, string Type, int? Season, int? Episode) ArtKey(string id) =>
        System.Text.RegularExpressions.Regex.Match(id, "^(tt[0-9]{7,10})_S([0-9]{1,3})E([0-9]{1,4})$") is { Success: true } m
            ? (m.Groups[1].Value, "still", int.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value))
            : (id, "poster", null, null);

    internal static string SourceOf(string url) =>
        url.Contains("tmdb.org", StringComparison.OrdinalIgnoreCase) ? "tmdb"
        : url.Contains(":32400", StringComparison.Ordinal) ? "plex"
        : url.Contains("media-amazon.com", StringComparison.OrdinalIgnoreCase) ? "omdb"
        : "web";

    /// <summary>Which of these ids have a poster to serve: in the cache, or with a source to fetch it from.</summary>
    public async Task<IReadOnlySet<string>> AvailableAsync(IReadOnlyCollection<string> imdbIds, CancellationToken ct = default)
    {
        var available = new HashSet<string>(await library.ExistingImagesAsync(imdbIds, ct).ConfigureAwait(false), StringComparer.Ordinal);
        foreach (var id in imdbIds.Where(i => !string.IsNullOrEmpty(i) && !available.Contains(i)).Distinct(StringComparer.Ordinal))
            if ((await SourcesAsync(id, ct).ConfigureAwait(false)).Count > 0) available.Add(id);
        return available;
    }

    private string? PlexToken => options?.Value.PlexToken is { Length: > 0 } token ? token : null;

    /// <summary>
    /// Addresses to fetch the poster from, best first: TMDb, the library's other public addresses, then Plex (only
    /// with the token; the poster Plex has now, from <see cref="PlexPosterIndex"/>, resized by Plex to 500 px like
    /// TMDb's). The Plex addresses stored in the library are not used: they go stale when Plex rebuilds its library.
    /// </summary>
    internal async Task<IReadOnlyList<string>> SourcesAsync(string imdbId, CancellationToken ct, bool waitForPlex = false)
    {
        var list = (await library.PosterSourcesAsync(imdbId, ct).ConfigureAwait(false))
            .Select(s => s.StartsWith('/') ? TmdbImageBase + s : s)
            .Where(IsFetchable)
            .ToList();
        if (PlexToken is not null && plex is not null
            && await plex.PosterAsync(imdbId, waitForPlex && list.Count == 0, ct).ConfigureAwait(false) is { } thumb
            && PlexPosterUrl(thumb) is { } url)
            list.Add(url);
        return list.Distinct(StringComparer.Ordinal).ToList();
    }

    /// <summary>
    /// Plex's photo transcoder for a library image, 500×750 like TMDb's posters (Plex's originals can be several MB).
    /// Any token in the stored address is dropped; the token is sent as a header.
    /// </summary>
    internal static string? PlexPosterUrl(string url)
    {
        if (!Uri.TryCreate(SilverScreenRepository.WithoutPlexToken(url), UriKind.Absolute, out var uri)) return null;
        var server = uri.GetLeftPart(UriPartial.Authority);
        return $"{server}/photo/:/transcode?width=500&height=750&minSize=1&upscale=0&url={Uri.EscapeDataString(uri.PathAndQuery)}";
    }

    /// <summary>Public http(s) addresses only: not Plex (needs a token), not "N/A", not local paths.</summary>
    internal static bool IsFetchable(string url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri)
        && uri.Scheme is "http" or "https"
        && !SilverScreenEnricher.IsPlexAddress(url);

    private async Task<byte[]?> DownloadAsync(string url, CancellationToken ct)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            if (SilverScreenEnricher.IsPlexAddress(url) && PlexToken is { } token) request.Headers.Add("X-Plex-Token", token);
            using var response = await http.CreateClient(HttpClientName).SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                log.LogInformation("Poster {Url}: {Status}", url, (int)response.StatusCode);
                return null;
            }
            if (response.Content.Headers.ContentType is MediaTypeHeaderValue type && !type.MediaType!.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                return null;
            if (response.Content.Headers.ContentLength > MaxBytes) return null;
            var data = await response.Content.ReadAsByteArrayAsync(ct).ConfigureAwait(false);
            return data.Length is > 0 and <= MaxBytes && SilverScreenController.ContentType(data).StartsWith("image/", StringComparison.Ordinal) ? data : null;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException && !ct.IsCancellationRequested)
        {
            log.LogInformation("Poster {Url} not fetched: {Message}", url, ex.Message);
            return null;
        }
    }
}
