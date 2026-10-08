using Kolibri.Kino.Controllers.Settings;
using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.WinForms.Forms;

/// <summary>
/// Edit the user settings in the database (port of SilverScreen's "Innstillinger" property grid):
/// API keys, Plex server and token, folders. DialogResult.OK when saved.
/// </summary>
public partial class SettingsForm : AsyncForm
{
    public override string HelpTopic => "keys";

    private readonly SettingsController _controller;
    private readonly UserSettings _edited;
    private readonly string? _current;

    public SettingsForm(SettingsController controller)
    {
        _controller = controller;
        _edited = controller.GetEditableCopy();
        _current = _edited.LiteDBFilePath;
        InitializeComponent();
        grid.SelectedObject = _edited;
        txtResults.Text = _edited.UsesDefaultKeys()
            ? SettingsController.DefaultKeysMessage
            : "Change a value, then Test connections to try the keys and the Plex token before saving.";
        if (_edited.UsesDefaultKeys()) SelectOmdbKey();
    }

    /// <summary>Puts the cursor on the OMDb key, the one to replace first.</summary>
    private void SelectOmdbKey()
    {
        var root = grid.SelectedGridItem;
        while (root?.Parent is not null) root = root.Parent;
        var key = root?.GridItems.Cast<GridItem>().SelectMany(c => c.GridItems.Cast<GridItem>())
            .FirstOrDefault(i => i.PropertyDescriptor?.Name == nameof(UserSettings.OMDBkey));
        if (key is not null) grid.SelectedGridItem = key;
    }

    private async void btnTest_Click(object sender, EventArgs e) =>
        await RunAsync("Testing…", async ct =>
        {
            txtResults.Text = "Testing OMDb, TMDb and Plex…";
            var checks = await _controller.TestAsync(_edited, ct);
            txtResults.Text = string.Join(Environment.NewLine, checks.Select(c => $"{(c.Ok ? "OK  " : "FAIL")}  {c.Service}: {c.Message}"));
        });

    private async void btnSave_Click(object sender, EventArgs e)
    {
        if (!ConfirmDatabaseChoice(out var newDatabase)) return;
        var restart = false;

        await RunAsync("Saving…", async ct =>
        {
            // Saved in the current database too, so it points on to the new one (SilverScreen looks there).
            await _controller.SaveAsync(_edited, ct);
            if (newDatabase is not null)
            {
                DatabaseLocation.SaveUserChoice(newDatabase);
                restart = MessageBox.Show(this, $"Kino opens {newDatabase} after a restart. Restart now?", "Database",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
            }
            DialogResult = DialogResult.OK;
            Close();
        });

        if (restart) Application.Restart();
    }

    /// <summary>
    /// A changed "LiteDB file": its folder must exist; without a database there, the user agrees to a new, empty one.
    /// False to stay in the window. <paramref name="newDatabase"/> is the full file path, or null when unchanged.
    /// </summary>
    private bool ConfirmDatabaseChoice(out string? newDatabase)
    {
        newDatabase = null;
        DatabaseChoice? choice;
        try
        {
            choice = SettingsController.CheckDatabasePath(_edited.LiteDBFilePath, _current);
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException or PathTooLongException)
        {
            MessageBox.Show(this, $"\"{_edited.LiteDBFilePath}\" is not a valid path: {ex.Message}", "Database",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (choice is null)
        {
            _edited.LiteDBFilePath = _current;
            return true;
        }

        if (!choice.FolderExists)
        {
            MessageBox.Show(this, $"The folder {Path.GetDirectoryName(choice.Path)} doesn't exist.", "Database",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (!choice.FileExists && MessageBox.Show(this,
                $"There is no database in {Path.GetDirectoryName(choice.Path)}.{Environment.NewLine}" +
                $"Make a new, empty one there ({Path.GetFileName(choice.Path)})?",
                "Database", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
            return false;

        _edited.LiteDBFilePath = newDatabase = choice.Path;
        grid.Refresh();
        return true;
    }

    protected override void OnBusyChanged(bool busy)
    {
        base.OnBusyChanged(busy);
        grid.Enabled = btnTest.Enabled = btnSave.Enabled = !busy;
    }

    protected override void DisplayStatus(string text)
    {
        if (text.StartsWith("Error", StringComparison.Ordinal)) txtResults.Text = text;
    }
}
