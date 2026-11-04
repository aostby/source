using System.Globalization;
using System.Text.RegularExpressions;
using Kolibri.Kino.Core;

namespace Kolibri.Kino.Controllers.Library;

/// <summary>One thing to move: a movie folder (with all in it), or a loose movie file (with its subtitles and the like).</summary>
/// <param name="Source">The folder or file now.</param>
/// <param name="Destination">The folder it ends up as (or in, for a file): "_byYear\2007\Grindhouse (2007)".</param>
/// <param name="Companions">For a file: the files next to it with the same name (Movie (2007).srt, .nfo …), moved along.</param>
public sealed record YearMove(string Source, string Destination, int Year, bool IsFile, IReadOnlyList<string> Companions);

/// <summary>What sorting a folder by year will do: the moves, and what's left where it is and why.</summary>
/// <param name="NoYear">Folders and files with no year in the name.</param>
/// <param name="Taken">Folders and files whose place in the year folder is taken already.</param>
public sealed record YearPlan(string Root, string Target, IReadOnlyList<YearMove> Moves, IReadOnlyList<string> NoYear, IReadOnlyList<string> Taken);

/// <summary>What moving gave: how many were moved, and the errors (one line each).</summary>
public sealed record YearResult(int Moved, IReadOnlyList<string> Errors);

/// <summary>
/// File → Tools → Sort folder by year (SilverScreen's "Large movie folder … move directories based on movie year"): every
/// movie folder in a big folder is moved to "_byYear\{year}\{folder}", the year from the folder's name (else from its
/// biggest movie file's name). A movie file lying loose in the folder gets a folder of its own, named as the file:
/// "_byYear\{year}\{file name}\{file}", with the files of the same name next to it (subtitles, .nfo …).
/// Only the folder's own folders and files are moved, whole, so a Sample or Subs folder stays in its movie folder.
/// Moves are renames on the same drive, so they're quick and nothing is copied.
/// </summary>
public static partial class SortByYear
{
    /// <summary>The folder made in the chosen folder for the year folders (as SilverScreen did).</summary>
    public const string TargetName = "_byYear";

    /// <summary>Looks through <paramref name="root"/> and says what would be moved where, without moving anything.</summary>
    public static YearPlan Plan(string root, DateTime? today = null)
    {
        var rootDir = new DirectoryInfo(root);
        if (!rootDir.Exists) throw new DirectoryNotFoundException($"{root} doesn't exist.");
        var target = Path.Combine(rootDir.FullName, TargetName);
        var maxYear = (today ?? DateTime.Today).Year + 2;
        var moves = new List<YearMove>();
        var noYear = new List<string>();
        var taken = new List<string>();
        var planned = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var dir in rootDir.EnumerateDirectories().OrderBy(d => d.Name, StringComparer.CurrentCultureIgnoreCase))
        {
            // The year folders themselves (also year folders from earlier, "2007"), and hidden or system folders, stay.
            if (dir.Name.Equals(TargetName, StringComparison.OrdinalIgnoreCase) || IsYearFolder().IsMatch(dir.Name)
                || (dir.Attributes & (FileAttributes.Hidden | FileAttributes.System)) != 0) continue;
            var videos = Videos(dir).ToList();
            if (videos.Count == 0) continue; // not a movie folder

            var year = YearOf(dir.Name, maxYear) ?? YearOf(Path.GetFileNameWithoutExtension(videos.MaxBy(v => v.Length)!.Name), maxYear);
            if (year is not { } y)
            {
                noYear.Add(dir.Name);
                continue;
            }
            var destination = Path.Combine(target, y.ToString(CultureInfo.InvariantCulture), dir.Name);
            if (Directory.Exists(destination) || File.Exists(destination) || !planned.Add(destination))
            {
                taken.Add(dir.Name);
                continue;
            }
            moves.Add(new YearMove(dir.FullName, destination, y, IsFile: false, []));
        }

        var files = rootDir.EnumerateFiles().ToList();
        foreach (var file in files.Where(f => MovieFileRules.VideoExtensions.Contains(f.Extension)).OrderBy(f => f.Name, StringComparer.CurrentCultureIgnoreCase))
        {
            var name = Path.GetFileNameWithoutExtension(file.Name).Trim();
            if (YearOf(name, maxYear) is not { } y)
            {
                noYear.Add(file.Name);
                continue;
            }
            // In a folder named as the file; one there already (a folder of the same name moved just before) is used.
            var destination = Path.Combine(target, y.ToString(CultureInfo.InvariantCulture), name);
            if (File.Exists(Path.Combine(destination, file.Name)) || File.Exists(destination))
            {
                taken.Add(file.Name);
                continue;
            }
            var companions = files
                .Where(f => f != file && !MovieFileRules.VideoExtensions.Contains(f.Extension)
                            && f.Name.StartsWith(name + ".", StringComparison.OrdinalIgnoreCase))
                .Select(f => f.FullName)
                .ToList();
            moves.Add(new YearMove(file.FullName, destination, y, IsFile: true, companions));
        }

        return new YearPlan(rootDir.FullName, target, moves, noYear, taken);
    }

    /// <summary>Does the moves; one that fails is told about and the rest go on.</summary>
    public static YearResult Execute(YearPlan plan, IProgress<string>? progress = null, CancellationToken ct = default)
    {
        var moved = 0;
        var errors = new List<string>();
        foreach (var move in plan.Moves)
        {
            ct.ThrowIfCancellationRequested();
            var name = Path.GetFileName(move.Source);
            progress?.Report($"{moved + errors.Count + 1} of {plan.Moves.Count}: {name} → {move.Year}");
            try
            {
                Directory.CreateDirectory(move.IsFile ? move.Destination : Path.GetDirectoryName(move.Destination)!);
                if (move.IsFile)
                {
                    File.Move(move.Source, Path.Combine(move.Destination, name));
                    foreach (var companion in move.Companions.Where(File.Exists))
                    {
                        var to = Path.Combine(move.Destination, Path.GetFileName(companion));
                        if (!File.Exists(to)) File.Move(companion, to);
                    }
                }
                else
                {
                    Directory.Move(move.Source, move.Destination);
                }
                moved++;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                errors.Add($"{name}: {ex.Message}");
            }
        }
        return new YearResult(moved, errors);
    }

    /// <summary>
    /// The year in a movie's folder or file name: one in brackets first ("Blade Runner 2049 (2017)"), else the last one
    /// standing alone ("Blade.Runner.2049.2017.1080p", "2001 A Space Odyssey 1968"). Not one after
    /// <paramref name="maxYear"/>, so "Blade Runner 2049" alone has none. Null without one.
    /// </summary>
    public static int? YearOf(string name, int maxYear)
    {
        static int Year(Match m) => int.Parse(m.Groups["y"].Value, CultureInfo.InvariantCulture);
        if (BracketYear().Matches(name).Select(Year).Where(y => y <= maxYear).LastOrDefault() is > 0 and var inBrackets) return inBrackets;
        return LooseYear().Matches(name).Select(Year).Where(y => y <= maxYear).LastOrDefault() is > 0 and var loose ? loose : null;
    }

    private static IEnumerable<FileInfo> Videos(DirectoryInfo dir)
    {
        try
        {
            return dir.EnumerateFiles("*", new EnumerationOptions { RecurseSubdirectories = true, IgnoreInaccessible = true })
                .Where(f => MovieFileRules.VideoExtensions.Contains(f.Extension))
                .ToList();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return [];
        }
    }

    [GeneratedRegex(@"[\(\[]\s*(?<y>(?:19|20)\d\d)\s*[\)\]]")]
    private static partial Regex BracketYear();

    [GeneratedRegex(@"(?<![\d])(?<y>(?:19|20)\d\d)(?![\d])")]
    private static partial Regex LooseYear();

    [GeneratedRegex(@"^(19|20)\d\d$")]
    private static partial Regex IsYearFolder();
}
