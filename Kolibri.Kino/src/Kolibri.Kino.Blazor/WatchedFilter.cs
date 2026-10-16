namespace Kolibri.Kino.Blazor;

/// <summary>
/// The "Not watched / All / Watched" filter shared by the Library page, its Recently added rows and the Recently added
/// page. In the address it is "seen": none means Not watched (the default), "all" or "yes".
/// </summary>
public static class WatchedFilter
{
    /// <summary>The choices, in the order of the dropdown: (value in the dropdown, label).</summary>
    public static readonly (string Value, string Label)[] Choices = [("", "Not watched"), ("all", "All"), ("yes", "Watched")];

    /// <summary>"no" (the default), "yes", or null for all.</summary>
    public static string? Key(string? seen) => seen switch { "all" => null, "yes" => "yes", _ => "no" };

    /// <summary>The dropdown's value for a key: "" for Not watched, "all", "yes".</summary>
    public static string Value(string? key) => key switch { "no" => "", null => "all", var k => k };

    /// <summary>Whether a title passes; <paramref name="watched"/> null (not known yet) lets everything through.</summary>
    public static bool Matches(string? key, IReadOnlySet<string>? watched, string imdbId) => key switch
    {
        "no" => watched is null || !watched.Contains(imdbId),
        "yes" => watched is null || watched.Contains(imdbId),
        _ => true,
    };

    /// <summary>" not watched", " watched" or "", for summaries ("12 not watched item(s)").</summary>
    public static string Words(string? key) => key switch { "no" => " not watched", "yes" => " watched", _ => "" };
}
