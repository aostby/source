using System.Drawing.Drawing2D;

namespace Kolibri.Etiketter;

/// <summary>
/// Standardbildet på nye etiketter: en 5,25" diskett, tegnet her i koden (ingen logo, tekst eller lånt bilde).
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

        // Den firkantede svarte konvolutten, med skrivebeskyttelseshakket på høyre side.
        var jacket = new RectangleF(2f, 2f, 96f, 96f);
        using (var path = new GraphicsPath { FillMode = FillMode.Alternate })
        {
            AddRounded(path, jacket, 1.5f);
            using var body = new LinearGradientBrush(jacket, Color.FromArgb(52, 52, 56), Color.FromArgb(14, 14, 16), LinearGradientMode.ForwardDiagonal);
            g.FillPath(body, path);
            using var edge = new Pen(Color.FromArgb(8, 8, 10), 0.5f);
            g.DrawPath(edge, path);
        }

        // Sømmen (de brettede klaffene) bak på konvolutten, svakt synlig.
        using (var seam = new Pen(Color.FromArgb(40, 255, 255, 255), 0.35f))
        {
            g.DrawLine(seam, 4f, 95.5f, 96f, 95.5f);
            g.DrawLine(seam, 4.5f, 4f, 4.5f, 96f);
        }

        // Den hvite papiretiketten øverst, med svake skrivelinjer.
        var label = new RectangleF(8f, 6f, 62f, 17f);
        using (var paper = new SolidBrush(Color.FromArgb(248, 247, 240)))
            g.FillRectangle(paper, label);
        using (var line = new Pen(Color.FromArgb(205, 212, 225), 0.4f))
        {
            for (float y = label.Y + 5f; y < label.Bottom - 1.5f; y += 4.5f)
                g.DrawLine(line, label.X + 3f, y, label.Right - 3f, y);
        }

        // Midthullet: den brune magnetplaten med forsterkningsringen (navet), og hullet i midten.
        var center = new PointF(50f, 52f);
        using (var media = new SolidBrush(Color.FromArgb(92, 62, 40)))
            FillCircle(g, media, center, 15.5f);
        using (var hub = new LinearGradientBrush(new RectangleF(center.X - 13f, center.Y - 13f, 26f, 26f),
                   Color.FromArgb(176, 150, 112), Color.FromArgb(118, 92, 64), LinearGradientMode.ForwardDiagonal))
            FillCircle(g, hub, center, 13f);
        using (var jacketRing = new Pen(Color.FromArgb(10, 10, 12), 0.6f))
            g.DrawEllipse(jacketRing, center.X - 15.5f, center.Y - 15.5f, 31f, 31f);

        // Det avlange lese-/skrivevinduet under midthullet, der magnetplaten synes.
        var slot = new RectangleF(45.5f, 72f, 9f, 23f);
        using (var slotPath = new GraphicsPath())
        {
            AddRounded(slotPath, slot, 4.5f);
            using var media = new LinearGradientBrush(slot, Color.FromArgb(104, 72, 46), Color.FromArgb(70, 46, 30), LinearGradientMode.Horizontal);
            g.FillPath(media, slotPath);
        }

        // Indekshullet ved siden av midthullet.
        using (var media = new SolidBrush(Color.FromArgb(80, 54, 34)))
            FillCircle(g, media, new PointF(69f, 56f), 1.8f);

        // Hullet i midten og skrivebeskyttelseshakket er gjennomsiktige.
        g.CompositingMode = CompositingMode.SourceCopy;
        using (var clear = new SolidBrush(Color.Transparent))
        {
            FillCircle(g, clear, center, 7f);
            g.FillRectangle(clear, 93.5f, 17f, 6.5f, 6f);
        }
        g.CompositingMode = CompositingMode.SourceOver;
        using (var holeEdge = new Pen(Color.FromArgb(90, 70, 48), 0.5f))
            g.DrawEllipse(holeEdge, center.X - 7f, center.Y - 7f, 14f, 14f);

        return bitmap;
    }

    private static void FillCircle(Graphics g, Brush brush, PointF center, float radius) =>
        g.FillEllipse(brush, center.X - radius, center.Y - radius, radius * 2, radius * 2);

    private static void AddRounded(GraphicsPath path, RectangleF r, float radius)
    {
        float d = radius * 2;
        path.AddArc(r.X, r.Y, d, d, 180, 90);
        path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
    }
}
