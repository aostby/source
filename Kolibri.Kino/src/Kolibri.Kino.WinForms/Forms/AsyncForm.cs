namespace Kolibri.Kino.WinForms.Forms;

/// <summary>
/// Base for Kino windows: Kino's icon, one controller call at a time, status and error reporting,
/// and cancelling the running call when the window closes.
/// </summary>
/// <remarks>
/// A call can still be finishing after its window was closed and disposed (e.g. a dialog closed during its
/// search). Its controls are then gone, so the busy state, status and error box are skipped instead of crashing.
/// </remarks>
public class AsyncForm : Form
{
    private CancellationTokenSource? _cts;

    public AsyncForm() => Kolibri.Kino.WinForms.Controls.AppIcon.Apply(this);

    /// <summary>Closed and disposed (or being disposed): its controls must not be touched any more.</summary>
    protected bool IsGone => IsDisposed || Disposing;

    /// <summary>Runs <paramref name="action"/>, cancelling any call that is still running.</summary>
    protected async Task RunAsync(string status, Func<CancellationToken, Task> action)
    {
        _cts?.Cancel();
        _cts?.Dispose();
        var cts = _cts = new CancellationTokenSource();

        SetBusy(true);
        ShowStatus(status);
        try
        {
            await action(cts.Token);
        }
        catch (OperationCanceledException) when (cts.IsCancellationRequested)
        {
            // Replaced by a newer call or the window is closing.
        }
        catch (Exception ex) when (!IsGone)
        {
            ShowStatus("Error: " + ex.Message);
            MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception) when (IsGone)
        {
            // The window is closed; nobody to tell.
        }
        finally
        {
            // Only the latest call may clear the busy state.
            if (ReferenceEquals(cts, _cts)) SetBusy(false);
        }
    }

    /// <summary><see cref="OnBusyChanged"/>, unless the window is gone; a failure there never breaks the call.</summary>
    private void SetBusy(bool busy)
    {
        if (IsGone) return;
        try
        {
            OnBusyChanged(busy);
        }
        catch (Exception ex) when (ex is ObjectDisposedException or InvalidOperationException or NullReferenceException)
        {
            // The window was closed while the call ran.
            System.Diagnostics.Debug.WriteLine($"{Name}: busy={busy} skipped: {ex.Message}");
        }
    }

    /// <summary>Cancels the running call, if any (e.g. from a Cancel button).</summary>
    protected void CancelRunning() => _cts?.Cancel();

    /// <summary>Enable or disable input while a call runs.</summary>
    protected virtual void OnBusyChanged(bool busy) => UseWaitCursor = busy;

    protected virtual void ShowStatus(string text) { }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _cts?.Cancel();
        base.OnFormClosing(e);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _cts?.Dispose();
        base.Dispose(disposing);
    }
}
