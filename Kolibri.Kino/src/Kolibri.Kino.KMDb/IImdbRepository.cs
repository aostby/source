namespace Kolibri.Kino.KMDb;

/// <summary>Read-only lookups in the IMDb database (the IMDb data sets loaded into MariaDB).</summary>
public interface IImdbRepository
{
    Task<TitleRow?> GetTitleAsync(string imdbId, CancellationToken ct = default);

    /// <summary>The title named exactly <paramref name="title"/> (primary or alternative title) with the most votes.</summary>
    Task<TitleRow?> FindByTitleAsync(string title, IReadOnlyCollection<string> titleTypes, int? year, CancellationToken ct = default);

    /// <summary>Titles starting with <paramref name="query"/> (also after The/A/An, and alternative titles), most votes first.</summary>
    Task<SearchPage> SearchAsync(string query, IReadOnlyCollection<string> titleTypes, int? year, int page, int pageSize, CancellationToken ct = default);

    /// <summary>Names for nm ids; unknown ids are left out.</summary>
    Task<IReadOnlyDictionary<string, string>> NamesAsync(IReadOnlyCollection<string> nameIds, CancellationToken ct = default);

    /// <summary>The top-billed actors (actor, actress, self). Empty when the database has no cast for the title.</summary>
    Task<IReadOnlyList<string>> ActorsAsync(string imdbId, int max, CancellationToken ct = default);

    Task<EpisodeLink?> GetEpisodeLinkAsync(string episodeId, CancellationToken ct = default);

    /// <summary>The highest season number of a series; null when it has no episodes.</summary>
    Task<int?> TotalSeasonsAsync(string seriesId, CancellationToken ct = default);

    Task<IReadOnlyList<SeasonEpisodeRow>> SeasonAsync(string seriesId, int season, CancellationToken ct = default);

    Task<string?> EpisodeIdAsync(string seriesId, int season, int episode, CancellationToken ct = default);

    /// <summary>True when the database answers.</summary>
    Task<bool> PingAsync(CancellationToken ct = default);
}
