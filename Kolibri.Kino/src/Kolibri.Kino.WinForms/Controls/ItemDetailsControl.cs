using System.Diagnostics;
using Kolibri.Kino.Controllers.LocalMovies;
using OMDbApiNet.Model;

namespace Kolibri.Kino.WinForms.Controls;

/// <summary>
/// Details pane: poster, facts and plot for a library movie, or just the path for an unmatched file.
/// Display only; the owning form loads posters through its controller.
/// </summary>
public partial class ItemDetailsControl : UserControl
{
    private string? _filePath;

    public ItemDetailsControl()
    {
        InitializeComponent();
        Clear();
    }

    public void Clear()
    {
        Show(string.Empty, string.Empty, string.Empty, string.Empty, null, fileExists: false);
        SetPoster(null);
    }

    public void ShowMovie(LocalMovie movie)
    {
        if (movie.Item is { } item)
            ShowItem(item, movie.FilePath, movie.FileExists);
        else
        {
            Show(movie.Title, $"No details stored for {movie.ImdbId}", string.Empty, string.Empty, movie.FilePath, movie.FileExists);
            SetPoster(null);
        }
    }

    /// <summary>Shows a movie's details; <paramref name="filePath"/> is null when no file is involved.</summary>
    public void ShowItem(Item item, string? filePath = null, bool fileExists = false)
    {
        var facts = Join(" · ", item.Year, item.Runtime, item.Genre, item.ImdbRating is { } r && r != "N/A" ? $"IMDb {r}" : null, item.Rated);
        var people = Join(Environment.NewLine, Labelled("Director", item.Director), Labelled("Actors", item.Actors));

        Show(item.Title, facts, people, Clean(item.Plot) ?? string.Empty, filePath, fileExists);
        SetPoster(null);
    }

    public void ShowFile(UnmatchedFile file)
    {
        var facts = file.IsMultipart ? "Multipart or extra file, not matched on its own" : "Not in the library";
        Show(Path.GetFileNameWithoutExtension(file.FilePath), facts, string.Empty, string.Empty, file.FilePath, fileExists: true);
        SetPoster(null);
    }

    /// <summary>Poster share of the width at most; the text column keeps the rest.</summary>
    private const double MaxPosterShare = 0.55;

    /// <summary>Width / height of the picture: 2:3 for posters (default), 16:9 for episode screenshots.</summary>
    [System.ComponentModel.DefaultValue(2.0 / 3.0)]
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public double PosterAspect
    {
        get => _posterAspect;
        set
        {
            _posterAspect = value;
            PerformLayout();
        }
    }

    private double _posterAspect = 2.0 / 3.0;

    private const int MinTextWidth = 220;

    protected override void OnLayout(LayoutEventArgs e)
    {
        LayoutPoster();
        base.OnLayout(e);
        SizePlot();
    }

    /// <summary>
    /// The plot box is as tall as its text (plots are usually short), up to half the pane; longer text scrolls.
    /// </summary>
    private void SizePlot()
    {
        var width = txtPlot.ClientSize.Width;
        if (width <= 0) return;

        var measured = TextRenderer.MeasureText(txtPlot.Text.Length == 0 ? " " : txtPlot.Text, txtPlot.Font,
            new Size(width, int.MaxValue), TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl).Height + 4;
        var max = Math.Max(txtPlot.Font.Height * 3, ClientSize.Height / 2);
        var height = Math.Min(measured, max);
        var scrollBars = measured > max ? ScrollBars.Vertical : ScrollBars.None;

        if (txtPlot.Height != height) txtPlot.Height = height;
        if (txtPlot.ScrollBars != scrollBars) txtPlot.ScrollBars = scrollBars;
    }

    /// <summary>
    /// Poster in the upper right, as large as its 2:3 shape allows within the height, but never more than
    /// <see cref="MaxPosterShare"/> of the width or so wide that the text gets under <see cref="MinTextWidth"/>.
    /// </summary>
    private void LayoutPoster()
    {
        var padding = posterPanel.Padding;
        var availableHeight = Math.Max(0, ClientSize.Height - padding.Vertical);
        var byHeight = (int)(availableHeight * _posterAspect);
        var byWidth = Math.Min((int)(ClientSize.Width * MaxPosterShare), ClientSize.Width - MinTextWidth);
        var posterWidth = Math.Max(0, Math.Min(byHeight, byWidth));

        posterPanel.Width = posterWidth + padding.Horizontal;
        picPoster.Height = Math.Min(availableHeight, (int)(posterWidth / _posterAspect));
    }

    /// <summary>Takes ownership of <paramref name="image"/> and disposes the previous one.</summary>
    public void SetPoster(Image? image)
    {
        var previous = picPoster.Image;
        picPoster.Image = image;
        previous?.Dispose();
    }

    private void Show(string title, string facts, string people, string plot, string? filePath, bool fileExists)
    {
        _filePath = filePath;
        lblTitle.Text = title;
        lblFacts.Text = facts;
        lblPeople.Text = people;
        txtPlot.Text = plot;
        SizePlot();
        lblFile.Text = filePath is null ? string.Empty : fileExists ? filePath : $"{filePath}  (file not found)";
        btnOpenFolder.Enabled = filePath is not null;
        // A series passes its folder: Open folder works, Play only for a video file.
        btnPlay.Enabled = filePath is not null && fileExists && Kolibri.Kino.Core.MovieFileRules.IsVideoFile(filePath);
    }

    private void btnOpenFolder_Click(object sender, EventArgs e)
    {
        if (_filePath is null) return;
        if (!Explorer.Show(_filePath))
            MessageBox.Show(this, $"The folder no longer exists:{Environment.NewLine}{Path.GetDirectoryName(_filePath)}", "Open folder");
    }

    private void btnPlay_Click(object sender, EventArgs e)
    {
        if (_filePath is not null)
            Process.Start(new ProcessStartInfo(_filePath) { UseShellExecute = true });
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) || value == "N/A" ? null : value;

    private static string? Labelled(string label, string? value) => Clean(value) is { } v ? $"{label}: {v}" : null;

    private static string Join(string separator, params string?[] parts) =>
        string.Join(separator, parts.Select(Clean).OfType<string>());
}
