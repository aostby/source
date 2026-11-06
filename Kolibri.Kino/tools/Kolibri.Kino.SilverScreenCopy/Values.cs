using System.Globalization;
using System.Text;
using System.Text.Json;
using LiteDB;

namespace Kolibri.Kino.SilverScreenCopy;

/// <summary>Turns LiteDB values into what the MariaDB columns hold.</summary>
public static class Values
{
    /// <summary>MariaDB's DATETIME starts at year 1000; LiteDB's DateTime.MinValue (unset dates) becomes NULL.</summary>
    private static readonly DateTime EarliestDate = new(1000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static object? ToColumn(BsonValue? value, ColumnKind kind)
    {
        if (value is null || value.IsNull) return null;
        return kind switch
        {
            ColumnKind.Int => value.AsInt32,
            ColumnKind.BigInt => value.AsInt64,
            ColumnKind.Double => value.AsDouble,
            ColumnKind.Decimal => value.AsDecimal,
            ColumnKind.Bool => value.AsBoolean,
            ColumnKind.DateTime => Utc(value.AsDateTime) is var d && d >= EarliestDate ? d : null,
            ColumnKind.Blob => value.AsBinary,
            ColumnKind.Json => ToJson(value),
            _ => value.IsString ? value.AsString : ScalarText(value),
        };
    }

    /// <summary>LiteDB reads dates as local time; the database keeps UTC.</summary>
    public static DateTime Utc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc),
    };

    /// <summary>
    /// Plain JSON (not LiteDB's $date/$numberLong form), so SQL (JSON_VALUE) and web clients read it directly:
    /// dates as ISO 8601 UTC, ObjectIds as hex, binary as base64.
    /// </summary>
    public static string ToJson(BsonValue value)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
            Write(writer, value);
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    private static void Write(Utf8JsonWriter w, BsonValue v)
    {
        switch (v.Type)
        {
            case BsonType.Null or BsonType.MinValue or BsonType.MaxValue: w.WriteNullValue(); break;
            case BsonType.Document:
                w.WriteStartObject();
                foreach (var (key, item) in v.AsDocument) { w.WritePropertyName(key); Write(w, item); }
                w.WriteEndObject();
                break;
            case BsonType.Array:
                w.WriteStartArray();
                foreach (var item in v.AsArray) Write(w, item);
                w.WriteEndArray();
                break;
            case BsonType.Int32: w.WriteNumberValue(v.AsInt32); break;
            case BsonType.Int64: w.WriteNumberValue(v.AsInt64); break;
            case BsonType.Double:
                if (double.IsFinite(v.AsDouble)) w.WriteNumberValue(v.AsDouble); else w.WriteNullValue();
                break;
            case BsonType.Decimal: w.WriteNumberValue(v.AsDecimal); break;
            case BsonType.Boolean: w.WriteBooleanValue(v.AsBoolean); break;
            case BsonType.DateTime: w.WriteStringValue(Utc(v.AsDateTime).ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture)); break;
            case BsonType.Binary: w.WriteBase64StringValue(v.AsBinary); break;
            default: w.WriteStringValue(v.IsString ? v.AsString : ScalarText(v)); break;
        }
    }

    /// <summary>Text for a value in a text column that isn't a string: an ObjectId as hex, a Guid as text, else JSON.</summary>
    private static string ScalarText(BsonValue v) => v.Type switch
    {
        BsonType.ObjectId => v.AsObjectId.ToString(),
        BsonType.Guid => v.AsGuid.ToString(),
        _ => ToJson(v),
    };
}
