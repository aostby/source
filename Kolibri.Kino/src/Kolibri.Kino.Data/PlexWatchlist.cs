using System.Text.Json;
using Kolibri.Kino.Core.Abstractions;

namespace Kolibri.Kino.Data;

/// <summary>
/// The Plex account's watchlist ("Huskeliste") from Plex's Discover service on plex.tv, the list app.plex.tv shows at
/// /desktop/#!/media/tv.plex.provider.discover?source=watchlist. 100 titles a request, with their guids
/// (includeGuids=1), so each comes with its IMDb id. The token goes in the X-Plex-Token header, never in an address.
/// </summary>
public sealed class PlexWatchlist(ISettingsStore settings, HttpMessageHandler? handler = null) : IPlexWatchlist
{
    public const string WatchlistUrl = "https://discover.provider.plex.tv/library/sections/watchlist/all";
    private const int PageSize = 100;

    private readonly HttpClient _http = handler is null
        ? new HttpClient { Timeout = TimeSpan.FromSeconds(30) }
        : new HttpClient(handler, disposeHandler: false) { Timeout = TimeSpan.FromSeconds(30) };

    private string? Token => settings.Current.XPlexToken is { Length: > 0 } token ? token.Trim() : null;

    public bool IsConfigured => Token is not null;

    public async Task<IReadOnlyList<PlexWatchlistItem>> GetWatchlistAsync(CancellationToken ct = default)
    {
        var token = Token ?? throw new InvalidOperationException("No Plex token in Settings: the watchlist belongs to a Plex account.");
        var items = new List<PlexWatchlistItem>();
        for (var start = 0; ; start += PageSize)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get,
                $"{WatchlistUrl}?includeGuids=1&X-Plex-Container-Start={start}&X-Plex-Container-Size={PageSize}");
            request.Headers.Add("X-Plex-Token", token);
            request.Headers.Add("X-Plex-Client-Identifier", "kolibri-kino");
            request.Headers.Add("X-Plex-Product", "Kolibri.Kino");
            request.Headers.Accept.ParseAdd("application/json");
            using var response = await _http.SendAsync(request, ct).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new InvalidOperationException("Plex did not accept the token in Settings for the watchlist.");
            response.EnsureSuccessStatusCode();

            using var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false), cancellationToken: ct).ConfigureAwait(false);
            var (page, total) = Parse(json);
            items.AddRange(page);
            if (page.Count < PageSize || items.Count >= total) return items;
        }
    }

    /// <summary>One page of Plex's answer: its titles and the watchlist's total size.</summary>
    internal static (IReadOnlyList<PlexWatchlistItem> Items, int Total) Parse(JsonDocument json)
    {
        if (!json.RootElement.TryGetProperty("MediaContainer", out var container)) return ([], 0);
        var total = container.TryGetProperty("totalSize", out var t) && t.TryGetInt32(out var n) ? n : 0;
        if (!container.TryGetProperty("Metadata", out var metadata) || metadata.ValueKind != JsonValueKind.Array) return ([], total);

        var items = new List<PlexWatchlistItem>();
        foreach (var m in metadata.EnumerateArray())
        {
            var title = Text(m, "title");
            if (title is null) continue;
            var year = m.TryGetProperty("year", out var y) && y.TryGetInt32(out var yr) ? yr.ToString(System.Globalization.CultureInfo.InvariantCulture) : null;
            var type = Text(m, "type") == "show" ? "series" : "movie";
            // "Guid" (capital G) is the list of external ids; "guid" (small g) is Plex's own plex://movie/… id.
            string? imdb = null;
            if (m.TryGetProperty("Guid", out var guids) && guids.ValueKind == JsonValueKind.Array)
                foreach (var g in guids.EnumerateArray())
                    if (Text(g, "id") is { } id && id.StartsWith("imdb://", StringComparison.OrdinalIgnoreCase))
                        imdb = id["imdb://".Length..];
            var poster = Text(m, "thumb") is { } thumb && thumb.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ? thumb : null;
            items.Add(new PlexWatchlistItem(title, year, type, imdb, poster));
        }
        return (items, total);
    }

    private static string? Text(JsonElement e, string name) =>
        e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.String && v.GetString() is { Length: > 0 } s ? s : null;
}
