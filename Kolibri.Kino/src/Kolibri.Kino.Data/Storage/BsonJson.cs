using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using LiteDB;

namespace Kolibri.Kino.Data.Storage;

/// <summary>
/// LiteDB documents as KMDb's JSON and back. KMDb writes plain JSON: dates as ISO 8601 UTC text
/// ("2026-11-06T10:00:00.000Z"), binary fields as the address that serves them. Reading turns such date text back
/// into dates, so BsonMapper fills DateTime properties as from the LiteDB file.
/// </summary>
public static partial class BsonJson
{
    public static string Serialize(BsonValue value)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream)) Write(writer, value);
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    public static BsonDocument ToDocument(JsonElement element) => (BsonDocument)ToBson(element);

    public static BsonValue ToBson(JsonElement e) => e.ValueKind switch
    {
        JsonValueKind.Object => new BsonDocument(e.EnumerateObject().ToDictionary(p => p.Name, p => ToBson(p.Value))),
        JsonValueKind.Array => new BsonArray(e.EnumerateArray().Select(ToBson)),
        JsonValueKind.String => FromText(e.GetString()!),
        JsonValueKind.Number => e.TryGetInt32(out var i) ? new BsonValue(i)
            : e.TryGetInt64(out var l) ? new BsonValue(l)
            : new BsonValue(e.GetDouble()),
        JsonValueKind.True => new BsonValue(true),
        JsonValueKind.False => new BsonValue(false),
        _ => BsonValue.Null,
    };

    /// <summary>KMDb's date text (exactly ISO 8601 with Z) as a date; other text as it is.</summary>
    private static BsonValue FromText(string text) =>
        IsoDate().IsMatch(text) && DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var d)
            ? new BsonValue(DateTime.SpecifyKind(d, DateTimeKind.Utc))
            : new BsonValue(text);

    /// <summary>Binary fields come back from KMDb as their address (/silverscreen/…); they are not part of the document here.</summary>
    public static bool IsBinaryAddress(BsonValue value) => value.IsString && value.AsString.StartsWith("/silverscreen/", StringComparison.Ordinal);

    private static void Write(Utf8JsonWriter w, BsonValue v)
    {
        switch (v.Type)
        {
            case BsonType.Document:
                w.WriteStartObject();
                foreach (var (key, item) in v.AsDocument)
                {
                    // Binary data goes to KMDb on its own (images); in a document it would be megabytes of base64.
                    if (item.IsBinary || IsBinaryAddress(item)) continue;
                    w.WritePropertyName(key);
                    Write(w, item);
                }
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
            case BsonType.String: w.WriteStringValue(v.AsString); break;
            case BsonType.ObjectId: w.WriteStringValue(v.AsObjectId.ToString()); break;
            case BsonType.Guid: w.WriteStringValue(v.AsGuid.ToString()); break;
            case BsonType.Binary: w.WriteBase64StringValue(v.AsBinary); break;
            default: w.WriteNullValue(); break;
        }
    }

    private static DateTime Utc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc),
    };

    [GeneratedRegex(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\.\d{1,7})?Z$")]
    private static partial Regex IsoDate();
}
