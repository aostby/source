using System.Drawing.Printing;
using System.Text;

namespace Kolibri.Etiketter;

/// <summary>
/// «Hjelp > Skriverdiagnose»: viser hva som faktisk sendes til skriverdriveren (papir, kant, kopier, tosidig,
/// antall sider), uten å skrive ut noe.
/// </summary>
public static class PrinterDiagnostics
{
    private const float HundredthInchToMm = 0.254f;

    public static string Build(LabelDesign design, LabelSheet sheet, IReadOnlyList<int> positions, int sheetCount,
        string? printerName, PrinterProfile profile)
    {
        var text = new StringBuilder();
        text.AppendLine($"Kolibri.Etiketter – skriverdiagnose {DateTime.Now:g}");
        text.AppendLine();
        text.AppendLine("ETIKETTARK");
        text.AppendLine($"  {sheet}");
        text.AppendLine($"  {sheet.Columns} × {sheet.Rows}, etikett {sheet.LabelWidthMm:0.##} × {sheet.LabelHeightMm:0.##} mm, " +
                        $"første etikett {sheet.TopMm:0.##} mm fra toppen og {sheet.LeftMm:0.##} mm fra venstre, " +
                        $"mellomrom {sheet.GapXMm:0.##} / {sheet.GapYMm:0.##} mm");
        text.AppendLine($"  Valgte posisjoner: {positions.Count} av {sheet.LabelsPerSheet} ({string.Join(", ", positions.Select(p => p + 1))})");
        text.AppendLine($"  Antall ark valgt: {sheetCount}");
        text.AppendLine();

        text.AppendLine("SKRIVER");
        if (string.IsNullOrEmpty(printerName))
        {
            text.AppendLine("  Ingen skriver valgt eller installert.");
            return text.ToString();
        }
        text.AppendLine($"  {printerName}{(PrinterModel.Find(printerName) is { } model ? $" ({model.Name})" : "")}");
        text.AppendLine($"  Justering i programmet: høyre {profile.OffsetXMm:0.0} mm, ned {profile.OffsetYMm:0.0} mm" +
                        (profile.CalibratedAt is { } at ? $", kalibrert {at:g}" : ", ikke kalibrert"));

        using var job = new LabelPrintJob(design, sheet, positions, sheetCount, profile.OffsetXMm, profile.OffsetYMm, false);
        job.TrySelectPrinter(printerName);
        try
        {
            job.ApplyPageSettings();
            PrinterSettings printer = job.Document.PrinterSettings;
            PageSettings page = job.Document.DefaultPageSettings;
            text.AppendLine($"  Gyldig: {(printer.IsValid ? "ja" : "NEI")}, Windows' standardskriver: {(printer.IsDefaultPrinter ? "ja" : "nei")}");
            text.AppendLine($"  Farger: {(printer.SupportsColor ? "ja" : "nei")} (programmet ber om {(page.Color ? "farger" : "gråtoner")})");
            text.AppendLine($"  Kopier: {printer.Copies}, sortert: {(printer.Collate ? "ja" : "nei")}, tosidig: {printer.Duplex}");
            text.AppendLine();
            text.AppendLine("PAPIR SOM SENDES TIL DRIVEREN");
            text.AppendLine($"  A4 funnet i driverens papirliste: {(job.A4Found ? "ja" : "NEI – driveren kan legge ut siden feil")}");
            text.AppendLine($"  Papir: {page.PaperSize.PaperName} ({page.PaperSize.Kind}), " +
                            $"{page.PaperSize.Width * HundredthInchToMm:0.#} × {page.PaperSize.Height * HundredthInchToMm:0.#} mm, " +
                            $"{(page.Landscape ? "LIGGENDE" : "stående")}");
            RectangleF area = page.PrintableArea;
            text.AppendLine($"  Ikke-utskrivbar kant (HardMargin): venstre {page.HardMarginX * HundredthInchToMm:0.#} mm, topp {page.HardMarginY * HundredthInchToMm:0.#} mm");
            text.AppendLine($"  Utskrivbart område: {area.X * HundredthInchToMm:0.#}, {area.Y * HundredthInchToMm:0.#} mm, " +
                            $"{area.Width * HundredthInchToMm:0.#} × {area.Height * HundredthInchToMm:0.#} mm");

            // Tell sidene uten å skrive ut.
            int pages = 0;
            job.Document.PrintPage += (_, _) => pages++;
            job.Document.PrintController = new PreviewPrintController();
            job.Document.Print();
            text.AppendLine();
            text.AppendLine($"SIDER: programmet lager {pages} side(r) × {printer.Copies} kopi(er) = {pages * printer.Copies} ark.");
            if (pages * printer.Copies > sheetCount)
                text.AppendLine("  OBS: flere ark enn valgt – sjekk antall kopier i skriverdialogen.");
            text.AppendLine("  Kommer det likevel flere ark ut, er det skriveren: ofte har to etikettark klistret seg sammen i skuffen.");
        }
        catch (Exception ex)
        {
            text.AppendLine($"  Kunne ikke lese fra driveren: {ex.Message}");
        }
        return text.ToString();
    }

    /// <summary>Viser diagnosen i et vindu der teksten kan kopieres.</summary>
    public static void Show(IWin32Window owner, string report)
    {
        using var form = new Form
        {
            Text = "Skriverdiagnose",
            Size = new Size(760, 560),
            StartPosition = FormStartPosition.CenterParent,
            ShowInTaskbar = false,
            MinimizeBox = false,
        };
        var box = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Both,
            WordWrap = false,
            Dock = DockStyle.Fill,
            Font = new Font("Consolas", 9.5f),
            Text = report.Replace("\n", "\r\n").Replace("\r\r\n", "\r\n"),
        };
        var copy = new Button { Text = "Kopier", AutoSize = true };
        copy.Click += (_, _) => Clipboard.SetText(report);
        var close = new Button { Text = "Lukk", AutoSize = true, DialogResult = DialogResult.OK };
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, AutoSize = true, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(6) };
        buttons.Controls.Add(close);
        buttons.Controls.Add(copy);
        form.Controls.Add(box);
        form.Controls.Add(buttons);
        form.AcceptButton = close;
        form.ShowDialog(owner);
    }
}
