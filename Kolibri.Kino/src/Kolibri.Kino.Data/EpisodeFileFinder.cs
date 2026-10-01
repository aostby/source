using System.Text.RegularExpressions;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.Data;

/// <summary>
/// Reads season and episode numbers from the video files in a series folder.
/// </summary>
/// <remarks>
/// Recognised, in order: "S01E02" (also "S01E01E02" / "S01E01-E02" for double episodes), "1x02", and an
/// episode-only marker ("E06", "EP51", "Episode 14") with the season taken from the folder name
/// ("Season 2", "Sesong 2", "S02", "Sauen Shaun 02"), or season 1 when the folder doesn't say.
/// </remarks>
public sealed partial class EpisodeFileFinder(IMediaFileScanner scanner) : IEpisodeFileFinder
{
    public async Task<(IReadOnlyList<EpisodeFile> Episodes, IReadOnlyList<string> Unrecognised)> FindAsync(string seriesFolder, CancellationToken ct = default)
    {
        var files = await scanner.FindVideoFilesAsync(seriesFolder, ct).ConfigureAwait(false);
        var episodes = new List<EpisodeFile>();
        var unrecognised = new List<string>();
        foreach (var file in files.Order(StringComparer.OrdinalIgnoreCase))
        {
            if (Parse(file) is { } episode) episodes.Add(episode);
            else unrecognised.Add(file);
        }
        return (episodes, unrecognised);
    }

    /// <summary>The episode a file holds, or null when its name doesn't say.</summary>
    public static EpisodeFile? Parse(string path)
    {
        var name = Path.GetFileNameWithoutExtension(path);

        if (SeasonEpisode().Match(name) is { Success: true } se)
        {
            var numbers = se.Groups["e"].Captures.Select(c => int.Parse(c.Value)).ToList();
            // "S01E01-E03" means episodes 1 to 3.
            if (numbers.Count == 2 && se.Value.Contains('-') && numbers[1] > numbers[0] + 1)
                numbers = Enumerable.Range(numbers[0], numbers[1] - numbers[0] + 1).ToList();
            return new EpisodeFile(path, int.Parse(se.Groups["s"].Value), numbers);
        }

        if (CrossFormat().Match(name) is { Success: true } x)
            return new EpisodeFile(path, int.Parse(x.Groups["s"].Value), [int.Parse(x.Groups["e"].Value)]);

        if (EpisodeOnly().Match(name) is { Success: true } e)
            return new EpisodeFile(path, SeasonFromFolder(path) ?? 1, [int.Parse(e.Groups["e"].Value)]);

        return null;
    }

    private static int? SeasonFromFolder(string path)
    {
        var folder = Path.GetFileName(Path.GetDirectoryName(path)) ?? "";
        if (FolderSeason().Match(folder) is { Success: true } m) return int.Parse(m.Groups["s"].Value);
        if (TrailingNumber().Match(folder) is { Success: true } t) return int.Parse(t.Groups["s"].Value);
        return null;
    }

    // (?<![a-z0-9]) rather than \b, so "SauenShaun_S06E02" matches after the underscore.
    [GeneratedRegex(@"(?<![a-z0-9])S(?<s>\d{1,2})[ ._-]?E(?<e>\d{1,3})(?:[ ._-]*-?[ ._-]*E(?<e>\d{1,3}))*", RegexOptions.IgnoreCase)]
    private static partial Regex SeasonEpisode();

    [GeneratedRegex(@"(?<![\dx])(?<s>\d{1,2})x(?<e>\d{2,3})(?!\d)", RegexOptions.IgnoreCase)]
    private static partial Regex CrossFormat();

    [GeneratedRegex(@"(?:\b|_)(?:Episode|Ep|EP|E)[ ._-]?(?<e>\d{1,3})(?!\d)", RegexOptions.IgnoreCase)]
    private static partial Regex EpisodeOnly();

    [GeneratedRegex(@"\b(?:Season|Sesong|Staffel|Series|S)[ ._-]*(?<s>\d{1,2})\b", RegexOptions.IgnoreCase)]
    private static partial Regex FolderSeason();

    [GeneratedRegex(@"[ _](?<s>\d{1,2})$")]
    private static partial Regex TrailingNumber();
}
