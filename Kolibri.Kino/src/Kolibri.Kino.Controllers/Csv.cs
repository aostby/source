namespace Kolibri.Kino.Controllers;

/// <summary>
/// CSV that Excel opens directly: semicolon-separated (Excel's separator with Norwegian settings), written as
/// UTF-8 with BOM by the caller. Replaces SilverScreen's Excel-interop export, which needed Office installed.
/// </summary>
public static class Csv
{
    /// <summary>One line: the fields joined with ';', each quoted when it needs to be. "N/A" from OMDb becomes empty.</summary>
    public static string Line(params string?[] fields) => string.Join(';', fields.Select(Field));

    private static string Field(string? value)
    {
        var text = value is null or "N/A" ? "" : value;
        return text.IndexOfAny([';', '"', '\n', '\r']) >= 0 ? $"\"{text.Replace("\"", "\"\"")}\"" : text;
    }
}
