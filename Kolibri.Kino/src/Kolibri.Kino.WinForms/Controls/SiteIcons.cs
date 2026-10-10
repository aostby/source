using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Kolibri.Kino.WinForms.Controls;

/// <summary>
/// Icons on the left of buttons and menu items: IMDb, TMDb and SubDL badges drawn in each site's colours (no logo
/// files), Kino's own icon for Play, Windows' folder for Open on disk, and the user's icon pack (assets/icons) for
/// details, watchlist, close and remove. All at the size of the control's font, so they scale with the display.
/// </summary>
internal static class SiteIcons
{
    private static readonly Color ImdbYellow = Color.FromArgb(0xF5, 0xC5, 0x18);
    private static readonly Color TmdbNavy = Color.FromArgb(0x0D, 0x25, 0x3F);
    private static readonly Color TmdbGreen = Color.FromArgb(0x90, 0xCE, 0xA1);
    private static readonly Color TmdbBlue = Color.FromArgb(0x01, 0xB4, 0xE4);
    private static readonly Color SubDlSlate = Color.FromArgb(0x1F, 0x29, 0x37);

    public static void Imdb(Button button) => OnButton(button, Badge("IMDb", button.Font, ImdbYellow, ImdbYellow, Color.Black));

    public static void Tmdb(Button button) => OnButton(button, TmdbBadge(button.Font));

    public static void SubDl(Button button) => OnButton(button, Badge("SubDL", button.Font, SubDlSlate, SubDlSlate, Color.White));

    public static void Imdb(ToolStripMenuItem item) => OnMenu(item, Badge("IMDb", item.Font, ImdbYellow, ImdbYellow, Color.Black));

    public static void Tmdb(ToolStripMenuItem item) => OnMenu(item, TmdbBadge(item.Font));

    /// <summary>Kino's own icon (the red film), as on every window.</summary>
    public static void Play(Button button)
    {
        if (AppIcon.Icon is not { } icon) return;
        var size = IconSize(button.Font);
        using var sized = new Icon(icon, size, size);
        OnButton(button, sized.ToBitmap());
    }

    /// <summary>Windows' own folder icon, as Explorer shows it.</summary>
    public static void Folder(Button button) => OnButton(button, Stock(StockIconId.Folder, button.Font));

    public static void Folder(ToolStripMenuItem item) => OnMenu(item, Stock(StockIconId.Folder, item.Font));

    /// <summary>The pack's blue "i", for Show details.</summary>
    public static void Details(ToolStripMenuItem item) => OnMenu(item, Pack("info", item.Font) ?? Stock(StockIconId.Info, item.Font));

    /// <summary>
    /// The pack's database with a small red ✕ on it: the entry leaves Kino's library, the file stays. (A bin
    /// would look like deleting the file.)
    /// </summary>
    public static void RemoveFromLibrary(ToolStripMenuItem item)
    {
        if (Pack("database", item.Font) is not { } database) return;
        var size = database.Width;
        var badge = size * 11 / 20;
        using (var g = Graphics.FromImage(database))
        using (var delete = PackIcon("delete", badge))
        {
            if (delete is not null) g.DrawIcon(delete, new Rectangle(size - badge, size - badge, badge, badge));
        }
        OnMenu(item, database);
    }

    /// <summary>The pack's pencil on paper, for Set IMDb rating.</summary>
    public static void Edit(ToolStripMenuItem item)
    {
        if (Pack("edit", item.Font) is { } image) OnMenu(item, image);
    }

    /// <summary>The pack's heart with a plus, for Add to watchlist.</summary>
    public static void Watchlist(Button button)
    {
        if (Pack("add-favorite", button.Font) is { } image) OnButton(button, image);
    }

    /// <summary>The pack's red ✕, for Close.</summary>
    public static void Close(Button button)
    {
        if (Pack("delete", button.Font) is { } image) OnButton(button, image);
    }

    private static Bitmap Stock(StockIconId id, Font font)
    {
        using var icon = SystemIcons.GetStockIcon(id, IconSize(font));
        return icon.ToBitmap();
    }

    /// <summary>An icon from assets/icons (embedded) at the size for <paramref name="font"/>, or null if it's missing.</summary>
    private static Bitmap? Pack(string name, Font font)
    {
        using var icon = PackIcon(name, IconSize(font));
        return icon?.ToBitmap();
    }

    private static Icon? PackIcon(string name, int size)
    {
        using var stream = typeof(SiteIcons).Assembly.GetManifestResourceStream($"Kolibri.Kino.WinForms.icons.{name}.ico");
        return stream is null ? null : new Icon(stream, size, size);
    }

    /// <summary>Square icons a little taller than the text, so they read as icons next to it.</summary>
    private static int IconSize(Font font) => Math.Max(16, font.Height + 2);

    private static Bitmap TmdbBadge(Font font) => Badge("TMDB", font, TmdbGreen, TmdbBlue, TmdbNavy);

    private static void OnButton(Button button, Bitmap badge)
    {
        button.Image = badge;
        button.ImageAlign = ContentAlignment.MiddleLeft;
        button.TextImageRelation = TextImageRelation.ImageBeforeText;
    }

    private static void OnMenu(ToolStripMenuItem item, Bitmap badge)
    {
        item.Image = badge;
        // Wider than the 16×16 image column: shown as drawn, not squeezed.
        item.ImageScaling = ToolStripItemImageScaling.None;
    }

    /// <summary>A rounded badge with bold text, as high as a line of <paramref name="font"/>, filled left to right.</summary>
    private static Bitmap Badge(string text, Font font, Color left, Color right, Color textColor)
    {
        var height = Math.Max(16, font.Height + 2);
        using var bold = new Font(font.FontFamily, height * 0.6f, FontStyle.Bold, GraphicsUnit.Pixel);
        var textWidth = TextRenderer.MeasureText(text, bold, Size.Empty, TextFormatFlags.NoPadding).Width;
        var bitmap = new Bitmap(textWidth + height / 2, height);

        using var g = Graphics.FromImage(bitmap);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

        var bounds = new Rectangle(0, 1, bitmap.Width - 1, height - 3);
        using (var path = Rounded(bounds, height / 5))
        using (var fill = new LinearGradientBrush(bounds, left, right, LinearGradientMode.Horizontal))
            g.FillPath(fill, path);

        using var brush = new SolidBrush(textColor);
        using var centred = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        g.DrawString(text, bold, brush, new RectangleF(0, 0, bitmap.Width, height - 1), centred);
        return bitmap;
    }

    private static GraphicsPath Rounded(Rectangle r, int radius)
    {
        var d = Math.Max(2, radius * 2);
        var path = new GraphicsPath();
        path.AddArc(r.Left, r.Top, d, d, 180, 90);
        path.AddArc(r.Right - d, r.Top, d, d, 270, 90);
        path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        path.AddArc(r.Left, r.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }
}
