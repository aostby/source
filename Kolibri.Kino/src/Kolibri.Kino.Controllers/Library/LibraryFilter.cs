using System.Globalization;
using Kolibri.Kino.Controllers.Watchlists;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Controllers.Library;

/// <summary>
/// The Library page's filters on top of the title search: a genre, a year range, a lowest IMDb rating and an
/// actor or director. Null means not filtered. Items without a year or rating don't pass those filters.
/// </summary>
public sealed record LibraryFilter(string? Genre = null, int? YearFrom = null, int? YearTo = null, double? MinRating = null, string? Person = null)
{
    private static readonly CompareInfo Compare = CultureInfo.InvariantCulture.CompareInfo;

    /// <summary>Case and accents don't matter: "zoe" finds Zoë Saldaña.</summary>
    private const CompareOptions Loose = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace;

    public bool IsEmpty => Genre is null && YearFrom is null && YearTo is null && MinRating is null && string.IsNullOrWhiteSpace(Person);

    public bool Matches(Item item)
    {
        if (Genre is not null && !GenresOf(item).Any(g => Compare.Compare(g, Genre, Loose) == 0)) return false;

        if (YearFrom is not null || YearTo is not null)
        {
            if (YearOf(item) is not { } year || year < (YearFrom ?? int.MinValue) || year > (YearTo ?? int.MaxValue)) return false;
        }

        if (MinRating is { } min && WatchlistController.Rating(item.ImdbRating) < min) return false;

        if (!string.IsNullOrWhiteSpace(Person))
        {
            var name = Person.Trim();
            if (Compare.IndexOf(item.Actors ?? string.Empty, name, Loose) < 0 && Compare.IndexOf(item.Director ?? string.Empty, name, Loose) < 0) return false;
        }
        return true;
    }

    /// <summary>OMDb's "Action, Adventure, Sci-Fi" as separate genres; none for empty or "N/A".</summary>
    public static IEnumerable<string> GenresOf(Item item) =>
        (item.Genre ?? string.Empty).Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Where(g => g != "N/A");

    /// <summary>The first year: "2019" and the series' "2019–2023" are both 2019. Null when there is none.</summary>
    public static int? YearOf(Item item) =>
        item.Year is { Length: >= 4 } y && int.TryParse(y.AsSpan(0, 4), NumberStyles.None, CultureInfo.InvariantCulture, out var year) ? year : null;

    /// <summary>Each genre in <paramref name="items"/> with how many have it, alphabetically.</summary>
    public static IReadOnlyList<(string Genre, int Count)> Genres(IEnumerable<Item> items) =>
        items.SelectMany(GenresOf)
            .GroupBy(g => g, StringComparer.OrdinalIgnoreCase)
            .Select(g => (g.First(), g.Count()))
            .OrderBy(g => g.Item1, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
}
