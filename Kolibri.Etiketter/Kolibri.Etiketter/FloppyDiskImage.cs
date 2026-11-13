using System.Drawing.Drawing2D;

namespace Kolibri.Etiketter;

/// <summary>
/// Standardbildet på nye etiketter: en 3,5" diskett, tegnet her i koden (ingen logo, tekst eller lånt bilde).
/// </summary>
public static class FloppyDiskImage
{
    /// <summary>Tegner disketten på gjennomsiktig bakgrunn, <paramref name="size"/> × <paramref name="size"/> piksler.</summary>
    public static Bitmap Create(int size = 600)
    {
        var bitmap = new Bitmap(size, size, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(bitmap);
        g.Clear(Color.Transparent);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        // Tegn i en 100 × 100-rute, uansett bildestørrelse.
        g.ScaleTransform(size / 100f, size / 100f);

        // Kroppen: mørk plast med avrundede hjørner og ett avskåret hjørne øverst til høyre.
        const float l = 3f, t = 3f, r = 97f, b = 97f, round = 2.5f, chamfer = 6f;
        using (var body = new GraphicsPath())
        {
            body.AddArc(l, t, round * 2, round * 2, 180, 90);
            body.AddLine(l + round, t, r - chamfer, t);
            body.AddLine(r - chamfer, t, r, t + chamfer);
            body.AddArc(r - round * 2, b - round * 2, round * 2, round * 2, 0, 90);
            body.AddArc(l, b - round * 2, round * 2, round * 2, 90, 90);
            body.CloseFigure();

            using var bodyBrush = new LinearGradientBrush(new RectangleF(l, t, r - l, b - t),
                Color.FromArgb(70, 76, 88), Color.FromArgb(34, 37, 44), LinearGradientMode.ForwardDiagonal);
            g.FillPath(bodyBrush, body);
            using var edge = new Pen(Color.FromArgb(20, 22, 26), 0.6f);
            g.DrawPath(edge, body);
        }

        // Fordypningen der lukkeren glir.
        using (var recess = new SolidBrush(Color.FromArgb(28, 30, 36)))
            g.FillRectangle(recess, 20f, t, 62f, 34f);

        // Metallukkeren med lese-/skriveåpningen.
        var shutter = new RectangleF(26f, t, 44f, 33f);
        using (var metal = new LinearGradientBrush(shutter, Color.FromArgb(222, 225, 230), Color.FromArgb(150, 156, 165), LinearGradientMode.Horizontal))
            g.FillRectangle(metal, shutter);
        using (var metalEdge = new Pen(Color.FromArgb(110, 115, 124), 0.5f))
            g.DrawRectangle(metalEdge, shutter.X, shutter.Y, shutter.Width, shutter.Height);
        using (var slot = new SolidBrush(Color.FromArgb(45, 48, 55)))
            g.FillRectangle(slot, 52f, t + 5f, 11f, 23f);
        using (var shine = new SolidBrush(Color.FromArgb(70, 255, 255, 255)))
            g.FillRectangle(shine, 28f, t + 2f, 3f, 29f);

        // Papiretiketten med svake skrivelinjer.
        var paper = new RectangleF(13f, 46f, 74f, 49f);
        using (var paperPath = RoundedTop(paper, 2f))
        {
            using var paperBrush = new SolidBrush(Color.FromArgb(250, 249, 244));
            g.FillPath(paperBrush, paperPath);
        }
        using (var stripe = new SolidBrush(Color.FromArgb(205, 215, 230)))
            g.FillRectangle(stripe, paper.X, paper.Y + 5f, paper.Width, 1.4f);
        using (var line = new Pen(Color.FromArgb(215, 222, 232), 0.5f))
        {
            for (float y = paper.Y + 14f; y < paper.Bottom - 4f; y += 8f)
                g.DrawLine(line, paper.X + 5f, y, paper.Right - 5f, y);
        }

        // Skrivebeskyttelsen (nede til venstre) og hullet nede til høyre.
        using (var hole = new SolidBrush(Color.FromArgb(15, 16, 19)))
        {
            g.FillRectangle(hole, 5.5f, 86f, 5f, 6.5f);
            g.FillRectangle(hole, 89.5f, 86f, 5f, 6.5f);
        }
        using (var slider = new SolidBrush(Color.FromArgb(95, 100, 110)))
            g.FillRectangle(slider, 5.5f, 89.5f, 5f, 3f);

        return bitmap;
    }

    private static GraphicsPath RoundedTop(RectangleF r, float radius)
    {
        var path = new GraphicsPath();
        path.AddArc(r.X, r.Y, radius * 2, radius * 2, 180, 90);
        path.AddArc(r.Right - radius * 2, r.Y, radius * 2, radius * 2, 270, 90);
        path.AddLine(r.Right, r.Bottom, r.X, r.Bottom);
        path.CloseFigure();
        return path;
    }
}
