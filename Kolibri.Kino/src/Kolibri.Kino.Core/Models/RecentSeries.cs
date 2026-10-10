namespace Kolibri.Kino.Core.Models;

/// <summary>An episode a media server got lately: S02E03 "Title", added at <paramref name="Added"/> (local time).</summary>
public sealed record RecentEpisode(int Season, int Episode, string Title, DateTime Added)
{
    public string Code => $"S{Season:00}E{Episode:00}";
}

/// <summary>A series with new episodes on the media server, the newest added at <see cref="LastAdded"/>.</summary>
/// <param name="ImdbId">The series' IMDb id, when the server knows it.</param>
/// <param name="Episodes">The new episodes, newest first.</param>
public sealed record RecentSeriesUpdate(string? ImdbId, string Title, IReadOnlyList<RecentEpisode> Episodes)
{
    public DateTime LastAdded => Episodes.Count > 0 ? Episodes.Max(e => e.Added) : DateTime.MinValue;

    /// <summary>"S02E01–S02E03" or "S02E01, S04E02 (+3)": the new episodes in order, briefly.</summary>
    public string Describe()
    {
        var codes = Episodes.OrderBy(e => e.Season).ThenBy(e => e.Episode).Select(e => e.Code).Distinct().ToList();
        return codes.Count switch
        {
            0 => "",
            1 => codes[0],
            <= 4 => string.Join(", ", codes),
            _ => $"{codes[0]}–{codes[^1]} ({codes.Count})",
        };
    }
}
