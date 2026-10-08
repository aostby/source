using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.Data;
using LiteDB;

namespace Kolibri.Kino.Tests;

public sealed class SettingsStoreTests : IDisposable
{
    private readonly TempDatabase _temp = new();

    public void Dispose() => _temp.Dispose();

    [Fact]
    public void Reads_the_document_SilverScreen_saved()
    {
        SilverScreenSaves(new BsonDocument
        {
            ["OMDBkey"] = "omdb-key",
            ["TMDBkey"] = "tmdb-key",
            ["XPlexToken"] = "plex-token",
            ["XPlexServerName"] = "HTPC",
            ["FavoriteWatchList"] = "Jul",
            ["UserFilePaths"] = new BsonDocument { ["MoviesSourcePath"] = @"\\server\Movies" },
        });
        using var db = new KinoDatabase(_temp.Path);

        var settings = new LiteDbSettingsStore(db, _temp.Path).Current;

        Assert.Equal(("omdb-key", "tmdb-key", "plex-token", "http://HTPC:32400", "Jul", @"\\server\Movies", _temp.Path),
            (settings.OMDBkey, settings.TMDBkey, settings.XPlexToken, settings.GetPlexBaseUrl(), settings.FavoriteWatchList,
             settings.UserFilePaths.MoviesSourcePath, settings.LiteDBFilePath));
    }

    [Fact]
    public async Task Saving_keeps_unknown_fields_removes_cleared_values_and_raises_Changed()
    {
        SilverScreenSaves(new BsonDocument
        {
            ["OMDBkey"] = "old",
            ["XPlexToken"] = "token",
            ["SomethingOnlySilverScreenKnows"] = 42,
            ["UserFilePaths"] = new BsonDocument { ["MoviesSourcePath"] = @"D:\Movies", ["BeerXMLPath"] = @"D:\Beer" },
        });
        using var db = new KinoDatabase(_temp.Path);
        var store = new LiteDbSettingsStore(db);
        var changed = 0;
        store.Changed += (_, _) => changed++;

        var edited = store.Current.Clone();
        edited.OMDBkey = "  new  ";
        edited.XPlexToken = "";
        edited.UserFilePaths.SeriesSourcePath = @"D:\Series";
        await store.SaveAsync(edited);

        var doc = db.Database.GetCollection("UserSettings").FindById(Environment.UserName);
        Assert.Equal("new", doc["OMDBkey"].AsString);
        Assert.False(doc.ContainsKey("XPlexToken"));             // cleared: SilverScreen falls back to its default
        Assert.Equal(UserSettings.DefaultTmdbKey, doc["TMDBkey"].AsString); // never set: the shared default
        Assert.Equal(42, doc["SomethingOnlySilverScreenKnows"].AsInt32);
        Assert.Equal((@"D:\Movies", @"D:\Series", @"D:\Beer"),
            (doc["UserFilePaths"]["MoviesSourcePath"].AsString, doc["UserFilePaths"]["SeriesSourcePath"].AsString, doc["UserFilePaths"]["BeerXMLPath"].AsString));
        Assert.Equal(("new", null), (store.Current.OMDBkey, store.Current.XPlexToken));
        Assert.Equal(1, changed);
    }

    [Fact]
    public async Task SettingsUser_reads_and_saves_another_users_document_as_a_container_does()
    {
        using (var legacy = new LiteDatabase(_temp.Path))
            legacy.GetCollection("UserSettings").Insert("asoes", new BsonDocument { ["UserName"] = "asoes", ["OMDBkey"] = "windows-key" });
        using var db = new KinoDatabase(_temp.Path);
        var options = Microsoft.Extensions.Options.Options.Create(new KinoOptions { LiteDbPath = _temp.Path, SettingsUser = "asoes" });
        var store = new LiteDbSettingsStore(db, options);

        Assert.Equal(("windows-key", "asoes"), (store.Current.OMDBkey, store.Current.UserName));

        var edited = store.Current.Clone();
        edited.FavoriteWatchList = "Jul";
        await store.SaveAsync(edited);
        var documents = db.Database.GetCollection("UserSettings");
        Assert.Equal("Jul", documents.FindById("asoes")["FavoriteWatchList"].AsString);
        Assert.Equal(1, documents.Count());
    }

    [Fact]
    public void A_new_database_uses_the_shared_default_keys_so_it_works_at_once()
    {
        using var db = new KinoDatabase(_temp.Path);

        var settings = new LiteDbSettingsStore(db).Current;

        Assert.Equal((UserSettings.DefaultOmdbKey, UserSettings.DefaultTmdbKey), (settings.OMDBkey, settings.TMDBkey));
        Assert.Equal("https://datasets.imdbws.com/", settings.IMDbDataFiles);
        Assert.True(settings.UsesDefaultKeys());
    }

    [Fact]
    public void An_empty_IMDb_data_files_address_means_the_default()
    {
        SilverScreenSaves(new BsonDocument { ["IMDbDataFiles"] = "" });
        using var db = new KinoDatabase(_temp.Path);

        Assert.Equal(UserSettings.DefaultImdbDataFiles, new LiteDbSettingsStore(db).Current.IMDbDataFiles);
    }

    [Fact]
    public async Task Own_keys_stop_the_prompt_and_clearing_a_key_goes_back_to_the_default()
    {
        SilverScreenSaves(new BsonDocument { ["OMDBkey"] = "", ["TMDBkey"] = "my-tmdb" });
        using var db = new KinoDatabase(_temp.Path);
        var store = new LiteDbSettingsStore(db);

        Assert.Equal(UserSettings.DefaultOmdbKey, store.Current.OMDBkey); // stored empty = default
        Assert.True(store.Current.UsesDefaultKeys());

        var own = store.Current.Clone();
        own.OMDBkey = "my-omdb";
        await store.SaveAsync(own);
        Assert.False(store.Current.UsesDefaultKeys());

        var cleared = store.Current.Clone();
        cleared.TMDBkey = " ";
        await store.SaveAsync(cleared);
        Assert.Equal(UserSettings.DefaultTmdbKey, store.Current.TMDBkey);
        Assert.True(store.Current.UsesDefaultKeys());
    }

    [Fact]
    public async Task Missing_OMDb_key_stops_with_a_clear_message_instead_of_calling_OMDb()
    {
        var omdb = new OmdbMovieInfoProvider(new InMemorySettingsStore(new UserSettings { OMDBkey = null }));

        var ex = await Assert.ThrowsAsync<MovieInfoUnavailableException>(() => omdb.GetByImdbIdAsync("tt0133093"));
        Assert.Contains("Settings", ex.Message);
    }

    [Fact]
    public void Reads_the_database_path_stored_in_the_database_without_creating_missing_files()
    {
        SilverScreenSaves(new BsonDocument { ["LiteDBFilePath"] = @"E:\RELEASE\SilverScreenDB\SilverScreen.db" });
        var missing = System.IO.Path.ChangeExtension(_temp.Path, ".missing.db");

        Assert.Equal(@"E:\RELEASE\SilverScreenDB\SilverScreen.db", LiteDbSettingsStore.ReadStoredDbPath(_temp.Path, Environment.UserName));
        Assert.Null(LiteDbSettingsStore.ReadStoredDbPath(_temp.Path, "someone-else"));
        Assert.Null(LiteDbSettingsStore.ReadStoredDbPath(missing, Environment.UserName));
        Assert.False(File.Exists(missing));
    }

    private void SilverScreenSaves(BsonDocument settings)
    {
        using var legacy = new LiteDatabase(_temp.Path);
        settings["UserName"] = Environment.UserName;
        legacy.GetCollection("UserSettings").Insert(Environment.UserName, settings);
    }
}
