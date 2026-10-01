using System.Text.RegularExpressions;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using MovieFileLibrary;

namespace Kolibri.Kino.Data;

/// <summary>
/// Reads title, year and IMDb id from a video file's name and its folder's name.
/// </summary>
/// <remarks>
/// MovieFileLibrary only sees the file name, but folders often carry the real title, the year and
/// an "{imdb-tt…}" tag (e.g. "Baby Einstein {imdb-tt9808494}\06BE_MACDONALD.avi"), so both are used.
/// </remarks>
public sealed partial class MovieFileNameParser : IMovieFileNameParser
{
    private readonly MovieDetector _detector = new();
    private readonly Lock _lock = new();

    public ParsedMovieFile Parse(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        var folderName = Path.GetFileName(Path.GetDirectoryName(filePath)) ?? string.Empty;

        var file = Detect(fileName);
        var folder = folderName.Length > 0 ? Detect(folderName + ".mkv") : null;

        var fileYear = ValidYear(file?.Year);
        var folderYear = ValidYear(YearInBrackets().Match(folderName).Groups[1].Value) ?? ValidYear(folder?.Year);

        string?[] fromFile = [file?.Title];
        string?[] fromFolder = [folder?.Title, AfterLastDash(folderName)];

        // The name that carries the year is usually the deliberate one.
        var ordered = fileYear is null && folderYear is not null ? fromFolder.Concat(fromFile) : fromFile.Concat(fromFolder);
        var titles = ordered
            .Select(Clean)
            .Where(t => t.Count(char.IsLetter) >= 2)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return new ParsedMovieFile(
            titles,
            fileYear ?? folderYear,
            FindImdbId(fileName) ?? FindImdbId(folderName),
            file?.IsSeries == true);
    }

    private MovieFile? Detect(string name)
    {
        lock (_lock)
        {
            var info = _detector.GetInfo(name);
            return info.IsSuccess ? info : null;
        }
    }

    private static string? FindImdbId(string name) =>
        ImdbId().Match(name) is { Success: true } m ? m.Groups[1].Value.ToLowerInvariant() : null;

    private static int? ValidYear(string? text) =>
        int.TryParse(text, out var year) && year >= 1900 && year <= DateTime.Now.Year + 1 ? year : null;

    /// <summary>"Arthurs julegaverace (2011) - Arthur Christmas" → "Arthur Christmas" (often the original title).</summary>
    private static string? AfterLastDash(string name)
    {
        var at = name.LastIndexOf(" - ", StringComparison.Ordinal);
        return at > 0 ? name[(at + 3)..] : null;
    }

    private static string Clean(string? title)
    {
        if (string.IsNullOrWhiteSpace(title)) return string.Empty;
        var cleaned = Noise().Replace(title, " ");
        cleaned = Whitespace().Replace(cleaned.Replace('.', ' ').Replace('_', ' '), " ");
        return cleaned.Trim(' ', '-', '(', '[');
    }

    /// <summary>"{imdb-tt0133093}" as Plex/Jellyfin name it, or a bare "tt0133093".</summary>
    [GeneratedRegex(@"(?:\{imdb-|\b)(tt\d{7,9})\b", RegexOptions.IgnoreCase)]
    private static partial Regex ImdbId();

    [GeneratedRegex(@"[\(\[]((?:19|20)\d{2})[\)\]]")]
    private static partial Regex YearInBrackets();

    /// <summary>Release-group and language tags seen in the library that are never part of a title.</summary>
    [GeneratedRegex(@"\b(?:norsk|svensk|dansk)(?:\s+(?:tale|tekst))?\b|\bnorwegian\b|\bnorge\b|\bxvid\b|\bdvd\s?rip\b|\bnicemaniac\b|\brippet\s+av\s+\S+|\bripp\s+\S+|\{[^}]*\}|\[[^\]]*\]|\((?:19|20)\d{2}\)", RegexOptions.IgnoreCase)]
    private static partial Regex Noise();

    [GeneratedRegex(@"\s{2,}")]
    private static partial Regex Whitespace();
}
