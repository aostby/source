using System.Runtime.InteropServices;
using Kolibri.Kino.Core;
using Kolibri.Kino.WinForms.Controls;
using Microsoft.Extensions.Options;

namespace Kolibri.Kino.WinForms.Forms;

/// <summary>Help → About: name, version, and which database and settings file this run uses.</summary>
public partial class AboutForm : Form
{
    public AboutForm(IOptions<KinoOptions> options)
    {
        InitializeComponent();
        AppIcon.Apply(this);
        if (AppIcon.Icon is { } icon) picIcon.Image = new Icon(icon, 64, 64).ToBitmap();

        var version = typeof(AboutForm).Assembly.GetName().Version?.ToString(3);
        Text = $"About {Application.ProductName}";
        lblProduct.Text = Application.ProductName;
        lblVersion.Text = $"Version {version}";

        lnkTmdb.LinkArea = new LinkArea(lnkTmdb.Text.IndexOf("TMDb", StringComparison.Ordinal), "TMDb".Length);

        var kino = options.Value;
        txtInfo.Text = string.Join(Environment.NewLine,
            $"Database:      {kino.LiteDbPath}",
            $"Image cache:   {kino.ResolveImageDbPath()}",
            $"Log:           {kino.ResolveLogDbPath()} (kept {kino.LogRetentionDays} days)",
            $"User settings: {(File.Exists(DatabaseLocation.UserFile) ? DatabaseLocation.UserFile : "none (appsettings.json applies)")}",
            $"Windows user:  {kino.ResolveSettingsUser()}",
            $"Runtime:       {RuntimeInformation.FrameworkDescription}, {RuntimeInformation.OSDescription}");
    }

    /// <summary>The usage report is a window in the main window, so About closes and the main window opens it.</summary>
    public bool ShowUsageReport { get; private set; }

    private void btnUsage_Click(object sender, EventArgs e)
    {
        ShowUsageReport = true;
        DialogResult = DialogResult.OK;
    }

    /// <summary>Like <see cref="ShowUsageReport"/>, for the log.</summary>
    public bool ShowLog { get; private set; }

    private void btnLog_Click(object sender, EventArgs e)
    {
        ShowLog = true;
        DialogResult = DialogResult.OK;
    }

    private void lnkTmdb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) =>
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://www.themoviedb.org/") { UseShellExecute = true });

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            picIcon.Image?.Dispose();
            components?.Dispose();
        }
        base.Dispose(disposing);
    }
}
