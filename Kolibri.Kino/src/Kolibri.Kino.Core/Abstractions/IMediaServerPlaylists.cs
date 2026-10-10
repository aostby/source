namespace Kolibri.Kino.Core.Abstractions;

/// <param name="Added">Movies added to the playlist.</param>
/// <param name="AlreadyThere">Movies that were already in it.</param>
/// <param name="NotOnServer">IMDb ids the server doesn't have.</param>
/// <param name="Created">The playlist didn't exist and was created.</param>
public sealed record PlaylistSyncResult(int Added, int AlreadyThere, IReadOnlyList<string> NotOnServer, bool Created);

/// <summary>A title in a media server playlist; <paramref name="Watched"/> when the server's user has seen it.</summary>
public sealed record PlaylistEntry(string? ImdbId, string Title, string? Year, string? Type, bool Watched);

/// <summary>
/// Video playlists on the media server (Plex). Port of PlexController's playlist methods.
/// </summary>
public interface IMediaServerPlaylists
{
    bool IsConfigured { get; }

    Task<IReadOnlyList<string>> GetPlaylistNamesAsync(CancellationToken ct = default);

    /// <summary>Adds the movies to the playlist (creating it if needed), skipping ones already in it.</summary>
    Task<PlaylistSyncResult> AddToPlaylistAsync(string playlistName, IReadOnlyList<string> imdbIds, CancellationToken ct = default);

    /// <summary>The playlist's page in the server's own web app, or null when there is no playlist of that name.</summary>
    Task<string?> GetPlaylistWebPageAsync(string playlistName, CancellationToken ct = default);

    /// <summary>The titles in the playlist, in its order; null when there is no playlist of that name.</summary>
    Task<IReadOnlyList<PlaylistEntry>?> GetPlaylistEntriesAsync(string playlistName, CancellationToken ct = default);

    /// <summary>False when the playlist or the movie in it isn't found.</summary>
    Task<bool> RemoveFromPlaylistAsync(string playlistName, string imdbId, CancellationToken ct = default);
}
