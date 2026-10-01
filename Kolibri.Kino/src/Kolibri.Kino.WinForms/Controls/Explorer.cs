using System.Diagnostics;

namespace Kolibri.Kino.WinForms.Controls;

/// <summary>
/// Shows paths in Windows Explorer (SilverScreen's FileUtilities.OpenFolderHighlightFile / OpenFolderInExplorer).
/// </summary>
internal static class Explorer
{
    /// <summary>
    /// A file: its folder with the file selected. A folder: that folder. A file that is gone: the folder it was in.
    /// Returns false when none of these exist.
    /// </summary>
    public static bool Show(string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) return false;

        if (File.Exists(path))
            Process.Start("explorer.exe", $"/select,\"{path}\"");
        else if (Directory.Exists(path))
            Process.Start("explorer.exe", $"\"{path}\"");
        else if (Path.GetDirectoryName(path) is { } folder && Directory.Exists(folder))
            Process.Start("explorer.exe", $"\"{folder}\"");
        else
            return false;
        return true;
    }
}
