using System.Drawing.Printing;

namespace Kolibri.Etiketter;

/// <summary>Skriver ut samme etikett på de valgte posisjonene, på et gitt antall ark.</summary>
public sealed class LabelPrintJob : IDisposable
{
    private const float HundredthInchToMm = 0.254f;

    private readonly LabelDesign _design;
    private readonly LabelSheet _sheet;
    private readonly int[] _positions;
    private readonly int _sheetCount;
    private readonly bool _printGuides;
    private int _sheetsPrinted;

    public LabelPrintJob(LabelDesign design, LabelSheet sheet, IEnumerable<int> positions, int sheetCount, float offsetXMm, float offsetYMm, bool printGuides)
    {
        _design = design;
        _sheet = sheet;
        _positions = positions.Where(i => i >= 0 && i < sheet.LabelsPerSheet).Distinct().Order().ToArray();
        _sheetCount = Math.Max(1, sheetCount);
        OffsetXMm = offsetXMm;
        OffsetYMm = offsetYMm;
        _printGuides = printGuides;

        Document = new PrintDocument { DocumentName = "Etiketter" };
        Document.BeginPrint += (_, _) => _sheetsPrinted = 0;
        Document.PrintPage += OnPrintPage;
    }

    public PrintDocument Document { get; }

    /// <summary>Justering mot høyre i mm. Kan endres etter skriverdialogen, når skriveren er valgt.</summary>
    public float OffsetXMm { get; set; }

    /// <summary>Justering nedover i mm.</summary>
    public float OffsetYMm { get; set; }

    /// <summary>Antall ark utskriften bruker.</summary>
    public int SheetCount => _sheetCount;

    /// <summary>Velger en installert skriver ved navn, dersom den finnes. Ellers brukes Windows' standardskriver.</summary>
    public void TrySelectPrinter(string? printerName) => TrySelectPrinter(Document, printerName);

    /// <summary>A4 stående uten marger, i farger på fargeskrivere. Kalles etter at skriveren er valgt.</summary>
    public void ApplyPageSettings() => A4Found = ApplyPageSettings(Document);

    /// <summary>False dersom skriverdriveren ikke har A4 (da ber programmet om et eget A4-format, som ikke alle drivere godtar).</summary>
    public bool A4Found { get; private set; } = true;

    /// <inheritdoc cref="TrySelectPrinter(string?)"/>
    public static void TrySelectPrinter(PrintDocument document, string? printerName)
    {
        if (string.IsNullOrEmpty(printerName))
            return;
        foreach (string installed in PrinterSettings.InstalledPrinters)
        {
            if (string.Equals(installed, printerName, StringComparison.OrdinalIgnoreCase))
            {
                document.PrinterSettings.PrinterName = installed;
                return;
            }
        }
    }

    /// <inheritdoc cref="ApplyPageSettings()"/>
    public static bool ApplyPageSettings(PrintDocument document)
    {
        PageSettings page = document.DefaultPageSettings;
        page.Landscape = false;
        page.Margins = new Margins(0, 0, 0, 0);
        try
        {
            PaperSize? a4 = FindA4(document.PrinterSettings);
            page.PaperSize = a4 ?? new PaperSize("A4", A4WidthHundredthInch, A4HeightHundredthInch);
            // Mange fargeskrivere (bl.a. HP Color Laser 150nw) skriver i gråtoner til programmet ber om farger.
            page.Color = document.PrinterSettings.SupportsColor;
            return a4 is not null;
        }
        catch (InvalidPrinterException)
        {
            // Skriveren er ikke tilgjengelig; utskriften feiler senere med en tydelig melding.
            return true;
        }
    }

    private const int A4WidthHundredthInch = 827;
    private const int A4HeightHundredthInch = 1169;

    /// <summary>A4 i driverens papirliste: helst etter type, ellers etter navn eller mål (noen drivere merker A4 som egendefinert).</summary>
    public static PaperSize? FindA4(PrinterSettings printer)
    {
        PaperSize[] sizes = printer.PaperSizes.Cast<PaperSize>().ToArray();
        return sizes.FirstOrDefault(s => s.Kind == PaperKind.A4)
            ?? sizes.FirstOrDefault(s => Math.Abs(s.Width - A4WidthHundredthInch) <= 3 && Math.Abs(s.Height - A4HeightHundredthInch) <= 3)
            ?? sizes.FirstOrDefault(s => s.PaperName.Trim().Equals("A4", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Setter mm som enhet og flytter origo til arkets øvre venstre hjørne. På papir starter koordinatene
    /// i skriverens utskrivbare område, ikke i arkets hjørne.
    /// </summary>
    public static void UsePaperCoordinates(PrintDocument document, PrintPageEventArgs e)
    {
        Graphics g = e.Graphics!;
        g.PageUnit = GraphicsUnit.Millimeter;
        g.PageScale = 1f;
        if (document.PrintController?.IsPreview != true)
        {
            g.TranslateTransform(
                -e.PageSettings.HardMarginX * HundredthInchToMm,
                -e.PageSettings.HardMarginY * HundredthInchToMm);
        }
    }

    private void OnPrintPage(object? sender, PrintPageEventArgs e)
    {
        Graphics g = e.Graphics!;
        UsePaperCoordinates(Document, e);

        LabelRenderMode mode = _printGuides ? LabelRenderMode.PrintWithGuides : LabelRenderMode.Print;
        foreach (int position in _positions)
        {
            RectangleF label = _sheet.GetLabelRect(position, OffsetXMm, OffsetYMm);
            LabelRenderer.Draw(g, _design, _sheet, label, mode);
        }

        _sheetsPrinted++;
        e.HasMorePages = _sheetsPrinted < _sheetCount;
    }

    public void Dispose() => Document.Dispose();
}
