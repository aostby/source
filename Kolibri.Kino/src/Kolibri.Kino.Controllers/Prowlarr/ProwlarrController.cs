using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kolibri.Kino.Controllers.Library;
using Kolibri.Kino.Controllers.Wishlist;
using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kolibri.Kino.Controllers.Prowlarr;

/// <summary>One of Prowlarr's search results (a torrent from one of its indexers).</summary>
/// <param name="MagnetUrl">Usually Prowlarr's own link that leads on to the magnet link (some indexers give only this).</param>
/// <param name="DownloadUrl">Prowlarr's link to the .torrent file (some indexers give only this).</param>
/// <param name="ImdbId">The indexer's IMDb id for it, when it gives one.</param>
public sealed record ProwlarrRelease(
    string Title, string Indexer, long Size, int Seeders, int Leechers, DateTime? Published,
    string? MagnetUrl, string? DownloadUrl, string? InfoUrl, string? ImdbId)
{
    /// <summary>The size in GB (1 GB = 1,000,000,000 bytes, as Prowlarr shows it).</summary>
    public double SizeGb => Size / 1e9;

    /// <summary>"magnet", "torrent" or both, as the link column shows.</summary>
    public string Links => string.Join(" + ", new[] { MagnetUrl is null ? null : "magnet", DownloadUrl is null ? null : "torrent" }.OfType<string>());
}

/// <summary>A release and what Kino made of it.</summary>
/// <param name="Quality">2160p, 1080p, 720p or SD from its name; null when the name says none.</param>
/// <param name="LeftOut">Why it isn't one of the wanted ones (another title, too few seeders …); null when it is.</param>
public sealed record ReleaseCheck(ProwlarrRelease Release, string? Quality, string? LeftOut);

/// <summary>A search for one wanted title.</summary>
/// <param name="Best">The wanted releases, most seeders first, at most <see cref="ProwlarrOptions.TopResults"/>.</param>
/// <param name="All">Every release Prowlarr found: the wanted ones (most seeders first), then the rest.</param>
public sealed record ProwlarrSearch(WishlistItem Wanted, string Query, IReadOnlyList<ReleaseCheck> Best, IReadOnlyList<ReleaseCheck> All);

/// <summary>What getting a release gave: a magnet link to open, or a .torrent file's bytes and name.</summary>
public sealed record ReleaseLink(string? Magnet, byte[]? Torrent, string FileName);

/// <summary>
/// File → Prowlarr: searches Prowlarr (on the NAS) for wanted titles, by "title year" since its indexers here can't
/// search by IMDb id, and keeps the releases that are the title (<see cref="ReleaseMatch"/>), in a wanted quality, with
/// enough seeders (appsettings.json, Kino:Prowlarr). Also says which wanted titles you have, and finds the IMDb id and
/// type (movie or series) of titles a wishlist file gave without them. Prowlarr's address and API key are user
/// settings (Settings, category Prowlarr). Links from Prowlarr carry its API key, so they're never logged.
/// </summary>
public sealed class ProwlarrController(
    ISettingsStore settings,
    IOptions<KinoOptions> options,
    WishlistController wishlist,
    ILogger<ProwlarrController> logger,
    LibraryFiles? libraryFiles = null,
    HttpMessageHandler? handler = null)
{
    /// <summary>
    /// Prowlarr asks all its indexers and answers when they have (or have timed out), which can take a while. Redirects
    /// aren't followed by themselves: a link that leads to a magnet link must be caught, not followed.
    /// </summary>
    private static readonly HttpClient SharedHttp = new(new SocketsHttpHandler { AllowAutoRedirect = false }) { Timeout = TimeSpan.FromMinutes(2) };

    private readonly HttpClient _http = handler is null ? SharedHttp : new HttpClient(handler) { Timeout = TimeSpan.FromMinutes(2) };

    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public ProwlarrOptions Options => options.Value.Prowlarr;

    public bool IsConfigured => ConfigProblem is null;

    /// <summary>What's wrong with Prowlarr's URL or API key in the settings, in words; null when both are there.</summary>
    public string? ConfigProblem
    {
        get
        {
            var current = settings.Current;
            var problems = new List<string>();
            if (string.IsNullOrWhiteSpace(current.ProwlarrUrl)) problems.Add("no Prowlarr URL");
            else if (current.GetProwlarrBaseUrl() is null) problems.Add($"the Prowlarr URL \"{current.ProwlarrUrl.Trim()}\" isn't an http(s) address");
            if (string.IsNullOrWhiteSpace(current.ProwlarrApiKey)) problems.Add("no Prowlarr API key");
            return problems.Count == 0 ? null : string.Join(" and ", problems);
        }
    }

    /// <summary>
    /// Searches Prowlarr for <paramref name="wanted"/> in the movie or TV categories, and sorts out the releases that are it.
    /// </summary>
    /// <exception cref="InvalidOperationException">Prowlarr isn't set up, refused the key, or answered with an error; the message says so in words.</exception>
    public async Task<ProwlarrSearch> SearchAsync(WishlistItem wanted, CancellationToken ct = default)
    {
        var current = settings.Current;
        if (ConfigProblem is { } problem || current.GetProwlarrBaseUrl() is not { } baseUrl || string.IsNullOrWhiteSpace(current.ProwlarrApiKey))
            throw new InvalidOperationException($"Settings has {ConfigProblem ?? "no Prowlarr URL or API key"}: set them under Prowlarr in Settings first.");

        var query = ReleaseMatch.Query(wanted);
        var categories = ReleaseMatch.KindOf(wanted) == WantedKind.Movie ? Options.EffectiveMovieCategories : Options.EffectiveSeriesCategories;
        var url = new StringBuilder($"{baseUrl}api/v1/search?query={Uri.EscapeDataString(query)}&type=search");
        foreach (var category in categories) url.Append("&categories=").Append(category.ToString(CultureInfo.InvariantCulture));

        using var request = new HttpRequestMessage(HttpMethod.Get, url.ToString());
        request.Headers.Add("X-Api-Key", current.ProwlarrApiKey.Trim());
        request.Headers.Accept.ParseAdd("application/json");
        HttpResponseMessage response;
        try
        {
            response = await _http.SendAsync(request, ct).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"Prowlarr at {baseUrl} couldn't be reached: {ex.Message}", ex);
        }
        catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
        {
            throw new InvalidOperationException($"Prowlarr didn't answer within {_http.Timeout.TotalMinutes:0} minutes.", ex);
        }

        using (response)
        {
            if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
                throw new InvalidOperationException("Prowlarr didn't accept the API key (Settings → Prowlarr API key).");
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Prowlarr answered {(int)response.StatusCode} {response.ReasonPhrase} for \"{query}\".");

            var found = await response.Content.ReadFromJsonAsync<List<ResultDto>>(Json, ct).ConfigureAwait(false) ?? [];
            var result = Check(wanted, query, found.Select(ToRelease).ToList(), Options);
            logger.LogInformation("Prowlarr: \"{Query}\" gave {All} release(s), {Wanted} of them wanted", query, result.All.Count,
                result.All.Count(r => r.LeftOut is null));
            return result;
        }
    }

    /// <summary>Sorts <paramref name="releases"/> into the wanted ones (most seeders first, the best few) and the rest.</summary>
    public static ProwlarrSearch Check(WishlistItem wanted, string query, IReadOnlyList<ProwlarrRelease> releases, ProwlarrOptions options)
    {
        var checks = releases
            .Select(r =>
            {
                var quality = ReleaseMatch.QualityOf(r.Title);
                return new ReleaseCheck(r, quality, ReleaseMatch.WhyNot(wanted, r, quality, options));
            })
            .OrderBy(c => c.LeftOut is not null)
            .ThenByDescending(c => c.Release.Seeders)
            .ThenBy(c => c.Release.Size)
            .ToList();
        var best = checks.Where(c => c.LeftOut is null).Take(Math.Max(1, options.TopResults)).ToList();
        return new ProwlarrSearch(wanted, query, best, checks);
    }

    /// <summary>
    /// The release's magnet link, or its .torrent file: Prowlarr's magnet link leads on (a redirect) to the magnet
    /// link; its download link gives the .torrent file, or leads to a magnet link too.
    /// </summary>
    /// <exception cref="InvalidOperationException">Neither link gave anything usable.</exception>
    public async Task<ReleaseLink> GetLinkAsync(ProwlarrRelease release, CancellationToken ct = default)
    {
        var fileName = SafeFileName(release.Title) + ".torrent";
        var problems = new List<string>();
        foreach (var link in new[] { release.MagnetUrl, release.DownloadUrl }.OfType<string>().Where(l => l.Length > 0))
        {
            if (link.StartsWith("magnet:", StringComparison.OrdinalIgnoreCase)) return new ReleaseLink(Named(link, release.Title), null, fileName);
            try
            {
                if (await FollowAsync(link, ct).ConfigureAwait(false) is { } got) return got with { Magnet = got.Magnet is { } magnet ? Named(magnet, release.Title) : null, FileName = fileName };
                problems.Add("no magnet link or .torrent file");
            }
            catch (HttpRequestException ex)
            {
                problems.Add(ex.Message);
            }
        }
        // The links hold Prowlarr's API key: say what happened, not where.
        logger.LogWarning("Getting {Title} from {Indexer} failed: {Problems}", release.Title, release.Indexer, string.Join("; ", problems));
        throw new InvalidOperationException($"{release.Indexer} gave neither a magnet link nor a .torrent file for \"{release.Title}\"" +
                                            (problems.Count > 0 ? $": {string.Join("; ", problems.Distinct())}." : "."));
    }

    /// <summary>
    /// The magnet link with a name (dn=, "display name"): some indexers give only the hash, and a torrent program then
    /// shows "magnet (047d66c2…)" until it has the file list. A link that has a name keeps its own.
    /// </summary>
    public static string Named(string magnet, string title)
    {
        var at = magnet.IndexOf('?');
        var query = at >= 0 ? magnet[(at + 1)..] : "";
        if (query.Split('&').Any(p => p.StartsWith("dn=", StringComparison.OrdinalIgnoreCase) && p.Length > 3) || string.IsNullOrWhiteSpace(title))
            return magnet;
        return magnet + (at < 0 ? "?" : query.Length == 0 || magnet.EndsWith('&') ? "" : "&") + "dn=" + Uri.EscapeDataString(title.Trim());
    }

    private async Task<ReleaseLink?> FollowAsync(string link, CancellationToken ct)
    {
        var url = new Uri(link);
        for (var hop = 0; hop < 5; hop++)
        {
            using var response = await _http.GetAsync(url, HttpCompletionOption.ResponseContentRead, ct).ConfigureAwait(false);
            if ((int)response.StatusCode is >= 300 and < 400 && response.Headers.Location is { } next)
            {
                var target = next.IsAbsoluteUri ? next : new Uri(url, next);
                if (target.Scheme.Equals("magnet", StringComparison.OrdinalIgnoreCase)) return new ReleaseLink(target.OriginalString, null, "");
                url = target;
                continue;
            }
            if (!response.IsSuccessStatusCode) throw new HttpRequestException($"answered {(int)response.StatusCode} {response.ReasonPhrase}");

            var bytes = await response.Content.ReadAsByteArrayAsync(ct).ConfigureAwait(false);
            var start = Encoding.ASCII.GetString(bytes, 0, Math.Min(bytes.Length, 7));
            if (start.Equals("magnet:", StringComparison.OrdinalIgnoreCase)) return new ReleaseLink(Encoding.UTF8.GetString(bytes).Trim(), null, "");
            // A .torrent file is bencoded: a dictionary, "d8:announce…" or "d13:announce-list…" and so on.
            return bytes.Length > 1 && bytes[0] == (byte)'d' && char.IsAsciiDigit((char)bytes[1]) ? new ReleaseLink(null, bytes, "") : null;
        }
        throw new HttpRequestException("too many redirects");
    }

    /// <summary>The IMDb ids among <paramref name="imdbIds"/> you have: in the library with a file that's there.</summary>
    public async Task<IReadOnlySet<string>> HaveAsync(IEnumerable<string> imdbIds, CancellationToken ct = default)
    {
        var ids = imdbIds.Where(i => !string.IsNullOrEmpty(i)).ToList();
        if (ids.Count == 0 || libraryFiles is null) return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        return await libraryFiles.WithFileAsync(ids, ct).ConfigureAwait(false);
    }

    /// <summary>The release dates known (a day, not just a year) for these wanted titles; see <see cref="WishlistController.ReleaseDatesAsync"/>.</summary>
    public Task<IReadOnlyDictionary<string, DateTime>> ReleaseDatesAsync(IEnumerable<WishlistItem> titles, CancellationToken ct = default) =>
        wishlist.ReleaseDatesAsync(titles.Select(t => (t.ImdbId, t.Year)), ct: ct);

    /// <summary>
    /// Finds a title from a wishlist file without an IMDb id (or type) in the library, on OMDb and on TMDb, by title
    /// and year, as the Wishlist window's search does, and returns it with the id and type filled in. Only a hit with
    /// the same title and year counts; without one, null.
    /// </summary>
    public async Task<WishlistItem?> LookUpAsync(WishlistItem wanted, CancellationToken ct = default)
    {
        var year = wanted.Year?.Trim() is { Length: >= 4 } y ? y[..4] : null;
        var found = await wishlist.SearchAsync(year is null ? wanted.Title : $"{wanted.Title}, {year}", ct).ConfigureAwait(false);
        var title = ReleaseMatch.Words(wanted.Title);
        var same = found.Hits
            .Where(h => ReleaseMatch.Words(h.Title).SequenceEqual(title) && (year is null || (h.Year ?? "").StartsWith(year, StringComparison.Ordinal)))
            .OrderBy(h => wanted.Type is null || string.Equals(h.Type, wanted.Type, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
            .FirstOrDefault();
        if (same is null) return null;
        return new WishlistItem
        {
            ImdbId = string.IsNullOrEmpty(wanted.ImdbId) ? same.ImdbId : wanted.ImdbId,
            Title = wanted.Title,
            Year = wanted.Year,
            Type = wanted.Type ?? same.Type?.ToLowerInvariant(),
            Genre = wanted.Genre,
            ImdbRating = wanted.ImdbRating,
            Poster = wanted.Poster,
            Added = wanted.Added,
        };
    }

    private static string SafeFileName(string title)
    {
        var name = new string(title.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? ' ' : c).ToArray()).Trim();
        return name.Length > 150 ? name[..150].Trim() : name.Length == 0 ? "release" : name;
    }

    private static ProwlarrRelease ToRelease(ResultDto r) => new(
        r.Title ?? "", r.Indexer ?? "", r.Size, r.Seeders ?? 0, r.Leechers ?? 0, r.PublishDate,
        string.IsNullOrWhiteSpace(r.MagnetUrl) ? null : r.MagnetUrl,
        string.IsNullOrWhiteSpace(r.DownloadUrl) ? null : r.DownloadUrl,
        string.IsNullOrWhiteSpace(r.InfoUrl) ? null : r.InfoUrl,
        r.ImdbId > 0 ? "tt" + r.ImdbId.Value.ToString("0000000", CultureInfo.InvariantCulture) : null);

    /// <summary>The fields of /api/v1/search's answer that Kino uses.</summary>
    private sealed class ResultDto
    {
        public string? Title { get; set; }
        public string? Indexer { get; set; }
        public long Size { get; set; }
        public int? Seeders { get; set; }
        public int? Leechers { get; set; }
        public DateTime? PublishDate { get; set; }
        public string? MagnetUrl { get; set; }
        public string? DownloadUrl { get; set; }
        public string? InfoUrl { get; set; }

        [JsonPropertyName("imdbId")]
        public long? ImdbId { get; set; }
    }
}
