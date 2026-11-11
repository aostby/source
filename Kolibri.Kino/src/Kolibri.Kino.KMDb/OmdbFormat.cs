using System.Globalization;
using OMDbApiNet.Model;

namespace Kolibri.Kino.KMDb;

/// <summary>Turns IMDb rows into OMDb's text formats ("1999", "2008–2013", "136 min", "2,291,970", "N/A" …).</summary>
public static class OmdbFormat
{
    public const string NotAvailable = "N/A";
    public const string ImdbSource = "Internet Movie Database";

    private static readonly CultureInfo En = CultureInfo.InvariantCulture;

    /// <summary>IMDb title types per OMDb type (the type= parameter and the Type field).</summary>
    public static readonly IReadOnlyDictionary<string, string[]> TitleTypes = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
    {
        ["movie"] = ["movie", "tvMovie", "short", "tvShort", "video", "tvSpecial"],
        ["series"] = ["tvSeries", "tvMiniSeries"],
        ["episode"] = ["tvEpisode"],
        ["game"] = ["videoGame"],
    };

    /// <summary>What a search without type= covers: everything but single episodes, which would drown the rest.</summary>
    public static readonly string[] DefaultSearchTypes = [.. TitleTypes["movie"], .. TitleTypes["series"], .. TitleTypes["game"]];

    /// <summary>What t= without type= covers: everything, episodes too.</summary>
    public static readonly string[] AllTypes = [.. DefaultSearchTypes, .. TitleTypes["episode"]];

    /// <summary>OMDb's type for an IMDb title type: movie, series, episode or game.</summary>
    public static string OmdbType(string titleType) =>
        TitleTypes.FirstOrDefault(t => t.Value.Contains(titleType, StringComparer.OrdinalIgnoreCase)).Key ?? "movie";

    /// <summary>"1999" for a movie; "2008–2013" or "2019–" for a series (en dash, as OMDb writes it).</summary>
    public static string Year(string titleType, int? startYear, int? endYear)
    {
        if (startYear is null) return NotAvailable;
        if (OmdbType(titleType) != "series") return startYear.Value.ToString(En);
        if (endYear is null) return $"{startYear}–";
        return endYear == startYear ? startYear.Value.ToString(En) : $"{startYear}–{endYear}";
    }

    public static string Runtime(int? minutes) => minutes is > 0 ? $"{minutes} min" : NotAvailable;

    /// <summary>"Action, Sci-Fi" from IMDb's "Action,Sci-Fi".</summary>
    public static string Genre(string? genres) =>
        string.IsNullOrWhiteSpace(genres) ? NotAvailable : string.Join(", ", Split(genres));

    public static string Rating(decimal? rating) => rating?.ToString("0.0", En) ?? NotAvailable;

    public static string Votes(int? votes) => votes?.ToString("N0", En) ?? NotAvailable;

    public static List<Rating> Ratings(decimal? rating) =>
        rating is null ? [] : [new Rating { Source = ImdbSource, Value = $"{Rating(rating)}/10" }];

    /// <summary>The ids in a comma list, in order, without repeats.</summary>
    public static IReadOnlyList<string> Split(string? list) =>
        string.IsNullOrWhiteSpace(list)
            ? []
            : list.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Distinct(StringComparer.Ordinal).ToList();

    /// <summary>Names for the first <paramref name="max"/> ids that have one, comma separated; "N/A" when none.</summary>
    public static string Names(IEnumerable<string> ids, IReadOnlyDictionary<string, string> names, int max)
    {
        var list = ids.Select(id => names.GetValueOrDefault(id)).OfType<string>().Take(max).ToList();
        return list.Count == 0 ? NotAvailable : string.Join(", ", list);
    }

    public static string Join(IReadOnlyList<string> values) => values.Count == 0 ? NotAvailable : string.Join(", ", values);

    /// <summary>OMDb's answer when nothing is found or the request is wrong.</summary>
    public static ErrorResponse Error(string message) => new(message);
}

/// <summary>OMDb's error body: {"Response":"False","Error":"Movie not found!"}.</summary>
public sealed record ErrorResponse(
    [property: Newtonsoft.Json.JsonProperty("Error")] string Error)
{
    [Newtonsoft.Json.JsonProperty("Response", Order = -2)]
    public string Response => "False";
}
