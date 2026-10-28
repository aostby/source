using System.Diagnostics;
using Kolibri.Kino.Controllers.Trending;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.WinForms.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace Kolibri.Kino.WinForms.Forms;

/// <summary>
/// What's Up → Trending: TMDb's trending movies, TV shows or people this week, as cards like Similar movies. A title
/// opens its details; a person, the library's titles with them (what an actor's name does on the web). In SilverScreen
/// is shown in green, as elsewhere.
/// </summary>
public sealed class TrendingForm : AsyncForm
{
    public override string HelpTopic => "trending";


    private readonly TrendingController _trending;
    private readonly IServiceProvider _services;
    private readonly ToolTip _toolTip = new();
    private readonly Dictionary<TrendingKind, RadioButton> _lists = [];
    private readonly Button btnRefresh = new() { Text = "Refresh", AutoSize = true, Margin = new Padding(16, 3, 3, 3) };
    private readonly FlowLayoutPanel cards = new() { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(10) };
    private readonly Label lblStatus = new() { Dock = DockStyle.Bottom, AutoSize = false, Height = 24, Padding = new Padding(8, 4, 8, 0), AutoEllipsis = true };
    private TrendingKind _kind;
    private CancellationTokenSource? _imagesCts;

    public TrendingForm(TrendingController trending, IServiceProvider services)
    {
        _trending = trending;
        _services = services;
        Text = "Trending";
        ClientSize = new Size(1000, 720);
        StartPosition = FormStartPosition.CenterParent;

        var top = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(8, 8, 8, 0), WrapContents = false };
        foreach (var kind in Enum.GetValues<TrendingKind>())
        {
            var button = new RadioButton
            {
                Appearance = Appearance.Button, AutoSize = true, MinimumSize = new Size(90, 0), TextAlign = ContentAlignment.MiddleCenter,
                Text = kind switch { TrendingKind.Movies => "Movies", TrendingKind.Series => "TV shows", _ => "People" },
                Checked = kind == _kind,
            };
            button.CheckedChanged += async (_, _) =>
            {
                if (!button.Checked || kind == _kind) return;
                _kind = kind;
                await LoadAsync(refresh: false);
            };
            _lists[kind] = button;
            top.Controls.Add(button);
        }
        btnRefresh.Click += async (_, _) => await LoadAsync(refresh: true);
        top.Controls.Add(btnRefresh);
        _toolTip.SetToolTip(btnRefresh, "Asks TMDb again now. Lists are otherwise kept a few hours.");

        Controls.AddRange([cards, top, lblStatus]);
    }

    /// <summary>Opens on Movies (the buttons at the top switch lists).</summary>
    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        await LoadAsync(refresh: false);
    }

    private Task LoadAsync(bool refresh) =>
        RunAsync("Asking TMDb what's trending…", async ct =>
        {
            Text = TrendingController.Label(_kind);
            var result = await _trending.GetAsync(_kind, refresh, ct);
            Fill(result);
            ShowStatus(result.Note ?? $"{TrendingController.Label(result.Kind)} on TMDb this week: {result.Entries.Count}"
                + (result.Kind == TrendingKind.People ? "" : $", {result.Entries.Count(e => e.HasFile)} in SilverScreen")
                + (result.FetchedUtc is { } at ? $". Updated {at.ToLocalTime():g}." : "."));
        });

    private void Fill(TrendingResult result)
    {
        _imagesCts?.Cancel();
        _imagesCts?.Dispose();
        _imagesCts = new CancellationTokenSource();

        cards.SuspendLayout();
        foreach (Control old in cards.Controls) old.Dispose();
        cards.Controls.Clear();
        var posters = new List<(PictureBox Box, TrendingItem Item)>();
        foreach (var entry in result.Entries)
        {
            var (card, poster) = Card(result.Kind, entry);
            cards.Controls.Add(card);
            posters.Add((poster, entry.Item));
        }
        cards.ResumeLayout();
        cards.AutoScrollPosition = Point.Empty;

        _ = LoadImagesAsync(result.Kind, posters, _imagesCts.Token);
    }

    /// <summary>Posters and photos one by one, from the image cache (TMDb the first time), after the cards are shown.</summary>
    private async Task LoadImagesAsync(TrendingKind kind, List<(PictureBox Box, TrendingItem Item)> posters, CancellationToken ct)
    {
        foreach (var (box, item) in posters)
        {
            if (ct.IsCancellationRequested || IsGone) return;
            try
            {
                if (await _trending.GetImageAsync(kind, item, ct) is not { } bytes || ct.IsCancellationRequested || box.IsDisposed) continue;
                using var stream = new MemoryStream(bytes);
                using var image = Image.FromStream(stream);
                box.Image = new Bitmap(image);
            }
            catch (Exception ex) when (ex is OperationCanceledException or ArgumentException or ObjectDisposedException)
            {
                // Cancelled, a bad image, or the window closed: the card keeps its grey box.
            }
        }
    }

    /// <summary>A title's card (TitleCards), or a person's: photo, name, how many library titles, and known-for.</summary>
    private (Control Card, PictureBox Poster) Card(TrendingKind kind, TrendingEntry entry)
    {
        var item = entry.Item;
        if (kind != TrendingKind.People) return TitleCards.ForTitle(entry, Font, _toolTip, () => Open(kind, item));

        var (card, poster, title) = TitleCards.Frame(item.Title, Font);
        if (entry.LibraryTitles > 0) card.Controls.Add(TitleCards.Green($"{entry.LibraryTitles} in SilverScreen"));
        if (item.KnownFor.Count > 0) card.Controls.Add(TitleCards.Small($"Known for {string.Join(", ", item.KnownFor)}", Font));
        _toolTip.SetToolTip(poster, $"{item.Title}: the library's titles with them");
        poster.Click += (_, _) => Open(kind, item);
        title.LinkClicked += (_, _) => Open(kind, item);
        return (card, poster);
    }

    private void Open(TrendingKind kind, TrendingItem item)
    {
        if (kind == TrendingKind.People)
        {
            _ = ShowPersonAsync(item);
            return;
        }
        if (item.ImdbId is { } id) ActivatorUtilities.CreateInstance<MovieDetailsForm>(_services, id).ShowFrom(this);
        else Process.Start(new ProcessStartInfo(TrendingController.TmdbPage(kind, item.TmdbId)) { UseShellExecute = true });
    }

    /// <summary>The library's titles with the person, best rated first; double-click one for its details.</summary>
    private async Task ShowPersonAsync(TrendingItem person)
    {
        IReadOnlyList<OMDbApiNet.Model.Item> titles = [];
        await RunAsync($"Looking for {person.Title} in the library…", async ct => titles = await _trending.GetLibraryTitlesWithAsync(person.Title, ct));
        if (IsGone) return;

        if (titles.Count == 0)
        {
            if (MessageBox.Show(this, $"{person.Title} isn't in any title in the library (as actor or director).{Environment.NewLine}{Environment.NewLine}Open their page on TMDb?",
                    person.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                Process.Start(new ProcessStartInfo(TrendingController.TmdbPage(TrendingKind.People, person.TmdbId)) { UseShellExecute = true });
            return;
        }

        var dialog = new Form
        {
            Text = $"{person.Title} in the library", StartPosition = FormStartPosition.CenterParent, ClientSize = new Size(520, 360),
            MinimizeBox = false, ShowInTaskbar = false, FormBorderStyle = FormBorderStyle.SizableToolWindow,
        };
        var list = new ListView { Dock = DockStyle.Fill, View = View.Details, FullRowSelect = true, HideSelection = false };
        list.Columns.Add("Title", 300);
        list.Columns.Add("Year", 70);
        list.Columns.Add("IMDb", 60);
        foreach (var t in titles) list.Items.Add(new ListViewItem([t.Title, t.Year ?? "", t.ImdbRating ?? ""]) { Tag = t.ImdbId });
        list.ItemActivate += (_, _) =>
        {
            if (list.SelectedItems.Count == 1 && list.SelectedItems[0].Tag is string id)
                ActivatorUtilities.CreateInstance<MovieDetailsForm>(_services, id).ShowFrom(this);
        };
        var hint = new Label { Dock = DockStyle.Bottom, Height = 22, Padding = new Padding(6, 4, 0, 0), ForeColor = SystemColors.GrayText,
            Text = $"{titles.Count} title(s), best rated first. Double-click one for its details." };
        dialog.Controls.AddRange([list, hint]);
        dialog.FormClosed += (_, _) => dialog.Dispose();
        dialog.Show(this);
    }

    protected override void OnBusyChanged(bool busy)
    {
        base.OnBusyChanged(busy);
        foreach (var button in _lists.Values) button.Enabled = !busy;
        btnRefresh.Enabled = !busy;
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
