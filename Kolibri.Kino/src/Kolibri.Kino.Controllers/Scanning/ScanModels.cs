namespace Kolibri.Kino.Controllers.Scanning;

public enum ScanMode
{
    /// <summary>Match only files the library doesn't know yet ("Nye").</summary>
    NewFiles,

    /// <summary>Also refresh OMDb details for files that are already linked ("Alle").</summary>
    All,
}

public enum ScanOutcome
{
    /// <summary>Already linked; nothing to do.</summary>
    AlreadyLinked,

    /// <summary>Matched and linked to the library.</summary>
    Linked,

    /// <summary>Already linked; details refreshed from OMDb.</summary>
    Refreshed,

    /// <summary>Matched a movie that is already linked to another existing file elsewhere. Not changed.</summary>
    Duplicate,

    /// <summary>
    /// Another part of a multipart movie (CD2, Part 2, …) whose first linked part is in the same folder. Not a problem.
    /// </summary>
    OtherPart,

    /// <summary>Looks like a series episode; movie matching skipped.</summary>
    Skipped,

    NotFound,

    Failed,
}

public sealed record ScanProgress(int Done, int Total, string CurrentFile);

/// <param name="Source">Where the match came from: "IMDb id in name", "library", "Plex", "TMDb" or "OMDb".</param>
/// <param name="LinkedFile">For <see cref="ScanOutcome.Duplicate"/>: the other file the movie is already linked to.</param>
public sealed record ScanEntry(
    string FilePath, ScanOutcome Outcome, string? ImdbId = null, string? Title = null, string? Source = null, string? Detail = null,
    string? LinkedFile = null);

/// <param name="StoppedReason">Set when the scan ended early (cancelled, or OMDb became unavailable).</param>
/// <param name="Notes">Optional sources that were skipped (Plex offline, TMDb key rejected, ...).</param>
/// <param name="Folders">Every folder that held a scanned video file; where leftover-file cleanup applies.</param>
public sealed record ScanReport(
    string Folder, ScanMode Mode, IReadOnlyList<ScanEntry> Entries, string? StoppedReason,
    IReadOnlyList<string> Notes, IReadOnlyList<string> Folders)
{
    public int Count(ScanOutcome outcome) => Entries.Count(e => e.Outcome == outcome);
}
