using OMDbApiNet.Model;

namespace Kolibri.Kino.Blazor;

/// <summary>
/// The last library or OMDb search in this browser tab (one per Blazor circuit), so going back from a movie
/// doesn't search again. OMDb searches count against the daily limit.
/// </summary>
public sealed class LastSearch
{
    public string? Key { get; private set; }

    public IReadOnlyList<Item> Local { get; private set; } = [];

    public IReadOnlyList<SearchItem> Online { get; private set; } = [];

    public void Remember(string key, IReadOnlyList<Item> local, IReadOnlyList<SearchItem> online) =>
        (Key, Local, Online) = (key, local, online);

    /// <summary>After the library changed (import, remove), so the next visit reads it again.</summary>
    public void Forget() => (Key, Local, Online) = (null, [], []);
}
