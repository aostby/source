using System.Drawing.Printing;

namespace Kolibri.Etiketter;

/// <summary>Kalibreringen som huskes for én skriver.</summary>
public sealed class PrinterProfile
{
    /// <summary>Flytter alle etiketter mot høyre (negativ = mot venstre), i mm.</summary>
    public float OffsetXMm { get; set; }

    /// <summary>Flytter alle etiketter nedover (negativ = oppover), i mm.</summary>
    public float OffsetYMm { get; set; }

    /// <summary>Når justeringen sist ble målt med kalibreringsarket.</summary>
    public DateTime? CalibratedAt { get; set; }
}

/// <summary>En skrivermodell programmet kjenner, gjenkjent på en del av skrivernavnet i Windows.</summary>
/// <param name="Match">Tekst som skal finnes i skrivernavnet (uten hensyn til store og små bokstaver).</param>
/// <param name="Name">Navnet som vises.</param>
/// <param name="Advice">Råd om etikettutskrift på denne skriveren.</param>
public sealed record PrinterModel(string Match, string Name, string Advice)
{
    private const string PaperTypeAdvice =
        "Velg papirtype «Etiketter» (Labels) under Innstillinger/Egenskaper i utskriftsdialogen, eller som standard i " +
        "Windows' utskriftsinnstillinger for skriveren, så fester toneren seg godt på etikettene.";

    /// <summary>Skrivere med egne råd. Justeringen starter på 0 til skriveren er kalibrert.</summary>
    public static IReadOnlyList<PrinterModel> Known { get; } =
    [
        new("Color Laser 15", "HP Color Laser 150-serien (150a/150nw)",
            "Fargeskriver: farget tekst og bilder skrives ut i farger. " + PaperTypeAdvice +
            " Tar arket skjevt, legg færre etikettark i skuffen og skyv papirførerne inntil arkene."),
        new("150nw", "HP Color Laser 150nw",
            "Fargeskriver: farget tekst og bilder skrives ut i farger. " + PaperTypeAdvice),
        new("M402", "HP LaserJet Pro M402dne",
            "Svart-hvitt-skriver: farger skrives ut som gråtoner. " + PaperTypeAdvice +
            " Skriv ikke tosidig på etikettark."),
    ];

    /// <summary>Råd for skrivere programmet ikke kjenner.</summary>
    public static string GenericAdvice =>
        "Har skriveren papirtypen «Etiketter» (Labels), velg den under Innstillinger/Egenskaper i utskriftsdialogen.";

    /// <summary>Modellen som passer til skrivernavnet, eller null.</summary>
    public static PrinterModel? Find(string? printerName) =>
        string.IsNullOrEmpty(printerName)
            ? null
            : Known.FirstOrDefault(m => printerName.Contains(m.Match, StringComparison.OrdinalIgnoreCase));
}

/// <summary>Skriverens ikke-utskrivbare kant på A4 stående, i mm.</summary>
public readonly record struct PrinterMarginsMm(float Left, float Top, float Right, float Bottom)
{
    public bool IsZero => Left <= 0.05f && Top <= 0.05f && Right <= 0.05f && Bottom <= 0.05f;

    /// <summary>Det utskrivbare området, målt fra arkets øvre venstre hjørne.</summary>
    public RectangleF PrintableArea => RectangleF.FromLTRB(Left, Top, LabelSheet.PageWidthMm - Right, LabelSheet.PageHeightMm - Bottom);

    /// <summary>F.eks. «venstre 4,2, topp 4,2, høyre 4,2, bunn 4,2 mm».</summary>
    public override string ToString() => $"venstre {Left:0.#}, topp {Top:0.#}, høyre {Right:0.#}, bunn {Bottom:0.#} mm";
}

/// <summary>Det programmet trenger å vite om en installert skriver.</summary>
public sealed record PrinterInfo(string Name, bool IsValid, bool SupportsColor, PrinterMarginsMm Margins)
{
    private const float HundredthInchToMm = 0.254f;
    private static readonly Dictionary<string, PrinterInfo> Cache = new(StringComparer.OrdinalIgnoreCase);

    public PrinterModel? Model => PrinterModel.Find(Name);

    /// <summary>Navnet på Windows' standardskriver, eller null.</summary>
    public static string? DefaultPrinterName
    {
        get
        {
            try
            {
                string name = new PrinterSettings().PrinterName;
                return string.IsNullOrEmpty(name) ? null : name;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    /// <summary>Spør skriverdriveren (svaret huskes, siden nettverksskrivere kan bruke litt tid).</summary>
    public static PrinterInfo Get(string printerName)
    {
        if (Cache.TryGetValue(printerName, out PrinterInfo? cached))
            return cached;
        PrinterInfo info = Query(printerName);
        Cache[printerName] = info;
        return info;
    }

    private static PrinterInfo Query(string printerName)
    {
        try
        {
            var printer = new PrinterSettings { PrinterName = printerName };
            if (!printer.IsValid)
                return new PrinterInfo(printerName, false, false, default);

            PageSettings page = printer.DefaultPageSettings;
            page.Landscape = false;
            foreach (PaperSize size in printer.PaperSizes)
            {
                if (size.Kind == PaperKind.A4)
                {
                    page.PaperSize = size;
                    break;
                }
            }

            RectangleF area = page.PrintableArea;
            var margins = new PrinterMarginsMm(
                Math.Max(0f, area.X * HundredthInchToMm),
                Math.Max(0f, area.Y * HundredthInchToMm),
                Math.Max(0f, LabelSheet.PageWidthMm - area.Right * HundredthInchToMm),
                Math.Max(0f, LabelSheet.PageHeightMm - area.Bottom * HundredthInchToMm));
            return new PrinterInfo(printerName, true, printer.SupportsColor, margins);
        }
        catch (Exception)
        {
            return new PrinterInfo(printerName, false, false, default);
        }
    }
}
