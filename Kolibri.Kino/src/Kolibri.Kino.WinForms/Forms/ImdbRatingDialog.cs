using Kolibri.Kino.Controllers;
using Kolibri.Kino.WinForms.Controls;

namespace Kolibri.Kino.WinForms.Forms;

/// <summary>
/// Sets a library entry's IMDb rating by hand, for titles OMDb and TMDb gave none. "Open on IMDb" and "Open on
/// TMDb" show the pages to read it from; "Get from TMDb" fills in TMDb's own score when IMDb has none either.
/// OK with <see cref="Rating"/> = the new rating, or null to clear it; Cancel changes nothing.
/// </summary>
public sealed class ImdbRatingDialog : Form
{
    private readonly MovieController _movies;
    private readonly string _imdbId;
    private readonly string _title;
    private readonly TextBox _txtRating = new() { Width = 70 };
    private readonly Button _btnOk = new() { Text = "Save", DialogResult = DialogResult.OK, AutoSize = true };
    private readonly Button _btnTmdbPage = new() { Text = "Open on TMDb", AutoSize = true };
    private readonly Button _btnTmdbScore = new() { Text = "Get from TMDb", AutoSize = true };
    private readonly Label _lblNote = new() { AutoSize = true, MaximumSize = new Size(460, 0) };
    private readonly CancellationTokenSource _cts = new();

    public ImdbRatingDialog(MovieController movies, string imdbId, string title, string? year, string? current)
    {
        (_movies, _imdbId, _title) = (movies, imdbId, title);
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
        btnImdb.Click += (_, _) => Open($"https://www.imdb.com/title/{imdbId}/");
        SiteIcons.Tmdb(_btnTmdbPage);
        _btnTmdbPage.Click += OpenTmdbPage;
        SiteIcons.Tmdb(_btnTmdbScore);
        _btnTmdbScore.Click += GetTmdbScore;
        var btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, AutoSize = true };

        var layout = new TableLayoutPanel { AutoSize = true, ColumnCount = 1, Dock = DockStyle.Fill };
        var heading = string.IsNullOrWhiteSpace(year) || year == "N/A" ? title : $"{title} ({year})";
        layout.Controls.Add(new Label { Text = $"{heading} ({imdbId})", AutoSize = true, Font = new Font(Font, FontStyle.Bold), Margin = new Padding(3, 3, 3, 6) });
        layout.Controls.Add(new Label
        {
            Text = $"Current rating: {(known is null ? "none" : known.Value.ToString("0.0"))}. Look it up on IMDb, then type it here " +
                   $"(e.g. 7.3, or 7,3). If IMDb has none, Get from TMDb fills in TMDb's score.{Environment.NewLine}" +
                   "Leave it empty to clear the rating.",
            AutoSize = true,
            MaximumSize = new Size(460, 0),
        });
        var row = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Margin = new Padding(0, 8, 0, 0) };
        row.Controls.Add(new Label { Text = "IMDb rating:", AutoSize = true, Margin = new Padding(3, 7, 3, 3) });
        row.Controls.Add(_txtRating);
        row.Controls.Add(new Label { Text = "/ 10", AutoSize = true, Margin = new Padding(3, 7, 12, 3) });
        layout.Controls.Add(row);
        var lookups = new FlowLayoutPanel { AutoSize = true, WrapContents = true, Margin = new Padding(0, 4, 0, 0) };
        lookups.Controls.Add(btnImdb);
        lookups.Controls.Add(_btnTmdbPage);
        lookups.Controls.Add(_btnTmdbScore);
        layout.Controls.Add(lookups);
        layout.Controls.Add(_lblNote);
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
            Note("Type a rating from 0 to 10, e.g. 7.3.", error: true);
            DialogResult = DialogResult.None;
            _txtRating.Focus();
            _txtRating.SelectAll();
        };
        Shown += (_, _) => { _txtRating.Focus(); _txtRating.SelectAll(); };
        FormClosed += (_, _) => { _cts.Cancel(); _cts.Dispose(); };
    }

    /// <summary>The rating typed (OK), or null when the box was left empty (clear it).</summary>
    public double? Rating => MovieController.ParseImdbRating(_txtRating.Text);

    /// <summary>TMDb pages use TMDb's own ids: asks TMDb for it (one request), else opens a TMDb search for the title.</summary>
    private async void OpenTmdbPage(object? sender, EventArgs e)
    {
        _btnTmdbPage.Enabled = false;
        try
        {
            Open(await _movies.GetTmdbPageAsync(_imdbId, _title, _cts.Token));
        }
        catch (OperationCanceledException)
        {
            return; // the dialog was closed
        }
        if (!IsDisposed) _btnTmdbPage.Enabled = true;
    }

    private async void GetTmdbScore(object? sender, EventArgs e)
    {
        _btnTmdbScore.Enabled = false;
        Note("Asking TMDb…");
        (double Rating, int Votes)? score;
        try
        {
            score = await _movies.GetTmdbRatingAsync(_imdbId, _cts.Token);
        }
        catch (OperationCanceledException)
        {
            return;
        }
        if (IsDisposed) return;

        _btnTmdbScore.Enabled = true;
        if (score is not { } s)
        {
            Note("TMDb has no score for this title (or can't be reached; is the TMDb key set in Settings?).", error: true);
            return;
        }
        _txtRating.Text = s.Rating.ToString("0.0");
        Note($"Filled in TMDb's score: {s.Rating:0.0} from {s.Votes:N0} vote(s). It is TMDb's, not IMDb's; Save keeps it as the rating.");
        _txtRating.Focus();
    }

    private void Note(string text, bool error = false)
    {
        _lblNote.Text = text;
        _lblNote.ForeColor = error ? Color.Firebrick : SystemColors.GrayText;
    }

    private static void Open(string url) =>
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
}
