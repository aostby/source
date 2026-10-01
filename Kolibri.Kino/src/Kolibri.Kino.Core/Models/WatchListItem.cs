using OMDbApiNet.Model;

namespace Kolibri.Kino.Core.Models;

/// <summary>
/// A movie on a watchlist. Stored in the "WatchListItem" collection with _id = ImdbId, like SilverScreen's
/// Kolibri.net.Common.Dal.Entities.WatchListItem, so a movie is on one watchlist at a time.
/// </summary>
public sealed class WatchListItem : Item
{
    public string WatchListName { get; set; } = "MyMovies";

    /// <summary>"Y" or "N", as SilverScreen stores it.</summary>
    public string Watched { get; set; } = "N";

    public string? Trailer { get; set; }

    /// <summary>Folder of the movie's file, if it has one.</summary>
    public string? FilePath { get; set; }
}
