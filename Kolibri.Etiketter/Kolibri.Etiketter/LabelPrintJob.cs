using System.Drawing.Printing;

namespace Kolibri.Etiketter;

/// <summary>Skriver ut samme etikett på de valgte posisjonene, på et gitt antall ark.</summary>
public sealed class LabelPrintJob : IDisposable
{
    private const float HundredthInchToMm = 0.254f;

    private readonly LabelDesign _design;
    private readonly int[] _positions;
    private readonly int _sheetCount;
    private readonly float _offsetXMm;
    private readonly float _offsetYMm;
    private readonly bool _printGuides;
    private int _sheetsPrinted;

    public LabelPrintJob(LabelDesign design, IEnumerable<int> positions, int sheetCount, float offsetXMm, float offsetYMm, bool printGuides)
    {
        _design = design;
        _positions = positions.Where(i => i >= 0 && i < LabelSheet.LabelsPerSheet).Distinct().Order().ToArray();
        _sheetCount = Math.Max(1, sheetCount);
        _offsetXMm = offsetXMm;
        _offsetYMm = offsetYMm;
        _printGuides = printGuides;

        Document = new PrintDocument { DocumentName = "Etiketter" };
        Document.BeginPrint += (_, _) => _sheetsPrinted = 0;
        Document.PrintPage += OnPrintPage;
    }

    public PrintDocument Document { get; }

    /// <summary>Antall ark utskriften bruker.</summary>
    public int SheetCount => _sheetCount;

    /// <summary>Velger en installert skriver ved navn, dersom den finnes. Ellers brukes Windows' standardskriver.</summary>
    public void TrySelectPrinter(string? printerName)
    {
        if (string.IsNullOrEmpty(printerName))
            return;
        foreach (string installed in PrinterSettings.InstalledPrinters)
        {
            if (string.Equals(installed, printerName, StringComparison.OrdinalIgnoreCase))
            {
                Document.PrinterSettings.PrinterName = installed;
                return;
            }
        }
    }

    /// <summary>A4 stående uten marger. Kalles etter at skriveren er valgt.</summary>
    public void ApplyPageSettings()
    {
        PageSettings page = Document.DefaultPageSettings;
        page.Landscape = false;
        page.Margins = new Margins(0, 0, 0, 0);
        try
        {
            foreach (PaperSize size in Document.PrinterSettings.PaperSizes)
            {
                if (size.Kind == PaperKind.A4)
                {
                    page.PaperSize = size;
                    break;
                }
            }
        }
        catch (InvalidPrinterException)
        {
            // Skriveren er ikke tilgjengelig; utskriften feiler senere med en tydelig melding.
        }
    }

    private void OnPrintPage(object? sender, PrintPageEventArgs e)
    {
        Graphics g = e.Graphics!;
        g.PageUnit = GraphicsUnit.Millimeter;
        g.PageScale = 1f;

        // På papir starter koordinatene i skriverens utskrivbare område, ikke i arkets hjørne.
        // Flytt origo tilbake til arkets hjørne slik at etikettene havner riktig.
        bool isPreview = Document.PrintController?.IsPreview == true;
        if (!isPreview)
        {
            g.TranslateTransform(
                -e.PageSettings.HardMarginX * HundredthInchToMm,
                -e.PageSettings.HardMarginY * HundredthInchToMm);
        }

        LabelRenderMode mode = _printGuides ? LabelRenderMode.PrintWithGuides : LabelRenderMode.Print;
        foreach (int position in _positions)
        {
            RectangleF label = LabelSheet.GetLabelRect(position, _offsetXMm, _offsetYMm);
            LabelRenderer.Draw(g, _design, label, mode);
        }

        _sheetsPrinted++;
        e.HasMorePages = _sheetsPrinted < _sheetCount;
    }

    public void Dispose() => Document.Dispose();
}
