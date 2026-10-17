namespace Kolibri.Kino.Core.Models;

/// <summary>
/// A title wanted, perhaps not owned (the wishlist, "KinoWishlist"). Its details are a copy from the library or OMDb,
/// taken when it was added, so a title that isn't in SilverScreen.db needs no library entry (and no file).
/// Whether it is in the library is looked up each time the list is shown.
/// A series' missing episode can be on it too, with an id of its own (see <see cref="EpisodeId"/>).
/// </summary>
public sealed class WishlistItem
{
    /// <summary>The title's IMDb id, or for an episode "{series' IMDb id}_S06E03".</summary>
    public string ImdbId { get; set; } = "";
    public string Title { get; set; } = "";
    public string? Year { get; set; }
    public string? Type { get; set; }
    public string? Genre { get; set; }
    public string? ImdbRating { get; set; }
    public string? Poster { get; set; }

    /// <summary>When it was put on the list (local time).</summary>
    public DateTime Added { get; set; }

    /// <summary>An episode of a series (see <see cref="EpisodeId"/>), not a title of its own.</summary>
    public bool IsEpisode => ImdbId.Contains('_');

    /// <summary>The IMDb id for links and details: the series' for an episode, else the title's own.</summary>
    public string TitleImdbId => IsEpisode ? ImdbId[..ImdbId.IndexOf('_')] : ImdbId;

    /// <summary>The wishlist id of a series' episode: "tt5875444_S06E03".</summary>
    public static string EpisodeId(string seriesImdbId, int season, int episode) => $"{seriesImdbId}_S{season:00}E{episode:00}";
}
