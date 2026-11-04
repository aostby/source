using System.Net;
using Kolibri.Kino.Core.Abstractions;

namespace Kolibri.Kino.Controllers.Wishlist;

/// <summary>
/// The newest wishlist Kino on the web sent to Uranus (get.php in uranus/apps/KolibriKino, type "Wishlist", any format),
/// read by <see cref="WishlistFile"/>. Needs the Uranus URL and token in the settings (<see cref="IsConfigured"/>), as
/// <see cref="Ratings.WebRatings"/> does. Nothing is put on this wishlist: File → Prowlarr only searches for the titles.
/// </summary>
public sealed class WebWishlist(ISettingsStore settings, HttpClient http)
{
    /// <summary>The name WebSync gives the file, without the date ("Wishlist - 2026-11-03.csv").</summary>
    public const string FileType = "Wishlist";

    public bool IsConfigured => settings.Current.GetUranusGetUrl() is not null && !string.IsNullOrWhiteSpace(settings.Current.UranusApiKey);

    /// <summary>Fetches and reads the newest wishlist; InvalidOperationException says what went wrong in words.</summary>
    /// <exception cref="FormatException">The file isn't a wishlist <see cref="WishlistFile"/> can read.</exception>
    public async Task<WishlistFileRead> GetAsync(CancellationToken ct = default)
    {
        var current = settings.Current;
        if (current.GetUranusGetUrl() is not { } url || string.IsNullOrWhiteSpace(current.UranusApiKey))
            throw new InvalidOperationException("Set the Uranus URL and token in Settings first.");

        using var request = new HttpRequestMessage(HttpMethod.Get, $"{url}?type={Uri.EscapeDataString(FileType)}");
        request.Headers.Add("X-Api-Key", current.UranusApiKey.Trim());
        using var response = await http.SendAsync(request, ct).ConfigureAwait(false);
        var body = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
        return response.StatusCode switch
        {
            HttpStatusCode.OK => WishlistFile.Parse(body),
            HttpStatusCode.NotFound => throw new InvalidOperationException("Uranus has no wishlist yet: change the wishlist in Kino on the web first."),
            HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden => throw new InvalidOperationException("Uranus didn't accept the token (Settings → Uranus token)."),
            _ => throw new InvalidOperationException($"Uranus answered {(int)response.StatusCode} {response.ReasonPhrase}."),
        };
    }
}
