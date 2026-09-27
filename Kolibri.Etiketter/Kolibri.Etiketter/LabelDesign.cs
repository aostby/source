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

    public Font CreateFont(float? size = null)
    {
        float emSize = Math.Max(1f, size ?? FontSize);
        try
        {
            return new Font(FontFamily, emSize, FontStyle, GraphicsUnit.Point);
        }
        catch (ArgumentException)
        {
            return new Font(SystemFonts.DefaultFont.FontFamily, emSize, FontStyle.Regular, GraphicsUnit.Point);
        }
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
