using Microsoft.AspNetCore.Components;

namespace Kolibri.Kino.Blazor.Components;

/// <summary>
/// Base for Kino pages, like the WinForms AsyncForm: one controller call at a time, status and error reporting,
/// and cancelling the running call when the page is left.
/// </summary>
public abstract class KinoPage : ComponentBase, IDisposable
{
    private CancellationTokenSource? _cts;

    protected bool Busy { get; private set; }

    /// <summary>While busy, what is happening; afterwards, a result message the call may set.</summary>
    protected string? Status { get; set; }

    protected string? Error { get; set; }

    /// <summary>Runs <paramref name="action"/>, cancelling any call that is still running.</summary>
    protected async Task RunAsync(string status, Func<CancellationToken, Task> action)
    {
        _cts?.Cancel();
        _cts?.Dispose();
        var cts = _cts = new CancellationTokenSource();

        Busy = true;
        Status = status;
        Error = null;
        StateHasChanged();
        try
        {
            await action(cts.Token);
            if (Status == status) Status = null;
        }
        catch (OperationCanceledException) when (cts.IsCancellationRequested)
        {
            // Replaced by a newer call, or the page was left.
        }
        catch (Exception ex)
        {
            Status = null;
            Error = ex.Message;
        }
        finally
        {
            // Only the latest call may clear the busy state.
            if (ReferenceEquals(cts, _cts))
            {
                Busy = false;
                StateHasChanged();
            }
        }
    }

    public void Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        GC.SuppressFinalize(this);
    }
}
