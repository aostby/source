using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Tests;

/// <summary>In-memory OMDb: movies by id and by "title|year".</summary>
internal sealed class FakeOmdb : IMovieInfoProvider
{
    public Dictionary<string, Item> ById { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, Item> ByTitle { get; } = new(StringComparer.OrdinalIgnoreCase);
    public string? UnavailableMessage { get; set; }
    public int Calls { get; private set; }

    public FakeOmdb Add(Item item)
    {
        ById[item.ImdbId] = item;
        ByTitle[$"{item.Title}|{item.Year}"] = item;
        return this;
    }

    public Task<IReadOnlyList<SearchItem>> SearchAsync(string query, int page = 1, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<SearchItem>>([]);

    public Task<Item?> GetByImdbIdAsync(string imdbId, CancellationToken ct = default) =>
        Answer(ById.GetValueOrDefault(imdbId));

    public Task<Item?> GetMovieByTitleAsync(string title, int? year, CancellationToken ct = default) =>
        Answer(ByTitle.GetValueOrDefault($"{title}|{year}"));

    public Task<Item?> GetSeriesByTitleAsync(string title, int? year, CancellationToken ct = default) =>
        Answer(ByTitle.GetValueOrDefault($"{title}|{year}") is { Type: "series" } series ? series : null);

    // A copy, like a real API response, so tests see what was stored rather than a shared instance.
    private Task<Item?> Answer(Item? item)
    {
        Calls++;
        if (UnavailableMessage is not null) throw new MovieInfoUnavailableException(UnavailableMessage);
        return Task.FromResult(item is null ? null : Copy(item));
    }

    private static Item Copy(Item i) => new()
    {
        ImdbId = i.ImdbId, Title = i.Title, Year = i.Year, Type = i.Type, ImdbRating = i.ImdbRating, Response = i.Response,
    };
}

internal sealed class FakeTmdb : IImdbIdResolver
{
    public Dictionary<string, string> Ids { get; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Series ids by "title|year".</summary>
    public Dictionary<string, string> SeriesIds { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Exception? Throws { get; set; }
    public int Calls { get; private set; }

    public Task<string?> FindImdbIdAsync(string title, int? year, CancellationToken ct = default)
    {
        Calls++;
        return Throws is not null ? Task.FromException<string?>(Throws) : Task.FromResult(Ids.GetValueOrDefault($"{title}|{year}"));
    }

    public Task<string?> FindSeriesImdbIdAsync(string title, int year, CancellationToken ct = default)
    {
        Calls++;
        return Throws is not null ? Task.FromException<string?>(Throws) : Task.FromResult(SeriesIds.GetValueOrDefault($"{title}|{year}"));
    }
}

internal sealed class InMemorySettingsStore(UserSettings? settings = null) : ISettingsStore
{
    public UserSettings Current { get; private set; } = settings ?? new UserSettings();
    public event EventHandler? Changed;

    public Task SaveAsync(UserSettings settings, CancellationToken ct = default)
    {
        Current = settings.Clone();
        Changed?.Invoke(this, EventArgs.Empty);
        return Task.CompletedTask;
    }
}

/// <summary>In-memory Plex: exact title + year, or by id; playlists as name → IMDb ids.</summary>
internal sealed class FakePlex : IMediaServerLibrary, IMediaServerPlaylists
{
    public Dictionary<string, List<string>> Playlists { get; } = new(StringComparer.OrdinalIgnoreCase);

    public Task<IReadOnlyList<string>> GetPlaylistNamesAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<string>>(Playlists.Keys.ToList());

    public Task<PlaylistSyncResult> AddToPlaylistAsync(string playlistName, IReadOnlyList<string> imdbIds, CancellationToken ct = default)
    {
        var created = !Playlists.ContainsKey(playlistName);
        var list = Playlists.TryGetValue(playlistName, out var existing) ? existing : Playlists[playlistName] = [];
        var known = imdbIds.Where(id => Movies.Any(m => m.ImdbId == id)).ToList();
        var added = known.Where(id => !list.Contains(id)).ToList();
        list.AddRange(added);
        return Task.FromResult(new PlaylistSyncResult(added.Count, known.Count - added.Count, imdbIds.Except(known).ToList(), created));
    }

    public Task<bool> RemoveFromPlaylistAsync(string playlistName, string imdbId, CancellationToken ct = default) =>
        Task.FromResult(Playlists.TryGetValue(playlistName, out var list) && list.Remove(imdbId));

    public List<Item> Movies { get; } = [];
    public bool IsConfigured { get; set; } = true;
    public Exception? Throws { get; set; }

    public FakePlex Add(Item item)
    {
        Movies.Add(item);
        return this;
    }

    public Task RefreshAsync(CancellationToken ct = default) => Throws is not null ? Task.FromException(Throws) : Task.CompletedTask;

    public Task<Item?> FindByImdbIdAsync(string imdbId, CancellationToken ct = default) =>
        Answer(Movies.FirstOrDefault(m => m.ImdbId == imdbId));

    public Task<Item?> FindMovieByTitleAsync(string title, int? year, CancellationToken ct = default) =>
        Answer(Movies.FirstOrDefault(m => string.Equals(m.Title, title, StringComparison.OrdinalIgnoreCase) && (year is null || m.Year == year.ToString())));

    public Task<IReadOnlyList<Item>> SearchAsync(string text, CancellationToken ct = default) =>
        Throws is not null
            ? Task.FromException<IReadOnlyList<Item>>(Throws)
            : Task.FromResult<IReadOnlyList<Item>>(Movies.Where(m => m.Title.Contains(text, StringComparison.OrdinalIgnoreCase)).ToList());

    private Task<Item?> Answer(Item? item) => Throws is not null ? Task.FromException<Item?>(Throws) : Task.FromResult(item);
}
