using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kolibri.Kino.Data;

public static class DataServiceCollectionExtensions
{
    /// <summary>
    /// Registers storage, the image cache, file scanning, cleanup and the online services. API keys and the Plex
    /// token come from the user settings in the database, read on each use, so changes in Settings apply at once.
    /// </summary>
    public static IServiceCollection AddKinoData(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KinoOptions>(configuration.GetSection(KinoOptions.SectionName));

        // One LiteDatabase per file per process; LiteDB is thread-safe within a single instance.
        services.AddSingleton<KinoDatabase>();
        services.AddSingleton<LiteDbImageStore>();

        services.AddSingleton<ISettingsStore, LiteDbSettingsStore>();
        services.AddSingleton<IMovieRepository, LiteDbMovieRepository>();
        services.AddSingleton<IFileItemRepository, LiteDbFileItemRepository>();
        services.AddSingleton<IWatchListRepository, LiteDbWatchListRepository>();
        services.AddSingleton<IMediaFileScanner, FileSystemMediaScanner>();
        services.AddSingleton<IMovieFileNameParser, MovieFileNameParser>();
        services.AddSingleton<IFolderCleaner, FileSystemFolderCleaner>();
        services.AddSingleton<IFolderRenamer, FileSystemFolderRenamer>();

        services.AddSingleton(new HttpClient { Timeout = TimeSpan.FromSeconds(30) });
        services.AddSingleton<IPosterProvider, CachedPosterProvider>();
        services.AddSingleton<IConnectionTester, ConnectionTester>();

        services.AddSingleton<IMovieInfoProvider, OmdbMovieInfoProvider>();
        services.AddSingleton<ISeriesRepository, LiteDbSeriesRepository>();
        services.AddSingleton<ISeriesInfoProvider, SeriesInfoProvider>();
        services.AddSingleton<IEpisodeFileFinder, EpisodeFileFinder>();
        services.AddSingleton<IImdbIdResolver, TmdbImdbIdResolver>();
        services.AddSingleton<ITmdbLinks, TmdbLinks>();

        // Plex's movie sections can be large, so it gets its own client with a longer timeout.
        services.AddSingleton(sp => new PlexLibrary(new HttpClient { Timeout = TimeSpan.FromMinutes(2) }, sp.GetRequiredService<ISettingsStore>()));
        services.AddSingleton<IMediaServerLibrary>(sp => sp.GetRequiredService<PlexLibrary>());
        services.AddSingleton<IMediaServerPlaylists>(sp => sp.GetRequiredService<PlexLibrary>());

        return services;
    }
}
