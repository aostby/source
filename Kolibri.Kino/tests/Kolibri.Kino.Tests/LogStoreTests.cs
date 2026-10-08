using Kolibri.Kino.Core.Models;
using Kolibri.Kino.Data;
using Microsoft.Extensions.Logging;

namespace Kolibri.Kino.Tests;

public sealed class LogStoreTests : IDisposable
{
    private readonly TempDatabase _temp = new();
    private readonly LiteDbLogStore _store;

    public LogStoreTests() => _store = new LiteDbLogStore(Path.ChangeExtension(_temp.Path, ".logdb"), retentionDays: 30);

    public void Dispose()
    {
        _store.Dispose();
        _temp.Dispose();
    }

    [Fact]
    public async Task Entries_are_read_back_newest_first_by_period_level_and_text()
    {
        var now = DateTime.Now;
        _store.Add(new LogEntry(now.AddHours(-1), LogLevel.Information, "KinoForm", "4,106 item(s) in library."));
        _store.Add(new LogEntry(now.AddMinutes(-5), LogLevel.Error, "LocalMoviesForm", "Local movies: disk gone", "System.IO.IOException: disk gone"));
        _store.Add(new LogEntry(now.AddDays(-3), LogLevel.Debug, "LocalMoviesForm", "Scanning 1 of 2: a.mkv"));

        var all = await _store.GetAsync(null, LogLevel.Trace, null, 100);
        Assert.Equal(["Local movies: disk gone", "4,106 item(s) in library.", "Scanning 1 of 2: a.mkv"], all.Select(e => e.Message));
        Assert.Equal("System.IO.IOException: disk gone", all[0].Error);
        Assert.Equal(now.AddMinutes(-5), all[0].Time, TimeSpan.FromMilliseconds(1));

        Assert.Equal(2, (await _store.GetAsync(DateTime.Today.AddDays(-1), LogLevel.Trace, null, 100)).Count);
        Assert.Single(await _store.GetAsync(null, LogLevel.Warning, null, 100));
        Assert.Equal(2, (await _store.GetAsync(null, LogLevel.Trace, "localmovies", 100)).Count);
        Assert.Single(await _store.GetAsync(null, LogLevel.Trace, "IOException", 100));
        Assert.Single(await _store.GetAsync(null, LogLevel.Trace, null, 1));
    }

    [Fact]
    public async Task Entries_older_than_the_retention_are_deleted()
    {
        _store.Add(new LogEntry(DateTime.Now.AddDays(-31), LogLevel.Information, "KinoForm", "old"));
        _store.Add(new LogEntry(DateTime.Now.AddDays(-29), LogLevel.Information, "KinoForm", "kept"));

        Assert.Equal(["kept"], (await _store.GetAsync(null, LogLevel.Trace, null, 100)).Select(e => e.Message));
    }

    [Fact]
    public async Task Only_Kinos_own_categories_are_written()
    {
        using var factory = LoggerFactory.Create(b => b.SetMinimumLevel(LogLevel.Information).AddProvider(new LiteDbLoggerProvider(_store)));

        factory.CreateLogger("Kolibri.Kino.Controllers.Settings.SettingsController").LogInformation("Settings saved for {User}", "anne");
        factory.CreateLogger("Kolibri.Kino.WinForms.Forms.KinoForm").LogDebug("Loading library…"); // below the level
        factory.CreateLogger("Microsoft.Hosting.Lifetime").LogInformation("Application started");
        factory.CreateLogger("System.Net.Http.HttpClient.Default").LogInformation("Sending HTTP request");

        var entry = Assert.Single(await _store.GetAsync(null, LogLevel.Trace, null, 100));
        Assert.Equal(("SettingsController", "Settings saved for anne", LogLevel.Information), (entry.Source, entry.Message, entry.Level));
    }
}
