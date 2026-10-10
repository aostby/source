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
    private readonly ILiteCollection<BsonDocument> _dates = db.Database.GetCollection("KinoFileDate");

    public IReadOnlyList<StoredFileDate> LoadAll() =>
        _dates.FindAll()
            .Select(d => new StoredFileDate(
                d["_id"].AsString,
                d["Added"].IsDateTime ? DateTime.SpecifyKind(d["Added"].AsDateTime.ToUniversalTime(), DateTimeKind.Utc) : null,
                d["Folder"].IsBoolean && d["Folder"].AsBoolean,
                DateTime.SpecifyKind(d["Checked"].AsDateTime.ToUniversalTime(), DateTimeKind.Utc)))
            .ToList();

    public void Save(IEnumerable<StoredFileDate> dates)
    {
        var docs = dates.Select(d => new BsonDocument
        {
            ["_id"] = d.Path,
            ["Added"] = d.AddedUtc is { } added ? new BsonValue(added) : BsonValue.Null,
            ["Folder"] = d.IsFolder,
            ["Checked"] = d.CheckedUtc,
        }).ToList();
        if (docs.Count > 0) _dates.Upsert(docs);
    }
}
