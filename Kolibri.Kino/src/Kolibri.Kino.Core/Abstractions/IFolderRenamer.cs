namespace Kolibri.Kino.Core.Abstractions;

/// <summary>Renames folders on disk (e.g. to take a wrong {imdb-tt…} tag out of a folder name).</summary>
public interface IFolderRenamer
{
    /// <summary>Renames <paramref name="folder"/> to <paramref name="newName"/> in the same parent folder; returns the new path.</summary>
    /// <exception cref="IOException">A folder with that name already exists, or the folder is in use.</exception>
    Task<string> RenameAsync(string folder, string newName, CancellationToken ct = default);
}
