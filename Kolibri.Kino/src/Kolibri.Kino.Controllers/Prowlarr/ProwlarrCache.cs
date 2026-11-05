using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Kolibri.Kino.Controllers.Prowlarr;

/// <summary>
/// Prowlarr's answers kept a short while in files (the "Cache" folder next to the .exe, not in the database), so a title
/// searched again soon after isn't asked about again: Prowlarr asks all its indexers, which takes a while. One file per
/// IMDb id ("tt0462322.json", an episode "tt5875444_S06E03.json") with every release found, unfiltered, so the rules in
/// appsettings.json are applied anew each time. A file older than <see cref="MaxAge"/> isn't used, and is deleted when
/// the Prowlarr window opens. Prowlarr's links carry its API key; it's replaced by a placeholder in the files and put
/// back when read. Problems with the files never stop a search: they're logged (Trace) and Prowlarr is asked instead.
/// </summary>
public sealed class ProwlarrCache(string folder, TimeSpan maxAge, ILogger<ProwlarrCache> logger, TimeProvider? clock = null)
{
    private const string KeyPlaceholder = "{prowlarr-api-key}";
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web) { WriteIndented = false };
    private readonly TimeProvider _clock = clock ?? TimeProvider.System;

    public string Folder => folder;
    public TimeSpan MaxAge => maxAge;

    /// <summary>Off with 0 minutes (Kino:Prowlarr:CacheMinutes).</summary>
    public bool Enabled => maxAge > TimeSpan.Zero;

    /// <summary>Makes the folder if it isn't there, and deletes the files that are too old.</summary>
    public void Prepare()
    {
        if (!Enabled) return;
        try
        {
            Directory.CreateDirectory(folder);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            logger.LogTrace(ex, "Prowlarr cache: the folder {Folder} couldn't be made", folder);
            return;
        }

        var deleted = 0;
        try
        {
            foreach (var file in new DirectoryInfo(folder).EnumerateFiles("*.json"))
            {
                if (!TooOld(file)) continue;
                try
                {
                    file.Delete();
                    deleted++;
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    logger.LogTrace(ex, "Prowlarr cache: {File} couldn't be deleted", file.Name);
                }
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            logger.LogTrace(ex, "Prowlarr cache: {Folder} couldn't be looked through", folder);
        }
        if (deleted > 0) logger.LogTrace("Prowlarr cache: deleted {Count} file(s) older than {Minutes} minutes", deleted, maxAge.TotalMinutes);
    }

    /// <summary>The releases kept for <paramref name="imdbId"/> from a search in these categories, if fresh enough; else null.</summary>
    public IReadOnlyList<ProwlarrRelease>? Get(string? imdbId, IReadOnlyList<int> categories, string apiKey)
    {
        if (!Enabled || PathOf(imdbId) is not { } path) return null;
        try
        {
            var file = new FileInfo(path);
            if (!file.Exists) return null;
            if (TooOld(file))
            {
                logger.LogTrace("Prowlarr cache: {File} is older than {Minutes} minutes, not used", file.Name, maxAge.TotalMinutes);
                return null;
            }
            var entry = JsonSerializer.Deserialize<Entry>(File.ReadAllText(path), Json);
            if (entry?.Releases is null || !entry.Categories.SequenceEqual(categories)) return null;
            return entry.Releases.Select(r => r with
            {
                MagnetUrl = WithKey(r.MagnetUrl, apiKey),
                DownloadUrl = WithKey(r.DownloadUrl, apiKey),
                InfoUrl = WithKey(r.InfoUrl, apiKey),
            }).ToList();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            logger.LogTrace(ex, "Prowlarr cache: {Path} couldn't be read", path);
            return null;
        }
    }

    /// <summary>Keeps every release Prowlarr found for <paramref name="imdbId"/> (nothing without an id).</summary>
    public void Put(string? imdbId, string query, IReadOnlyList<int> categories, IReadOnlyList<ProwlarrRelease> releases, string apiKey)
    {
        if (!Enabled || PathOf(imdbId) is not { } path) return;
        try
        {
            Directory.CreateDirectory(folder);
            var entry = new Entry(query, categories.ToList(), _clock.GetUtcNow(), releases.Select(r => r with
            {
                MagnetUrl = WithoutKey(r.MagnetUrl, apiKey),
                DownloadUrl = WithoutKey(r.DownloadUrl, apiKey),
                InfoUrl = WithoutKey(r.InfoUrl, apiKey),
            }).ToList());
            // Written beside it first, so a file half written is never read.
            var temp = path + ".tmp";
            File.WriteAllText(temp, JsonSerializer.Serialize(entry, Json));
            File.Move(temp, path, overwrite: true);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            logger.LogTrace(ex, "Prowlarr cache: {Path} couldn't be written", path);
        }
    }

    private bool TooOld(FileInfo file) => _clock.GetUtcNow() - file.LastWriteTimeUtc > maxAge;

    /// <summary>"tt0462322" → Cache\tt0462322.json; null without an id, or with one that isn't a plain IMDb id.</summary>
    private string? PathOf(string? imdbId)
    {
        var id = imdbId?.Trim();
        if (string.IsNullOrEmpty(id) || !id.StartsWith("tt", StringComparison.OrdinalIgnoreCase)
            || id.Any(c => !char.IsAsciiLetterOrDigit(c) && c != '_')) return null;
        return Path.Combine(folder, id.ToLowerInvariant() + ".json");
    }

    private static string? WithoutKey(string? url, string apiKey) =>
        url is null || apiKey.Length == 0 ? url : url.Replace(apiKey, KeyPlaceholder, StringComparison.Ordinal);

    private static string? WithKey(string? url, string apiKey) =>
        url?.Replace(KeyPlaceholder, apiKey, StringComparison.Ordinal);

    private sealed record Entry(string Query, List<int> Categories, DateTimeOffset Saved, List<ProwlarrRelease> Releases);
}
