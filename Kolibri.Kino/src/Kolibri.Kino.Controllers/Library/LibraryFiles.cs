using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Microsoft.Extensions.Options;

namespace Kolibri.Kino.Controllers.Library;

/// <summary>
/// Whether library titles have a file: linked to one (a series: its folder) that's there. "In SilverScreen" in green
/// means this; a title in the library without one (e.g. imported from OMDb) is shown in salmon.
/// </summary>
public sealed class LibraryFiles(IFileItemRepository files, IMediaFileScanner disk, IOptions<KinoOptions> options)
{
    /// <summary>
    /// Of <paramref name="imdbIds"/>, those linked to a file or folder that's there. The stored Windows path is mapped to
    /// where the share is here (the container's mounts, <see cref="KinoOptions.PathMappings"/>).
    /// </summary>
    public async Task<IReadOnlySet<string>> WithFileAsync(IEnumerable<string> imdbIds, CancellationToken ct = default)
    {
        var links = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase); // path here → IMDb id
        foreach (var id in imdbIds.Distinct(StringComparer.OrdinalIgnoreCase))
            if (await files.GetByImdbIdAsync(id, ct).ConfigureAwait(false) is { FullName.Length: > 0 } link)
                links.TryAdd(options.Value.MapPath(link.FullName), id);
        if (links.Count == 0) return new HashSet<string>();

        var there = (await disk.FindExistingAsync(links.Keys, ct).ConfigureAwait(false))
            .Concat(await disk.FindExistingFoldersAsync(links.Keys, ct).ConfigureAwait(false));
        return there.Select(path => links[path]).ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}
