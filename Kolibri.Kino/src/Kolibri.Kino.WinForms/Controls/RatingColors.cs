using Kolibri.Kino.Core;

namespace Kolibri.Kino.WinForms.Controls;

/// <summary>Colours for <see cref="RatingBand"/>: red below 6, orange 6–6.9, green from 7.</summary>
internal static class RatingColors
{
    public static readonly Color Low = Color.FromArgb(192, 0, 0);
    public static readonly Color Medium = Color.FromArgb(230, 120, 0);
    public static readonly Color High = Color.FromArgb(0, 140, 0);

    /// <summary>The band's colour, or null for an unknown rating.</summary>
    public static Color? For(string? imdbRating) => RatingBands.Of(imdbRating) switch
    {
        RatingBand.Low => Low,
        RatingBand.Medium => Medium,
        RatingBand.High => High,
        _ => null,
    };
}
