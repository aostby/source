using System.Diagnostics;
using Kolibri.Kino.Controllers;
using Kolibri.Kino.Controllers.Ratings;
using Kolibri.Kino.Controllers.Subtitles;
using Kolibri.Kino.Controllers.Watchlists;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.Core;
using Kolibri.Kino.WinForms.Controls;
using Microsoft.Extensions.DependencyInjection;
using OMDbApiNet.Model;

namespace Kolibri.Kino.WinForms.Forms;

/// <summary>
/// All details of one title (movie, series, episode, game): poster, the IMDb rating as a coloured badge
/// (red below 6, orange 6–6.9, green from 7), plot, people, other ratings, and the file on disk.
/// Opened by double-clicking a row in the Kolibri.Kino window.
/// </summary>
public partial class MovieDetailsForm : AsyncForm
{
    public override string HelpTopic => "search";

    private readonly MovieController _movies;
    private readonly SubtitleController _subtitles;
    private readonly WatchlistController _watchlists;
    private readonly RatingController _ratings;
    private readonly ToolTip _toolTip = new();

    /// <summary>"My rating": stars in halves after the IMDb badge and Watched, as on the web page; a click saves.</summary>
    private readonly StarRating myRating = new() { Margin = new Padding(0, 4, 6, 0), Enabled = false };
    private readonly Label lblMyRating = new() { AutoSize = true, ForeColor = SystemColors.GrayText, Margin = new Padding(0, 9, 8, 0), Text = "My rating" };
    private readonly LinkLabel lnkReview = new() { AutoSize = true, Margin = new Padding(0, 9, 10, 0), Text = "Review…", Enabled = false };
    private MovieRating? _myRating;

    /// <summary>Similar movies (TMDb's recommendations), under the facts; shown when there are some.</summary>
    private readonly Label lblSimilar = new()
    {
        AutoSize = true, Visible = false, Text = "Similar movies",
        Font = new Font("Segoe UI", 11F, FontStyle.Bold), Margin = new Padding(0, 16, 0, 2),
    };
    private readonly FlowLayoutPanel similarRow = new() { AutoSize = true, Visible = false, Margin = new Padding(0) };
    private readonly IServiceProvider _services;
    private readonly string _imdbId;
    private Item? _item;
    private string? _path;
    private bool _watched;

    public MovieDetailsForm(MovieController movies, SubtitleController subtitles, WatchlistController watchlists, RatingController ratings,
        IServiceProvider services, string imdbId)
    {
        _movies = movies;
        _subtitles = subtitles;
        _watchlists = watchlists;
        _ratings = ratings;
        _services = services;
        _imdbId = imdbId;
        InitializeComponent();
        SiteIcons.Imdb(btnImdb);
        SiteIcons.Tmdb(btnTmdb);
        SiteIcons.SubDl(btnSubtitles);
        SiteIcons.Play(btnPlay);
        SiteIcons.Folder(btnOpenOnDisk);
        SiteIcons.Watchlist(btnWatchlist);
        SiteIcons.Close(btnClose);
        CopyOnDoubleClick.Enable(lblTitle, "Title", () => _item?.Title);
        CopyOnDoubleClick.Enable(lblRating, "IMDb rating", () => Clean(_item?.ImdbRating));
        CopyOnDoubleClick.Enable(lblVotes, "IMDb votes", () => Clean(_item?.ImdbVotes));
        CopyOnDoubleClick.Enable(lblHeadline);
        CopyOnDoubleClick.Enable(lblGenre, "Genre");
        CopyOnDoubleClick.Enable(lblPlot, "Plot");
        this.Describe(
            (btnPlay, "Plays the movie file in your default player."),
            (btnOpenOnDisk, "Shows the movie file in Explorer."),
            (btnImdb, "Opens the title's page on IMDb."),
            (btnTmdb, "Opens the title's page on TMDb (found by its IMDb id)."),
            (btnWatchlist, "Puts this title on a Kino watchlist and/or the Plex playlist of that name; the dialog says where each list is."),
            (btnMarkWatched, "Marks this title watched: it stays on its watchlist and shows on the Watched list (on no watchlist, it's added to Watched). Marked watched on Plex too when Plex has it."));
        Text = imdbId;
        layout.Controls.AddRange([lblSimilar, similarRow]);
        SetUpMyRating();
        _toolTip.SetToolTip(lblSimilar, "TMDb's suggestions. One you'd like to see or get: + Wishlist searches for it in the Wishlist window.");
    }

    private async void MovieDetailsForm_Load(object sender, EventArgs e) =>
        await RunAsync("Loading details…", async ct =>
        {
            _item = await _movies.GetDetailsAsync(_imdbId, ct);
            if (_item is null)
            {
                lblTitle.Text = _imdbId;
                ShowStatus($"No details found for {_imdbId} in the library or on OMDb.");
                return;
            }

            _path = await _movies.GetLinkedPathAsync(_imdbId, ct);
            _watched = await _watchlists.IsWatchedAsync(_imdbId, ct);
            _myRating = await _ratings.GetAsync(_imdbId, ct);
            ShowItem(_item);
            ShowMyRating();
            ShowStatus(_path is null ? "Not linked to a file." : File.Exists(_path) || Directory.Exists(_path) ? _path : $"{_path} (not found)");

            var poster = await _movies.GetPosterAsync(_item, ct);
            if (poster is not null)
            {
                using var stream = new MemoryStream(poster);
                using var image = Image.FromStream(stream);
                picPoster.Image = new Bitmap(image);
            }
            _ = ShowSimilarAsync();
        });

    /// <summary>After the details: movies like this one (not for a series), each one a small card.</summary>
    private async Task ShowSimilarAsync()
    {
        if (string.Equals(_item?.Type, "series", StringComparison.OrdinalIgnoreCase)) return;
        var similar = await _movies.GetSimilarAsync(_imdbId);
        if (IsGone || similar.Count == 0) return;

        similarRow.SuspendLayout();
        foreach (var s in similar) similarRow.Controls.Add(SimilarCard(s));
        similarRow.ResumeLayout();
        lblSimilar.Visible = similarRow.Visible = true;
    }

    /// <summary>Poster and title (both open its details), year, "In SilverScreen" when it is, and + Wishlist.</summary>
    private Control SimilarCard(SimilarMovie s)
    {
        const int width = 116;
        var card = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoSize = true, Margin = new Padding(0, 0, 12, 12),
        };
        var poster = new PictureBox
        {
            Size = new Size(width, 172), SizeMode = PictureBoxSizeMode.Zoom, BackColor = SystemColors.ControlLight,
            Cursor = Cursors.Hand, Margin = new Padding(0),
        };
        if (s.Movie.Poster is { } url) poster.LoadAsync(url); // TMDb's small poster, straight from TMDb
        var title = new LinkLabel { Text = s.Movie.Title, AutoSize = true, MaximumSize = new Size(width, 0), Margin = new Padding(0, 4, 0, 0) };
        var meta = new Label
        {
            AutoSize = true, MaximumSize = new Size(width, 0), Margin = new Padding(0, 2, 0, 4),
            // Green with a file that's there; salmon in the library without one (e.g. imported from OMDb).
            Text = s.HasFile ? $"{s.YearAndRating} · In SilverScreen" : s.InLibrary ? $"{s.YearAndRating} · In SilverScreen, no file" : s.YearAndRating,
            ForeColor = s.HasFile ? Color.SeaGreen : s.InLibrary ? TitleCards.NoFile : SystemColors.GrayText,
        };
        var add = new Button { Text = "+ Wishlist", AutoSize = true, Margin = new Padding(0), UseVisualStyleBackColor = true };

        void OpenDetails() => ActivatorUtilities.CreateInstance<MovieDetailsForm>(_services, s.Movie.ImdbId).ShowFrom(this);
        poster.Click += (_, _) => OpenDetails();
        title.LinkClicked += (_, _) => OpenDetails();
        add.Click += (_, _) => this.ShowSingle<WishlistForm>(_services).SearchFor(s.WishlistQuery);
        _toolTip.SetToolTip(poster, $"{s.Movie.Title} ({s.Movie.Year}): details");
        _toolTip.SetToolTip(add, $"Searches for \"{s.WishlistQuery}\" in the Wishlist window, to check it and add it.");

        card.Controls.AddRange([poster, title, meta, add]);
        return card;
    }

    private void ShowItem(Item item)
    {
        Text = $"{item.Title} ({Clean(item.Year) ?? "?"})";
        lblTitle.Text = item.Title;

        // The IMDb rating badge.
        var rating = Clean(item.ImdbRating);
        lblRating.Text = rating is null ? "No IMDb rating" : $" IMDb {rating} ";
        lblRating.BackColor = RatingColors.For(rating) ?? SystemColors.ControlDark;
        lblVotes.Text = Clean(item.ImdbVotes) is { } votes ? $"{votes} votes" : string.Empty;

        lblHeadline.Text = Join(" · ",
            Clean(item.Year),
            Clean(item.Runtime),
            Clean(item.Rated),
            // A real Metascore is 0-100; SilverScreen's Plex import stored Plex's own id (e.g. 192326) there.
            int.TryParse(Clean(item.Metascore), out var meta) && meta is >= 0 and <= 100 ? $"Metascore {meta}" : null,
            Clean(item.Type));
        lblGenre.Text = Clean(item.Genre) ?? string.Empty;
        lblPlot.Text = Clean(item.Plot) ?? "No plot available.";

        BuildFacts(item);
        UpdateButtons();
    }

    /// <summary>Every other field that has a value, as "name: value" rows.</summary>
    private void BuildFacts(Item item)
    {
        facts.SuspendLayout();
        facts.Controls.Clear();
        facts.RowStyles.Clear();
        facts.RowCount = 0;

        var otherRatings = item.Ratings?
            .Where(r => r.Source != "Internet Movie Database" && Clean(r.Value) is not null)
            .Select(r => $"{r.Source} {r.Value}");

        AddFact("Director", item.Director);
        AddFact("Writer", item.Writer);
        AddFact("Actors", item.Actors);
        AddFact("Released", item.Released);
        AddFact("Language", item.Language);
        AddFact("Country", item.Country);
        AddFact("Awards", item.Awards);
        AddFact("Other ratings", otherRatings is null ? null : string.Join(" · ", otherRatings));
        AddFact("Seasons", item.TotalSeasons);
        AddFact("Box office", item.BoxOffice);
        AddFact("Production", item.Production);
        AddFact("DVD", item.Dvd);
        AddFact("Website", item.Website);
        AddFact("IMDb id", item.ImdbId);
        AddFact("File", _path is null ? null : File.Exists(_path) || Directory.Exists(_path) ? _path : $"{_path} (not found)");
        facts.ResumeLayout();
    }

    private void AddFact(string name, string? value)
    {
        if (Clean(value) is not { } text) return;

        var row = facts.RowCount++;
        facts.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        facts.Controls.Add(new Label
        {
            AutoSize = true,
            Font = new Font(Font, FontStyle.Bold),
            Margin = new Padding(0, 3, 12, 3),
            Text = name,
        }, 0, row);
        var valueLabel = new Label
        {
            AutoSize = true,
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 3, 0, 3),
            Text = text,
        };
        // The file row says "(not found)" after a missing path; copy just the path.
        CopyOnDoubleClick.Enable(valueLabel, name, name == "File" ? () => _path : null);
        facts.Controls.Add(valueLabel, 1, row);
    }

    private void UpdateButtons()
    {
        var hasItem = _item is not null;
        btnPlay.Enabled = _path is not null && MovieFileRules.IsVideoFile(_path) && File.Exists(_path);
        btnOpenOnDisk.Enabled = _path is not null;
        btnImdb.Enabled = btnTmdb.Enabled = btnWatchlist.Enabled = hasItem;
        ShowWatched();
        ShowSubtitleStatus();
    }

    #region My rating

    /// <summary>The stars, their text and the review link go in the rating row, after the IMDb badge and Watched.</summary>
    private void SetUpMyRating()
    {
        // After the votes, so the IMDb part stays together.
        ratingRow.Controls.AddRange([myRating, lblMyRating, lnkReview]);
        myRating.Chosen += async (_, rating) => await SaveMyRatingAsync(rating, _myRating?.Review);
        myRating.PreviewChanged += (_, preview) => lblMyRating.Text = preview is { } p ? RatingController.StarsText(p / 2.0) : MyRatingText();
        lnkReview.LinkClicked += async (_, _) => await EditReviewAsync();

        var menu = new ContextMenuStrip();
        menu.Items.Add("Remove my rating", null, async (_, _) => await RemoveMyRatingAsync());
        menu.Opening += (_, e) => e.Cancel = _myRating is null;
        myRating.ContextMenuStrip = menu;

        _toolTip.SetToolTip(myRating, _ratings.PlexConfigured
            ? "My rating: click a star (the left half is half a star). Saved in Kino and set on Plex, where only you see it. Right-click to remove it."
            : "My rating: click a star (the left half is half a star). Right-click to remove it.");
        _toolTip.SetToolTip(lnkReview, "Your own words about it. Kept in Kino only (Plex keeps reviews online, not on the server).");
    }

    private string MyRatingText() => _myRating is { } r ? $"My rating: {RatingController.StarsText(r.Stars)}" : "Rate it";

    private void ShowMyRating()
    {
        myRating.Enabled = _item is not null;
        myRating.Value = _myRating?.Rating;
        lblMyRating.Text = MyRatingText();
        lnkReview.Enabled = _myRating is not null;
        lnkReview.Text = _myRating?.Review is null ? "Review…" : "Edit review…";
        if (_myRating?.Review is { } review) _toolTip.SetToolTip(lnkReview, review);
    }

    private Task SaveMyRatingAsync(int rating, string? review) =>
        RunAsync("Saving your rating…", async ct =>
        {
            var result = await _ratings.SaveAsync(_imdbId, rating, review, _item?.Title, ct);
            _myRating = result.Rating;
            ShowMyRating();
            ShowStatus(result.Message);
        });

    private async Task RemoveMyRatingAsync() =>
        await RunAsync("Removing your rating…", async ct =>
        {
            var result = await _ratings.ClearAsync(_imdbId, ct);
            _myRating = null;
            ShowMyRating();
            ShowStatus(result.Message);
        });

    /// <summary>The review, in a small window; it's saved with the rating (a title needs a rating first).</summary>
    private async Task EditReviewAsync()
    {
        if (_myRating is not { } rating) return;
        using var dialog = new Form
        {
            Text = $"My review of {_item?.Title ?? _imdbId}", FormBorderStyle = FormBorderStyle.SizableToolWindow,
            StartPosition = FormStartPosition.CenterParent, ClientSize = new Size(480, 220), MinimizeBox = false, MaximizeBox = false,
            ShowInTaskbar = false,
        };
        var text = new TextBox { Multiline = true, ScrollBars = ScrollBars.Vertical, Dock = DockStyle.Fill, AcceptsReturn = true, Text = rating.Review ?? "" };
        var ok = new Button { Text = "Save", DialogResult = DialogResult.OK, AutoSize = true };
        var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, AutoSize = true };
        var row = new FlowLayoutPanel { Dock = DockStyle.Bottom, AutoSize = true, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(6) };
        row.Controls.AddRange([cancel, ok]);
        dialog.Controls.AddRange([text, row]);
        dialog.AcceptButton = null; // Enter makes a new line
        dialog.CancelButton = cancel;
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        await SaveMyRatingAsync(rating.Rating, text.Text);
    }

    #endregion

    /// <summary>The "✓ Watched" badge next to the rating, as the web page's tag; Mark as watched only while it isn't.</summary>
    private void ShowWatched()
    {
        lblWatched.Visible = _watched;
        btnMarkWatched.Enabled = _item is not null && !_watched;
        btnMarkWatched.Text = _watched ? "Watched ✓" : "Mark as watched";
    }

    private async void btnMarkWatched_Click(object sender, EventArgs e)
    {
        if (_item is null || _watched) return;
        await RunAsync($"Marking {_item.Title} as watched…", async ct =>
        {
            ShowStatus(await _watchlists.MarkWatchedAsync(_imdbId, ct));
            _watched = true;
            ShowWatched();
        });
    }

    /// <summary>
    /// SilverScreen's colours, plus one: green with a .srt next to the movie, light blue with only a Subs folder (subtitles
    /// downloaded, none yet named after the movie), light orange for an .mkv without either (it may have them built in),
    /// salmon for other files without.
    /// </summary>
    private void ShowSubtitleStatus()
    {
        var status = _item is not null && _path is not null && MovieFileRules.IsVideoFile(_path)
            ? _subtitles.GetStatus(_path)
            : new SubtitleStatus(SubtitleState.NoFile);
        btnSubtitles.Enabled = status.State != SubtitleState.NoFile;
        btnSubtitles.UseVisualStyleBackColor = status.State is SubtitleState.NoFile;
        (btnSubtitles.BackColor, var tip) = status.State switch
        {
            SubtitleState.SrtFile => (Color.LightGreen, $"Subtitles: {Path.GetFileName(status.Path)}. Click to show it."),
            SubtitleState.SubsFolder => (Color.LightSkyBlue,
                $"{status.Files} subtitle file(s) in {status.Path}, but no {Path.GetFileNameWithoutExtension(_path)}.srt next to the movie. " +
                "Click to copy the one named for this release next to the movie (then green); if none is, the Subs folder opens."),
            SubtitleState.NoneMaybeBuiltIn => (Color.PapayaWhip,
                $"No .srt or Subs folder (an .mkv may have subtitles built in). Click to download {_subtitles.Languages} subtitles from SubDL."),
            SubtitleState.None => (Color.LightSalmon,
                $"No .srt or Subs folder. Click to download {_subtitles.Languages} subtitles from SubDL."),
            _ => (SystemColors.Control, "Subtitles need a linked movie file."),
        };
        _toolTip.SetToolTip(btnSubtitles, tip);
    }

    private async void btnSubtitles_Click(object sender, EventArgs e)
    {
        if (_item is null || _path is null) return;
        var status = _subtitles.GetStatus(_path);
        if (status.State is SubtitleState.SrtFile)
        {
            Explorer.Show(status.Path);
            return;
        }

        if (status.State is SubtitleState.SubsFolder)
        {
            // One in Subs fits this release: next to the movie with its name, and the button turns green.
            try
            {
                if (_subtitles.UseMatchFromSubs(_path) is { } copied)
                {
                    ShowStatus($"Copied the subtitle that fits this release as {Path.GetFileName(copied)}.");
                    ShowSubtitleStatus();
                    return;
                }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                ShowStatus($"Error: couldn't copy the subtitle next to the movie: {ex.Message}");
                return;
            }

            OfferLargestSubtitle();
            return;
        }

        if (!_subtitles.HasKey)
        {
            if (MessageBox.Show(this, "Downloading subtitles needs a SubDL API key. It's free: see Help, Prerequisites, " +
                                      "then enter it in File → Settings." + Environment.NewLine + Environment.NewLine + "Open the help now?", "Subtitles",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                HelpForm.Show(this, "subdl");
            return;
        }

        SubtitleDownload? result = null;
        await RunAsync("Searching SubDL…", async ct =>
            result = await _subtitles.DownloadAsync(_item.ImdbId, _path, new Progress<string>(ShowStatus), ct));
        if (IsGone) return;
        ShowSubtitleStatus();
        if (result is null) return; // the error is already shown

        if (result.Found == 0)
        {
            ShowStatus($"SubDL has no {result.Languages} subtitles for {_item.Title}.");
            return;
        }
        ShowStatus($"Saved {result.Files} subtitle file(s) from {result.Saved} of {result.Found} download(s)" +
                   (result.Failed.Count > 0 ? $"; {result.Failed.Count} failed ({result.Failed[0]})" : "") +
                   (result.Copied is { } fit ? $"; the one that fits this release is now {Path.GetFileName(fit)}." : "."));
        if (result.Files > 0 && result.Copied is null) OfferLargestSubtitle();
    }

    /// <summary>
    /// None of the downloaded subtitles is for this release: lists them all by size, largest first and selected (often the
    /// right one). The one chosen is copied next to the movie as "&lt;movie&gt;.srt" (green); Cancel changes nothing.
    /// </summary>
    private void OfferLargestSubtitle()
    {
        if (_path is null) return;
        var files = _subtitles.GetSubsFiles(_path);
        if (files.Count == 0) return;

        using var dialog = new SubtitleChoiceDialog(files, _path);
        if (dialog.ShowDialog(this) != DialogResult.OK || dialog.Chosen is not { } chosen)
        {
            ShowStatus("No subtitle copied.");
            return;
        }

        try
        {
            var copied = _subtitles.UseSubtitle(_path, chosen.Path);
            ShowStatus($"Copied {Path.GetFileName(chosen.Path)} ({Size(chosen.Bytes)}) as {Path.GetFileName(copied)}.");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            ShowStatus($"Error: couldn't copy the subtitle next to the movie: {ex.Message}");
        }
        ShowSubtitleStatus();

        static string Size(long bytes) => bytes >= 1024 * 1024 ? $"{bytes / 1024d / 1024:N1} MB" : $"{bytes / 1024d:N1} KB";
    }

    private void btnPlay_Click(object sender, EventArgs e)
    {
        if (_path is not null) Process.Start(new ProcessStartInfo(_path) { UseShellExecute = true });
    }

    private void btnOpenOnDisk_Click(object sender, EventArgs e)
    {
        if (!Explorer.Show(_path)) ShowStatus($"{_path} no longer exists.");
    }

    private void btnImdb_Click(object sender, EventArgs e) =>
        Process.Start(new ProcessStartInfo($"https://www.imdb.com/title/{_imdbId}/") { UseShellExecute = true });

    private async void btnTmdb_Click(object sender, EventArgs e)
    {
        string? page = null;
        await RunAsync("Finding it on TMDb…", async ct => page = await _movies.GetTmdbPageAsync(_imdbId, _item?.Title, ct));
        if (page is not null) Process.Start(new ProcessStartInfo(page) { UseShellExecute = true });
    }

    private void btnWatchlist_Click(object sender, EventArgs e)
    {
        if (_item is null) return;
        using var dialog = ActivatorUtilities.CreateInstance<AddToWatchlistDialog>(_services, _item.ImdbId, _item.Title);
        if (dialog.ShowDialog(this) == DialogResult.OK) ShowStatus(dialog.ResultMessage);
    }

    private void btnClose_Click(object sender, EventArgs e) => Close();

    protected override void OnBusyChanged(bool busy)
    {
        base.OnBusyChanged(busy);
        buttons.Enabled = !busy;
    }

    protected override void DisplayStatus(string text) => lblStatus.Text = text;

    /// <summary>Keeps the plot and facts wrapping to the window's width.</summary>
    private void scroll_Resize(object sender, EventArgs e)
    {
        var width = Math.Max(200, scroll.ClientSize.Width - scroll.Padding.Horizontal - SystemInformation.VerticalScrollBarWidth);
        foreach (var label in new[] { lblTitle, lblHeadline, lblGenre, lblPlot })
            label.MaximumSize = new Size(width, 0);
        facts.MaximumSize = new Size(width, 0);
        similarRow.MaximumSize = new Size(width, 0);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        picPoster.Image?.Dispose();
        _toolTip.Dispose();
        base.OnFormClosed(e);
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) || value == "N/A" ? null : value.Trim();

    private static string Join(string separator, params string?[] parts) => string.Join(separator, parts.OfType<string>());
}
