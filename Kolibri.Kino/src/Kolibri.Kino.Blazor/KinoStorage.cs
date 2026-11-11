using Kolibri.Kino.Data;

namespace Kolibri.Kino.Blazor;

/// <summary>
/// Keeps the library's storage chosen: MariaDB through KMDb while it answers (checked every minute), else the LiteDB
/// file; sends changes kept while KMDb was away and copies KMDb's changes into the file. See Data's KmdbConnection.
/// </summary>
public sealed class KinoStorage(IServiceProvider services) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken) => services.RunKinoStorageAsync(stoppingToken);
}
