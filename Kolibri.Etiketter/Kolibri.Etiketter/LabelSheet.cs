namespace Kolibri.Etiketter;

/// <summary>
/// Geometri for Lyreco etikettark: A4, 3 x 8 etiketter à 70 x 37 mm, helt inntil hverandre.
/// Alle mål er i millimeter.
/// </summary>
public static class LabelSheet
{
    public const float PageWidthMm = 210f;
    public const float PageHeightMm = 297f;
    public const float LabelWidthMm = 70f;
    public const float LabelHeightMm = 37f;
    public const int Columns = 3;
    public const int Rows = 8;
    public const int LabelsPerSheet = Columns * Rows;

    /// <summary>Indre marg på hver etikett, siden etikettene ligger helt inntil hverandre.</summary>
    public const float InnerMarginMm = 1f;

    /// <summary>Avstand mellom bilde og tekst.</summary>
    public const float ImageTextGapMm = 1f;

    /// <summary>Venstre kant av etikettfeltet på arket (3 x 70 = 210 mm, altså 0).</summary>
    public static float LeftMm => (PageWidthMm - Columns * LabelWidthMm) / 2f;

    /// <summary>Øvre kant av etikettfeltet på arket (8 x 37 = 296 mm, altså 0,5 mm).</summary>
    public static float TopMm => (PageHeightMm - Rows * LabelHeightMm) / 2f;

    /// <summary>
    /// Rektangelet til etikett nummer <paramref name="index"/> (0–23, radvis fra øverst til venstre)
    /// i millimeter, målt fra arkets øvre venstre hjørne.
    /// </summary>
    public static RectangleF GetLabelRect(int index, float offsetXMm = 0f, float offsetYMm = 0f)
    {
        if (index < 0 || index >= LabelsPerSheet)
            throw new ArgumentOutOfRangeException(nameof(index));

        int column = index % Columns;
        int row = index / Columns;
        return new RectangleF(
            LeftMm + column * LabelWidthMm + offsetXMm,
            TopMm + row * LabelHeightMm + offsetYMm,
            LabelWidthMm,
            LabelHeightMm);
    }
}
