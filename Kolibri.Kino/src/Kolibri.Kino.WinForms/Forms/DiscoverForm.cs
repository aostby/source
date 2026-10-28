using System.Diagnostics;
using Kolibri.Kino.Controllers.Discover;
using Kolibri.Kino.Controllers.Trending;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.WinForms.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace Kolibri.Kino.WinForms.Forms;

/// <summary>
/// What's Up → Discover: TMDb's movies or TV shows for a season (the buttons at the top), with genre, years, rating and
/// sort, 20 at a time with Show more. Opens on the season we're in. Cards as on Trending; Hide what I have leaves out
/// the titles with a file.
/// </summary>
public sealed class DiscoverForm : AsyncForm
{
    public override string HelpTopic => "discover";

    private readonly DiscoverController _discover;
    private readonly IServiceProvider _services;
    private readonly ToolTip _toolTip = new();
    private readonly Dictionary<DiscoverSeason, RadioButton> _seasons = [];
    private readonly RadioButton rbMovies = Toggle("Movies", 80);
    private readonly RadioButton rbSeries = Toggle("TV shows", 80);
    private readonly ComboBox cboGenre = Choice(130);
    private readonly ComboBox cboYear = Choice(100);
    private readonly ComboBox cboRating = Choice(95);
    private readonly ComboBox cboSort = Choice(105);
    private readonly CheckBox chkHide = new() { Text = "Hide what I have", AutoSize = true, Margin = new Padding(10, 6, 3, 3) };
    private readonly Button btnRefresh = new() { Text = "Refresh", AutoSize = true, Margin = new Padding(12, 3, 3, 3) };
    private readonly FlowLayoutPanel cards = new() { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(10) };
    private readonly Button btnMore = new() { Text = "Show more", AutoSize = true, Margin = new Padding(0, 100, 14, 16), MinimumSize = new Size(110, 32) };
    private readonly Label lblStatus = new() { Dock = DockStyle.Bottom, AutoSize = false, Height = 24, Padding = new Padding(8, 4, 8, 0), AutoEllipsis = true };

    private DiscoverFilter _filter = new(DiscoverController.CurrentSeason());
    private readonly List<TrendingEntry> _entries = [];
    private DiscoverResult? _last;
    private CancellationTokenSource? _imagesCts;
    private bool _loadingChoices;

    public DiscoverForm(DiscoverController discover, IServiceProvider services)
    {
        _discover = discover;
        _services = services;
        Text = "Discover";
        ClientSize = new Size(1000, 760);
        StartPosition = FormStartPosition.CenterParent;

        var seasonRow = Row();
        seasonRow.Controls.Add(Caption("Season"));
        foreach (var season in DiscoverSeasons.All)
        {
            var button = Toggle(season.Label, 0);
            button.Checked = season.Season == _filter.Season;
            button.CheckedChanged += async (_, _) =>
            {
                if (!button.Checked || season.Season == _filter.Season) return;
                _filter = _filter with { Season = season.Season };
                await SearchAsync();
            };
            _seasons[season.Season] = button;
            seasonRow.Controls.Add(button);
        }

        var showRow = Row();
        showRow.Controls.Add(Caption("Show"));
        rbMovies.Checked = true;
        rbSeries.CheckedChanged += async (_, _) =>
        {
            if (_filter.Series == rbSeries.Checked) return;
            _filter = _filter with { Series = rbSeries.Checked, GenreId = null };
            await LoadGenresAsync();
            await SearchAsync();
        };
        _loadingChoices = true;
        cboYear.Items.AddRange([.. DiscoverChoices.Years.Select(y => y.Label)]);
        cboRating.Items.AddRange([.. DiscoverChoices.Ratings.Select(r => r.Label)]);
        cboSort.Items.AddRange([.. DiscoverChoices.Sorts.Select(s => s.Label)]);
        cboGenre.Items.Add("All genres");
        cboYear.SelectedIndex = cboRating.SelectedIndex = cboSort.SelectedIndex = cboGenre.SelectedIndex = 0;
        _loadingChoices = false;
        cboGenre.SelectedIndexChanged += async (_, _) =>
        {
            if (_loadingChoices) return;
            _filter = _filter with { GenreId = (cboGenre.SelectedItem as GenreChoice)?.Genre.Id };
            await SearchAsync();
        };
        cboYear.SelectedIndexChanged += async (_, _) =>
        {
            var (_, _, from, to) = DiscoverChoices.Years[cboYear.SelectedIndex];
            _filter = _filter with { YearFrom = from, YearTo = to };
            await SearchAsync();
        };
        cboRating.SelectedIndexChanged += async (_, _) =>
        {
            _filter = _filter with { MinRating = DiscoverChoices.Ratings[cboRating.SelectedIndex].Min };
            await SearchAsync();
        };
        cboSort.SelectedIndexChanged += async (_, _) =>
        {
            _filter = _filter with { Sort = DiscoverChoices.Sorts[cboSort.SelectedIndex].Sort };
            await SearchAsync();
        };
        chkHide.CheckedChanged += (_, _) => ShowEntries(scrollToTop: true);
        btnRefresh.Click += async (_, _) => await SearchAsync(refresh: true);
        btnMore.Click += async (_, _) => await ShowMoreAsync();
        showRow.Controls.AddRange([rbMovies, rbSeries, cboGenre, cboYear, cboRating, cboSort, chkHide, btnRefresh]);

        _toolTip.SetToolTip(cboRating, "TMDb's users' average (the cards show IMDb's rating for titles in your library).");
        _toolTip.SetToolTip(chkHide, "Leaves out the titles you have (In SilverScreen, with a file).");
        _toolTip.SetToolTip(btnRefresh, "Asks TMDb again now. Pages are otherwise kept a few hours.");
        _toolTip.SetToolTip(btnMore, "The next 20.");

        var top = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(6, 6, 6, 2) };
        top.Controls.AddRange([seasonRow, showRow]);
        Controls.AddRange([cards, top, lblStatus]);
    }

    /// <summary>Opens on the season we're in, movies, most popular.</summary>
    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        await LoadGenresAsync();
        await SearchAsync();
    }

    /// <summary>TMDb's genres for movies or TV shows; a genre of the same name stays chosen.</summary>
    private async Task LoadGenresAsync()
    {
        IReadOnlyList<DiscoverGenre> genres = [];
        await RunAsync("Fetching TMDb's genres…", async ct => genres = await _discover.GetGenresAsync(_filter.Series, ct));
        if (IsGone) return;
        _loadingChoices = true;
        cboGenre.Items.Clear();
        cboGenre.Items.Add("All genres");
        cboGenre.Items.AddRange([.. genres.Select(g => new GenreChoice(g))]);
        cboGenre.SelectedIndex = 0;
        _loadingChoices = false;
    }

    /// <summary>Page 1 of the current choices, in place of what's shown.</summary>
    private Task SearchAsync(bool refresh = false) =>
        RunAsync($"Discovering {DiscoverSeasons.Of(_filter.Season).Label}…", async ct =>
        {
            Text = $"Discover · {DiscoverSeasons.Of(_filter.Season).Label}";
            var result = await _discover.GetAsync(_filter, 1, refresh, ct);
            _entries.Clear();
            _entries.AddRange(result.Entries);
            _last = result;
            ShowEntries(scrollToTop: true);
        });

    /// <summary>The next 20, added under the ones shown.</summary>
    private Task ShowMoreAsync() =>
        _last is not { HasMore: true } last ? Task.CompletedTask
        : RunAsync("Fetching 20 more…", async ct =>
        {
            var result = await _discover.GetAsync(_filter, last.Page + 1, ct: ct);
            var known = _entries.Select(e => e.Item.TmdbId).ToHashSet();
            _entries.AddRange(result.Entries.Where(e => known.Add(e.Item.TmdbId))); // TMDb's pages can overlap when the order shifts
            _last = result;
            ShowEntries(scrollToTop: false);
        });

    private void ShowEntries(bool scrollToTop)
    {
        if (_last is not { } result) return;
        _imagesCts?.Cancel();
        _imagesCts?.Dispose();
        _imagesCts = new CancellationTokenSource();
        var scroll = cards.AutoScrollPosition;

        var shown = _entries.Where(e => !chkHide.Checked || !e.HasFile).ToList();
        cards.SuspendLayout();
        cards.Controls.Remove(btnMore);
        foreach (Control old in cards.Controls) old.Dispose();
        cards.Controls.Clear();
        var posters = new List<(PictureBox Box, TrendingItem Item)>();
        foreach (var entry in shown)
        {
            var item = entry.Item;
            var (card, poster) = TitleCards.ForTitle(entry, Font, _toolTip, () => Open(item));
            cards.Controls.Add(card);
            posters.Add((poster, item));
        }
        if (result.HasMore) cards.Controls.Add(btnMore);
        cards.ResumeLayout();
        cards.AutoScrollPosition = scrollToTop ? Point.Empty : new Point(-scroll.X, -scroll.Y);

        var hidden = _entries.Count - shown.Count;
        var what = _filter.Series ? "TV shows" : "movies";
        ShowStatus(result.Note ?? $"{DiscoverSeasons.Of(_filter.Season).Label} {what} on TMDb: {_entries.Count} of {result.TotalResults:N0}, "
            + $"{_entries.Count(e => e.HasFile)} in SilverScreen" + (hidden > 0 ? $" ({hidden} hidden)" : "")
            + (result.FetchedUtc is { } at ? $". Updated {at.ToLocalTime():g}." : "."));

        _ = LoadImagesAsync(posters, _imagesCts.Token);
    }

    /// <summary>Posters one by one, from the image cache (TMDb the first time), after the cards are shown.</summary>
    private async Task LoadImagesAsync(List<(PictureBox Box, TrendingItem Item)> posters, CancellationToken ct)
    {
        foreach (var (box, item) in posters)
        {
            if (ct.IsCancellationRequested || IsGone) return;
            try
            {
                if (await _discover.GetImageAsync(_filter.Series, item, ct) is { } bytes && !ct.IsCancellationRequested && !box.IsDisposed)
                    TitleCards.SetImage(box, bytes);
            }
            catch (Exception ex) when (ex is OperationCanceledException or ObjectDisposedException)
            {
                // Cancelled, or the window closed.
            }
        }
    }

    private void Open(TrendingItem item)
    {
        if (item.ImdbId is { } id) ActivatorUtilities.CreateInstance<MovieDetailsForm>(_services, id).ShowFrom(this);
        else Process.Start(new ProcessStartInfo(TrendingController.TmdbPage(_filter.Series ? TrendingKind.Series : TrendingKind.Movies, item.TmdbId)) { UseShellExecute = true });
    }

    private static FlowLayoutPanel Row() => new() { AutoSize = true, WrapContents = true, Margin = new Padding(0, 0, 0, 2) };

    private static Label Caption(string text) => new()
    {
        Text = text, AutoSize = false, Size = new Size(52, 26), TextAlign = ContentAlignment.MiddleLeft, ForeColor = SystemColors.GrayText,
    };

    private static RadioButton Toggle(string text, int minWidth) => new()
    {
        Appearance = Appearance.Button, AutoSize = true, MinimumSize = new Size(minWidth, 0), TextAlign = ContentAlignment.MiddleCenter, Text = text,
    };

    private static ComboBox Choice(int width) => new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = width, Margin = new Padding(6, 4, 0, 3) };

    /// <summary>A genre in the list, shown by name.</summary>
    private sealed record GenreChoice(DiscoverGenre Genre)
    {
        public override string ToString() => Genre.Name;
    }

    protected override void OnBusyChanged(bool busy)
    {
        base.OnBusyChanged(busy);
        foreach (var button in _seasons.Values) button.Enabled = !busy;
        rbMovies.Enabled = rbSeries.Enabled = cboGenre.Enabled = cboYear.Enabled = cboRating.Enabled = cboSort.Enabled =
            chkHide.Enabled = btnRefresh.Enabled = btnMore.Enabled = !busy;
    }

    protected override void DisplayStatus(string text) => lblStatus.Text = text;

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _imagesCts?.Cancel();
        _imagesCts?.Dispose();
        _toolTip.Dispose();
        base.OnFormClosed(e);
    }
}
