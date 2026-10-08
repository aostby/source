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
                PlexAsync(settings, ct))
            .ConfigureAwait(false);
        return checks;
    }

    private static async Task<ConnectionCheck> OmdbAsync(string? key, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(key)) return new("OMDb", false, "No key. Details, posters and title lookups need one.");
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
        if (string.IsNullOrWhiteSpace(key)) return new("TMDb", true, "No key; scans skip TMDb (optional).");
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
        if (string.IsNullOrWhiteSpace(key)) return new("SubDL", true, "No key; subtitles can't be downloaded (optional).");
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
        if (url is null || string.IsNullOrWhiteSpace(settings.XPlexToken))
            return new("Plex", true, "Server name or token not set; Plex is skipped (optional).");
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
}
