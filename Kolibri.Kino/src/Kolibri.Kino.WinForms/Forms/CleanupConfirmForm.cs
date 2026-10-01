using Kolibri.Kino.Controllers.Cleanup;

namespace Kolibri.Kino.WinForms.Forms;

/// <summary>
/// Lists the leftover files a cleanup would delete. DialogResult.OK means "delete them".
/// </summary>
public partial class CleanupConfirmForm : Form
{
    public CleanupConfirmForm(CleanupPlan plan, string rootFolder, bool deleteEmptyFolders)
    {
        InitializeComponent();
        Kolibri.Kino.WinForms.Controls.AppIcon.Apply(this);

        lblInfo.Text =
            $"{plan.Files.Count} file(s) in {rootFolder} match the cleanup patterns (Kino:Cleanup:FilePatterns in appsettings.json)." +
            $"{Environment.NewLine}They are deleted permanently, not moved to the Recycle Bin." +
            (deleteEmptyFolders ? " Folders left empty are removed too." : string.Empty);
        btnDelete.Text = $"Delete {plan.Files.Count} file(s)";

        lstFiles.BeginUpdate();
        foreach (var file in plan.Files)
            lstFiles.Items.Add(Path.GetRelativePath(rootFolder, file));
        lstFiles.EndUpdate();
    }
}
