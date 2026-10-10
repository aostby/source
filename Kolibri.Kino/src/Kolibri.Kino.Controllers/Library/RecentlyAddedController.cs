using System.Globalization;
using Kolibri.Kino.Core.Abstractions;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Controllers.Library;

/// <summary>A library item and when its file arrived (UTC).</summary>
public sealed record RecentlyAddedItem(Item Item, DateTime AddedUtc);

/// <summary>
/// The library newest first. <see cref="WithoutDate"/> counts the items whose file couldn't be reached
/// (another machine's share, a moved file), which are left out of <see cref="Items"/>.
/// </summary>
public sealed record RecentlyAddedReport(IReadOnlyList<RecentlyAddedItem> Items, int WithoutDate);

/// <summary>
/// Recently added, as on Plex's home page: library items in the order their files arrived. The database has no
/// date for that, so it comes from the files the "FileItem" links point at.
/// </summary>
public sealed class RecentlyAddedController(IMovieRepository movies, IFileItemRepository files, IFileDates dates)
{
    public async Task<RecentlyAddedReport> GetAsync(CancellationToken ct = default)
    {
        var links = await files.GetAllAsync(ct).ConfigureAwait(false);
        // Nearly every item has a file, so one pass over the library beats a lookup per link.
        var items = (await movies.GetAllAsync(ct: ct).ConfigureAwait(false))
            .GroupBy(i => i.ImdbId, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
        var linked = links.Where(f => items.ContainsKey(f.ImdbId)).ToList();
        var added = await dates.GetAddedAsync(linked.Select(f => f.FullName), ct).ConfigureAwait(false);

        var recent = linked
            .Where(f => added.ContainsKey(f.FullName))
            .Select(f => new RecentlyAddedItem(items[f.ImdbId], added[f.FullName]))
            .OrderByDescending(r => r.AddedUtc)
            .ThenBy(r => r.Item.Title, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
        return new RecentlyAddedReport(recent, linked.Count - recent.Count);
    }

    /// <summary>
    /// The heading an item is listed under: Today, Yesterday, Last 7 days, Last 30 days, then the month
    /// ("September 2026"). Both times are local.
    /// </summary>
    public static string PeriodOf(DateTime addedLocal, DateTime nowLocal)
    {
        var days = (nowLocal.Date - addedLocal.Date).Days;
        return days switch
        {
            <= 0 => "Today",
            1 => "Yesterday",
            < 7 => "Last 7 days",
            < 30 => "Last 30 days",
            _ => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(addedLocal.ToString("MMMM yyyy", CultureInfo.CurrentCulture)),
        };
    }

    /// <summary>Short "how long ago" for a card: "2 h ago", "3 days ago", "5 weeks ago", or the date.</summary>
    public static string Ago(DateTime addedUtc, DateTime nowUtc)
    {
        var span = nowUtc - addedUtc;
        return span.TotalHours switch
        {
            < 1 => "Just now",
            < 24 => $"{(int)span.TotalHours} h ago",
            < 48 => "1 day ago",
            _ when span.TotalDays < 14 => $"{(int)span.TotalDays} days ago",
            _ when span.TotalDays < 60 => $"{(int)(span.TotalDays / 7)} weeks ago",
            _ => addedUtc.ToLocalTime().ToString("d", CultureInfo.CurrentCulture),
        };
    }
}
