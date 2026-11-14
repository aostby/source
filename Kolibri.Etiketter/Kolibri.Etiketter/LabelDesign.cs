using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kolibri.Etiketter;

public enum LabelTextAlignment
{
    Center,
    Left,
}

public enum LabelImagePosition
{
    Left,
    Right,

    /// <summary>Bildet sentreres på hele etiketten, og teksten skrives ikke ut.</summary>
    Center,
}

/// <summary>Egen skrift for én linje i teksten. Linjer uten egen skrift bruker etikettens standardskrift.</summary>
public sealed class LabelLineStyle
{
    public string FontFamily { get; set; } = "Segoe UI";
    public float FontSize { get; set; } = 12f;
    public FontStyle FontStyle { get; set; } = FontStyle.Regular;
    public int TextColorArgb { get; set; } = Color.Black.ToArgb();

    [JsonIgnore]
    public Color TextColor => Color.FromArgb(TextColorArgb);

    public static LabelLineStyle From(Font font, Color color) => new()
    {
        FontFamily = font.FontFamily.Name,
        FontSize = font.SizeInPoints,
        FontStyle = font.Style,
        TextColorArgb = color.ToArgb(),
    };

    /// <summary>Skriften for linjen; faller tilbake til systemskriften om skrifttypen mangler.</summary>
    public Font CreateFont(float sizePt) => LabelDesign.CreateFont(FontFamily, sizePt, FontStyle);
}

/// <summary>Innholdet på én etikett. Lagres som JSON (.etikett) med bildet innebygd.</summary>
public sealed class LabelDesign : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
    };

    private Image? _image;
    private byte[]? _imageData;

    public string Text { get; set; } = "";
    public string FontFamily { get; set; } = "Segoe UI";
    public float FontSize { get; set; } = 12f;
    public FontStyle FontStyle { get; set; } = FontStyle.Regular;
    public int TextColorArgb { get; set; } = Color.Black.ToArgb();
    public LabelTextAlignment Alignment { get; set; } = LabelTextAlignment.Center;

    /// <summary>Krymp skriften automatisk dersom teksten ikke får plass.</summary>
    public bool AutoShrinkText { get; set; } = true;

    /// <summary>Hvor stor del av etikettens bredde bildet får (standard 1/3).</summary>
    public float ImageWidthPercent { get; set; } = 100f / 3f;

    public LabelImagePosition ImagePosition { get; set; } = LabelImagePosition.Left;

    /// <summary>Originalfilen til bildet (png, jpg, bmp, gif ...).</summary>
    public byte[]? ImageData
    {
        get => _imageData;
        set
        {
            _image?.Dispose();
            _image = null;
            _imageData = value is { Length: > 0 } ? value : null;
        }
    }

    [JsonIgnore]
    public Color TextColor
    {
        get => Color.FromArgb(TextColorArgb);
        set => TextColorArgb = value.ToArgb();
    }

    [JsonIgnore]
    public bool HasImage => Image is not null;

    [JsonIgnore]
    public Image? Image
    {
        get
        {
            if (_image is null && _imageData is not null)
            {
                try
                {
                    using var stream = new MemoryStream(_imageData);
                    using var loaded = Image.FromStream(stream);
                    _image = new Bitmap(loaded);
                }
                catch (Exception)
                {
                    _imageData = null;
                }
            }
            return _image;
        }
    }

    public void SetImage(Image image)
    {
        using var stream = new MemoryStream();
        image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
        ImageData = stream.ToArray();
    }

    public void LoadImageFile(string path)
    {
        byte[] bytes = File.ReadAllBytes(path);
        // Sjekk at filen faktisk er et bilde før vi tar den i bruk.
        using (var stream = new MemoryStream(bytes))
        using (Image.FromStream(stream)) { }
        ImageData = bytes;
    }

    public void RemoveImage() => ImageData = null;

    /// <summary>
    /// En ny etikett med disketten som standardbilde. Lagrede etiketter lastes uten dette, så bildet deres beholdes.
    /// </summary>
    public static LabelDesign CreateNew(string text = "")
    {
        var design = new LabelDesign { Text = text };
        // Commodore 64-skriften er standard når den er installert eller bygget inn.
        if (EmbeddedFonts.Find(EmbeddedFonts.C64) is not null || EmbeddedFonts.IsInstalled(EmbeddedFonts.C64))
        {
            design.FontFamily = EmbeddedFonts.C64;
            design.FontSize = 12f;
            design.FontStyle = FontStyle.Bold | FontStyle.Italic;
        }
        using Bitmap floppy = FloppyDiskImage.Create();
        design.SetImage(floppy);
        return design;
    }

    public Font CreateFont(float? size = null) => CreateFont(FontFamily, size ?? FontSize, FontStyle);

    internal static Font CreateFont(string family, float sizePt, FontStyle style)
    {
        float emSize = Math.Max(1f, sizePt);
        Font? installed = null;
        try
        {
            installed = new Font(family, emSize, style, GraphicsUnit.Point);
            // Windows gir en reserveskrift uten feilmelding når skriften mangler; sjekk navnet.
            if (installed.FontFamily.Name.Equals(family, StringComparison.OrdinalIgnoreCase))
                return installed;
        }
        catch (ArgumentException)
        {
        }

        // Ikke installert: bruk den innebygde skriften (C64 Pro) om den finnes.
        if (EmbeddedFonts.Find(family) is { } embedded)
        {
            try
            {
                Font font = new(embedded, emSize, style, GraphicsUnit.Point);
                installed?.Dispose();
                return font;
            }
            catch (ArgumentException)
            {
            }
        }
        return installed ?? new Font(SystemFonts.DefaultFont.FontFamily, emSize, FontStyle.Regular, GraphicsUnit.Point);
    }

    /// <summary>
    /// Egen skrift per linje i <see cref="Text"/> (samme rekkefølge som linjene). Null, eller en kortere liste,
    /// betyr standardskriften.
    /// </summary>
    public List<LabelLineStyle?>? LineStyles { get; set; }

    /// <summary>Linjene i teksten (skilt med Enter).</summary>
    public static string[] SplitLines(string text) => text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

    /// <summary>Egen skrift for linje nummer <paramref name="line"/> (fra 0), eller null for standardskriften.</summary>
    public LabelLineStyle? GetLineStyle(int line) =>
        LineStyles is { } styles && line >= 0 && line < styles.Count ? styles[line] : null;

    /// <summary>Antall linjer med egen skrift.</summary>
    [JsonIgnore]
    public int LinesWithOwnFont => LineStyles?.Count(s => s is not null) ?? 0;

    /// <summary>Gir linjene fra <paramref name="first"/> til og med <paramref name="last"/> en egen skrift (null = standardskriften).</summary>
    public void SetLineStyle(int first, int last, LabelLineStyle? style)
    {
        var styles = LineStyles ?? [];
        while (styles.Count <= last) styles.Add(null);
        for (int i = Math.Max(0, first); i <= last; i++)
        {
            styles[i] = style is null ? null : new LabelLineStyle
            {
                FontFamily = style.FontFamily,
                FontSize = style.FontSize,
                FontStyle = style.FontStyle,
                TextColorArgb = style.TextColorArgb,
            };
        }
        LineStyles = styles.Any(s => s is not null) ? styles : null;
    }

    /// <summary>
    /// Setter ny tekst og flytter linjeskriftene med, så de følger linjene når linjer settes inn eller slettes.
    /// Nye linjer arver skriften fra linjen de ble delt fra (eller linjen over).
    /// </summary>
    public void SetTextKeepingLineStyles(string text)
    {
        if (LineStyles is null)
        {
            Text = text;
            return;
        }
        string[] oldLines = SplitLines(Text);
        string[] newLines = SplitLines(text);
        int prefix = 0;
        while (prefix < oldLines.Length && prefix < newLines.Length && oldLines[prefix] == newLines[prefix])
            prefix++;
        int suffix = 0;
        while (suffix < oldLines.Length - prefix && suffix < newLines.Length - prefix
               && oldLines[^(suffix + 1)] == newLines[^(suffix + 1)])
            suffix++;

        int oldMiddle = oldLines.Length - prefix - suffix;
        int newMiddle = newLines.Length - prefix - suffix;
        var styles = new List<LabelLineStyle?>(newLines.Length);
        for (int i = 0; i < prefix; i++)
            styles.Add(GetLineStyle(i));
        for (int i = 0; i < newMiddle; i++)
        {
            LabelLineStyle? inherited = oldMiddle > 0
                ? GetLineStyle(prefix + Math.Min(i, oldMiddle - 1))
                : GetLineStyle(prefix - 1);
            styles.Add(inherited);
        }
        for (int i = 0; i < suffix; i++)
            styles.Add(GetLineStyle(oldLines.Length - suffix + i));

        Text = text;
        LineStyles = styles.Any(s => s is not null) ? styles : null;
    }

    public void SetFont(Font font)
    {
        FontFamily = font.FontFamily.Name;
        FontSize = font.SizeInPoints;
        FontStyle = font.Style;
    }

    public void Save(string path) => File.WriteAllText(path, JsonSerializer.Serialize(this, JsonOptions));

    public static LabelDesign Load(string path) =>
        JsonSerializer.Deserialize<LabelDesign>(File.ReadAllText(path), JsonOptions) ?? new LabelDesign();

    public void Dispose()
    {
        _image?.Dispose();
        _image = null;
    }
}
