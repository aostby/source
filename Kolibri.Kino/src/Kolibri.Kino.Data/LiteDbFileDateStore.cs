using Kolibri.Kino.Data.Storage;
using LiteDB;

namespace Kolibri.Kino.Data;

/// <summary>
/// A stored file date: when the path's file arrived (null if it couldn't be reached), whether it is a folder
/// (a series, whose date moves with new episodes) and when that was checked.
/// </summary>
public sealed record StoredFileDate(string Path, DateTime? AddedUtc, bool IsFolder, DateTime CheckedUtc);

/// <summary>
/// "KinoFileDate": _id = the path as stored in "FileItem", Added = UTC or null, Folder, Checked = UTC. Kino's own
/// collection, so SilverScreen ignores it. Keeps Recently added from walking the disks on every start.
/// </summary>
public sealed class LiteDbFileDateStore(KinoDatabase db)
{
    private readonly IDocumentCollection _dates = db.Collection("KinoFileDate");

    public async Task<IReadOnlyList<StoredFileDate>> LoadAllAsync(CancellationToken ct = default) =>
        (await _dates.FindAllAsync(ct).ConfigureAwait(false))
            .Select(d => new StoredFileDate(
                d["_id"].AsString,
                d["Added"].IsDateTime ? DateTime.SpecifyKind(d["Added"].AsDateTime.ToUniversalTime(), DateTimeKind.Utc) : null,
                d["Folder"].IsBoolean && d["Folder"].AsBoolean,
                DateTime.SpecifyKind(d["Checked"].AsDateTime.ToUniversalTime(), DateTimeKind.Utc)))
            .ToList();

    public async Task SaveAsync(IEnumerable<StoredFileDate> dates, CancellationToken ct = default)
    {
        var docs = dates.Select(d => new BsonDocument
        {
            ["_id"] = d.Path,
            ["Added"] = d.AddedUtc is { } added ? new BsonValue(added) : BsonValue.Null,
            ["Folder"] = d.IsFolder,
            ["Checked"] = d.CheckedUtc,
        }).ToList();
        if (docs.Count > 0) await _dates.UpsertManyAsync(docs, ct).ConfigureAwait(false);
    }
}
