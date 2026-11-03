using Kolibri.Kino.Controllers.Ratings;
using Kolibri.Kino.Controllers.Suggestions;
using Kolibri.Kino.WinForms.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace Kolibri.Kino.WinForms.Forms;

/// <summary>
/// What's Up → Suggestions: movies like the ones you rated 3½ stars or more, as the web Library's "Suggestions for you"
/// row (<b>For you</b>: 20, the ones like a movie rated this week first, the rest a new pick each day) or all of them
/// (<b>See all</b>). Made from the similar movies kept when details were opened, so it asks TMDb nothing; a card opens
/// the movie's details. <b>Import my ratings</b> fetches the ratings Kino on the web sent to Uranus (off without the
/// Uranus URL and token in Settings).
/// </summary>
public sealed class SuggestionsForm : AsyncForm
{
    public override string HelpTopic => "suggestions";

    /// <summary>Gold, dark enough to read on white: like a movie you rated in the last week (the web's "New for you").</summary>
    private static readonly Color Fresh = Color.DarkGoldenrod;

    private readonly SuggestionsController _suggestions;
    private readonly WebRatings _webRatings;
    private readonly IServiceProvider _services;
    private readonly ToolTip _toolTip = new();
    private readonly RadioButton btnForYou = ListButton("For you", @checked: true);
    private readonly RadioButton btnAll = ListButton("See all", @checked: false);
    private readonly Button btnRefresh = new() { Text = "Refresh", AutoSize = true, Margin = new Padding(16, 3, 3, 3) };
    private readonly Button btnImport = new() { Text = "Import my ratings", AutoSize = true, Margin = new Padding(16, 3, 3, 3) };
    private readonly FlowLayoutPanel cards = new() { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(10) };
    private bool _busy;
    private readonly Label lblStatus = new() { Dock = DockStyle.Bottom, AutoSize = false, Height = 24, Padding = new Padding(8, 4, 8, 0), AutoEllipsis = true };

    public SuggestionsForm(SuggestionsController suggestions, WebRatings webRatings, IServiceProvider services)
    {
        _suggestions = suggestions;
        _webRatings = webRatings;
        _services = services;
        Text = "Suggestions for you";
        ClientSize = new Size(1000, 720);
        StartPosition = FormStartPosition.CenterParent;

        var top = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(8, 8, 8, 0), WrapContents = false };
        btnForYou.CheckedChanged += async (_, _) => { if (btnForYou.Checked) await LoadAsync(); };
        btnAll.CheckedChanged += async (_, _) => { if (btnAll.Checked) await LoadAsync(); };
        btnRefresh.Click += async (_, _) => await LoadAsync();
        btnImport.Click += async (_, _) => await ImportRatingsAsync();
        top.Controls.AddRange([btnForYou, btnAll, btnRefresh, btnImport]);
        _toolTip.SetToolTip(btnForYou, $"{SuggestionsController.Shown}, as on the web Library: the ones like a movie you rated this week first, the rest a new pick each day.");
        _toolTip.SetToolTip(btnAll, "Every suggestion, best first.");
        _toolTip.SetToolTip(btnRefresh, "Makes the list again, e.g. after rating a movie or opening a liked movie's details.");
        ShowImportState();

        Controls.AddRange([cards, top, lblStatus]);
    }

    private static RadioButton ListButton(string text, bool @checked) => new()
    {
        Appearance = Appearance.Button, AutoSize = true, MinimumSize = new Size(90, 0), TextAlign = ContentAlignment.MiddleCenter,
        Text = text, Checked = @checked,
    };

    /// <summary>Opens on For you (the buttons at the top switch).</summary>
    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        await LoadAsync();
    }

    /// <summary>Settings may have changed while the window was open: the button follows them.</summary>
    protected override void OnActivated(EventArgs e)
    {
        base.OnActivated(e);
        ShowImportState();
    }

    /// <summary>Import my ratings works only with the Uranus URL and token in Settings; the tooltip says which.</summary>
    private void ShowImportState()
    {
        btnImport.Enabled = !_busy && _webRatings.IsConfigured;
        _toolTip.SetToolTip(btnImport, _webRatings.IsConfigured
            ? "Fetches the ratings Kino on the web sent to Uranus (problembar.net) and takes them in: the newer rating of a title wins."
            : "Set the Uranus URL and token in Settings (category Uranus) to fetch the ratings Kino on the web sent there.");
    }

    private Task ImportRatingsAsync() =>
        RunAsync("Fetching your ratings from Uranus…", async ct =>
        {
            var r = await _webRatings.ImportAsync(ct);
            var text = $"Ratings from Uranus: {r.Added} new, {r.Updated} updated, {r.KeptNewer} kept (newer here)"
                       + (r.Invalid > 0 ? $", {r.Invalid} invalid left out" : "") + "."
                       + (r.SentToPlex > 0 || r.PlexNote is not null ? $" Plex: {r.SentToPlex} set." + (r.PlexNote is { } note ? $" {note}" : "") : "");
            MessageBox.Show(this, text, "Import my ratings", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await LoadListAsync(ct);
        });

    private Task LoadAsync() => RunAsync("Finding suggestions…", LoadListAsync);

    private async Task LoadListAsync(CancellationToken ct)
    {
        var all = btnAll.Checked;
        var list = all ? await _suggestions.GetAllAsync(ct) : await _suggestions.GetAsync(ct: ct);
        Fill(list);
        ShowStatus(list.Count == 0
            ? $"No suggestions yet: rate movies 3½ stars or more, and open their details so their similar movies are kept ({SuggestionsController.MinSources} are needed)."
            : all
                ? $"{list.Count} movie(s) like the ones you rated 3½ stars or more, best first."
                : $"{list.Count} of the movies like the ones you rated 3½ stars or more: the ones like a movie rated this week first, the rest picked from the best {SuggestionsController.Pool} for today.");
    }

    private void Fill(IReadOnlyList<Suggestion> list)
    {
        cards.SuspendLayout();
        foreach (Control old in cards.Controls) old.Dispose();
        cards.Controls.Clear();
        foreach (var s in list) cards.Controls.Add(Card(s));
        cards.ResumeLayout();
        cards.AutoScrollPosition = Point.Empty;
    }

    /// <summary>Poster and title (both open its details), year and IMDb rating, New for you, no file, and why it's suggested.</summary>
    private Control Card(Suggestion s)
    {
        var (card, poster, title) = TitleCards.Frame(s.Movie.Title, Font);
        if (s.Movie.Poster is { } url) poster.LoadAsync(url); // TMDb's small poster, straight from TMDb (as Similar movies)
        card.Controls.Add(new Label
        {
            AutoSize = true, MaximumSize = new Size(TitleCards.Width, 0), Margin = new Padding(0, 2, 0, 0), ForeColor = SystemColors.GrayText,
            Text = string.Join(" · ", new[] { s.Movie.Year, s.ImdbRating is { } r ? $"IMDb {r}" : null }.OfType<string>()),
        });
        if (s.Fresh)
        {
            var fresh = TitleCards.Green("New for you");
            fresh.ForeColor = Fresh;
            _toolTip.SetToolTip(fresh, "Like a movie you rated in the last week.");
            card.Controls.Add(fresh);
        }
        if (s.InLibrary)
        {
            var noFile = TitleCards.Green("In SilverScreen, no file");
            noFile.ForeColor = TitleCards.NoFile;
            _toolTip.SetToolTip(noFile, "In the library, but not linked to a file that's there.");
            card.Controls.Add(noFile);
        }
        var because = TitleCards.Small(SuggestionsController.BecauseText(s), Font);
        _toolTip.SetToolTip(because, $"Because you liked {string.Join(", ", s.Because)}"); // all of them, the card names up to three
        card.Controls.Add(because);

        void Open() => ActivatorUtilities.CreateInstance<MovieDetailsForm>(_services, s.Movie.ImdbId).ShowFrom(this);
        poster.Click += (_, _) => Open();
        title.LinkClicked += (_, _) => Open();
        _toolTip.SetToolTip(poster, $"{s.Movie.Title} ({s.Movie.Year}): details");
        return card;
    }

    protected override void OnBusyChanged(bool busy)
    {
        base.OnBusyChanged(busy);
        btnForYou.Enabled = btnAll.Enabled = btnRefresh.Enabled = !busy;
        _busy = busy;
        ShowImportState();
    }

    protected override void DisplayStatus(string text) => lblStatus.Text = text;

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _toolTip.Dispose();
        base.OnFormClosed(e);
    }
}
