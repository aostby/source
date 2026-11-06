using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.Data.Storage;
using LiteDB;

namespace Kolibri.Kino.Data;

/// <summary>
/// The "FileItem" collection: which file holds which title (_id = ImdbId), as SilverScreen stores it.
/// Through <see cref="KinoDatabase.Collection"/>: the LiteDB file, or MariaDB through KMDb when that is in use.
/// </summary>
public sealed class LiteDbFileItemRepository(KinoDatabase db) : IFileItemRepository
{
    private readonly IDocumentCollection _files = db.Collection("FileItem");

    public async Task<IReadOnlyList<FileItem>> GetAllAsync(CancellationToken ct = default) =>
        (await _files.FindAllAsync(ct).ConfigureAwait(false)).Select(ToFile).Where(f => !string.IsNullOrEmpty(f.FullName)).ToList();

    public async Task<FileItem?> GetByImdbIdAsync(string imdbId, CancellationToken ct = default) =>
        await _files.FindByIdAsync(imdbId, ct).ConfigureAwait(false) is { } doc ? ToFile(doc) : null;

    public async Task<IReadOnlyList<FileItem>> GetInFolderAsync(string folder, CancellationToken ct = default)
    {
        var prefix = Path.TrimEndingDirectorySeparator(Path.GetFullPath(folder)) + Path.DirectorySeparatorChar;
        // Filtered in memory: Windows paths are not case-sensitive.
        return (await GetAllAsync(ct).ConfigureAwait(false))
            .Where(f => f.FullName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public Task UpsertAsync(FileItem file, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(file.ImdbId);
        var doc = BsonMapper.Global.ToDocument(file);
        doc["_id"] = file.ImdbId;
        return _files.UpsertAsync(doc, ct);
    }

    public Task<bool> DeleteAsync(string imdbId, CancellationToken ct = default) => _files.DeleteAsync(imdbId, ct);

    private static FileItem ToFile(BsonDocument doc) => BsonMapper.Global.ToObject<FileItem>(doc);
}
