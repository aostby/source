namespace Kolibri.Kino.Controllers;

/// <summary>
/// CSV that Excel opens directly: semicolon-separated (Excel's separator with Norwegian settings), written as
/// UTF-8 with BOM by the caller. Replaces SilverScreen's Excel-interop export, which needed Office installed.
/// </summary>
public static class Csv
{
    /// <summary>One line: the fields joined with ';', each quoted when it needs to be. "N/A" from OMDb becomes empty.</summary>
    public static string Line(params string?[] fields) => string.Join(';', fields.Select(Field));

    /// <summary>
    /// Reads what <see cref="Line"/> writes (and what Excel saves): ';'-separated fields, quoted ones with "" for a quote
    /// and line breaks inside. Empty lines are skipped; a leading BOM is ignored.
    /// </summary>
    public static IReadOnlyList<string[]> Parse(string text)
    {
        var rows = new List<string[]>();
        var fields = new List<string>();
        var field = new System.Text.StringBuilder();
        var quoted = false;
        var start = text.Length > 0 && text[0] == '\uFEFF' ? 1 : 0;

        void EndField()
        {
            fields.Add(field.ToString());
            field.Clear();
        }
        void EndRow()
        {
            EndField();
            if (fields.Count > 1 || fields[0].Length > 0) rows.Add([.. fields]);
            fields.Clear();
        }

        for (var i = start; i < text.Length; i++)
        {
            var c = text[i];
            if (quoted)
            {
                if (c != '"') field.Append(c);
                else if (i + 1 < text.Length && text[i + 1] == '"') field.Append(text[++i]);
                else quoted = false;
            }
            else if (c == '"') quoted = true;
            else if (c == ';') EndField();
            else if (c == '\n') EndRow();
            else if (c != '\r') field.Append(c);
        }
        if (field.Length > 0 || fields.Count > 0) EndRow();
        return rows;
    }

    private static string Field(string? value)
    {
        var text = value is null or "N/A" ? "" : value;
        return text.IndexOfAny([';', '"', '\n', '\r']) >= 0 ? $"\"{text.Replace("\"", "\"\"")}\"" : text;
    }
}
