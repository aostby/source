using System.Net;
using Kolibri.Kino.Core.Abstractions;

namespace Kolibri.Kino.Controllers.Ratings;

/// <summary>
/// "Import my ratings": the newest ratings file Kino on the web sent to Uranus (get.php in uranus/apps/KolibriKino,
/// type "Kino ratings", JSON), taken in as <see cref="RatingController.ImportAsync"/> takes a file. Needs the Uranus URL
/// and token in the settings (<see cref="IsConfigured"/>).
/// </summary>
public sealed class WebRatings(ISettingsStore settings, HttpClient http, RatingController ratings)
{
    /// <summary>The name WebSync gives the file, without the date ("Kino ratings - 2026-11-03.json").</summary>
    public const string FileType = "Kino ratings";

    public bool IsConfigured => settings.Current.GetUranusGetUrl() is not null && !string.IsNullOrWhiteSpace(settings.Current.UranusApiKey);

    /// <summary>Fetches and imports the newest ratings file; InvalidOperationException says what went wrong in words.</summary>
    public async Task<RatingImportResult> ImportAsync(CancellationToken ct = default)
    {
        var current = settings.Current;
        if (current.GetUranusGetUrl() is not { } url || string.IsNullOrWhiteSpace(current.UranusApiKey))
            throw new InvalidOperationException("Set the Uranus URL and token in Settings first.");

        using var request = new HttpRequestMessage(HttpMethod.Get, $"{url}?type={Uri.EscapeDataString(FileType)}&format=json");
        request.Headers.Add("X-Api-Key", current.UranusApiKey.Trim());
        using var response = await http.SendAsync(request, ct).ConfigureAwait(false);
        var body = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
        return response.StatusCode switch
        {
            HttpStatusCode.OK => await ratings.ImportAsync(body, ct).ConfigureAwait(false),
            HttpStatusCode.NotFound => throw new InvalidOperationException("Uranus has no ratings yet: rate a movie in Kino on the web first."),
            HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden => throw new InvalidOperationException("Uranus didn't accept the token (Settings → Uranus token)."),
            _ => throw new InvalidOperationException($"Uranus answered {(int)response.StatusCode} {response.ReasonPhrase}."),
        };
    }
}
