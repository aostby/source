using Kolibri.Kino.Core;
using LiteDB;
using Microsoft.Extensions.Options;

namespace Kolibri.Kino.Data;

/// <summary>
/// The one open connection to the library file, shared by all repositories.
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
