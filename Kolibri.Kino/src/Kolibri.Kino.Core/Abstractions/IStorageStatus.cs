namespace Kolibri.Kino.Core.Abstractions;

/// <summary>Where the library is read and written: the LiteDB file, or MariaDB through Kolibri.Kino.KMDb.</summary>
public enum StorageKind
{
    LiteDb,
    Kmdb,
}

/// <summary>The storage in use, why, and when that was last checked.</summary>
/// <param name="Active">What the library uses now.</param>
/// <param name="KmdbUrl">KMDb's address when one is set (with or without it answering).</param>
/// <param name="Message">For people: e.g. "MariaDB through KMDb (http://greenlantern:8086/)" or why LiteDB is used.</param>
/// <param name="PendingChanges">Changes made in the LiteDB file while KMDb didn't answer, not yet sent there.</param>
public sealed record StorageState(StorageKind Active, string? KmdbUrl, string Message, DateTime CheckedUtc, int PendingChanges = 0)
{
    public bool UsesKmdb => Active == StorageKind.Kmdb;
}

/// <summary>
/// Chooses the library's storage: KMDb when its URL and key are set, /ping answers and the key is accepted; else the
/// LiteDB file. Checked at start, after a settings change, and every minute.
/// </summary>
public interface IStorageStatus
{
    StorageState Current { get; }

    /// <summary>Raised when <see cref="Current"/> switches between LiteDB and KMDb.</summary>
    event EventHandler? Changed;

    /// <summary>Checks KMDb now and returns the (possibly new) state.</summary>
    Task<StorageState> CheckAsync(CancellationToken ct = default);
}
