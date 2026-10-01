using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.Core.Abstractions;

/// <summary>
/// Stored season/episode details: Kino's own cache ("KinoSeason", filled from TMDb/OMDb when a series is opened)
/// and what SilverScreen stored ("KolibriSeason" + "Episode", from OMDb).
/// </summary>
public interface ISeriesRepository
{
    /// <summary>Seasons Kino fetched and cached earlier; empty if none.</summary>
    Task<IReadOnlyList<SeriesSeason>> GetCachedSeasonsAsync(string seriesImdbId, CancellationToken ct = default);

    /// <summary>Seasons SilverScreen stored (OMDb data); empty if none. Read-only.</summary>
    Task<IReadOnlyList<SeriesSeason>> GetSilverScreenSeasonsAsync(string seriesImdbId, CancellationToken ct = default);

    /// <summary>Replaces Kino's cached seasons for the series.</summary>
    Task SaveSeasonsAsync(string seriesImdbId, IReadOnlyList<SeriesSeason> seasons, CancellationToken ct = default);

    /// <summary>Folders in the series folder that "Find new series" should not ask about (not series, or not wanted).</summary>
    Task<IReadOnlySet<string>> GetIgnoredFoldersAsync(CancellationToken ct = default);

    Task IgnoreFolderAsync(string folder, CancellationToken ct = default);
}
