namespace Kolibri.Etiketter;

/// <summary>
/// Geometri for et etikettark på A4: antall etiketter, størrelse, marger og mellomrom.
/// Alle mål er i millimeter. Målene er hentet fra produsentenes maler.
/// </summary>
public sealed class LabelSheet
{
    public const float PageWidthMm = 210f;
    public const float PageHeightMm = 297f;

    /// <summary>Indre marg på hver etikett, siden mange ark har etikettene helt inntil hverandre.</summary>
    public const float InnerMarginMm = 1f;

    /// <summary>Avstand mellom bilde og tekst.</summary>
    public const float ImageTextGapMm = 1f;

    /// <summary>Lyreco-arket programmet ble laget for, og standardvalget.</summary>
    public static LabelSheet Default { get; } = new(
        "zweckform-3474", "24 per ark – 70 × 37 mm", "Lyreco, Avery Zweckform 3474",
        columns: 3, rows: 8, widthMm: 70f, heightMm: 37f, marginTopMm: 0.5f, marginLeftMm: 0f, gapXMm: 0f, gapYMm: 0f);

    /// <summary>Arkene man kan velge mellom.</summary>
    public static IReadOnlyList<LabelSheet> All { get; } =
    [
        new("avery-l7160", "21 per ark – 63,5 × 38,1 mm", "Avery L7160",
            columns: 3, rows: 7, widthMm: 63.5f, heightMm: 38.1f, marginTopMm: 15.15f, marginLeftMm: 7.21f, gapXMm: 2.54f, gapYMm: 0f),
        Default,
        new("herma-4360", "24 per ark – 70 × 36 mm", "Herma 4360, Herma 4453",
            columns: 3, rows: 8, widthMm: 70f, heightMm: 36f, marginTopMm: 4.5f, marginLeftMm: 0f, gapXMm: 0f, gapYMm: 0f),
        new("zweckform-3651", "40 per ark – 52,5 × 29,7 mm", "Avery Zweckform 3651",
            columns: 4, rows: 10, widthMm: 52.5f, heightMm: 29.7f, marginTopMm: 0f, marginLeftMm: 0f, gapXMm: 0f, gapYMm: 0f),
        new("avery-l7651", "65 per ark – 38,1 × 21,2 mm", "Avery L7651",
            columns: 5, rows: 13, widthMm: 38.1f, heightMm: 21.2f, marginTopMm: 10.7f, marginLeftMm: 4.75f, gapXMm: 2.5f, gapYMm: 0f),
        new("avery-l7780", "Runde Ø 40 mm – 24 per ark", "Avery L7780",
            columns: 4, rows: 6, widthMm: 40f, heightMm: 40f, marginTopMm: 13.5f, marginLeftMm: 16f, gapXMm: 6f, gapYMm: 6f, round: true),
    ];

    private LabelSheet(string id, string name, string products, int columns, int rows, float widthMm, float heightMm,
        float marginTopMm, float marginLeftMm, float gapXMm, float gapYMm, bool round = false)
    {
        Id = id;
        Name = name;
        Products = products;
        Columns = columns;
        Rows = rows;
        LabelWidthMm = widthMm;
        LabelHeightMm = heightMm;
        TopMm = marginTopMm;
        LeftMm = marginLeftMm;
        GapXMm = gapXMm;
        GapYMm = gapYMm;
        IsRound = round;
    }

    /// <summary>Lagres i innstillingene.</summary>
    public string Id { get; }

    public string Name { get; }

    /// <summary>Arkene (produktnumrene) som har disse målene.</summary>
    public string Products { get; }

    public int Columns { get; }
    public int Rows { get; }
    public float LabelWidthMm { get; }
    public float LabelHeightMm { get; }

    /// <summary>Øvre kant av den første etiketten på arket.</summary>
    public float TopMm { get; }

    /// <summary>Venstre kant av den første etiketten på arket.</summary>
    public float LeftMm { get; }

    public float GapXMm { get; }
    public float GapYMm { get; }

    /// <summary>Runde etiketter: <see cref="LabelWidthMm"/> er diameteren.</summary>
    public bool IsRound { get; }

    public int LabelsPerSheet => Columns * Rows;

    /// <summary>Teksten i listen over ark, f.eks. «24 per ark – 70 × 37 mm (Lyreco, Avery Zweckform 3474)».</summary>
    public override string ToString() => $"{Name} ({Products})";

    /// <summary>Arket med denne <paramref name="id"/>, eller <see cref="Default"/>.</summary>
    public static LabelSheet FindById(string? id) => All.FirstOrDefault(s => s.Id == id) ?? Default;

    /// <summary>
    /// Rektangelet til etikett nummer <paramref name="index"/> (fra 0, radvis fra øverst til venstre)
    /// i millimeter, målt fra arkets øvre venstre hjørne.
    /// </summary>
    public RectangleF GetLabelRect(int index, float offsetXMm = 0f, float offsetYMm = 0f)
    {
        if (index < 0 || index >= LabelsPerSheet)
            throw new ArgumentOutOfRangeException(nameof(index));

        int column = index % Columns;
        int row = index / Columns;
        return new RectangleF(
            LeftMm + column * (LabelWidthMm + GapXMm) + offsetXMm,
            TopMm + row * (LabelHeightMm + GapYMm) + offsetYMm,
            LabelWidthMm,
            LabelHeightMm);
    }

    /// <summary>
    /// Feltet der bilde og tekst tegnes: etiketten minus <see cref="InnerMarginMm"/>. På en rund etikett er det
    /// et liggende rektangel inni sirkelen (85 % av diameteren bredt), så teksten får mest mulig bredde.
    /// </summary>
    public RectangleF GetContentRect(RectangleF label)
    {
        if (!IsRound)
            return RectangleF.Inflate(label, -InnerMarginMm, -InnerMarginMm);

        float diameter = Math.Min(label.Width, label.Height) - 2 * InnerMarginMm;
        float width = diameter * 0.85f;
        float height = MathF.Sqrt(diameter * diameter - width * width);
        return new RectangleF(label.X + (label.Width - width) / 2f, label.Y + (label.Height - height) / 2f, width, height);
    }
}
