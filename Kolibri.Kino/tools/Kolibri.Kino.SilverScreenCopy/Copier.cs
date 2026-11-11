using System.Diagnostics;
using System.Text;
using LiteDB;
using MySqlConnector;

namespace Kolibri.Kino.SilverScreenCopy;

/// <summary>
/// Copies LiteDB collections to MariaDB tables with the same names. Each table is built as &lt;name&gt;__new and then
/// swapped in with one RENAME, so readers (KMDb) see either the old rows or the new ones, never a half-loaded table.
/// </summary>
public sealed class Copier(MySqlConnection connection, TextWriter log)
{
    /// <summary>One INSERT carries at most this much data (MariaDB on HTPC takes 256 MB packets; XAMPP 1 MB).</summary>
    public int MaxBatchBytes { get; init; } = 16 * 1024 * 1024;

    private const int MaxBatchRows = 500;

    public const string StatusTable = "_CopyStatus";

    /// <summary>Set by KMDb on every write; the copy leaves it empty.</summary>
    public const string ModifiedColumn = "_modified";

    /// <summary>
    /// Replace a table even when KMDb has changed rows in it since the last copy (WinForms or the web app synced
    /// there). Without it such tables are skipped, so a copy from LiteDB never throws away newer data.
    /// </summary>
    public bool Replace { get; init; }

    public async Task<int> CopyAsync(LiteDatabase source, string sourceFile, string collection, CancellationToken ct = default)
    {
        var watch = Stopwatch.StartNew();
        var col = source.GetCollection(collection);

        // First pass: the columns. LiteDB has no schema, so every document is looked at.
        var plan = new ColumnPlan();
        foreach (var doc in col.FindAll()) plan.Add(doc);
        var columns = plan.Columns();
        if (columns.Count == 0 || columns[0].Name != "_id")
        {
            log.WriteLine($"  {collection}: empty, skipped");
            return 0;
        }

        var table = collection;
        if (!Replace && await ChangedSinceCopyAsync(table, collection, ct) is > 0 and var changed)
        {
            log.WriteLine($"  {collection}: skipped, {changed:n0} rows changed through KMDb since the last copy (--replace overwrites them)");
            return 0;
        }
        var temp = table + "__new";
        await ExecuteAsync($"DROP TABLE IF EXISTS {Q(temp)}", ct);
        await ExecuteAsync(CreateTable(temp, columns, collection), ct);

        // Second pass: the rows, in batches.
        var rows = 0;
        var batch = new List<object?[]>();
        var batchBytes = 0L;
        foreach (var doc in col.FindAll())
        {
            var values = columns.Select(c => Values.ToColumn(doc.TryGetValue(c.Name, out var v) ? v : null, c.Kind)).ToArray();
            var size = values.Sum(SizeOf);
            if (batch.Count > 0 && (batch.Count >= MaxBatchRows || batchBytes + size > MaxBatchBytes))
            {
                await InsertAsync(temp, columns, batch, ct);
                batch.Clear();
                batchBytes = 0;
            }
            batch.Add(values);
            batchBytes += size;
            rows++;
        }
        if (batch.Count > 0) await InsertAsync(temp, columns, batch, ct);

        await SwapAsync(table, temp, ct);
        await RecordAsync(collection, sourceFile, rows, ct);
        log.WriteLine($"  {collection}: {rows:n0} rows, {columns.Count} columns, {watch.Elapsed.TotalSeconds:n1} s");
        return rows;
    }

    public Task EnsureStatusTableAsync(CancellationToken ct = default) => ExecuteAsync($"""
        CREATE TABLE IF NOT EXISTS {Q(StatusTable)} (
          collection_name VARCHAR(100) NOT NULL PRIMARY KEY,
          source_file     VARCHAR(100) NOT NULL,
          row_count       INT NOT NULL,
          copied_utc      DATETIME(3) NOT NULL
        ) COMMENT='When each LiteDB collection was last copied (Kolibri.Kino.SilverScreenCopy)'
        """, ct);

    internal static string CreateTable(string name, IReadOnlyList<Column> columns, string collection)
    {
        var sb = new StringBuilder($"CREATE TABLE {Q(name)} (\n");
        foreach (var c in columns) sb.Append($"  {Q(c.Name)} {string.Format(c.SqlType, c.Name)},\n");
        sb.Append($"  {Q(ModifiedColumn)} DATETIME(3) NULL COMMENT 'UTC; set by KMDb when the row is written',\n");
        sb.Append("  PRIMARY KEY (`_id`),\n");
        sb.Append($"  KEY `ix_modified` ({Q(ModifiedColumn)})");
        // Lookups KMDb and Kino make besides the key.
        foreach (var key in new[] { "ImdbId", "SeriesId", "WatchListName" })
        {
            var column = columns.FirstOrDefault(c => c.Name == key);
            if (column is not null && column.SqlType.StartsWith("VARCHAR", StringComparison.Ordinal))
                sb.Append($",\n  KEY {Q("ix_" + key)} ({Q(key)})");
        }
        sb.Append($"\n) COMMENT='LiteDB collection {collection.Replace("'", "''")}'");
        return sb.ToString();
    }

    private async Task InsertAsync(string table, IReadOnlyList<Column> columns, List<object?[]> rows, CancellationToken ct)
    {
        await using var cmd = connection.CreateCommand();
        var sb = new StringBuilder($"INSERT INTO {Q(table)} ({string.Join(", ", columns.Select(c => Q(c.Name)))}) VALUES ");
        var p = 0;
        for (var r = 0; r < rows.Count; r++)
        {
            if (r > 0) sb.Append(", ");
            sb.Append('(');
            for (var c = 0; c < columns.Count; c++)
            {
                if (c > 0) sb.Append(", ");
                var name = "@p" + p++;
                sb.Append(name);
                cmd.Parameters.AddWithValue(name, rows[r][c] ?? DBNull.Value);
            }
            sb.Append(')');
        }
        cmd.CommandText = sb.ToString();
        await cmd.ExecuteNonQueryAsync(ct);
    }

    private async Task SwapAsync(string table, string temp, CancellationToken ct)
    {
        var old = table + "__old";
        await ExecuteAsync($"DROP TABLE IF EXISTS {Q(old)}", ct);
        if (await TableExistsAsync(table, ct))
        {
            await ExecuteAsync($"RENAME TABLE {Q(table)} TO {Q(old)}, {Q(temp)} TO {Q(table)}", ct);
            await ExecuteAsync($"DROP TABLE {Q(old)}", ct);
        }
        else
        {
            await ExecuteAsync($"RENAME TABLE {Q(temp)} TO {Q(table)}", ct);
        }
    }

    /// <summary>Rows KMDb wrote after the table was last copied; 0 when none or the table is new.</summary>
    private async Task<long> ChangedSinceCopyAsync(string table, string collection, CancellationToken ct)
    {
        await using var check = connection.CreateCommand();
        check.CommandText = "SELECT COUNT(*) FROM information_schema.columns WHERE table_schema = DATABASE() AND table_name = @t AND column_name = @c";
        check.Parameters.AddWithValue("@t", table);
        check.Parameters.AddWithValue("@c", ModifiedColumn);
        if (Convert.ToInt64(await check.ExecuteScalarAsync(ct)) == 0) return 0;

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = $"""
            SELECT COUNT(*) FROM {Q(table)}
             WHERE {Q(ModifiedColumn)} > COALESCE((SELECT copied_utc FROM {Q(StatusTable)} WHERE collection_name = @c), '1000-01-01')
            """;
        cmd.Parameters.AddWithValue("@c", collection);
        return Convert.ToInt64(await cmd.ExecuteScalarAsync(ct));
    }

    private async Task<bool> TableExistsAsync(string table, CancellationToken ct)
    {
        await using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = DATABASE() AND table_name = @t";
        cmd.Parameters.AddWithValue("@t", table);
        return Convert.ToInt64(await cmd.ExecuteScalarAsync(ct)) > 0;
    }

    private async Task RecordAsync(string collection, string sourceFile, int rows, CancellationToken ct)
    {
        await using var cmd = connection.CreateCommand();
        cmd.CommandText = $"REPLACE INTO {Q(StatusTable)} (collection_name, source_file, row_count, copied_utc) VALUES (@c, @f, @n, UTC_TIMESTAMP(3))";
        cmd.Parameters.AddWithValue("@c", collection);
        cmd.Parameters.AddWithValue("@f", sourceFile);
        cmd.Parameters.AddWithValue("@n", rows);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    private async Task ExecuteAsync(string sql, CancellationToken ct)
    {
        await using var cmd = connection.CreateCommand();
        cmd.CommandText = sql;
        await cmd.ExecuteNonQueryAsync(ct);
    }

    private static long SizeOf(object? value) => value switch
    {
        null => 4,
        string s => s.Length * 4L + 8,
        byte[] b => b.Length * 2L + 8, // sent as hex-safe binary; leave room
        _ => 16,
    };

    /// <summary>A quoted identifier (collection and field names come from the LiteDB file).</summary>
    internal static string Q(string name) => "`" + name.Replace("`", "``") + "`";
}
