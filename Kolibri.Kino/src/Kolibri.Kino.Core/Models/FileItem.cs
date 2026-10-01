namespace Kolibri.Kino.Core.Models;

/// <summary>
/// Links an IMDb id to a video file on disk. Stored in the "FileItem" collection with _id = ImdbId,
/// the same layout as Kolibri.net.Common.Dal.Entities.FileItem.
/// </summary>
public sealed class FileItem
{
    public string ImdbId { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;
}
