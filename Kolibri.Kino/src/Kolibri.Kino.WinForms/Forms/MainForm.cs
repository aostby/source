using Kolibri.Kino.WinForms.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kolibri.Kino.WinForms.Forms;

/// <summary>
/// Kino's MDI main window: the menus that open the other windows, which then stay inside its frame.
/// Opens <see cref="KinoForm"/> (search) at start.
/// </summary>
public partial class MainForm : Form, IStatusDisplay
{
    private readonly IServiceProvider _services;
    private readonly ILogger _log = KinoLog.Factory.CreateLogger<MainForm>();

    public MainForm(IServiceProvider services)
    {
        _services = services;
        InitializeComponent();
        AppIcon.Apply(this);
        // "Kolibri.Kino 1.0.0": <Product> and <Version> in the .csproj.
        Text = $"{Application.ProductName} {typeof(MainForm).Assembly.GetName().Version?.ToString(3)}";
    }

    /// <summary>
    /// Shows <paramref name="text"/> in the status bar at the bottom. Safe from any thread; ignored once closed.
    /// Other windows use <see cref="Mdi.ShowMainStatus"/>.
    /// </summary>
    public void SetStatus(string text)
    {
        if (IsDisposed || Disposing) return;
        if (InvokeRequired)
        {
            BeginInvoke(() => SetStatus(text));
            return;
        }
        lblStatus.Text = text;
    }

    /// <summary>The main window's own messages: shown and logged. (<see cref="SetStatus"/> only shows: other windows log their own.)</summary>
    private void Report(string text)
    {
        _log.Log(KinoLog.LevelOf(text), "{Status}", text);
        SetStatus(text);
    }

    private void MainForm_Shown(object sender, EventArgs e)
    {
        var kino = _services.GetRequiredService<Microsoft.Extensions.Options.IOptions<Kolibri.Kino.Core.KinoOptions>>().Value;
        _log.LogInformation("{Title} started; database {Database}", Text, kino.LiteDbPath);
        this.ShowSingle<KinoForm>(_services);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _log.LogInformation("{Title} closed", Text);
        base.OnFormClosed(e);
    }

    #region File

    private void menuSettings_Click(object sender, EventArgs e)
    {
        using var form = ActivatorUtilities.CreateInstance<SettingsForm>(_services);
        if (form.ShowDialog(this) == DialogResult.OK) Report("Settings saved.");
        foreach (var kino in MdiChildren.OfType<KinoForm>()) kino.UpdateDefaultKeysBanner();
    }

    private void menuExit_Click(object sender, EventArgs e) => Close();

    #endregion

    #region Library

    private void menuSearch_Click(object sender, EventArgs e) => this.ShowSingle<KinoForm>(_services);

    private void menuLocalMovies_Click(object sender, EventArgs e) => this.ShowSingle<LocalMoviesForm>(_services);

    private void menuLocalSeries_Click(object sender, EventArgs e) => this.ShowSingle<LocalSeriesForm>(_services);

    private void menuWatchlists_Click(object sender, EventArgs e) => this.ShowSingle<WatchlistsForm>(_services);

    #endregion

    #region Window (as in SilverScreen; the open windows are listed below)

    private void menuCascade_Click(object sender, EventArgs e) => LayoutMdi(MdiLayout.Cascade);

    private void menuTileHorizontal_Click(object sender, EventArgs e) => LayoutMdi(MdiLayout.TileHorizontal);

    private void menuTileVertical_Click(object sender, EventArgs e) => LayoutMdi(MdiLayout.TileVertical);

    private void menuArrangeIcons_Click(object sender, EventArgs e) => LayoutMdi(MdiLayout.ArrangeIcons);

    /// <summary>Closes (not just disposes) each window, so a running call is cancelled first.</summary>
    private void menuCloseAll_Click(object sender, EventArgs e)
    {
        var count = MdiChildren.Length;
        foreach (var child in MdiChildren) child.Close();
        Report($"{count - MdiChildren.Length} window(s) closed.");
    }

    #endregion

    #region Help

    /// <summary>F1: the help, at the part about the active window.</summary>
    private void menuHelpContents_Click(object sender, EventArgs e) =>
        HelpForm.Show(this, (ActiveMdiChild as AsyncForm)?.HelpTopic);

    private void menuAbout_Click(object sender, EventArgs e)
    {
        using var about = ActivatorUtilities.CreateInstance<AboutForm>(_services);
        about.ShowDialog(this);
        if (about.ShowUsageReport) this.ShowSingle<UsageReportForm>(_services);
        if (about.ShowLog) this.ShowSingle<LogForm>(_services);
    }

    private void menuLog_Click(object sender, EventArgs e) => this.ShowSingle<LogForm>(_services);

    private void menuUsage_Click(object sender, EventArgs e) => this.ShowSingle<UsageReportForm>(_services);

    #endregion
}
