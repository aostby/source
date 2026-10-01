using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.Core.Abstractions;

/// <summary>
/// Finds the episode video files in a series folder by their names (S01E02, 1x02, "Season 2" folders).
/// </summary>
public interface IEpisodeFileFinder
{
    /// <summary>Recognised episode files, plus the video files whose season/episode couldn't be read.</summary>
    Task<(IReadOnlyList<EpisodeFile> Episodes, IReadOnlyList<string> Unrecognised)> FindAsync(string seriesFolder, CancellationToken ct = default);
}
