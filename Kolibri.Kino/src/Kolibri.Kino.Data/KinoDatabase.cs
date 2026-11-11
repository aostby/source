using System.Collections.Concurrent;
using Kolibri.Kino.Core;
using Kolibri.Kino.Data.Storage;
using LiteDB;
using Microsoft.Extensions.Options;

namespace Kolibri.Kino.Data;

/// <summary>
/// The one open connection to the library file, shared by all repositories, and the collections they use
/// (<see cref="Collection"/>): the LiteDB file, or MariaDB through KMDb once <see cref="KmdbConnection"/> is set up.
/// </summary>
public sealed class KinoDatabase : IDisposable
{
    static KinoDatabase() => LiteDbMapping.WarmUp();

    public KinoDatabase(IOptions<KinoOptions> options)
        : this(options.Value.LiteDbPath)
    {
    }

    public KinoDatabase(string dbPath)
    {
        Database = LiteDbFile.OpenShared(dbPath, "Kino:LiteDbPath");
    }

    public LiteDatabase Database { get; }

    private readonly ConcurrentDictionary<string, SwitchingDocumentCollection> _collections = new(StringComparer.Ordinal);
    private PendingChanges? _pending;

    /// <summary>A collection of the library: in the LiteDB file, or in KMDb while that is in use (with the file kept up to date).</summary>
    public IDocumentCollection Collection(string name) => _collections.GetOrAdd(name, n => new SwitchingDocumentCollection(this, n));

    /// <summary>Changes kept for KMDb while it didn't answer.</summary>
    public PendingChanges Pending => _pending ??= new PendingChanges(Database);

    /// <summary>The KMDb connection, once made (null: only the LiteDB file, as in tests and without KMDb).</summary>
    internal KmdbConnection? Kmdb { get; private set; }

    internal KmdbClient? KmdbClient { get; private set; }

    /// <summary>Lets the collections use KMDb (called by <see cref="KmdbConnection"/> when it is made).</summary>
    internal void UseKmdb(KmdbConnection connection, KmdbClient client)
    {
        KmdbClient = client;
        Kmdb = connection;
    }

    public void Dispose() => Database.Dispose();
}

internal static class LiteDbFile
{
    /// <summary>
    /// Shared lets SilverScreen and Kino open the same file at once.
    /// No Upgrade: opening must never rewrite an existing file.
    /// </summary>
    public static LiteDatabase OpenShared(string path, string settingName)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new InvalidOperationException($"{settingName} is not configured.");

        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        return new LiteDatabase(new ConnectionString { Filename = path, Connection = Mode });
    }

    /// <summary>
    /// Shared on Windows. In a container (the .NET images set DOTNET_RUNNING_IN_CONTAINER) Kino is the only program
    /// using the files, so Direct: Shared locks with a machine-wide named mutex ("Global\…"), which newer .NET runtimes
    /// won't create for the NAS user the container runs as, and Kino then failed at startup.
    /// </summary>
    public static ConnectionType Mode { get; } =
        string.Equals(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), "true", StringComparison.OrdinalIgnoreCase)
            ? ConnectionType.Direct
            : ConnectionType.Shared;
}
