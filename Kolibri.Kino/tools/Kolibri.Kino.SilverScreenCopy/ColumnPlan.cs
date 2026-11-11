using LiteDB;

namespace Kolibri.Kino.SilverScreenCopy;

/// <summary>How one LiteDB field is stored in MariaDB.</summary>
public enum ColumnKind { Text, Int, BigInt, Double, Decimal, Bool, DateTime, Blob, Json }

/// <summary>One column of a table: the LiteDB field name, its SQL type and how values are written.</summary>
public sealed record Column(string Name, ColumnKind Kind, string SqlType);

/// <summary>
/// Works out the table for a collection from its documents: one column per top-level field, named as the field.
/// Nested documents and arrays (Episodes, Cast, Genres …) become JSON columns, binary data LONGBLOB, dates DATETIME(3) in UTC.
/// </summary>
public sealed class ColumnPlan
{
    private readonly Dictionary<string, (HashSet<BsonType> Types, int MaxLength)> _fields = new(StringComparer.Ordinal);
    private readonly List<string> _order = [];

    public void Add(BsonDocument doc)
    {
        foreach (var (key, value) in doc)
        {
            if (!_fields.TryGetValue(key, out var field))
            {
                field = (new HashSet<BsonType>(), 0);
                _order.Add(key);
            }
            if (!value.IsNull) field.Types.Add(value.Type);
            var length = value.IsString ? value.AsString.Length : 0;
            _fields[key] = (field.Types, Math.Max(field.MaxLength, length));
        }
    }

    /// <summary>The columns, _id first, then the fields in the order they were first seen.</summary>
    public IReadOnlyList<Column> Columns()
    {
        var columns = new List<Column>();
        if (_fields.ContainsKey("_id")) columns.Add(IdColumn(_fields["_id"].Types));
        foreach (var name in _order.Where(n => n != "_id"))
        {
            var (types, max) = _fields[name];
            columns.Add(new Column(name, KindOf(types), SqlTypeOf(KindOf(types), max)));
        }
        return columns;
    }

    /// <summary>
    /// The key column. String ids are compared byte for byte (utf8mb4_bin): LiteDB keeps ids that differ only in
    /// accents or case apart, and a _ci collation would see them as duplicates.
    /// </summary>
    internal static Column IdColumn(IReadOnlySet<BsonType> types) =>
        types.Count == 1 && types.Contains(BsonType.Int32) ? new Column("_id", ColumnKind.Int, "INT NOT NULL")
        : types.Count == 1 && types.Contains(BsonType.Int64) ? new Column("_id", ColumnKind.BigInt, "BIGINT NOT NULL")
        : new Column("_id", ColumnKind.Text, "VARCHAR(768) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL");

    internal static ColumnKind KindOf(IReadOnlySet<BsonType> types)
    {
        if (types.Count == 0) return ColumnKind.Text;
        if (types.All(t => t is BsonType.Document or BsonType.Array)) return ColumnKind.Json;
        if (types.All(t => t == BsonType.Binary)) return ColumnKind.Blob;
        if (types.All(t => t == BsonType.Boolean)) return ColumnKind.Bool;
        if (types.All(t => t == BsonType.DateTime)) return ColumnKind.DateTime;
        if (types.All(t => t == BsonType.Int32)) return ColumnKind.Int;
        if (types.All(t => t is BsonType.Int32 or BsonType.Int64)) return ColumnKind.BigInt;
        if (types.All(t => t is BsonType.Int32 or BsonType.Int64 or BsonType.Double)) return ColumnKind.Double;
        if (types.All(t => t is BsonType.Int32 or BsonType.Int64 or BsonType.Decimal)) return ColumnKind.Decimal;
        // Strings, ObjectIds, Guids, or a mix of kinds: text (a mix is written as JSON text).
        return ColumnKind.Text;
    }

    /// <summary>
    /// Text gets room to grow: a later copy may bring longer values than today's. Only short text is VARCHAR: a row
    /// may hold at most 65,535 bytes of VARCHAR (4 bytes a character in utf8mb4), and Item has 40 fields. TEXT is
    /// stored outside the row; it holds 65,535 bytes, so up to 15,000 characters to be safe, MEDIUMTEXT beyond.
    /// </summary>
    internal static string SqlTypeOf(ColumnKind kind, int maxLength) => kind switch
    {
        ColumnKind.Int => "INT NULL",
        ColumnKind.BigInt => "BIGINT NULL",
        ColumnKind.Double => "DOUBLE NULL",
        ColumnKind.Decimal => "DECIMAL(38,10) NULL",
        ColumnKind.Bool => "TINYINT(1) NULL",
        ColumnKind.DateTime => "DATETIME(3) NULL",
        ColumnKind.Blob => "LONGBLOB NULL",
        ColumnKind.Json => "LONGTEXT NULL CHECK (JSON_VALID(`{0}`))",
        _ when maxLength <= 100 => "VARCHAR(255) NULL",
        _ when maxLength <= 15000 => "TEXT NULL",
        _ => "MEDIUMTEXT NULL",
    };
}
