using System.Globalization;

namespace Kolibri.Kino.Core;

public enum RatingBand
{
    /// <summary>No rating ("N/A", empty).</summary>
    Unknown,

    /// <summary>Below 6 (shown red).</summary>
    Low,

    /// <summary>6.0 to 6.9 (shown orange).</summary>
    Medium,

    /// <summary>7.0 and higher (shown green).</summary>
    High,
}

/// <summary>
/// How good an IMDb rating (0–10) is, for colouring. Replaces SilverScreen's MovieUtilites.ColorFromRating, which
/// used only the first digit (so 10 counted as 1) and had overlapping ranges.
/// </summary>
public static class RatingBands
{
    public static RatingBand Of(string? imdbRating) =>
        double.TryParse(imdbRating, NumberStyles.Float, CultureInfo.InvariantCulture, out var rating) ? Of(rating) : RatingBand.Unknown;

    public static RatingBand Of(double rating) => rating switch
    {
        < 6.0 => RatingBand.Low,
        < 7.0 => RatingBand.Medium,
        _ => RatingBand.High,
    };
}
