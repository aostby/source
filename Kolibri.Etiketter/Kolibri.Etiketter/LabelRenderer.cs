using System.Drawing.Drawing2D;

namespace Kolibri.Etiketter;

public enum LabelRenderMode
{
    /// <summary>Skjermvisning: viser marger og bildefelt som hjelpelinjer.</summary>
    Screen,

    /// <summary>Vanlig utskrift: kun innholdet.</summary>
    Print,

    /// <summary>Testutskrift: innholdet pluss omriss av etiketten, for å kontrollere plassering.</summary>
    PrintWithGuides,
}

/// <summary>Resultatet av å tegne en etikett.</summary>
/// <param name="TextOverflow">True dersom teksten ikke fikk plass.</param>
/// <param name="FontSize">Skriftstørrelsen (punkt) som faktisk ble brukt, etter eventuell krymping.</param>
public readonly record struct LabelDrawResult(bool TextOverflow, float FontSize);

/// <summary>
/// Tegner en etikett. Forutsetter at <see cref="Graphics.PageUnit"/> er
/// <see cref="GraphicsUnit.Millimeter"/>, slik at alle koordinater er i mm.
/// Samme kode brukes til skjerm og skriver, så det du ser er det du får.
/// </summary>
public static class LabelRenderer
{
    /// <summary>Tegner etiketten og forteller om teksten fikk plass, og med hvilken skriftstørrelse.</summary>
    public static LabelDrawResult Draw(Graphics g, LabelDesign design, RectangleF label, LabelRenderMode mode)
    {
        GraphicsState state = g.Save();
        try
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            RectangleF content = RectangleF.Inflate(label, -LabelSheet.InnerMarginMm, -LabelSheet.InnerMarginMm);
            RectangleF textArea = content;
            RectangleF? imageArea = null;

            Image? image = design.Image;
            if (image is not null)
            {
                float percent = Math.Clamp(design.ImageWidthPercent, 5f, 95f);
                float imageWidth = content.Width * percent / 100f;
                float textWidth = Math.Max(0f, content.Width - imageWidth - LabelSheet.ImageTextGapMm);

                if (design.ImagePosition == LabelImagePosition.Left)
                {
                    imageArea = new RectangleF(content.X, content.Y, imageWidth, content.Height);
                    textArea = new RectangleF(content.Right - textWidth, content.Y, textWidth, content.Height);
                }
                else
                {
                    imageArea = new RectangleF(content.Right - imageWidth, content.Y, imageWidth, content.Height);
                    textArea = new RectangleF(content.X, content.Y, textWidth, content.Height);
                }

                g.DrawImage(image, FitInto(image.Size, imageArea.Value));
            }

            LabelDrawResult result = DrawText(g, design, textArea);

            if (mode == LabelRenderMode.Screen)
            {
                // Pen med bredde 0 = alltid én skjermpiksel.
                using var marginPen = new Pen(Color.FromArgb(120, 0, 120, 215), 0f) { DashStyle = DashStyle.Dash };
                g.DrawRectangle(marginPen, content.X, content.Y, content.Width, content.Height);
                if (imageArea is { } ia)
                    g.DrawRectangle(marginPen, ia.X, ia.Y, ia.Width, ia.Height);
                using var borderPen = new Pen(Color.Gray, 0f);
                g.DrawRectangle(borderPen, label.X, label.Y, label.Width, label.Height);
            }
            else if (mode == LabelRenderMode.PrintWithGuides)
            {
                using var guidePen = new Pen(Color.Gray, 0.1f);
                g.DrawRectangle(guidePen, label.X, label.Y, label.Width, label.Height);
                using var marginPen = new Pen(Color.LightGray, 0.1f) { DashStyle = DashStyle.Dash };
                g.DrawRectangle(marginPen, content.X, content.Y, content.Width, content.Height);
            }

            return result;
        }
        finally
        {
            g.Restore(state);
        }
    }

    // Teksten tegnes som omriss (GraphicsPath) i mm. Da er linjeskift og bredder helt uavhengige av
    // oppløsningen, så skjermen, forhåndsvisningen og skriveren bryter linjene likt.
    private static LabelDrawResult DrawText(Graphics g, LabelDesign design, RectangleF area)
    {
        if (string.IsNullOrWhiteSpace(design.Text) || area.Width <= 0.5f || area.Height <= 0.5f)
            return new LabelDrawResult(false, design.FontSize);

        using Font baseFont = design.CreateFont();
        FontFamily family = baseFont.FontFamily;
        int style = (int)baseFont.Style;

        using var format = (StringFormat)StringFormat.GenericTypographic.Clone();
        format.Alignment = design.Alignment == LabelTextAlignment.Left ? StringAlignment.Near : StringAlignment.Center;
        format.LineAlignment = StringAlignment.Near;
        format.Trimming = StringTrimming.None;
        format.FormatFlags = StringFormatFlags.NoClip;

        float size = baseFont.SizeInPoints;
        GraphicsPath path = BuildText(design.Text, family, style, size, area, format, out bool fits);
        try
        {
            while (!fits && design.AutoShrinkText && size > MinFontSize)
            {
                size = Math.Max(MinFontSize, size - 0.25f);
                path.Dispose();
                path = BuildText(design.Text, family, style, size, area, format, out fits);
            }

            // Midtstill det som faktisk blir svart på etiketten, loddrett.
            RectangleF ink = path.GetBounds();
            using (var move = new Matrix())
            {
                move.Translate(0f, area.Y + (area.Height - ink.Height) / 2f - ink.Y);
                path.Transform(move);
            }

            GraphicsState state = g.Save();
            g.SetClip(area);
            using var brush = new SolidBrush(design.TextColor);
            g.FillPath(brush, path);
            g.Restore(state);
            return new LabelDrawResult(!fits, size);
        }
        finally
        {
            path.Dispose();
        }
    }

    private const float MinFontSize = 4f;
    private const float PointToMm = 25.4f / 72f;

    private static GraphicsPath BuildText(string text, FontFamily family, int style, float sizePt, RectangleF area,
        StringFormat format, out bool fits)
    {
        float em = sizePt * PointToMm;
        var path = new GraphicsPath();
        path.AddString(text, family, style, em, new RectangleF(area.X, area.Y, area.Width, 100000f), format);
        RectangleF ink = path.GetBounds();
        fits = ink.Height <= area.Height + 0.01f
            && ink.Left >= area.Left - 0.01f
            && ink.Right <= area.Right + 0.01f
            && WordsFit(text, family, style, em, area.Width, format);
        return path;
    }

    /// <summary>Et ord som er bredere enn feltet blir delt midt i ordet. Det regnes som at teksten ikke får plass.</summary>
    private static bool WordsFit(string text, FontFamily family, int style, float em, float width, StringFormat format)
    {
        foreach (string word in text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
        {
            using var wordPath = new GraphicsPath();
            wordPath.AddString(word, family, style, em, PointF.Empty, format);
            if (wordPath.GetBounds().Width > width + 0.01f)
                return false;
        }
        return true;
    }

    /// <summary>Skalerer bildet så det fyller feltet mest mulig, uten å forvrenge det, sentrert.</summary>
    public static RectangleF FitInto(Size imageSize, RectangleF area)
    {
        if (imageSize.Width <= 0 || imageSize.Height <= 0)
            return area;

        float scale = Math.Min(area.Width / imageSize.Width, area.Height / imageSize.Height);
        float w = imageSize.Width * scale;
        float h = imageSize.Height * scale;
        return new RectangleF(area.X + (area.Width - w) / 2f, area.Y + (area.Height - h) / 2f, w, h);
    }
}
