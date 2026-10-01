using Kolibri.Kino.Controllers.Settings;
using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.WinForms.Forms;

/// <summary>
/// Edit the user settings in the database (port of SilverScreen's "Innstillinger" property grid):
/// API keys, Plex server and token, folders. DialogResult.OK when saved.
/// </summary>
public partial class SettingsForm : AsyncForm
{
    private readonly SettingsController _controller;
    private readonly UserSettings _edited;

    public SettingsForm(SettingsController controller)
    {
        _controller = controller;
        _edited = controller.GetEditableCopy();
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

    private async void btnSave_Click(object sender, EventArgs e) =>
        await RunAsync("Saving…", async ct =>
        {
            await _controller.SaveAsync(_edited, ct);
            DialogResult = DialogResult.OK;
            Close();
        });

    protected override void OnBusyChanged(bool busy)
    {
        base.OnBusyChanged(busy);
        grid.Enabled = btnTest.Enabled = btnSave.Enabled = !busy;
    }

    protected override void ShowStatus(string text)
    {
        if (text.StartsWith("Error", StringComparison.Ordinal)) txtResults.Text = text;
    }
}
