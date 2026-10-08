using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using LiteDB;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kolibri.Kino.Data;

/// <summary>
/// Kino's log in its own LiteDB file (normally SilverScreen.logdb next to the library, as the image cache is .imgdb),
/// collection "Log", one document per entry. Entries are kept in memory and saved every few seconds and on close,
/// so logging never waits for the disk. Entries older than Kino:LogRetentionDays (30) are deleted once a day.
/// </summary>
public sealed class LiteDbLogStore : ILogStore, IDisposable
{
    private static readonly TimeSpan SaveInterval = TimeSpan.FromSeconds(5);

    private readonly string _path;
    private readonly int _retentionDays;
    private readonly Lock _lock = new();
    private readonly SemaphoreSlim _saving = new(1, 1);
    private readonly Timer _timer;
    private List<LogEntry> _pending = [];
    private LiteDatabase? _db;
    private DateOnly _purged;

    public LiteDbLogStore(IOptions<KinoOptions> options)
        : this(options.Value.ResolveLogDbPath(), options.Value.LogRetentionDays)
    {
    }

    public LiteDbLogStore(string path, int retentionDays = 30)
    {
        _path = path;
        _retentionDays = Math.Max(1, retentionDays);
        _timer = new Timer(_ => _ = SaveQuietlyAsync(), null, SaveInterval, SaveInterval);
    }

    /// <summary>Queues <paramref name="entry"/>; never throws or blocks.</summary>
    public void Add(LogEntry entry)
    {
        lock (_lock) _pending.Add(entry);
    }

    public async Task FlushAsync(CancellationToken ct = default)
    {
        await _saving.WaitAsync(ct).ConfigureAwait(false);
        try
        {
            List<LogEntry> taken;
            lock (_lock)
            {
                taken = _pending;
                _pending = [];
            }

            await Task.Run(() =>
            {
                var log = Collection();
                if (taken.Count > 0) log.InsertBulk(taken.Select(ToDocument));
                PurgeOncePerDay(log);
            }, ct).ConfigureAwait(false);
        }
        finally
        {
            _saving.Release();
        }
    }

    public async Task<IReadOnlyList<LogEntry>> GetAsync(DateTime? from, LogLevel minLevel, string? text, int max, CancellationToken ct = default)
    {
        await FlushAsync(ct).ConfigureAwait(false);
        return await Task.Run(() =>
        {
            var log = Collection();
            var docs = from is { } f ? log.Find(Query.GTE("Time", f.ToUniversalTime())) : log.FindAll();
            var search = string.IsNullOrWhiteSpace(text) ? null : text.Trim();
            return (IReadOnlyList<LogEntry>)docs.ToList()
                .Select(FromDocument)
                .Where(e => e.Level >= minLevel)
                .Where(e => search is null
                            || e.Message.Contains(search, StringComparison.OrdinalIgnoreCase)
                            || e.Source.Contains(search, StringComparison.OrdinalIgnoreCase)
                            || e.Error?.Contains(search, StringComparison.OrdinalIgnoreCase) == true)
                .OrderByDescending(e => e.Time)
                .Take(max)
                .ToList();
        }, ct).ConfigureAwait(false);
    }

    private ILiteCollection<BsonDocument> Collection()
    {
        _db ??= LiteDbFile.OpenShared(_path, "Kino:LogDbPath");
        var log = _db.GetCollection("Log");
        log.EnsureIndex("Time");
        return log;
    }

    private void PurgeOncePerDay(ILiteCollection<BsonDocument> log)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        if (_purged == today) return;
        log.DeleteMany(Query.LT("Time", DateTime.UtcNow.AddDays(-_retentionDays)));
        _purged = today;
    }

    private static BsonDocument ToDocument(LogEntry e)
    {
        var doc = new BsonDocument
        {
            ["Time"] = e.Time.ToUniversalTime(),
            ["Level"] = e.Level.ToString(),
            ["Source"] = e.Source,
            ["Message"] = e.Message,
        };
        if (e.Error is not null) doc["Error"] = e.Error;
        return doc;
    }

    private static LogEntry FromDocument(BsonDocument doc) => new(
        doc["Time"].AsDateTime.ToLocalTime(),
        Enum.TryParse<LogLevel>(doc["Level"].AsString, out var level) ? level : LogLevel.Information,
        doc["Source"].AsString ?? "",
        doc["Message"].AsString ?? "",
        doc.TryGetValue("Error", out var error) && error.IsString ? error.AsString : null);

    private async Task SaveQuietlyAsync()
    {
        try
        {
            await FlushAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // Never log about logging; tried again at the next save.
            System.Diagnostics.Debug.WriteLine($"Log not saved: {ex.Message}");
        }
    }

    public void Dispose()
    {
        _timer.Dispose();
        SaveQuietlyAsync().GetAwaiter().GetResult();
        _db?.Dispose();
        _saving.Dispose();
    }
}

/// <summary>
/// Writes Kino's own log messages (categories starting with "Kolibri.Kino") to <see cref="LiteDbLogStore"/>.
/// Which levels are written is set by Logging:LogLevel in appsettings.json, like for every other log provider.
/// </summary>
[ProviderAlias("KinoLog")]
public sealed class LiteDbLoggerProvider(LiteDbLogStore store) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) =>
        categoryName.StartsWith("Kolibri.Kino", StringComparison.Ordinal)
            ? new Logger(store, categoryName[(categoryName.LastIndexOf('.') + 1)..])
            : Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;

    public void Dispose()
    {
    }

    private sealed class Logger(LiteDbLogStore store, string source) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;
            store.Add(new LogEntry(DateTime.Now, logLevel, source, formatter(state, exception), exception?.ToString()));
        }
    }
}
