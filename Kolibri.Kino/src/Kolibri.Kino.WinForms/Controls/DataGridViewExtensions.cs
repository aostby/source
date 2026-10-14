using Microsoft.Extensions.Logging;

namespace Kolibri.Kino.WinForms.Controls;

internal static class DataGridViewExtensions
{
    /// <summary>
    /// Handles DataError for every grid in <paramref name="root"/>, also grids added later (e.g. season tabs).
    /// When a list is replaced by a shorter one, WinForms can briefly ask for a row that's gone
    /// ("Index 4 does not have a value") and shows its default error box on every repaint; that is ignored.
    /// Other data errors go to the log (Help → Log) instead of a box per cell.
    /// </summary>
    public static void HandleDataErrors(this Control root)
    {
        if (root is DataGridView grid)
        {
            grid.DataError -= OnDataError;
            grid.DataError += OnDataError;
        }
        root.ControlAdded -= OnControlAdded;
        root.ControlAdded += OnControlAdded;
        foreach (Control child in root.Controls) child.HandleDataErrors();
    }

    private static void OnControlAdded(object? sender, ControlEventArgs e)
    {
        if (e.Control is { } added) added.HandleDataErrors();
    }

    private static void OnDataError(object? sender, DataGridViewDataErrorEventArgs e)
    {
        e.ThrowException = false;
        if (e.Exception is IndexOutOfRangeException) return;
        var form = (sender as Control)?.FindForm();
        KinoLog.Factory.CreateLogger(form?.GetType() ?? typeof(DataGridView)).LogWarning(
            e.Exception, "{Window}: grid row {Row}, column {Column}: {Message}", form?.Text, e.RowIndex, e.ColumnIndex, e.Exception?.Message);
    }

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
