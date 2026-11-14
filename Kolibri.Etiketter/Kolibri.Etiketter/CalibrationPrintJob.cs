using System.Drawing.Drawing2D;
using System.Drawing.Printing;

namespace Kolibri.Etiketter;

/// <summary>
/// Kalibreringsark: kryss med millimeterskala 20 mm fra hvert hjørne, omrisset av etikettene og skriverens
/// utskrivbare område. Skrives ut uten justering, så målene viser hvor mye skriveren bommer.
/// </summary>
public sealed class CalibrationPrintJob : IDisposable
{
    /// <summary>Avstanden fra arkets kanter til midten av hvert kryss.</summary>
    public const float CrossDistanceMm = 20f;

    private const float HundredthInchToMm = 0.254f;
    private readonly LabelSheet _sheet;

    public CalibrationPrintJob(LabelSheet sheet, string printerName)
    {
        _sheet = sheet;
        Document = new PrintDocument { DocumentName = "Kalibreringsark" };
        LabelPrintJob.TrySelectPrinter(Document, printerName);
        LabelPrintJob.ApplyPageSettings(Document);
        Document.PrintPage += OnPrintPage;
    }

    public PrintDocument Document { get; }

    private void OnPrintPage(object? sender, PrintPageEventArgs e)
    {
        Graphics g = e.Graphics!;
        LabelPrintJob.UsePaperCoordinates(Document, e);
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Etikettene (uten justering), lyst, så arket kan legges over et etikettark mot lyset.
        using (var labelPen = new Pen(Color.Silver, 0.1f))
        {
            for (int i = 0; i < _sheet.LabelsPerSheet; i++)
                LabelRenderer.DrawOutline(g, labelPen, _sheet, _sheet.GetLabelRect(i));
        }

        // Skriverens utskrivbare område: alt utenfor den stiplede linjen kuttes av skriveren.
        bool preview = Document.PrintController?.IsPreview == true;
        if (!preview)
        {
            RectangleF area = e.PageSettings.PrintableArea;
            var printable = new RectangleF(area.X * HundredthInchToMm, area.Y * HundredthInchToMm,
                area.Width * HundredthInchToMm, area.Height * HundredthInchToMm);
            printable.Inflate(-0.2f, -0.2f);
            using var areaPen = new Pen(Color.Gray, 0.15f) { DashStyle = DashStyle.Dash };
            g.DrawRectangle(areaPen, printable.X, printable.Y, printable.Width, printable.Height);
        }

        float right = LabelSheet.PageWidthMm - CrossDistanceMm;
        float bottom = LabelSheet.PageHeightMm - CrossDistanceMm;
        using var font = new Font("Segoe UI", 7f);
        using var small = new Font("Segoe UI", 6f);
        DrawCross(g, CrossDistanceMm, CrossDistanceMm, small, "A: mål fra venstre og øvre kant", true, true);
        DrawCross(g, right, CrossDistanceMm, small, "mål fra høyre og øvre kant", false, true);
        DrawCross(g, CrossDistanceMm, bottom, small, "mål fra venstre og nedre kant", true, false);
        DrawCross(g, right, bottom, small, "B: mål fra høyre og nedre kant", false, false);

        // Forklaring midt på arket.
        using var title = new Font("Segoe UI", 12f, FontStyle.Bold);
        string printer = Document.PrinterSettings.PrinterName;
        string text =
            $"Skriver: {printer}\n" +
            $"Etikettark: {_sheet}\n" +
            $"Skrevet ut {DateTime.Now:d. MMMM yyyy HH:mm}, uten justering.\n\n" +
            $"1. Mål med linjal fra arkets venstre kant og øvre kant til midten av kryss A. Begge skal være {CrossDistanceMm:0} mm.\n" +
            "2. Mål gjerne også kryss B fra høyre og nedre kant (gir et gjennomsnitt, og fanger opp om skriveren strekker arket).\n" +
            "3. Skriv målene inn i «Kalibrer skriver» i Kolibri.Etiketter. Programmet regner ut justeringen.\n\n" +
            "Stiplet linje: skriverens utskrivbare område. Tekst og bilder utenfor den blir kuttet bort.\n" +
            "Grå ruter: etikettene på arket. Legg arket over et etikettark mot lyset for å se treffet.";
        var box = new RectangleF(35f, 95f, LabelSheet.PageWidthMm - 70f, 110f);
        using (var background = new SolidBrush(Color.White))
            g.FillRectangle(background, box);
        g.DrawString("Kalibreringsark – Kolibri.Etiketter", title, Brushes.Black, box.X, box.Y);
        g.DrawString(text, font, Brushes.Black, new RectangleF(box.X, box.Y + 8f, box.Width, box.Height - 8f));

        // Fargeprøve: viser om skriveren faktisk skriver i farger (HP Color Laser 150nw) eller gråtoner.
        Color[] colors = [Color.Cyan, Color.Magenta, Color.Yellow, Color.Black, Color.Red, Color.Green, Color.Blue];
        float x = box.X;
        float y = box.Bottom - 12f;
        g.DrawString("Fargeprøve:", font, Brushes.Black, x, y - 4.5f);
        foreach (Color color in colors)
        {
            using var brush = new SolidBrush(color);
            g.FillRectangle(brush, x, y, 8f, 8f);
            x += 10f;
        }

        e.HasMorePages = false;
    }

    /// <summary>Kryss med millimeterstreker (lange streker for hver 5. mm) i ±12 mm rundt midten.</summary>
    private static void DrawCross(Graphics g, float cx, float cy, Font font, string caption, bool fromLeft, bool fromTop)
    {
        const float arm = 12f;
        using var pen = new Pen(Color.Black, 0.12f);
        g.DrawLine(pen, cx - arm, cy, cx + arm, cy);
        g.DrawLine(pen, cx, cy - arm, cx, cy + arm);
        g.DrawEllipse(pen, cx - 1f, cy - 1f, 2f, 2f);
        for (int mm = -(int)arm; mm <= arm; mm++)
        {
            if (mm == 0) continue;
            float tick = mm % 5 == 0 ? 1.5f : 0.7f;
            g.DrawLine(pen, cx + mm, cy - tick, cx + mm, cy + tick);
            g.DrawLine(pen, cx - tick, cy + mm, cx + tick, cy + mm);
        }

        SizeF size = g.MeasureString(caption, font);
        float tx = fromLeft ? cx + 2f : cx - 2f - size.Width;
        float ty = fromTop ? cy + arm + 1f : cy - arm - 1f - size.Height;
        g.DrawString(caption, font, Brushes.Black, tx, ty);
    }

    public void Dispose() => Document.Dispose();
}
