using System.Globalization;
using MySqlConnector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kolibri.Kino.KMDb;

/// <summary>The outcome of syncing one document: the merged document and whether the database changed.</summary>
public sealed record SyncResult(string Id, bool Changed, JObject Document);

/// <summary>Writing to the library: replace (PUT), merge (sync), delete, and binary fields such as images.</summary>
public interface ISilverScreenWriter
{
    /// <summary>Stores <paramref name="doc"/> as the whole document (fields it leaves out become empty; binary fields are kept).</summary>
    Task<JObject?> PutAsync(string collection, string id, JObject doc, CancellationToken ct = default);

    /// <summary>Stores many whole documents (each with its _id) in one go; false when the collection can't be written.</summary>
    Task<bool> PutManyAsync(string collection, IReadOnlyList<JObject> docs, CancellationToken ct = default);

    /// <summary>Merges each document into the stored one (<see cref="SilverScreenSync.Merge"/>) and returns the results.</summary>
    Task<IReadOnlyList<SyncResult>?> SyncAsync(string collection, IReadOnlyList<JObject> docs, CancellationToken ct = default);

    Task<bool?> DeleteAsync(string collection, string id, CancellationToken ct = default);

    /// <summary>Stores binary data in a field (an image in Image.Data); creates the document when it doesn't exist.</summary>
    Task<bool> PutBinaryAsync(string collection, string id, string field, byte[] data, CancellationToken ct = default);

    /// <summary>Stores an image in the cache with what it is: the title (or TMDb person), type, source, address and size.</summary>
    Task StoreImageAsync(string id, byte[] data, string? imdbId, string type, string source, string? sourceUrl,
        int? season = null, int? episode = null, int? tmdbId = null, CancellationToken ct = default);
}

public sealed partial class SilverScreenRepository : ISilverScreenWriter
{
    /// <summary>When a row last changed (UTC): set on every write, so clients can ask for what changed (?since=).</summary>
    public const string ModifiedColumn = "_modified";

    public async Task<JObject?> PutAsync(string collection, string id, JObject doc, CancellationToken ct = default)
    {
        if (await WritableColumnsAsync(collection, doc, ct).ConfigureAwait(false) is not { } columns) return null;
        await using var conn = await dataSource.OpenConnectionAsync(ct).ConfigureAwait(false);
        await UpsertAsync(conn, null, collection, columns, id, doc, replace: true, ct).ConfigureAwait(false);
        return await GetAsync(collection, id, ct).ConfigureAwait(false);
    }

    public async Task<bool> PutManyAsync(string collection, IReadOnlyList<JObject> docs, CancellationToken ct = default)
    {
        var all = new JObject();
        foreach (var doc in docs)
            foreach (var (name, value) in doc)
                if (all[name] is null || all[name]!.Type == JTokenType.Null) all[name] = value;
        if (await WritableColumnsAsync(collection, all, ct).ConfigureAwait(false) is not { } columns) return false;
        await using var conn = await dataSource.OpenConnectionAsync(ct).ConfigureAwait(false);
        await using var tx = await conn.BeginTransactionAsync(ct).ConfigureAwait(false);
        foreach (var doc in docs)
        {
            var id = IdOf(doc) ?? throw new ArgumentException("Every document needs an _id.");
            await UpsertAsync(conn, tx, collection, columns, id, doc, replace: true, ct).ConfigureAwait(false);
        }
        await tx.CommitAsync(ct).ConfigureAwait(false);
        return true;
    }

    public async Task<IReadOnlyList<SyncResult>?> SyncAsync(string collection, IReadOnlyList<JObject> docs, CancellationToken ct = default)
    {
        var all = new JObject();
        foreach (var doc in docs)
            foreach (var (name, value) in doc)
                all[name] = value;
        if (await WritableColumnsAsync(collection, all, ct).ConfigureAwait(false) is not { } columns) return null;

        var results = new List<SyncResult>();
        await using var conn = await dataSource.OpenConnectionAsync(ct).ConfigureAwait(false);
        foreach (var doc in docs)
        {
            var id = IdOf(doc) ?? throw new ArgumentException("Every document needs an _id.");
            await using var tx = await conn.BeginTransactionAsync(ct).ConfigureAwait(false);
            var stored = await ReadForUpdateAsync(conn, tx, collection, columns, id, ct).ConfigureAwait(false);
            var (merged, changed) = SilverScreenSync.Merge(stored, doc);
            if (changed) await UpsertAsync(conn, tx, collection, columns, id, merged, replace: false, ct).ConfigureAwait(false);
            await tx.CommitAsync(ct).ConfigureAwait(false);
            merged["_id"] = stored?["_id"] ?? doc["_id"];
            RemovePlexTokens(merged);
            results.Add(new SyncResult(id, changed, merged));
        }
        return results;
    }

    public async Task<bool?> DeleteAsync(string collection, string id, CancellationToken ct = default)
    {
        if (await ColumnsAsync(collection, ct).ConfigureAwait(false) is not { } columns) return null;
        await using var conn = await dataSource.OpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = new MySqlCommand($"DELETE FROM {Q(collection)} WHERE `_id` = @id", conn);
        cmd.Parameters.AddWithValue("@id", IdValue(columns, id));
        return await cmd.ExecuteNonQueryAsync(ct).ConfigureAwait(false) > 0;
    }

    public async Task<bool> PutBinaryAsync(string collection, string id, string field, byte[] data, CancellationToken ct = default)
    {
        var columns = await ColumnsAsync(collection, ct).ConfigureAwait(false);
        var name = columns?.Keys.FirstOrDefault(c => string.Equals(c, field, StringComparison.OrdinalIgnoreCase));
        if (columns is null || name is null || columns[name] != "longblob") return false;
        await EnsureModifiedColumnAsync(collection, columns, ct).ConfigureAwait(false);

        // Image documents also say when they were added, as Kino's image cache does.
        var added = columns.ContainsKey("Added") ? ", `Added` = UTC_TIMESTAMP(3)" : "";
        var addedInsert = columns.ContainsKey("Added") ? (", `Added`", ", UTC_TIMESTAMP(3)") : ("", "");
        await using var conn = await dataSource.OpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = new MySqlCommand($"""
            INSERT INTO {Q(collection)} (`_id`, {Q(name)}, `{ModifiedColumn}`{addedInsert.Item1})
            VALUES (@id, @data, UTC_TIMESTAMP(3){addedInsert.Item2})
            ON DUPLICATE KEY UPDATE {Q(name)} = @data, `{ModifiedColumn}` = UTC_TIMESTAMP(3){added}
            """, conn);
        cmd.Parameters.AddWithValue("@id", IdValue(columns, id));
        cmd.Parameters.AddWithValue("@data", data);
        await cmd.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
        return true;
    }

    public async Task StoreImageAsync(string id, byte[] data, string? imdbId, string type, string source, string? sourceUrl,
        int? season = null, int? episode = null, int? tmdbId = null, CancellationToken ct = default)
    {
        var size = Kolibri.Kino.Core.ImageInfo.Size(data);
        await using var conn = await dataSource.OpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = new MySqlCommand($"""
            INSERT INTO `Image` (`_id`, `Added`, `Data`, `ImdbId`, `ImageType`, `Season`, `Episode`, `TmdbId`, `Source`, `SourceUrl`, `Width`, `Height`, `{ModifiedColumn}`)
            VALUES (@id, UTC_TIMESTAMP(3), @data, @imdb, @type, @season, @episode, @tmdb, @source, @url, @w, @h, UTC_TIMESTAMP(3))
            ON DUPLICATE KEY UPDATE `Added` = UTC_TIMESTAMP(3), `Data` = @data, `ImdbId` = @imdb, `ImageType` = @type, `Season` = @season,
              `Episode` = @episode, `TmdbId` = @tmdb, `Source` = @source, `SourceUrl` = @url, `Width` = @w, `Height` = @h,
              `{ModifiedColumn}` = UTC_TIMESTAMP(3)
            """, conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@data", data);
        cmd.Parameters.AddWithValue("@imdb", imdbId);
        cmd.Parameters.AddWithValue("@type", type);
        cmd.Parameters.AddWithValue("@season", season);
        cmd.Parameters.AddWithValue("@episode", episode);
        cmd.Parameters.AddWithValue("@tmdb", tmdbId);
        cmd.Parameters.AddWithValue("@source", source);
        cmd.Parameters.AddWithValue("@url", sourceUrl is null ? null : WithoutPlexToken(sourceUrl));
        cmd.Parameters.AddWithValue("@w", size?.Width);
        cmd.Parameters.AddWithValue("@h", size?.Height);
        await cmd.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
        _columns.TryRemove("Image", out _);
    }

    /// <summary>
    /// The table's columns, after adding any field of <paramref name="doc"/> the table doesn't have yet (a new
    /// property in Kino becomes a new column). Null when the collection isn't served.
    /// </summary>
    private async Task<IReadOnlyDictionary<string, string>?> WritableColumnsAsync(string collection, JObject doc, CancellationToken ct)
    {
        var columns = await ColumnsAsync(collection, ct).ConfigureAwait(false) ?? await CreateCollectionAsync(collection, ct).ConfigureAwait(false);
        if (columns is null) return null;
        await EnsureModifiedColumnAsync(collection, columns, ct).ConfigureAwait(false);

        var missing = doc.Properties()
            .Where(p => p.Name != "_id" && !columns.Keys.Any(c => string.Equals(c, p.Name, StringComparison.OrdinalIgnoreCase)))
            .ToList();
        if (missing.Count == 0) return await ColumnsAsync(collection, ct).ConfigureAwait(false);

        foreach (var p in missing)
            if (!SilverScreenSync.IsFieldName(p.Name)) throw new ArgumentException($"'{p.Name}' can't be a field name.");
        await using var conn = await dataSource.OpenConnectionAsync(ct).ConfigureAwait(false);
        foreach (var p in missing)
        {
            await using var cmd = new MySqlCommand($"ALTER TABLE {Q(collection)} ADD COLUMN IF NOT EXISTS {Q(p.Name)} {SqlTypeFor(p.Value, p.Name)}", conn);
            await cmd.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
        }
        _columns.TryRemove(collection, out _);
        return await ColumnsAsync(collection, ct).ConfigureAwait(false);
    }

    /// <summary>
    /// A collection Kino writes that MariaDB doesn't have yet (one LiteDB made after the copy, e.g. KinoIgnoredFolder):
    /// a table with _id and _modified, listed in _CopyStatus; the fields become columns as they come.
    /// </summary>
    private async Task<IReadOnlyDictionary<string, string>?> CreateCollectionAsync(string collection, CancellationToken ct)
    {
        if (Hidden.Contains(collection) || !SilverScreenSync.IsFieldName(collection)) return null;
        await using var conn = await dataSource.OpenConnectionAsync(ct).ConfigureAwait(false);
        await using (var cmd = new MySqlCommand($"""
            CREATE TABLE IF NOT EXISTS {Q(collection)} (
              `_id` VARCHAR(768) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
              `{ModifiedColumn}` DATETIME(3) NULL,
              PRIMARY KEY (`_id`),
              KEY `ix_modified` (`{ModifiedColumn}`)
            ) COMMENT='LiteDB collection {collection}, made by KMDb when Kino first wrote it'
            """, conn))
            await cmd.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
        await using (var cmd = new MySqlCommand($"INSERT IGNORE INTO `{StatusTable}` (collection_name, source_file, row_count, copied_utc) VALUES (@c, 'kmdb', 0, UTC_TIMESTAMP(3))", conn))
        {
            cmd.Parameters.AddWithValue("@c", collection);
            await cmd.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
        }
        _columns.TryRemove(collection, out _);
        return await ColumnsAsync(collection, ct).ConfigureAwait(false);
    }

    private async Task EnsureModifiedColumnAsync(string collection, IReadOnlyDictionary<string, string> columns, CancellationToken ct)
    {
        if (columns.ContainsKey(ModifiedColumn)) return;
        await using var conn = await dataSource.OpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = new MySqlCommand(
            $"ALTER TABLE {Q(collection)} ADD COLUMN IF NOT EXISTS `{ModifiedColumn}` DATETIME(3) NULL, ADD INDEX IF NOT EXISTS `ix_modified` (`{ModifiedColumn}`)", conn);
        await cmd.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
        _columns.TryRemove(collection, out _);
    }

    /// <summary>The column type for a new field, from its first value (as the copy tool would choose).</summary>
    internal static string SqlTypeFor(JToken value, string name) => value.Type switch
    {
        JTokenType.Integer => "BIGINT NULL",
        JTokenType.Float => "DOUBLE NULL",
        JTokenType.Boolean => "TINYINT(1) NULL",
        JTokenType.Object or JTokenType.Array => $"LONGTEXT NULL CHECK (JSON_VALID({Q(name)}))",
        _ => "TEXT NULL",
    };

    private async Task<JObject?> ReadForUpdateAsync(MySqlConnection conn, MySqlTransaction tx, string collection,
        IReadOnlyDictionary<string, string> columns, string id, CancellationToken ct)
    {
        await using var cmd = new MySqlCommand($"SELECT {SelectList(columns)} FROM {Q(collection)} WHERE `_id` = @id FOR UPDATE", conn, tx);
        cmd.Parameters.AddWithValue("@id", IdValue(columns, id));
        await using var reader = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
        return await reader.ReadAsync(ct).ConfigureAwait(false) ? ToJson(reader, columns, collection, removeTokens: false) : null;
    }

    /// <summary>
    /// Writes a document. Binary fields are never written here (in documents they are only addresses). With
    /// <paramref name="replace"/>, fields the document leaves out become NULL; otherwise only its fields are set.
    /// </summary>
    private static async Task UpsertAsync(MySqlConnection conn, MySqlTransaction? tx, string collection,
        IReadOnlyDictionary<string, string> columns, string id, JObject doc, bool replace, CancellationToken ct)
    {
        var targets = columns
            .Where(c => c.Key != "_id" && c.Key != ModifiedColumn && c.Value != "longblob")
            .Where(c => replace || doc.Properties().Any(p => string.Equals(p.Name, c.Key, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        await using var cmd = new MySqlCommand { Connection = conn, Transaction = tx };
        cmd.Parameters.AddWithValue("@id", IdValue(columns, id));
        var names = new List<string>();
        var values = new List<string>();
        for (var i = 0; i < targets.Count; i++)
        {
            var (name, type) = targets[i];
            var token = doc.Properties().FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase))?.Value;
            cmd.Parameters.AddWithValue($"@v{i}", ToColumnValue(token, type) ?? DBNull.Value);
            names.Add(Q(name));
            values.Add($"@v{i}");
        }
        var insertColumns = string.Join("", names.Select(n => ", " + n));
        var insertValues = string.Join("", values.Select(v => ", " + v));
        var updates = string.Join("", names.Select((n, i) => $"{n} = {values[i]}, "));
        cmd.CommandText = $"""
            INSERT INTO {Q(collection)} (`_id`{insertColumns}, `{ModifiedColumn}`)
            VALUES (@id{insertValues}, UTC_TIMESTAMP(3))
            ON DUPLICATE KEY UPDATE {updates}`{ModifiedColumn}` = UTC_TIMESTAMP(3)
            """;
        await cmd.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
    }

    /// <summary>A JSON value as the column holds it.</summary>
    internal static object? ToColumnValue(JToken? token, string type)
    {
        if (token is null || token.Type is JTokenType.Null or JTokenType.Undefined) return null;
        switch (type)
        {
            case "longtext":
                return token.ToString(Formatting.None);
            case "int" or "bigint" or "smallint" or "mediumint":
                return token.Type is JTokenType.Integer or JTokenType.Float ? (long)token
                    : long.TryParse((string?)token, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) ? n : null;
            case "double" or "float" or "decimal":
                return token.Type is JTokenType.Integer or JTokenType.Float ? (double)token
                    : double.TryParse((string?)token, NumberStyles.Float, CultureInfo.InvariantCulture, out var d) ? d : null;
            case "tinyint":
                return token.Type == JTokenType.Boolean ? (bool)token : token.Type == JTokenType.Integer ? (long)token != 0 : null;
            case "datetime":
                if (token.Type == JTokenType.Date) return ((DateTime)token).ToUniversalTime();
                return DateTime.TryParse((string?)token, CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var t) ? t : null;
            default:
                return token.Type is JTokenType.Object or JTokenType.Array ? token.ToString(Formatting.None) : (string?)token;
        }
    }

    private static string? IdOf(JObject doc) => doc["_id"]?.Type switch
    {
        JTokenType.String or JTokenType.Integer => doc["_id"]!.ToString(),
        _ => null,
    };
}
