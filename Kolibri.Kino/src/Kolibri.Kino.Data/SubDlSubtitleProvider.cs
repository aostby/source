using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.Data;

/// <summary>
/// SubDL's API (https://subdl.com/api-doc), as SilverScreen's SubDLSubtitleController used it: search by IMDb id
/// and languages, then download each subtitle's zip from dl.subdl.com. The key is the SubDL key in Settings.
/// </summary>
public sealed class SubDlSubtitleProvider(HttpClient http, ISettingsStore settings) : ISubtitleProvider
{
    public Task<IReadOnlyList<SubtitleInfo>> SearchAsync(string imdbId, string languages, CancellationToken ct = default) =>
        SearchAsync(imdbId, languages, null, ct);

    public Task<IReadOnlyList<SubtitleInfo>> SearchEpisodeAsync(string seriesImdbId, int season, int episode, string languages,
        CancellationToken ct = default) =>
        SearchAsync(seriesImdbId, languages, (season, episode), ct);

    private async Task<IReadOnlyList<SubtitleInfo>> SearchAsync(string imdbId, string languages, (int Season, int Episode)? episode,
        CancellationToken ct)
    {
        var key = settings.Current.SUBDLkey?.Trim();
        if (string.IsNullOrEmpty(key))
            throw new MovieInfoUnavailableException(
                "SubDL: no API key. Get a free key at subdl.com (Help, Prerequisites) and enter it in Settings.");

        var answer = await SubDlApi.SearchAsync(http, key, imdbId, languages, ct, episode).ConfigureAwait(false);
        return answer.Subtitles?
            .Where(s => !string.IsNullOrWhiteSpace(s.Url))
            .Select(s => new SubtitleInfo(s.ReleaseName ?? s.Name ?? "subtitle", s.Language ?? s.Lang ?? "", s.Url!, s.HearingImpaired, s.Author))
            .ToList() ?? [];
    }

    /// <summary>Longest wait SubDL may ask for (Retry-After) before one more try; a longer one ends the run.</summary>
    internal static TimeSpan MaxRetryWait { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// The zip. "Too many requests" (HTTP 429) is tried once more after the wait SubDL asks for (or 10 seconds); when it
    /// still says so, or asks for a longer wait, <see cref="MovieInfoUnavailableException"/> is thrown so a run stops
    /// instead of failing every download.
    /// </summary>
    public async Task<byte[]> DownloadAsync(SubtitleInfo subtitle, CancellationToken ct = default)
    {
        var url = subtitle.DownloadUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)
            ? subtitle.DownloadUrl
            : SubDlApi.DownloadBase + subtitle.DownloadUrl;
        for (var attempt = 1; ; attempt++)
        {
            using var response = await http.GetAsync(url, ct).ConfigureAwait(false);
            if (response.StatusCode != System.Net.HttpStatusCode.TooManyRequests)
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsByteArrayAsync(ct).ConfigureAwait(false);
            }

            var wait = response.Headers.RetryAfter?.Delta
                       ?? (response.Headers.RetryAfter?.Date is { } at ? at - DateTimeOffset.UtcNow : (TimeSpan?)null)
                       ?? TimeSpan.FromSeconds(Math.Min(10, MaxRetryWait.TotalSeconds));
            if (attempt > 1 || wait > MaxRetryWait)
                throw new MovieInfoUnavailableException(
                    "SubDL: too many downloads (HTTP 429). SubDL allows a limited number of downloads per day and in a short time" +
                    (wait > TimeSpan.Zero ? $"; it asks to wait {Describe(wait)}" : "") + ". Try again later.");
            if (wait > TimeSpan.Zero) await Task.Delay(wait, ct).ConfigureAwait(false);
        }

        static string Describe(TimeSpan t) => t.TotalMinutes >= 1 ? $"{Math.Ceiling(t.TotalMinutes):0} min" : $"{Math.Ceiling(t.TotalSeconds):0} s";
    }
}

/// <summary>SubDL's search request and answer, shared with the connection test.</summary>
internal static class SubDlApi
{
    public const string SearchBase = "https://api.subdl.com/api/v1/subtitles";
    public const string DownloadBase = "https://dl.subdl.com";

    /// <summary>
    /// The answer for <paramref name="imdbId"/>. "Nothing found" is an empty answer; a refused key or the daily limit
    /// throws <see cref="MovieInfoUnavailableException"/> with SubDL's message. With <paramref name="episode"/>,
    /// <paramref name="imdbId"/> is the series' id and the search is for that episode (type=tv).
    /// </summary>
    public static async Task<Answer> SearchAsync(HttpClient http, string key, string imdbId, string languages, CancellationToken ct,
        (int Season, int Episode)? episode = null)
    {
        var type = episode is { } e
            ? FormattableString.Invariant($"type=tv&season_number={e.Season}&episode_number={e.Episode}")
            : "type=movie";
        var url = $"{SearchBase}?api_key={Uri.EscapeDataString(key)}&imdb_id={Uri.EscapeDataString(imdbId)}" +
                  $"&{type}&languages={Uri.EscapeDataString(languages)}";
        using var response = await http.GetAsync(url, ct).ConfigureAwait(false);
        Answer? answer = null;
        try
        {
            answer = await response.Content.ReadFromJsonAsync<Answer>(ct).ConfigureAwait(false);
        }
        catch (System.Text.Json.JsonException)
        {
            // Not JSON: judged by the status code below.
        }

        if (answer is { Status: true }) return answer;
        var message = answer?.Error ?? $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}";
        if (!response.IsSuccessStatusCode || IsAboutKey(message))
            throw new MovieInfoUnavailableException($"SubDL: {message}");
        return new Answer { Status = true, Subtitles = [] };
    }

    private static bool IsAboutKey(string message) =>
        message.Contains("key", StringComparison.OrdinalIgnoreCase) || message.Contains("limit", StringComparison.OrdinalIgnoreCase);

    internal sealed class Answer
    {
        [JsonPropertyName("status")] public bool Status { get; set; }
        [JsonPropertyName("error")] public string? Error { get; set; }
        [JsonPropertyName("subtitles")] public List<Subtitle>? Subtitles { get; set; }
    }

    internal sealed class Subtitle
    {
        [JsonPropertyName("release_name")] public string? ReleaseName { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("lang")] public string? Lang { get; set; }
        [JsonPropertyName("language")] public string? Language { get; set; }
        [JsonPropertyName("author")] public string? Author { get; set; }
        [JsonPropertyName("url")] public string? Url { get; set; }
        [JsonPropertyName("hi")] public bool HearingImpaired { get; set; }
    }
}
