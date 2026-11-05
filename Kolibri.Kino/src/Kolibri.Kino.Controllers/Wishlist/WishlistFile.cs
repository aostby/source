using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.Controllers.Wishlist;

/// <summary>What <see cref="WishlistFile.Parse"/> read.</summary>
/// <param name="Items">The titles, in the file's order; the IMDb id is "" where the file has none.</param>
/// <param name="Skipped">Rows without a title or a year.</param>
/// <param name="Format">CSV, JSON or XML.</param>
public sealed record WishlistFileRead(IReadOnlyList<WishlistItem> Items, int Skipped, string Format);

/// <summary>
/// Reads a wishlist from another program, or Kino's own: CSV (';', ',' or tab), JSON or XML. Each title needs a title
/// and a year; the field names may differ (Title, Name, Tittel …; Year, Released, År …) but must be ones that clearly
/// mean that, or the file is refused. An IMDb id (tt0462322, 462322 in an IMDb field, or an IMDb link) and the type
/// (movie, series, episode) are kept where there are.
/// </summary>
public static partial class WishlistFile
{
    // Field names compared without case, spaces, punctuation or accents ("IMDb id" = "imdb_id" = "imdbid", "År" = "ar").
    private static readonly string[] TitleNames =
        ["title", "name", "movietitle", "filmtitle", "seriestitle", "showtitle", "primarytitle", "originaltitle", "tittel", "navn", "filmnavn", "movie", "film"];
    private static readonly string[] YearNames =
        ["year", "releaseyear", "yearreleased", "startyear", "ar", "aar", "utgivelsesar", "produksjonsar"];
    private static readonly string[] DateNames =
        ["released", "releasedate", "releasedon", "firstairdate", "premiered", "premieredate"];
    private static readonly string[] ImdbNames =
        ["imdbid", "imdb", "imdbcode", "imdbtitleid", "imdbnumber", "imdburl", "imdblink", "tconst"];
    // Fields that may hold an IMDb id or link among other things: taken only when the value has a tt… id in it.
    private static readonly string[] OtherIdNames = ["id", "url", "link", "uri", "href"];
    private static readonly string[] TypeNames = ["type", "kind", "mediatype", "titletype"];
    private static readonly string[] GenreNames = ["genre", "genres", "sjanger"];
    private static readonly string[] RatingNames = ["imdbrating"];

    /// <summary>
    /// The titles of <paramref name="first"/> (Uranus' wishlist), then those of <paramref name="more"/> (Kino's own) it
    /// doesn't have: the same IMDb id, or without one the same title and year, is there once. Returns the list and how
    /// many were added from <paramref name="more"/>.
    /// </summary>
    public static (IReadOnlyList<WishlistItem> Items, int Added) Merge(IEnumerable<WishlistItem> first, IEnumerable<WishlistItem> more)
    {
        var items = new List<WishlistItem>();
        var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var titles = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
        var titlesWithoutId = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
        static string TitleKey(WishlistItem i) => $"{i.Title.Trim()}|{(i.Year?.Trim() is { Length: >= 4 } y ? y[..4] : "")}";

        bool Add(WishlistItem item)
        {
            var hasId = !string.IsNullOrWhiteSpace(item.ImdbId);
            var key = TitleKey(item);
            // Without an id, a title is the same as one there with that title and year; with one, the same as one with
            // that id, or one with that title and year and no id (two titles with ids are told apart by them).
            if (hasId ? ids.Contains(item.ImdbId.Trim()) || titlesWithoutId.Contains(key) : titles.Contains(key)) return false;
            if (hasId) ids.Add(item.ImdbId.Trim());
            else titlesWithoutId.Add(key);
            titles.Add(key);
            items.Add(item);
            return true;
        }

        foreach (var item in first) Add(item);
        var added = more.Count(Add);
        return (items, added);
    }

    /// <summary>
    /// The titles in <paramref name="text"/>: XML when it starts with '&lt;', JSON with '[' or '{', else CSV.
    /// </summary>
    /// <exception cref="FormatException">Empty, unreadable, or without a field that clearly is the title or the year.</exception>
    public static WishlistFileRead Parse(string text)
    {
        var body = text.TrimStart('﻿', ' ', '\t', '\r', '\n');
        if (body.Length == 0) throw new FormatException("The file is empty.");
        var (format, records) = body[0] switch
        {
            '<' => ("XML", XmlRecords(body)),
            '[' or '{' => ("JSON", JsonRecords(body)),
            _ => ("CSV", CsvRecords(body)),
        };

        var names = records.SelectMany(r => r.Keys).ToHashSet();
        if (!TitleNames.Any(names.Contains))
            throw new FormatException($"The {format} file has no field that is clearly the title (such as Title or Name).");
        if (!YearNames.Concat(DateNames).Any(names.Contains))
            throw new FormatException($"The {format} file has no field that is clearly the year (such as Year or Released).");

        var items = new List<WishlistItem>();
        var skipped = 0;
        foreach (var record in records)
        {
            var title = First(record, TitleNames);
            var year = YearOf(First(record, YearNames)) ?? YearOf(First(record, DateNames));
            if (title is null || year is null)
            {
                skipped++;
                continue;
            }
            items.Add(new WishlistItem
            {
                ImdbId = ImdbIdOf(record) ?? "",
                Title = title,
                Year = year,
                Type = TypeOf(First(record, TypeNames)),
                Genre = First(record, GenreNames),
                ImdbRating = First(record, RatingNames),
                Added = DateTime.Now,
            });
        }
        return new WishlistFileRead(items, skipped, format);
    }

    /// <summary>A field name as compared: lower case, letters and digits only, accents gone ("IMDb id" → "imdbid", "År" → "ar").</summary>
    public static string NameKey(string name)
    {
        var key = new StringBuilder(name.Length);
        foreach (var c in name.Normalize(NormalizationForm.FormD))
            if (char.IsLetterOrDigit(c)) key.Append(char.ToLowerInvariant(c));
        return key.ToString().Replace("ø", "o").Replace("æ", "ae");
    }

    private static string? First(Dictionary<string, string> record, string[] names) =>
        names.Select(n => record.GetValueOrDefault(n)).FirstOrDefault(v => !string.IsNullOrWhiteSpace(v) && v.Trim() != "N/A")?.Trim();

    /// <summary>"2007", "2008–2013" (a series' years), "2007-04-06" or "6 Apr 2007": the (first) year.</summary>
    private static string? YearOf(string? value) =>
        value is not null && Year().Match(value) is { Success: true } m ? m.Value : null;

    [GeneratedRegex(@"(?<!\d)(18|19|20)\d\d(?!\d)")]
    private static partial Regex Year();

    /// <summary>
    /// From an IMDb field: tt0462322, an IMDb link, or just the number (462322 → tt0462322); a Kino episode id
    /// (tt5875444_S06E03) is kept whole. From an id or link field, only a tt… id in it.
    /// </summary>
    private static string? ImdbIdOf(Dictionary<string, string> record)
    {
        foreach (var name in ImdbNames)
        {
            if (record.GetValueOrDefault(name)?.Trim() is not { Length: > 0 } value) continue;
            if (EpisodeId().Match(value) is { Success: true } episode)
                return WishlistItem.EpisodeId(episode.Groups["series"].Value.ToLowerInvariant(),
                    int.Parse(episode.Groups["season"].Value, CultureInfo.InvariantCulture), int.Parse(episode.Groups["episode"].Value, CultureInfo.InvariantCulture));
            if (TitleId().Match(value) is { Success: true } id) return id.Value.ToLowerInvariant();
            if (long.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var number) && number > 0)
                return "tt" + number.ToString("0000000", CultureInfo.InvariantCulture);
        }
        foreach (var name in OtherIdNames)
            if (record.GetValueOrDefault(name) is { } value && TitleId().Match(value) is { Success: true } id)
                return id.Value.ToLowerInvariant();
        return null;
    }

    [GeneratedRegex(@"\btt\d{7,9}\b", RegexOptions.IgnoreCase)]
    private static partial Regex TitleId();

    [GeneratedRegex(@"^(?<series>tt\d{7,9})_S(?<season>\d+)E(?<episode>\d+)$", RegexOptions.IgnoreCase)]
    private static partial Regex EpisodeId();

    /// <summary>movie, series or episode for the usual words (Film, TV series, tvEpisode …); else as written, in lower case.</summary>
    private static string? TypeOf(string? value) => NameKey(value ?? "") switch
    {
        "" => null,
        "movie" or "movies" or "film" or "feature" or "featurefilm" or "tvmovie" => "movie",
        "series" or "serie" or "tvseries" or "tvshow" or "show" or "tv" or "tvminiseries" or "miniseries" => "series",
        "episode" or "tvepisode" => "episode",
        _ => value!.Trim().ToLowerInvariant(),
    };

    /// <summary>One record per row after the header; the header's names as keys.</summary>
    private static List<Dictionary<string, string>> CsvRecords(string text)
    {
        var rows = Csv.Parse(text, Csv.GuessSeparator(text));
        if (rows.Count == 0) throw new FormatException("The file is empty.");
        var header = rows[0].Select(NameKey).ToList();
        return rows.Skip(1).Select(row =>
        {
            var record = new Dictionary<string, string>();
            for (var i = 0; i < header.Count && i < row.Length; i++) record.TryAdd(header[i], row[i]);
            return record;
        }).ToList();
    }

    /// <summary>
    /// An array of objects, or an object holding one (e.g. {"items": […]}), or a single object. Values in objects inside
    /// a record count under their own names (Trakt's {"movie": {"title": …, "ids": {"imdb": …}}}); the first of a name wins.
    /// </summary>
    private static List<Dictionary<string, string>> JsonRecords(string text)
    {
        JsonDocument document;
        try
        {
            document = JsonDocument.Parse(text, new JsonDocumentOptions { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip });
        }
        catch (JsonException ex)
        {
            throw new FormatException($"The file isn't valid JSON: {ex.Message}");
        }
        using (document)
        {
            var root = document.RootElement;
            var list = root.ValueKind == JsonValueKind.Array ? root
                : root.ValueKind == JsonValueKind.Object && root.EnumerateObject()
                    .Select(p => p.Value).FirstOrDefault(v => v.ValueKind == JsonValueKind.Array && v.EnumerateArray().Any(e => e.ValueKind == JsonValueKind.Object)) is { ValueKind: JsonValueKind.Array } inner
                    ? inner : default;
            var objects = list.ValueKind == JsonValueKind.Array
                ? list.EnumerateArray().Where(e => e.ValueKind == JsonValueKind.Object).ToList()
                : root.ValueKind == JsonValueKind.Object ? [root] : [];
            return objects.Select(o =>
            {
                var record = new Dictionary<string, string>();
                Flatten(o, record);
                return record;
            }).ToList();
        }

        static void Flatten(JsonElement element, Dictionary<string, string> record)
        {
            var nested = new List<JsonElement>();
            foreach (var property in element.EnumerateObject())
            {
                switch (property.Value.ValueKind)
                {
                    case JsonValueKind.String:
                        record.TryAdd(NameKey(property.Name), property.Value.GetString() ?? "");
                        break;
                    case JsonValueKind.Number:
                        record.TryAdd(NameKey(property.Name), property.Value.GetRawText());
                        break;
                    case JsonValueKind.Object:
                        nested.Add(property.Value);
                        break;
                }
            }
            foreach (var inner in nested) Flatten(inner, record); // the record's own values first
        }
    }

    /// <summary>
    /// Each element with a title (as an attribute or a child element) is a record: its attributes and the text of the
    /// elements inside it, the first of a name winning. Records inside a record aren't read twice.
    /// </summary>
    private static List<Dictionary<string, string>> XmlRecords(string text)
    {
        XDocument document;
        try
        {
            document = XDocument.Parse(text);
        }
        catch (XmlException ex)
        {
            throw new FormatException($"The file isn't valid XML: {ex.Message}");
        }

        // A child element counts as the title only when it is just text: <movie title="Heat"/> in a list is a record, not its list's title.
        bool HasTitle(XElement e) =>
            e.Attributes().Any(a => TitleNames.Contains(NameKey(a.Name.LocalName)))
            || e.Elements().Any(c => !c.HasElements && !c.HasAttributes && c.Value.Trim().Length > 0 && TitleNames.Contains(NameKey(c.Name.LocalName)));

        var records = new List<Dictionary<string, string>>();
        foreach (var element in document.Descendants().Where(HasTitle))
        {
            if (element.Ancestors().Any(HasTitle)) continue;
            var record = new Dictionary<string, string>();
            foreach (var attribute in element.Attributes()) record.TryAdd(NameKey(attribute.Name.LocalName), attribute.Value);
            foreach (var leaf in element.Descendants().Where(d => !d.HasElements))
            {
                record.TryAdd(NameKey(leaf.Name.LocalName), leaf.Value);
                foreach (var attribute in leaf.Attributes()) record.TryAdd(NameKey(attribute.Name.LocalName), attribute.Value);
            }
            records.Add(record);
        }
        return records;
    }
}
