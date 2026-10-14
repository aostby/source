using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Kolibri.Kino.WinForms.Controls;

/// <summary>"Write to Excel…": saves CSV (see Controllers.Csv) where the user chooses, then opens it in Excel.</summary>
internal static class ExcelExport
{
    /// <summary>
    /// Asks where to save, suggesting "&lt;parts joined by ' - '&gt; - &lt;date&gt;.csv" in Documents, writes the file and opens it.
    /// The first part is the filter shown, so the file name says what's in it. Returns the saved file, or null when
    /// cancelled or it failed (the user has been told).
    /// </summary>
    public static string? Save(IWin32Window owner, string csv, params string[] nameParts)
    {
        var parts = nameParts.Where(p => !string.IsNullOrWhiteSpace(p)).Append($"{DateTime.Now:yyyy-MM-dd}");
        var name = string.Join("_", $"{string.Join(" - ", parts)}.csv".Split(Path.GetInvalidFileNameChars()));
        using var dialog = new SaveFileDialog
        {
            Title = "Write to Excel",
            Filter = "CSV (Excel)|*.csv",
            FileName = name,
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        };
        if (dialog.ShowDialog(owner) != DialogResult.OK) return null;

        try
        {
            File.WriteAllText(dialog.FileName, csv, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
            Process.Start(new ProcessStartInfo(dialog.FileName) { UseShellExecute = true });
            return dialog.FileName;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or Win32Exception)
        {
            MessageBox.Show(owner, ex.Message, "Write to Excel", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return null;
        }
    }

    /// <summary>The last folder name of a path, e.g. "Movies" for \\GREENLANTERN\Movies\.</summary>
    public static string FolderName(string path) => Path.GetFileName(Path.TrimEndingDirectorySeparator(path));
}
