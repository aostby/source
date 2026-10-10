using Kolibri.Kino.Controllers;
using Kolibri.Kino.WinForms.Controls;

namespace Kolibri.Kino.WinForms.Forms;

/// <summary>
/// Sets a library entry's IMDb rating by hand, for titles OMDb and TMDb gave none. "Open on IMDb" shows the page
/// to read it from. OK with <see cref="Rating"/> = the new rating, or null to clear it; Cancel changes nothing.
/// </summary>
public sealed class ImdbRatingDialog : Form
{
    private readonly TextBox _txtRating = new() { Width = 70 };
    private readonly Button _btnOk = new() { Text = "Save", DialogResult = DialogResult.OK, AutoSize = true };
    private readonly Label _lblError = new() { AutoSize = true, ForeColor = Color.Firebrick };

    public ImdbRatingDialog(string imdbId, string title, string? current)
    {
        Text = "Set IMDb rating";
        AppIcon.Apply(this);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = MinimizeBox = false;
        ShowInTaskbar = false;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        Padding = new Padding(12);

        var known = MovieController.ParseImdbRating(current);
        _txtRating.Text = known?.ToString("0.0") ?? string.Empty;

        var btnImdb = new Button { Text = "Open on IMDb", AutoSize = true };
        SiteIcons.Imdb(btnImdb);
        btnImdb.Click += (_, _) => System.Diagnostics.Process.Start(
            new System.Diagnostics.ProcessStartInfo($"https://www.imdb.com/title/{imdbId}/") { UseShellExecute = true });
        var btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, AutoSize = true };

        var layout = new TableLayoutPanel { AutoSize = true, ColumnCount = 1, Dock = DockStyle.Fill };
        layout.Controls.Add(new Label { Text = $"{title} ({imdbId})", AutoSize = true, Font = new Font(Font, FontStyle.Bold), Margin = new Padding(3, 3, 3, 6) });
        layout.Controls.Add(new Label
        {
            Text = $"Current rating: {(known is null ? "none" : known.Value.ToString("0.0"))}. Look it up on IMDb, then type it here " +
                   $"(e.g. 7.3, or 7,3).{Environment.NewLine}Leave it empty to clear the rating.",
            AutoSize = true,
            MaximumSize = new Size(420, 0),
        });
        var row = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Margin = new Padding(0, 8, 0, 0) };
        row.Controls.Add(new Label { Text = "IMDb rating:", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(3, 7, 3, 3) });
        row.Controls.Add(_txtRating);
        row.Controls.Add(new Label { Text = "/ 10", AutoSize = true, Margin = new Padding(3, 7, 12, 3) });
        row.Controls.Add(btnImdb);
        layout.Controls.Add(row);
        layout.Controls.Add(_lblError);
        var buttons = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill, Margin = new Padding(0, 8, 0, 0) };
        buttons.Controls.Add(btnCancel);
        buttons.Controls.Add(_btnOk);
        layout.Controls.Add(buttons);
        Controls.Add(layout);

        AcceptButton = _btnOk;
        CancelButton = btnCancel;
        _btnOk.Click += (_, _) =>
        {
            // Empty clears; anything else must be a rating from 0 to 10.
            if (string.IsNullOrWhiteSpace(_txtRating.Text) || MovieController.ParseImdbRating(_txtRating.Text) is not null) return;
            _lblError.Text = "Type a rating from 0 to 10, e.g. 7.3.";
            DialogResult = DialogResult.None;
            _txtRating.Focus();
            _txtRating.SelectAll();
        };
        Shown += (_, _) => { _txtRating.Focus(); _txtRating.SelectAll(); };
    }

    /// <summary>The rating typed (OK), or null when the box was left empty (clear it).</summary>
    public double? Rating => MovieController.ParseImdbRating(_txtRating.Text);
}
