using Kolibri.Kino.Core.Abstractions;
using LiteDB;
using Microsoft.Extensions.Logging;

namespace Kolibri.Kino.Data.Storage;

/// <summary>
/// Chooses where the library lives: MariaDB through KMDb when the KMDb URL and the Uranus token are set, /ping answers
/// and the key is accepted; else the LiteDB file. Checked at start, after a settings change and every minute
/// (<see cref="RunAsync"/>). When KMDb comes (back), the changes made in the LiteDB file meanwhile are sent to it; while
/// it is in use, what changed there (also from other machines) is copied into the LiteDB file every ten minutes, so
/// the file is a current fallback.
/// </summary>
public sealed class KmdbConnection : IStorageStatus
{
    /// <summary>Collections copied from KMDb into the LiteDB file. Images are not (fetched as needed).</summary>
    public static readonly IReadOnlyList<string> SyncedCollections =
    [
        "Item", "FileItem", "WatchListItem", "KinoWishlist", "KinoRating", "KinoSimilar", "KinoSeason", "KinoSeriesFetch",
        "KinoIgnoredFolder", "KinoTrending", "KinoDiscover", "KinoTmdbImdb", "KinoFileDate", "KinoApiUsage", "KolibriSeason", "Episode",
    ];

    private static readonly TimeSpan CheckEvery = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan PullEvery = TimeSpan.FromMinutes(10);
    private const string SyncStateCollection = "KinoSyncState";

    private readonly KmdbClient _kmdb;
    private readonly LiteDatabase _db;
    private readonly PendingChanges _pending;
    private readonly ILogger<KmdbConnection> _log;
    private readonly SemaphoreSlim _checking = new(1, 1);
    private readonly SemaphoreSlim _syncing = new(1, 1);
    private volatile StorageState _state;
    private DateTime _lastPullUtc = DateTime.MinValue;

    public KmdbConnection(KmdbClient kmdb, KinoDatabase db, ISettingsStore settings, ILogger<KmdbConnection> log)
    {
        _kmdb = kmdb;
        _db = db.Database;
        _pending = db.Pending;
        _log = log;
        _state = new StorageState(StorageKind.LiteDb, kmdb.BaseUrl, kmdb.IsConfigured ? "KMDb not checked yet: the LiteDB file is used." : NotConfigured, DateTime.MinValue);
        db.UseKmdb(this, kmdb);
        // A new KMDb URL or token in Settings is checked at once.
        settings.Changed += (_, _) => _ = CheckAsync();
    }

    private const string NotConfigured = "No KMDb URL (or no Uranus token) in Settings: the LiteDB file is used.";

    public StorageState Current => _state;

    public event EventHandler? Changed;

    /// <summary>True while reads and writes go to KMDb.</summary>
    public bool UseKmdb => _state.UsesKmdb;

    /// <summary>A KMDb URL and token are set (KMDb answering or not): changes made meanwhile are kept for it.</summary>
    public bool IsConfigured => _kmdb.IsConfigured;

    /// <summary>A call to KMDb failed: back to the LiteDB file until the next check finds KMDb again.</summary>
    public void Failed(Exception ex) =>
        SetState(new StorageState(StorageKind.LiteDb, _kmdb.BaseUrl, $"KMDb stopped answering ({ex.Message}): the LiteDB file is used.", DateTime.UtcNow, _state.PendingChanges));

    public async Task<StorageState> CheckAsync(CancellationToken ct = default)
    {
        await _checking.WaitAsync(ct).ConfigureAwait(false);
        try
        {
            var pendingCount = await _pending.CountAsync(ct).ConfigureAwait(false);
            if (!_kmdb.IsConfigured)
            {
                SetState(new StorageState(StorageKind.LiteDb, _kmdb.BaseUrl, NotConfigured, DateTime.UtcNow, pendingCount));
                return _state;
            }
            var url = _kmdb.BaseUrl;
            if (!await _kmdb.PingAsync(ct).ConfigureAwait(false))
            {
                SetState(new StorageState(StorageKind.LiteDb, url, $"KMDb ({url}) does not answer /ping: the LiteDB file is used.", DateTime.UtcNow, pendingCount));
                return _state;
            }
            if (await _kmdb.CheckLibraryAsync(ct).ConfigureAwait(false) is { } problem)
            {
                SetState(new StorageState(StorageKind.LiteDb, url, problem + " The LiteDB file is used.", DateTime.UtcNow, pendingCount));
                return _state;
            }
            SetState(new StorageState(StorageKind.Kmdb, url, $"MariaDB through KMDb ({url}).", DateTime.UtcNow, pendingCount));
        }
        finally
        {
            _checking.Release();
        }
        await SyncAsync(ct).ConfigureAwait(false);
        return _state;
    }

    /// <summary>Checks KMDb every minute until <paramref name="ct"/> is cancelled (WinForms starts it; the web app hosts it).</summary>
    public async Task RunAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await CheckAsync(ct).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _log.LogWarning(ex, "Checking KMDb failed");
            }
            try { await Task.Delay(CheckEvery, ct).ConfigureAwait(false); }
            catch (OperationCanceledException) { return; }
        }
    }

    /// <summary>Sends the changes kept while KMDb didn't answer, then (every ten minutes) copies KMDb's changes into the LiteDB file.</summary>
    private async Task SyncAsync(CancellationToken ct)
    {
        if (!UseKmdb || !await _syncing.WaitAsync(0, ct).ConfigureAwait(false)) return;
        try
        {
            var sent = 0;
            foreach (var change in await _pending.AllAsync(ct).ConfigureAwait(false))
            {
                var lite = _db.GetCollection(change.Collection);
                if (change.Operation == PendingChanges.Delete)
                    await _kmdb.DeleteAsync(change.Collection, change.Id, ct).ConfigureAwait(false);
                else if (await Task.Run(() => lite.FindById(change.Id), ct).ConfigureAwait(false) is { } doc)
                    await _kmdb.PutAsync(change.Collection, doc, ct).ConfigureAwait(false);
                await _pending.RemoveAsync(change.Key, ct).ConfigureAwait(false);
                sent++;
            }
            if (sent > 0) _log.LogInformation("Sent {Count} changes made while KMDb didn't answer", sent);

            if (DateTime.UtcNow - _lastPullUtc >= PullEvery)
            {
                await PullAsync(ct).ConfigureAwait(false);
                _lastPullUtc = DateTime.UtcNow;
            }
            SetState(_state with { PendingChanges = await _pending.CountAsync(ct).ConfigureAwait(false) });
        }
        catch (KmdbUnavailableException ex)
        {
            Failed(ex);
            _log.LogWarning("Syncing with KMDb stopped: {Message}", ex.Message);
        }
        finally
        {
            _syncing.Release();
        }
    }

    /// <summary>Copies what changed in KMDb since the last time into the LiteDB file (per collection, by _modified).</summary>
    private async Task PullAsync(CancellationToken ct)
    {
        var state = _db.GetCollection(SyncStateCollection);
        var copied = 0;
        foreach (var name in SyncedCollections)
        {
            // Two minutes back: the PC's and the database server's clocks may differ a little.
            var started = DateTime.UtcNow.AddMinutes(-2);
            if (state.FindById(name)?["Pulled"] is not { IsDateTime: true } pulled)
            {
                // The first time only notes when: what KMDb holds now came from a LiteDB file, maybe an older one than
                // this, and must not overwrite newer data here. From now on, changes are copied.
                state.Upsert(new BsonDocument { ["_id"] = name, ["Pulled"] = started });
                continue;
            }
            var docs = await _kmdb.ChangedSinceAsync(name, pulled.AsDateTime.ToUniversalTime(), ct).ConfigureAwait(false);
            if (docs.Count > 0)
            {
                var lite = _db.GetCollection(name);
                await Task.Run(() =>
                {
                    foreach (var doc in docs)
                    {
                        // Binary fields stay as they are in the file (KMDb sends only their address).
                        foreach (var key in doc.Keys.Where(k => BsonJson.IsBinaryAddress(doc[k])).ToList()) doc.Remove(key);
                        var stored = lite.FindById(doc["_id"]);
                        if (stored is not null) foreach (var (key, value) in stored) if (value.IsBinary && !doc.ContainsKey(key)) doc[key] = value;
                        lite.Upsert(doc);
                    }
                }, ct).ConfigureAwait(false);
                copied += docs.Count;
            }
            state.Upsert(new BsonDocument { ["_id"] = name, ["Pulled"] = started });
        }
        if (copied > 0) _log.LogInformation("Copied {Count} documents changed in KMDb into the LiteDB file", copied);
    }

    private void SetState(StorageState state)
    {
        var switched = state.Active != _state.Active;
        _state = state;
        if (switched)
        {
            _log.LogInformation("Library storage: {Message}", state.Message);
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}
