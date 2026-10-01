using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using LiteDB;

namespace Kolibri.Kino.Data;

/// <summary>
/// The "FileItem" collection (_id = ImdbId), shared with SilverScreen.
/// </summary>
public sealed class LiteDbFileItemRepository(KinoDatabase db) : IFileItemRepository
{
    private readonly ILiteCollection<FileItem> _files = db.Database.GetCollection<FileItem>("FileItem");

    public Task<IReadOnlyList<FileItem>> GetAllAsync(CancellationToken ct = default) =>
        Task.Run(() => (IReadOnlyList<FileItem>)_files.FindAll().Where(f => !string.IsNullOrEmpty(f.FullName)).ToList(), ct);

    public Task<FileItem?> GetByImdbIdAsync(string imdbId, CancellationToken ct = default) =>
        Task.Run(() => (FileItem?)_files.FindById(imdbId), ct);

    public Task<IReadOnlyList<FileItem>> GetInFolderAsync(string folder, CancellationToken ct = default)
    {
        var prefix = Path.TrimEndingDirectorySeparator(Path.GetFullPath(folder)) + Path.DirectorySeparatorChar;

        // Filtered in memory: LiteDB's StartsWith is case-sensitive and Windows paths are not.
        return Task.Run(() => (IReadOnlyList<FileItem>)_files.FindAll()
            .Where(f => !string.IsNullOrEmpty(f.FullName) && f.FullName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .ToList(), ct);
    }

    public Task UpsertAsync(FileItem file, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(file.ImdbId);
        return Task.Run(() => _files.Upsert(new BsonValue(file.ImdbId), file), ct);
    }

    public Task<bool> DeleteAsync(string imdbId, CancellationToken ct = default) =>
        Task.Run(() => _files.Delete(imdbId), ct);
}
