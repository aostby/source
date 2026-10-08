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
    public async Task<IReadOnlyList<SubtitleInfo>> SearchAsync(string imdbId, string languages, CancellationToken ct = default)
    {
        var key = settings.Current.SUBDLkey?.Trim();
        if (string.IsNullOrEmpty(key))
            throw new MovieInfoUnavailableException(
                "SubDL: no API key. Get a free key at subdl.com (Help, Prerequisites) and enter it in Settings.");

        var answer = await SubDlApi.SearchAsync(http, key, imdbId, languages, ct).ConfigureAwait(false);
        return answer.Subtitles?
            .Where(s => !string.IsNullOrWhiteSpace(s.Url))
            .Select(s => new SubtitleInfo(s.ReleaseName ?? s.Name ?? "subtitle", s.Language ?? s.Lang ?? "", s.Url!, s.HearingImpaired, s.Author))
            .ToList() ?? [];
    }

    public async Task<byte[]> DownloadAsync(SubtitleInfo subtitle, CancellationToken ct = default)
    {
        var url = subtitle.DownloadUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)
            ? subtitle.DownloadUrl
            : SubDlApi.DownloadBase + subtitle.DownloadUrl;
        return await http.GetByteArrayAsync(url, ct).ConfigureAwait(false);
    }
}

/// <summary>SubDL's search request and answer, shared with the connection test.</summary>
internal static class SubDlApi
{
    public const string SearchBase = "https://api.subdl.com/api/v1/subtitles";
    public const string DownloadBase = "https://dl.subdl.com";

    /// <summary>
    /// The answer for <paramref name="imdbId"/>. "Nothing found" is an empty answer; a refused key or the daily limit
    /// throws <see cref="MovieInfoUnavailableException"/> with SubDL's message.
    /// </summary>
    public static async Task<Answer> SearchAsync(HttpClient http, string key, string imdbId, string languages, CancellationToken ct)
    {
        var url = $"{SearchBase}?api_key={Uri.EscapeDataString(key)}&imdb_id={Uri.EscapeDataString(imdbId)}" +
                  $"&type=movie&languages={Uri.EscapeDataString(languages)}";
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
