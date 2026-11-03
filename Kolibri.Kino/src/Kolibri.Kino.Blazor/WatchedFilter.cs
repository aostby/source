namespace Kolibri.Kino.Blazor;

/// <summary>
/// The "Not watched / All / Watched" filter shared by the Library page, its Recently added rows and the Recently added
/// page. In the address it is "seen": "no", "all" or "yes"; none means All, the default, so a page opens without asking
/// Plex what's watched (which takes a moment). Only "no" and "yes" ask Plex and the watchlists.
/// </summary>
public static class WatchedFilter
{
    /// <summary>
    /// The choices, in the order of the dropdown: (value in the dropdown and the address, label). "Status - All" says
    /// what the dropdown is about when nothing is filtered.
    /// </summary>
    public static readonly (string Value, string Label)[] Choices = [("no", "Not watched"), ("all", "Status - All"), ("yes", "Watched")];

    /// <summary>"no", "yes", or null for all (the default, also "seen=all").</summary>
    public static string? Key(string? seen) => seen is "no" or "yes" ? seen : null;

    /// <summary>The dropdown's value for a key: "no", "yes", or "all".</summary>
    public static string Value(string? key) => key ?? "all";

    /// <summary>What a dropdown choice puts in the address: nothing for All (the default, so the address stays plain), else the choice.</summary>
    public static string ForAddress(string? value) => value is null or "all" ? string.Empty : value;

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
