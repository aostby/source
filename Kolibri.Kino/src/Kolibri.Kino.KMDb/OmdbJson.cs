using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kolibri.Kino.KMDb;

/// <summary>Reading the library's OMDb documents, which LiteDB stored under the C# property names.</summary>
public static class OmdbJson
{
    /// <summary>
    /// Reads JSON by C# property name (SeasonNumber, ImdbId), not OMDb's names ([JsonProperty("Season")], "imdbID"):
    /// LiteDB's BsonMapper ignores Newtonsoft's attributes, so the library holds the C# names.
    /// </summary>
    public static readonly JsonSerializer Reader = JsonSerializer.Create(new JsonSerializerSettings
    {
        ContractResolver = new ClrNames(),
        NullValueHandling = NullValueHandling.Ignore,
        MissingMemberHandling = MissingMemberHandling.Ignore,
    });

    /// <summary>A real value: not empty and not OMDb's "N/A".</summary>
    public static bool HasValue(string? value) => !string.IsNullOrWhiteSpace(value) && value != OmdbFormat.NotAvailable;

    private sealed class ClrNames : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            property.PropertyName = member.Name;
            return property;
        }
    }
}
