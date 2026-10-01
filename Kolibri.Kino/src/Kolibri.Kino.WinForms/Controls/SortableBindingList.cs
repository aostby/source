using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Kolibri.Kino.WinForms.Controls;

/// <summary>
/// A list a DataGridView can sort by clicking a column header (a plain List can't be sorted by the grid).
/// Sorts by value: numbers as numbers, also inside text ("8.7", "1,234,567", "2008–2013", "142 min"), dates as dates,
/// other text alphabetically ignoring case; empty and "N/A" always last.
/// </summary>
internal sealed partial class SortableBindingList<T> : BindingList<T>
{
    private bool _sorted;
    private ListSortDirection _direction;
    private PropertyDescriptor? _property;

    public SortableBindingList(IEnumerable<T> items) : base(items.ToList())
    {
    }

    protected override bool SupportsSortingCore => true;
    protected override bool IsSortedCore => _sorted;
    protected override ListSortDirection SortDirectionCore => _direction;
    protected override PropertyDescriptor? SortPropertyCore => _property;

    protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
    {
        var items = (List<T>)Items;
        var keyed = items.Select(item => (Item: item, Key: SortKey.Of(property.GetValue(item)))).ToList();

        var present = keyed.Where(k => !k.Key.Missing).OrderBy(k => k.Key, SortKey.Comparer); // stable
        var ordered = direction == ListSortDirection.Ascending ? present.ToList() : present.Reverse().ToList();
        ordered.AddRange(keyed.Where(k => k.Key.Missing)); // empty values last either way

        items.Clear();
        items.AddRange(ordered.Select(k => k.Item));
        (_property, _direction, _sorted) = (property, direction, true);
        OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
    }

    protected override void RemoveSortCore() => (_property, _sorted) = (null, false);

    /// <summary>What a cell value sorts by.</summary>
    private readonly partial record struct SortKey(bool Missing, double? Number, string Text)
    {
        public static readonly IComparer<SortKey> Comparer = Comparer<SortKey>.Create((a, b) =>
            (a.Number, b.Number) switch
            {
                ({ } x, { } y) => x.CompareTo(y) is var c and not 0 ? c : string.Compare(a.Text, b.Text, StringComparison.CurrentCultureIgnoreCase),
                ({ }, null) => -1, // numbers before text
                (null, { }) => 1,
                _ => string.Compare(a.Text, b.Text, StringComparison.CurrentCultureIgnoreCase),
            });

        public static SortKey Of(object? value) => value switch
        {
            null => new(true, null, ""),
            bool b => new(false, b ? 1 : 0, b.ToString()),
            DateTime d => new(false, d.Ticks, d.ToString("s", CultureInfo.InvariantCulture)),
            IConvertible c when value is not string => new(false, c.ToDouble(CultureInfo.InvariantCulture), c.ToString(CultureInfo.InvariantCulture)),
            _ => FromText(value.ToString()),
        };

        private static SortKey FromText(string? text)
        {
            var trimmed = text?.Trim() ?? "";
            if (trimmed.Length == 0 || trimmed == "N/A") return new(true, null, "");

            // A leading number, with thousands separators: "8.7", "1,234,567 votes", "2008–2013", "142 min".
            var match = LeadingNumber().Match(trimmed);
            return match.Success && double.TryParse(match.Value.Replace(",", ""), NumberStyles.Float, CultureInfo.InvariantCulture, out var number)
                ? new(false, number, trimmed)
                : new(false, null, trimmed);
        }

        [GeneratedRegex(@"^[-+]?\d{1,3}(,\d{3})+(\.\d+)?|^[-+]?\d+(\.\d+)?")]
        private static partial Regex LeadingNumber();
    }
}

internal static class SortableBindingExtensions
{
    /// <summary>
    /// Shows <paramref name="rows"/> in a sortable list, keeping the column and direction the user sorted by before.
    /// </summary>
    public static void SetRows<T>(this BindingSource source, IEnumerable<T> rows)
    {
        var previous = source.List as IBindingList;
        var sortedBy = previous is { IsSorted: true } ? previous.SortProperty?.Name : null;
        var direction = previous?.SortDirection ?? ListSortDirection.Ascending;

        IBindingList list = new SortableBindingList<T>(rows);
        if (sortedBy is not null && TypeDescriptor.GetProperties(typeof(T))[sortedBy] is { } property)
            list.ApplySort(property, direction);
        source.DataSource = list;
    }
}
