using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Kolibri.Kino.Core;
using MySqlConnector;

namespace Kolibri.Kino.SilverScreenCopy;

/// <summary>One movie in a Kodi library export (videodb.xml): its IMDb id and the art addresses Kodi chose.</summary>
public sealed record KodiMovie(string Title, string? Year, string ImdbId, IReadOnlyDictionary<string, string> Art);

/// <summary>An image file from a Kodi export, matched to a title.</summary>
public sealed record KodiImage(string ImdbId, string Type, string KodiType, string File, string? SourceUrl, DateTime Export);

/// <summary>
/// Brings in the movie art of Kodi library exports (Kodi → Export library, "single file": videodb.xml with
/// movies/Title_Year-poster.jpg, -fanart.jpg, -landscape.jpg, -thumb.jpg, -clearlogo.jpg …) for the titles in Item.
/// The files are matched to videodb.xml by title and year, and so to an IMDb id. Kodi's fanart is a backdrop; the
/// other names are kept (poster, landscape, thumb, clearlogo …). When several exports have a title's image, the newest
/// export wins; a "thumb" that is the same image as the poster is left out.
/// </summary>
/// <remarks>
/// Rows are keyed "{imdbId}:{type}:kodi", except a poster for a title with no image at all, which gets the IMDb id itself
/// as key (as Kino keys posters), so Kino finds it too. They carry _modified, so a copy from LiteDB doesn't overwrite them.
/// </remarks>
public sealed partial class KodiImport(MySqlConnection connection, TextWriter log)
{
    public int MaxBatchBytes { get; init; } = 16 * 1024 * 1024;

    /// <summary>Kodi's art names → the image types used here (Kodi's "fanart" is what TMDb calls a backdrop).</summary>
    internal static string TypeOf(string kodiType) => kodiType.ToLowerInvariant() switch
    {
        "fanart" => "backdrop",
        var t => t,
    };

    public async Task RunAsync(string root, CancellationToken ct = default)
    {
        await ImageTable.EnsureColumnsAsync(connection, ct);
        var wanted = await ItemIdsAsync(ct);
        log.WriteLine($"  titles in Item: {wanted.Count:n0}");

        var exports = Directory.GetFiles(root, "videodb.xml", SearchOption.AllDirectories)
            .Select(Path.GetDirectoryName).OfType<string>()
            .Select(d => (Dir: d, Date: ExportDate(d)))
            .OrderByDescending(e => e.Date)
            .ToList();

        var chosen = new Dictionary<(string ImdbId, string Type), KodiImage>();
        foreach (var (dir, date) in exports)
        {
            var movies = ReadMovies(Path.Combine(dir, "videodb.xml")).Where(m => wanted.Contains(m.ImdbId)).ToList();
            var byName = new Dictionary<string, KodiMovie>(StringComparer.Ordinal);
            var ambiguous = new HashSet<string>(StringComparer.Ordinal);
            foreach (var m in movies)
            {
                var key = NameKey(m.Title + m.Year);
                if (!byName.TryAdd(key, m) && byName[key].ImdbId != m.ImdbId) ambiguous.Add(key);
            }

            var folder = Path.Combine(dir, "movies");
            var found = 0;
            if (Directory.Exists(folder))
            {
                foreach (var file in Directory.EnumerateFiles(folder))
                {
                    if (ArtFile().Match(Path.GetFileNameWithoutExtension(file)) is not { Success: true } match) continue;
                    var key = NameKey(match.Groups["name"].Value);
                    if (ambiguous.Contains(key) || !byName.TryGetValue(key, out var movie)) continue;
                    var kodiType = match.Groups["type"].Value.ToLowerInvariant();
                    var type = TypeOf(kodiType);
                    found++;
                    chosen.TryAdd((movie.ImdbId, type), new KodiImage(movie.ImdbId, type, kodiType, file, movie.Art.GetValueOrDefault(kodiType), date));
                }
            }
            log.WriteLine($"  {Path.GetFileName(Path.GetDirectoryName(dir))}\\{Path.GetFileName(dir)}: {movies.Count:n0} of its movies are in Item, {found:n0} image files matched");
        }

        var posterKeys = await ExistingKeysAsync(chosen.Keys.Select(k => k.ImdbId).Distinct().ToList(), ct);
        var hashes = new Dictionary<(string, string), string>();
        var batch = new List<(string Id, KodiImage Image, byte[] Data, (int Width, int Height)? Size)>();
        long batchBytes = 0, written = 0, skipped = 0, bytes = 0;
        var perType = new Dictionary<string, int>();

        // Posters before thumbs, so a thumb equal to its poster is recognised.
        foreach (var image in chosen.Values.OrderBy(i => i.ImdbId, StringComparer.Ordinal).ThenBy(i => i.Type == "thumb" ? 1 : 0))
        {
            var data = await File.ReadAllBytesAsync(image.File, ct);
            if (data.Length == 0 || !ImageInfo.ContentType(data).StartsWith("image/", StringComparison.Ordinal)) { skipped++; continue; }
            var hash = Convert.ToHexString(SHA1.HashData(data));
            if (image.Type == "thumb" && hashes.TryGetValue((image.ImdbId, "poster"), out var posterHash) && posterHash == hash) { skipped++; continue; }
            hashes[(image.ImdbId, image.Type)] = hash;

            var id = image.Type == "poster" && !posterKeys.Contains(image.ImdbId) ? image.ImdbId : $"{image.ImdbId}:{image.Type}:kodi";
            if (batch.Count > 0 && (batch.Count >= 200 || batchBytes + data.Length * 2L > MaxBatchBytes))
            {
                await WriteAsync(batch, ct);
                batch.Clear();
                batchBytes = 0;
            }
            batch.Add((id, image, data, ImageInfo.Size(data)));
            batchBytes += data.Length * 2L;
            bytes += data.Length;
            written++;
            perType[image.Type] = perType.GetValueOrDefault(image.Type) + 1;
        }
        if (batch.Count > 0) await WriteAsync(batch, ct);

        log.WriteLine($"  stored {written:n0} images ({bytes / 1024.0 / 1024:n0} MB), skipped {skipped:n0} (thumbs equal to the poster, empty files)");
        foreach (var (type, count) in perType.OrderByDescending(p => p.Value)) log.WriteLine($"    {type,-12} {count,6:n0}");
    }

    private async Task WriteAsync(List<(string Id, KodiImage Image, byte[] Data, (int Width, int Height)? Size)> rows, CancellationToken ct)
    {
        await using var cmd = connection.CreateCommand();
        var sb = new StringBuilder("""
            INSERT INTO `Image` (`_id`, `Added`, `Data`, `ImdbId`, `ImageType`, `Source`, `SourceUrl`, `Width`, `Height`, `_modified`) VALUES
            """);
        for (var i = 0; i < rows.Count; i++)
        {
            var (id, image, data, size) = rows[i];
            sb.Append(i == 0 ? "\n" : ",\n").Append($"(@id{i}, @added{i}, @data{i}, @imdb{i}, @type{i}, 'kodi', @url{i}, @w{i}, @h{i}, UTC_TIMESTAMP(3))");
            cmd.Parameters.AddWithValue($"@id{i}", id);
            cmd.Parameters.AddWithValue($"@added{i}", File.GetLastWriteTimeUtc(image.File));
            cmd.Parameters.AddWithValue($"@data{i}", data);
            cmd.Parameters.AddWithValue($"@imdb{i}", image.ImdbId);
            cmd.Parameters.AddWithValue($"@type{i}", image.Type);
            cmd.Parameters.AddWithValue($"@url{i}", image.SourceUrl);
            cmd.Parameters.AddWithValue($"@w{i}", size?.Width);
            cmd.Parameters.AddWithValue($"@h{i}", size?.Height);
        }
        sb.Append("""

            ON DUPLICATE KEY UPDATE `Added` = VALUES(`Added`), `Data` = VALUES(`Data`), `ImdbId` = VALUES(`ImdbId`),
              `ImageType` = VALUES(`ImageType`), `Source` = 'kodi', `SourceUrl` = VALUES(`SourceUrl`), `Width` = VALUES(`Width`),
              `Height` = VALUES(`Height`), `_modified` = UTC_TIMESTAMP(3)
            """);
        cmd.CommandText = sb.ToString();
        await cmd.ExecuteNonQueryAsync(ct);
    }

    /// <summary>The movies (and shows) of a videodb.xml with an IMDb id, read as a stream (the file can be 70 MB).</summary>
    internal static IEnumerable<KodiMovie> ReadMovies(string path)
    {
        using var reader = XmlReader.Create(path, new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore, IgnoreWhitespace = true });
        reader.MoveToContent();
        reader.Read();
        while (!reader.EOF)
        {
            if (reader.NodeType != XmlNodeType.Element) { reader.Read(); continue; }
            var e = (XElement)XNode.ReadFrom(reader);
            if (e.Name.LocalName != "movie") continue;
            var imdb = e.Elements("uniqueid").FirstOrDefault(u => (string?)u.Attribute("type") == "imdb")?.Value?.Trim()
                       ?? e.Element("imdbid")?.Value?.Trim();
            if (imdb is null || !imdb.StartsWith("tt", StringComparison.Ordinal)) continue;
            var art = (e.Element("art")?.Elements() ?? [])
                .GroupBy(a => a.Name.LocalName.ToLowerInvariant())
                .ToDictionary(g => g.Key, g => g.First().Value.Trim());
            yield return new KodiMovie(e.Element("title")?.Value ?? "", e.Element("year")?.Value ?? e.Element("premiered")?.Value?[..4], imdb, art);
        }
    }

    /// <summary>
    /// A title and year reduced to letters and digits, lower case: how a file name "13_Hours__The_Secret_Soldiers_of_Benghazi_2016"
    /// is matched to "13 Hours: The Secret Soldiers of Benghazi" (2016), whatever Kodi replaced in the name.
    /// </summary>
    internal static string NameKey(string text)
    {
        var sb = new StringBuilder(text.Length);
        foreach (var ch in text.Normalize(NormalizationForm.FormC))
            if (char.IsLetterOrDigit(ch)) sb.Append(char.ToLowerInvariant(ch));
        return sb.ToString();
    }

    /// <summary>The export's date from its folder name (kodi_videodb_2024-10-04), else the folder's time.</summary>
    internal static DateTime ExportDate(string dir) =>
        ExportFolder().Match(Path.GetFileName(dir)) is { Success: true } m && DateTime.TryParse(m.Groups[1].Value, out var d)
            ? d
            : Directory.GetLastWriteTimeUtc(dir);

    private async Task<HashSet<string>> ItemIdsAsync(CancellationToken ct)
    {
        var ids = new HashSet<string>(StringComparer.Ordinal);
        await using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT `_id` FROM `Item`";
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct)) ids.Add(reader.GetString(0));
        return ids;
    }

    /// <summary>Which of these IMDb ids already have an image keyed by the id itself (Kino's poster).</summary>
    private async Task<HashSet<string>> ExistingKeysAsync(IReadOnlyList<string> ids, CancellationToken ct)
    {
        var found = new HashSet<string>(StringComparer.Ordinal);
        foreach (var chunk in ids.Chunk(500))
        {
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT `_id` FROM `Image` WHERE `_id` IN ({string.Join(", ", chunk.Select((_, i) => $"@p{i}"))})";
            for (var i = 0; i < chunk.Length; i++) cmd.Parameters.AddWithValue($"@p{i}", chunk[i]);
            await using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct)) found.Add(reader.GetString(0));
        }
        return found;
    }

    [GeneratedRegex("^(?<name>.+)-(?<type>poster|fanart|landscape|thumb|clearlogo|clearart|banner|discart|keyart|characterart)$", RegexOptions.IgnoreCase)]
    private static partial Regex ArtFile();

    [GeneratedRegex("kodi_videodb_([0-9]{4}-[0-9]{2}-[0-9]{2})")]
    private static partial Regex ExportFolder();
}
