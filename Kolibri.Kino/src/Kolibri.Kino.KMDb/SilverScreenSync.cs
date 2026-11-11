using System.Globalization;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Kolibri.Kino.KMDb;

/// <summary>
/// How two copies of a library document are merged when WinForms or the web app syncs (POST …/sync): what one side
/// lacks is filled from the other, and when both documents carry a time (Updated, Fetched, Checked, Added) the newer
/// one's values win. Edits a user makes on purpose go through PUT (replace) instead, since a merge keeps values.
/// </summary>
public static partial class SilverScreenSync
{
    /// <summary>The fields that tell how fresh a document is, in the order they are looked for.</summary>
    public static readonly string[] TimeFields = ["Updated", "Fetched", "Checked", "Added"];

    /// <summary>The merged document, and whether it differs from <paramref name="stored"/>.</summary>
    public static (JObject Merged, bool Changed) Merge(JObject? stored, JObject incoming)
    {
        if (stored is null) return ((JObject)incoming.DeepClone(), true);

        var incomingNewer = IsNewer(incoming, stored);
        var merged = (JObject)stored.DeepClone();
        var changed = false;
        foreach (var (name, value) in incoming)
        {
            if (name == "_id" || value is null || IsEmpty(value)) continue;
            var current = merged[name];
            if (current is null || IsEmpty(current) || (incomingNewer && !JToken.DeepEquals(current, value)))
            {
                merged[name] = value.DeepClone();
                changed = true;
            }
        }
        return (merged, changed);
    }

    /// <summary>True when <paramref name="a"/> has a later time than <paramref name="b"/> in the first time field both have.</summary>
    internal static bool IsNewer(JObject a, JObject b)
    {
        foreach (var field in TimeFields)
        {
            if (Time(a[field]) is { } ta && Time(b[field]) is { } tb) return ta > tb;
        }
        return false;
    }

    private static DateTime? Time(JToken? token) => token?.Type switch
    {
        JTokenType.Date => ((DateTime)token).ToUniversalTime(),
        JTokenType.String when DateTime.TryParse((string?)token, CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var t) => t,
        _ => null,
    };

    /// <summary>Nothing worth keeping: null, "", OMDb's "N/A", or an empty list or document.</summary>
    public static bool IsEmpty(JToken token) => token.Type switch
    {
        JTokenType.Null or JTokenType.Undefined => true,
        JTokenType.String => string.IsNullOrWhiteSpace((string?)token) || (string?)token == OmdbFormat.NotAvailable,
        JTokenType.Array => !token.HasValues,
        JTokenType.Object => !token.HasValues,
        _ => false,
    };

    /// <summary>Field names that may become columns: letters, digits and _, at most 64 characters, not starting with _.</summary>
    public static bool IsFieldName(string name) => FieldName().IsMatch(name);

    [GeneratedRegex("^[A-Za-z][A-Za-z0-9_]{0,63}$")]
    private static partial Regex FieldName();
}
