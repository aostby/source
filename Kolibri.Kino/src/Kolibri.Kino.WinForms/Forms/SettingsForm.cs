using Kolibri.Kino.Controllers.Ratings;
using Kolibri.Kino.Controllers.Settings;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.WinForms.Controls;

namespace Kolibri.Kino.WinForms.Forms;

/// <summary>
/// Edit the user settings in the database (port of SilverScreen's "Innstillinger" property grid):
/// API keys, Plex server and token, folders. DialogResult.OK when saved.
/// </summary>
public partial class SettingsForm : AsyncForm
{
    public override string HelpTopic => "keys";

    private readonly SettingsController _controller;
    private readonly RatingController _ratings;
    private readonly UserSettings _edited;
    private readonly string? _current;

    public SettingsForm(SettingsController controller, RatingController ratings)
    {
        _controller = controller;
        _ratings = ratings;
        _edited = controller.GetEditableCopy();
        _current = _edited.LiteDBFilePath;
        InitializeComponent();
        this.Describe(
            (btnTest, "Tries the OMDb, TMDb and SubDL keys, the Plex token, Uranus and Prowlarr as entered, without saving. " +
                      "Says which settings are missing."),
            (btnSave, "Saves the settings. New keys apply at once; a new database after a restart."),
            (btnImportRatings, "Takes your ratings from a file exported in Kino on the web (Settings → Export ratings). " +
                               "The newer rating of a title wins; the new ones are set on Plex too."));
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
            txtResults.Text = "Testing OMDb, TMDb, SubDL, Plex, Uranus and Prowlarr…";
            ShowChecks(await _controller.TestAsync(_edited, ct));
        });

    /// <summary>
    /// A line on what's missing in Settings and one on what failed, then one line per service: OK (green), FAIL (red),
    /// or MISSING (orange; not tried, the settings it needs aren't filled in).
    /// </summary>
    private void ShowChecks(IReadOnlyList<Kolibri.Kino.Core.Abstractions.ConnectionCheck> checks)
    {
        txtResults.Clear();
        var bold = new Font(txtResults.Font, FontStyle.Bold);
        void Write(string text, Color color, Font? font = null)
        {
            txtResults.SelectionStart = txtResults.TextLength;
            txtResults.SelectionColor = color;
            txtResults.SelectionFont = font ?? txtResults.Font;
            txtResults.AppendText(text);
        }

        var missing = checks.Where(c => c.Missing is not null).ToList();
        var failed = checks.Where(c => c.Missing is null && !c.Ok).ToList();
        if (missing.Count > 0)
            Write($"Missing settings: {string.Join(", ", missing.Select(c => $"{c.Service} ({c.Missing})"))}.{Environment.NewLine}", Missing, bold);
        if (failed.Count > 0)
            Write($"Failed: {string.Join(", ", failed.Select(c => c.Service))}.{Environment.NewLine}", Color.Firebrick, bold);
        if (missing.Count + failed.Count == 0)
            Write($"All connections work.{Environment.NewLine}", Color.ForestGreen, bold);
        Write(Environment.NewLine, txtResults.ForeColor);

        foreach (var c in checks)
        {
            var (label, color) = c.Missing is not null ? ("MISSING", Missing) : c.Ok ? ("OK", Color.ForestGreen) : ("FAIL", Color.Firebrick);
            Write($"{label,-8}", color, bold);
            Write($"{c.Service}: {c.Message}{Environment.NewLine}", txtResults.ForeColor);
        }
        txtResults.SelectionStart = 0;
        txtResults.ScrollToCaret();
    }

    /// <summary>Dark orange, readable on the box's grey: settings not filled in.</summary>
    private static readonly Color Missing = Color.FromArgb(200, 100, 0);

    /// <summary>Ratings exported from Kino on the web (the NAS's database), into this database.</summary>
    private async void btnImportRatings_Click(object sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Importer vurderinger fra fil",
            Filter = "Kino ratings (*.json)|*.json|All files (*.*)|*.*",
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        await RunAsync("Importing ratings…", async ct =>
        {
            var r = await _ratings.ImportAsync(await File.ReadAllTextAsync(dialog.FileName, ct), ct);
            var text = $"Ratings from {Path.GetFileName(dialog.FileName)}: {r.Added} new, {r.Updated} updated, " +
                       $"{r.KeptNewer} kept (newer here)" + (r.Invalid > 0 ? $", {r.Invalid} invalid left out" : "") + "." +
                       (_ratings.PlexConfigured ? $" Plex: {r.SentToPlex} set." + (r.PlexNote is { } note ? $" {note}" : "") : "");
            txtResults.Text = text;
            MessageBox.Show(this, text, "Importer vurderinger fra fil", MessageBoxButtons.OK, MessageBoxIcon.Information);
        });
    }

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
        grid.Enabled = btnTest.Enabled = btnSave.Enabled = btnImportRatings.Enabled = !busy;
    }

    protected override void DisplayStatus(string text)
    {
        if (text.StartsWith("Error", StringComparison.Ordinal)) txtResults.Text = text;
    }
}
