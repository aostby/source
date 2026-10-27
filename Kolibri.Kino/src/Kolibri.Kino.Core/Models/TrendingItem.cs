namespace Kolibri.Kino.Core.Models;

/// <summary>TMDb's three trending lists.</summary>
public enum TrendingKind
{
    Movies,
    Series,
    People,
}

/// <summary>
/// One title or person on a TMDb trending list (this week), kept in Kino's "KinoTrending" collection between visits.
/// </summary>
/// <param name="TmdbId">TMDb's id: the movie, TV show or person.</param>
/// <param name="Title">The title, or the person's name.</param>
/// <param name="Year">The year it came out (a series: its first year); null for a person.</param>
/// <param name="ImdbId">The IMDb id of a movie or series, when TMDb knows it; null for a person.</param>
/// <param name="ImageUrl">TMDb's poster, or a person's photo; null when there is none.</param>
/// <param name="TmdbRating">TMDb's users' average, 0–10; null for a person or when nobody has voted.</param>
/// <param name="Overview">The plot, as TMDb has it; null for a person.</param>
/// <param name="KnownFor">A person's best known titles; empty for a movie or series.</param>
public sealed record TrendingItem(
    int TmdbId, string Title, string? Year, string? ImdbId, string? ImageUrl, double? TmdbRating, string? Overview,
    IReadOnlyList<string> KnownFor);
