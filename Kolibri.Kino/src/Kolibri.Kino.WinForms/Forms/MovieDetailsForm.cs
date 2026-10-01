using System.Diagnostics;
using Kolibri.Kino.Controllers;
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
    private readonly MovieController _movies;
    private readonly IServiceProvider _services;
    private readonly string _imdbId;
    private Item? _item;
    private string? _path;

    public MovieDetailsForm(MovieController movies, IServiceProvider services, string imdbId)
    {
        _movies = movies;
        _services = services;
        _imdbId = imdbId;
        InitializeComponent();
        Text = imdbId;
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
            ShowItem(_item);
            ShowStatus(_path is null ? "Not linked to a file." : File.Exists(_path) || Directory.Exists(_path) ? _path : $"{_path} (not found)");

            var poster = await _movies.GetPosterAsync(_item, ct);
            if (poster is not null)
            {
                using var stream = new MemoryStream(poster);
                using var image = Image.FromStream(stream);
                picPoster.Image = new Bitmap(image);
            }
        });

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
        facts.Controls.Add(new Label
        {
            AutoSize = true,
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 3, 0, 3),
            Text = text,
        }, 1, row);
    }

    private void UpdateButtons()
    {
        var hasItem = _item is not null;
        btnPlay.Enabled = _path is not null && MovieFileRules.IsVideoFile(_path) && File.Exists(_path);
        btnOpenOnDisk.Enabled = _path is not null;
        btnImdb.Enabled = btnTmdb.Enabled = btnWatchlist.Enabled = hasItem;
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

    protected override void ShowStatus(string text) => lblStatus.Text = text;

    /// <summary>Keeps the plot and facts wrapping to the window's width.</summary>
    private void scroll_Resize(object sender, EventArgs e)
    {
        var width = Math.Max(200, scroll.ClientSize.Width - scroll.Padding.Horizontal - SystemInformation.VerticalScrollBarWidth);
        foreach (var label in new[] { lblTitle, lblHeadline, lblGenre, lblPlot })
            label.MaximumSize = new Size(width, 0);
        facts.MaximumSize = new Size(width, 0);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        picPoster.Image?.Dispose();
        base.OnFormClosed(e);
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) || value == "N/A" ? null : value.Trim();

    private static string Join(string separator, params string?[] parts) => string.Join(separator, parts.OfType<string>());
}
