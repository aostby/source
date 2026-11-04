using System.Xml.Linq;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using OMDbApiNet;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;

namespace Kolibri.Kino.Data;

/// <summary>
/// One small request per service with the given (possibly unsaved) settings.
/// </summary>
public sealed class ConnectionTester(HttpClient http) : IConnectionTester
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(15);

    public async Task<IReadOnlyList<ConnectionCheck>> TestAsync(UserSettings settings, CancellationToken ct = default)
    {
        var checks = await Task.WhenAll(OmdbAsync(settings.OMDBkey, ct), TmdbAsync(settings.TMDBkey, ct), SubDlAsync(settings.SUBDLkey, ct),
                PlexAsync(settings, ct), UranusAsync(settings, ct), ProwlarrAsync(settings, ct))
            .ConfigureAwait(false);
        return checks;
    }

    private static async Task<ConnectionCheck> OmdbAsync(string? key, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(key)) return new("OMDb", false, "No key. Details, posters and title lookups need one.", "key");
        try
        {
            var item = await new AsyncOmdbClient(key.Trim(), false).GetItemByIdAsync("tt0133093", false).WaitAsync(Timeout, ct).ConfigureAwait(false);
            return new("OMDb", item?.Response == "True", item?.Response == "True" ? "Key works." : "No answer for a known movie.");
        }
        catch (NullReferenceException ex) when (OmdbErrors.IsLibraryCrash(ex))
        {
            // OMDbApiNet crashes on HTTP 401 ("Request limit reached!", "Invalid API key!"); show OMDb's own words.
            return new("OMDb", false, await OmdbErrors.ExplainAsync(key.Trim(), ct).ConfigureAwait(false));
        }
        catch (Exception ex) when (ex is not OperationCanceledException || !ct.IsCancellationRequested)
        {
            return new("OMDb", false, ex.Message);
        }
    }

    private static async Task<ConnectionCheck> TmdbAsync(string? key, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(key)) return new("TMDb", true, "No key; scans skip TMDb (optional).", "key");
        try
        {
            using var client = new TMDbClient(key.Trim());
            var movie = await client.GetMovieAsync(603, MovieMethods.Undefined, ct).WaitAsync(Timeout, ct).ConfigureAwait(false);
            return new("TMDb", movie is not null, movie is not null ? "Key works." : "Key rejected or no answer.");
        }
        catch (Exception ex) when (ex is not OperationCanceledException || !ct.IsCancellationRequested)
        {
            return new("TMDb", false, ex.Message);
        }
    }

    private async Task<ConnectionCheck> SubDlAsync(string? key, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(key)) return new("SubDL", true, "No key; subtitles can't be downloaded (optional).", "key");
        try
        {
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
            timeout.CancelAfter(Timeout);
            await SubDlApi.SearchAsync(http, key.Trim(), "tt0133093", "EN", timeout.Token).ConfigureAwait(false);
            return new("SubDL", true, "Key works.");
        }
        catch (Exception ex) when (ex is not OperationCanceledException || !ct.IsCancellationRequested)
        {
            return new("SubDL", false, ex.Message);
        }
    }

    private async Task<ConnectionCheck> PlexAsync(UserSettings settings, CancellationToken ct)
    {
        var url = settings.GetPlexBaseUrl();
        if (Missing((url is null, "server name"), (string.IsNullOrWhiteSpace(settings.XPlexToken), "token")) is { } missing)
            return new("Plex", true, $"No {missing}; Plex is skipped (optional).", missing);
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url + "/");
            request.Headers.Add("X-Plex-Token", settings.XPlexToken.Trim());
            request.Headers.Add("Accept", "application/xml");
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
            timeout.CancelAfter(Timeout);
            using var response = await http.SendAsync(request, timeout.Token).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return new("Plex", false, $"{url} rejected the token.");
            response.EnsureSuccessStatusCode();

            var root = XDocument.Parse(await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false)).Root;
            return new("Plex", true, $"Connected to {(string?)root?.Attribute("friendlyName") ?? url} (Plex {(string?)root?.Attribute("version")}).");
        }
        catch (Exception ex) when (ex is not OperationCanceledException || !ct.IsCancellationRequested)
        {
            return new("Plex", false, $"{url}: {ex.Message}");
        }
    }

    /// <summary>get.php's list of files on Uranus (Import my ratings, File → Prowlarr's wishlist).</summary>
    private async Task<ConnectionCheck> UranusAsync(UserSettings settings, CancellationToken ct)
    {
        var url = settings.GetUranusGetUrl();
        if (Missing((url is null, string.IsNullOrWhiteSpace(settings.UranusUrl) ? "URL" : "valid URL (http or https)"),
                (string.IsNullOrWhiteSpace(settings.UranusApiKey), "token")) is { } missing)
            return new("Uranus", true, $"No {missing}; Import my ratings and the wishlist from Uranus are off (optional).", missing);
        return await GetAsync("Uranus", url!, "X-Api-Key", settings.UranusApiKey!, "the token",
            _ => "Connected; the files Kino on the web sent can be fetched.", ct).ConfigureAwait(false);
    }

    /// <summary>Prowlarr's system status (File → Prowlarr).</summary>
    private async Task<ConnectionCheck> ProwlarrAsync(UserSettings settings, CancellationToken ct)
    {
        var url = settings.GetProwlarrBaseUrl();
        if (Missing((url is null, string.IsNullOrWhiteSpace(settings.ProwlarrUrl) ? "URL" : "valid URL (http or https)"),
                (string.IsNullOrWhiteSpace(settings.ProwlarrApiKey), "API key")) is { } missing)
            return new("Prowlarr", false, $"No {missing}; File → Prowlarr can't search.", missing);
        return await GetAsync("Prowlarr", url + "api/v1/system/status", "X-Api-Key", settings.ProwlarrApiKey!, "the API key", body =>
        {
            using var json = System.Text.Json.JsonDocument.Parse(body);
            var version = json.RootElement.TryGetProperty("version", out var v) ? v.GetString() : null;
            return $"Connected to {url} (Prowlarr {version}).";
        }, ct).ConfigureAwait(false);
    }

    /// <summary>One GET with a key header: OK with <paramref name="describe"/>'s words, or why not.</summary>
    private async Task<ConnectionCheck> GetAsync(string service, string url, string header, string key, string keyName,
        Func<string, string> describe, CancellationToken ct)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add(header, key.Trim());
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
            timeout.CancelAfter(Timeout);
            using var response = await http.SendAsync(request, timeout.Token).ConfigureAwait(false);
            if (response.StatusCode is System.Net.HttpStatusCode.Unauthorized or System.Net.HttpStatusCode.Forbidden)
                return new(service, false, $"{url} rejected {keyName}.");
            response.EnsureSuccessStatusCode();
            return new(service, true, describe(await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false)));
        }
        catch (Exception ex) when (ex is not OperationCanceledException || !ct.IsCancellationRequested)
        {
            return new(service, false, $"{url}: {ex.Message}");
        }
    }

    /// <summary>"server name and token": the names of the settings that are missing, or null when none is.</summary>
    private static string? Missing(params (bool IsMissing, string Name)[] settings) =>
        settings.Where(s => s.IsMissing).Select(s => s.Name).ToList() is { Count: > 0 } names ? string.Join(" and ", names) : null;
}
