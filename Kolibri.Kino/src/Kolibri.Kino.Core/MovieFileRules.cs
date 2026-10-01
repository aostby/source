namespace Kolibri.Kino.Core;

/// <summary>
/// Which files count as movies. Mirrors MovieUtilites.MoviesCommonFileExt and MulitpartFilter.
/// </summary>
public static class MovieFileRules
{
    public static readonly IReadOnlySet<string> VideoExtensions =
        new HashSet<string>([".avi", ".mkv", ".mp4", ".mpg", ".mpeg", ".ts", ".divx"], StringComparer.OrdinalIgnoreCase);

    /// <summary>Markers for multipart / extra files that are never matched on their own.</summary>
    private static readonly string[] MultipartMarkers = ["CD", ".PART", " PART", "Disk0", "Extra"];

    /// <summary>Synology/QNAP thumbnail folders; never real movies.</summary>
    private const string ThumbnailFolderMarker = "@__thumb";

    public static bool IsVideoFile(string path) =>
        VideoExtensions.Contains(Path.GetExtension(path))
        && !path.Contains(ThumbnailFolderMarker, StringComparison.OrdinalIgnoreCase);

    /// <remarks>
    /// Case-insensitive like the original, so "CD" also matches words such as "Decoded".
    /// Kept as-is to show the same result as SilverScreen.
    /// </remarks>
    public static bool IsMultipart(string path) =>
        MultipartMarkers.Any(m => path.Contains(m, StringComparison.OrdinalIgnoreCase));
}
