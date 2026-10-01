namespace Kolibri.Kino.WinForms.Controls;

internal static class DataGridViewExtensions
{
    /// <summary>
    /// The object bound to the current row, or null when there is none.
    /// </summary>
    /// <remarks>
    /// While a grid's DataSource is being replaced, WinForms raises SelectionChanged with a CurrentRow that can still
    /// point into the old, longer list; DataBoundItem then throws IndexOutOfRangeException. That is treated as
    /// "nothing selected" (the event fires again once the new list is bound).
    /// </remarks>
    public static object? CurrentItem(this DataGridView grid)
    {
        try
        {
            return grid.CurrentRow?.DataBoundItem;
        }
        catch (IndexOutOfRangeException)
        {
            return null;
        }
    }

    /// <summary>The object bound to row <paramref name="rowIndex"/>, or null (same rebinding case as <see cref="CurrentItem"/>).</summary>
    public static object? ItemAt(this DataGridView grid, int rowIndex)
    {
        if (rowIndex < 0 || rowIndex >= grid.Rows.Count) return null;
        try
        {
            return grid.Rows[rowIndex].DataBoundItem;
        }
        catch (IndexOutOfRangeException)
        {
            return null;
        }
    }
}
