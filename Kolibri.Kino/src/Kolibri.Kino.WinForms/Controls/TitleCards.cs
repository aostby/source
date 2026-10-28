using System.Globalization;
using Kolibri.Kino.Controllers.Trending;

namespace Kolibri.Kino.WinForms.Controls;

/// <summary>
/// The poster cards of What's Up (Trending, Discover): poster and title (both open it), year and rating, In SilverScreen
/// in green (salmon without a file), and the start of the plot.
/// </summary>
internal static class TitleCards
{
    public const int Width = 150;

    /// <summary>Salmon, a little darker than Color.Salmon so it reads on white: in the library without a file.</summary>
    public static readonly Color NoFile = Color.FromArgb(226, 104, 86);

    /// <summary>An empty card: the poster box and the title link; the caller adds the rest and hooks up opening it.</summary>
    public static (FlowLayoutPanel Card, PictureBox Poster, LinkLabel Title) Frame(string title, Font font)
    {
        var card = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoSize = true, Margin = new Padding(0, 0, 14, 16) };
        var poster = new PictureBox
        {
            Size = new Size(Width, 225), SizeMode = PictureBoxSizeMode.Zoom, BackColor = SystemColors.ControlLight,
            Cursor = Cursors.Hand, Margin = new Padding(0),
        };
        var link = new LinkLabel
        {
            Text = title, AutoSize = true, MaximumSize = new Size(Width, 0), Margin = new Padding(0, 4, 0, 0),
            Font = new Font(font, FontStyle.Bold),
        };
        card.Controls.AddRange([poster, link]);
        return (card, poster, link);
    }

    /// <summary>A movie's or series' card, opening with <paramref name="open"/>.</summary>
    public static (Control Card, PictureBox Poster) ForTitle(TrendingEntry entry, Font font, ToolTip tip, Action open)
    {
        var item = entry.Item;
        var (card, poster, title) = Frame(item.Title, font);
        var rating = entry.ImdbRating is { } imdb ? $"IMDb {imdb}"
            : item.TmdbRating is { } tmdb ? $"TMDb {tmdb.ToString("0.0", CultureInfo.InvariantCulture)}" : null;
        card.Controls.Add(new Label
        {
            AutoSize = true, MaximumSize = new Size(Width, 0), Margin = new Padding(0, 2, 0, 0), ForeColor = SystemColors.GrayText,
            Text = string.Join(" · ", new[] { item.Year, rating }.OfType<string>()),
        });
        if (entry.HasFile) card.Controls.Add(Green("In SilverScreen"));
        else if (entry.InLibrary)
        {
            var noFile = Green("In SilverScreen, no file");
            noFile.ForeColor = NoFile;
            tip.SetToolTip(noFile, "In the library, but not linked to a file that's there.");
            card.Controls.Add(noFile);
        }
        if (item.Overview is { } overview)
        {
            var plot = Small(overview, font);
            tip.SetToolTip(plot, overview);
            card.Controls.Add(plot);
        }
        tip.SetToolTip(poster, item.ImdbId is null ? $"{item.Title}: TMDb's page" : $"{item.Title} ({item.Year}): details");
        poster.Click += (_, _) => open();
        title.LinkClicked += (_, _) => open();
        return (card, poster);
    }

    public static Label Green(string text) => new()
    {
        AutoSize = true, Text = text, ForeColor = Color.SeaGreen, Margin = new Padding(0, 2, 0, 0),
    };

    /// <summary>A three-line, small grey text that ends in "…" when longer.</summary>
    public static Label Small(string text, Font font) => new()
    {
        AutoSize = false, Size = new Size(Width, 46), AutoEllipsis = true, Text = text, ForeColor = SystemColors.GrayText,
        Font = new Font(font.FontFamily, font.Size - 1), Margin = new Padding(0, 4, 0, 0),
    };

    /// <summary>Shows the encoded image in the box; a bad image leaves the grey box.</summary>
    public static void SetImage(PictureBox box, byte[] bytes)
    {
        try
        {
            using var stream = new MemoryStream(bytes);
            using var image = Image.FromStream(stream);
            box.Image = new Bitmap(image);
        }
        catch (ArgumentException)
        {
        }
    }
}
