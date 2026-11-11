using System.Xml.Linq;
using Microsoft.Extensions.Options;

namespace Kolibri.Kino.KMDb;

/// <summary>
/// IMDb id → the poster (thumb) Plex has now, for every movie and show in Plex's movie and TV sections. The library's
/// stored Plex addresses go stale when Plex rebuilds its library (new ratingKeys, 404), so posters are looked up here.
/// Loaded in the background at start and every 6 hours (about 5 s for 4,000 titles); while Plex doesn't answer the
/// last list stays in use, and lookups never wait for Plex unless asked to.
/// </summary>
public sealed class PlexPosterIndex(IHttpClientFactory http, IOptions<KmdbOptions> options, ILogger<PlexPosterIndex> log) : BackgroundService
{
    public const string HttpClientName = "plex";
    private static readonly TimeSpan Refresh = TimeSpan.FromHours(6);
    private static readonly TimeSpan RetryAfterFailure = TimeSpan.FromMinutes(10);

    private readonly SemaphoreSlim _loading = new(1, 1);
    private IReadOnlyDictionary<string, string> _thumbs = new Dictionary<string, string>();
    private DateTime _loadedUtc = DateTime.MinValue;
    private DateTime _nextTryUtc = DateTime.MinValue;

    public bool Enabled => !string.IsNullOrEmpty(options.Value.PlexToken);

    /// <summary>Titles in the list, and when it was loaded (for /health and metrics).</summary>
    public (int Titles, DateTime LoadedUtc) Status => (_thumbs.Count, _loadedUtc);

    /// <summary>
    /// The address of the title's poster on Plex (without the token), or null. With <paramref name="wait"/> a missing
    /// or stale list is loaded first; without, the current list answers and a reload starts in the background.
    /// </summary>
    public async Task<string?> PosterAsync(string imdbId, bool wait, CancellationToken ct = default)
    {
        if (!Enabled) return null;
        if (DateTime.UtcNow - _loadedUtc > Refresh && DateTime.UtcNow >= _nextTryUtc)
        {
            var load = LoadAsync(CancellationToken.None);
            if (wait && _thumbs.Count == 0) await load.WaitAsync(TimeSpan.FromSeconds(60), ct).ConfigureAwait(false);
        }
        return _thumbs.TryGetValue(imdbId, out var thumb) ? PlexUrl + thumb : null;
    }

    private string PlexUrl => (options.Value.PlexUrl ?? "http://HTPC:32400").TrimEnd('/');

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!Enabled) return;
        while (!stoppingToken.IsCancellationRequested)
        {
            await LoadAsync(stoppingToken).ConfigureAwait(false);
            await Task.Delay(_thumbs.Count > 0 ? Refresh : RetryAfterFailure, stoppingToken).ConfigureAwait(false);
        }
    }

    private async Task LoadAsync(CancellationToken ct)
    {
        if (!await _loading.WaitAsync(0, ct).ConfigureAwait(false)) return; // already loading
        try
        {
            var thumbs = new Dictionary<string, string>(StringComparer.Ordinal);
            var sections = await GetAsync("/library/sections", ct).ConfigureAwait(false);
            foreach (var section in sections.Descendants("Directory"))
            {
                var (key, type) = ((string?)section.Attribute("key"), (string?)section.Attribute("type"));
                if (key is null || type is not ("movie" or "show")) continue;
                var list = await GetAsync($"/library/sections/{Uri.EscapeDataString(key)}/all?type={(type == "movie" ? 1 : 2)}&includeGuids=1", ct).ConfigureAwait(false);
                foreach (var (imdbId, thumb) in ThumbsFrom(list)) thumbs.TryAdd(imdbId, thumb);
            }
            _thumbs = thumbs;
            _loadedUtc = DateTime.UtcNow;
            log.LogInformation("Plex posters: {Count} titles", thumbs.Count);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or System.Xml.XmlException && !ct.IsCancellationRequested)
        {
            _nextTryUtc = DateTime.UtcNow + RetryAfterFailure;
            log.LogWarning("Plex posters not loaded ({Message}); {Count} titles from before stay in use", ex.Message, _thumbs.Count);
        }
        finally
        {
            _loading.Release();
        }
    }

    /// <summary>(IMDb id, thumb path) for each movie (Video) or show (Directory) with an imdb:// guid and a thumb.</summary>
    internal static IEnumerable<(string ImdbId, string Thumb)> ThumbsFrom(XDocument list) =>
        list.Root?.Elements()
            .Where(e => e.Name.LocalName is "Video" or "Directory")
            .Select(e => (
                ImdbId: e.Elements("Guid").Select(g => (string?)g.Attribute("id")).FirstOrDefault(id => id?.StartsWith("imdb://", StringComparison.Ordinal) == true)?["imdb://".Length..],
                Thumb: (string?)e.Attribute("thumb")))
            .Where(t => !string.IsNullOrEmpty(t.ImdbId) && t.Thumb?.StartsWith('/') == true)
            .Select(t => (t.ImdbId!, t.Thumb!))
        ?? [];

    private async Task<XDocument> GetAsync(string path, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, PlexUrl + path);
        request.Headers.Add("X-Plex-Token", options.Value.PlexToken);
        request.Headers.Accept.ParseAdd("application/xml");
        using var response = await http.CreateClient(HttpClientName).SendAsync(request, ct).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
        return await XDocument.LoadAsync(stream, LoadOptions.None, ct).ConfigureAwait(false);
    }

    public override void Dispose()
    {
        _loading.Dispose();
        base.Dispose();
    }
}
