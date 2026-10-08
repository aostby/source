using Kolibri.Kino.Core.Models;
using Kolibri.Kino.WinForms.Controls;

namespace Kolibri.Kino.WinForms.Forms;

/// <summary>
/// None of the downloaded subtitles is named for the movie's release: lists them all with their sizes, largest first
/// (often the right one) and selected. OK with <see cref="Chosen"/> = the one to copy next to the movie; Cancel changes nothing.
/// </summary>
public partial class SubtitleChoiceDialog : Form
{
    public SubtitleChoiceDialog(IReadOnlyList<SubtitleFile> files, string movieFile)
    {
        InitializeComponent();
        AppIcon.Apply(this);
        lblInfo.Text = $"None of the {files.Count} downloaded subtitle(s) is named for this release. The largest is often the right one." +
                       $"{Environment.NewLine}The one you choose is copied next to the movie as {Path.GetFileNameWithoutExtension(movieFile)}.srt.";
        foreach (var file in files)
        {
            lstFiles.Items.Add(new ListViewItem([FormatSize(file.Bytes), Path.GetFileName(file.Path)]) { Tag = file });
        }
        if (lstFiles.Items.Count > 0) lstFiles.Items[0].Selected = lstFiles.Items[0].Focused = true;
        colFile.Width = -2; // fill to the last column's text
    }

    /// <summary>The subtitle chosen (OK), or null.</summary>
    public SubtitleFile? Chosen => lstFiles.SelectedItems.Count == 1 ? lstFiles.SelectedItems[0].Tag as SubtitleFile : null;

    private void lstFiles_SelectedIndexChanged(object sender, EventArgs e) => btnUse.Enabled = Chosen is not null;

    private void lstFiles_DoubleClick(object sender, EventArgs e)
    {
        if (Chosen is null) return;
        DialogResult = DialogResult.OK;
    }

    private static string FormatSize(long bytes) => bytes >= 1024 * 1024 ? $"{bytes / 1024d / 1024:N1} MB" : $"{bytes / 1024d:N1} KB";
}
