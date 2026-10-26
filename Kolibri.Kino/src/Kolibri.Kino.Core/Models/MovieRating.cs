namespace Kolibri.Kino.Core.Models;

/// <summary>
/// Your own rating of a movie or series, kept in Kino's "KinoRating" collection (_id = IMDb id) and sent to Plex as
/// your private rating there.
/// </summary>
/// <param name="Rating">1–10, Plex's scale: half a star each, so 7 is three and a half stars of five.</param>
/// <param name="Review">Your own words about it, or null. Only in Kino: Plex keeps reviews online, not on the server.</param>
/// <param name="Title">The title when it was rated, so an exported file can be read by a person.</param>
/// <param name="UpdatedUtc">When it was set; on import the newer of two ratings for a title wins.</param>
public sealed record MovieRating(string ImdbId, int Rating, string? Review, string? Title, DateTime UpdatedUtc)
{
    public const int Min = 1;
    public const int Max = 10;

    /// <summary>The rating as stars of five, e.g. 3.5.</summary>
    public double Stars => Rating / 2.0;
}
