using System.IO.Compression;
using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.Data;

/// <summary>
/// SilverScreen's subtitle layout: Movie.srt next to Movie.mkv, or downloaded subtitles in a "Subs" folder next to it.
/// An episode (S01E02 in its name) gets "Subs\<episode file name>" instead, as season packs are laid out, since the
/// season's episodes share the folder; only that episode's files are kept there.
/// </summary>
public sealed class FileSystemSubtitleFolder : ISubtitleFolder
{
    public const string SubsFolderName = "Subs";

    /// <summary>Smaller .srt files are taken as empty or broken, as in SilverScreen.</summary>
    private const long MinimumSrtBytes = 1000;

    private static readonly HashSet<string> SubtitleExtensions =
        new([".srt", ".sub", ".idx", ".ass", ".ssa", ".vtt", ".smi"], StringComparer.OrdinalIgnoreCase);

    public SubtitleStatus GetStatus(string? videoPath)
    {
        if (string.IsNullOrWhiteSpace(videoPath) || !File.Exists(videoPath)) return new(SubtitleState.NoFile);
        var folder = Path.GetDirectoryName(videoPath)!;

        var srt = Directory.EnumerateFiles(folder, Path.GetFileNameWithoutExtension(videoPath) + "*.srt")
            .FirstOrDefault(f => new FileInfo(f).Length > MinimumSrtBytes);
        if (srt is not null) return new(SubtitleState.SrtFile, srt);

        var subs = SubsFolder(videoPath);
        var files = Directory.Exists(subs)
            ? Directory.EnumerateFiles(subs, "*", SearchOption.AllDirectories)
                .Count(f => SubtitleExtensions.Contains(Path.GetExtension(f)) && SubtitleMatching.FitsEpisode(videoPath, f))
            : 0;
        if (files > 0) return new(SubtitleState.SubsFolder, subs, files);

        return new(Path.GetExtension(videoPath).Equals(".mkv", StringComparison.OrdinalIgnoreCase)
            ? SubtitleState.NoneMaybeBuiltIn
            : SubtitleState.None);
    }

    public string? CopyMatchFromSubs(string videoPath)
    {
        var folder = Path.GetDirectoryName(videoPath)!;
        var subs = SubsFolder(videoPath);
        var target = Path.Combine(folder, Path.GetFileNameWithoutExtension(videoPath) + ".srt");
        if (!Directory.Exists(subs) || File.Exists(target)) return null;

        var match = SubtitleMatching.BestMatch(videoPath, Directory.EnumerateFiles(subs, "*.srt", SearchOption.AllDirectories));
        if (match is null) return null;
        File.Copy(match, target);
        return target;
    }

    public IReadOnlyList<SubtitleFile> GetSubsFiles(string videoPath)
    {
        var subs = SubsFolder(videoPath);
        return Directory.Exists(subs)
            ? Directory.EnumerateFiles(subs, "*.srt", SearchOption.AllDirectories)
                .Where(f => SubtitleMatching.FitsEpisode(videoPath, f))
                .Select(f => new SubtitleFile(f, new FileInfo(f).Length))
                .OrderByDescending(f => f.Bytes)
                .ToList()
            : [];
    }

    public string CopyNextToMovie(string videoPath, string subtitlePath)
    {
        var target = Path.Combine(Path.GetDirectoryName(videoPath)!, Path.GetFileNameWithoutExtension(videoPath) + ".srt");
        File.Copy(subtitlePath, target, overwrite: false);
        return target;
    }

    public Task<int> SaveZipAsync(string videoPath, byte[] zip, CancellationToken ct = default) =>
        Task.Run(() =>
        {
            var subs = Directory.CreateDirectory(SubsFolder(videoPath)).FullName;
            using var archive = new ZipArchive(new MemoryStream(zip), ZipArchiveMode.Read);
            var written = 0;
            foreach (var entry in archive.Entries)
            {
                ct.ThrowIfCancellationRequested();
                // Only the file name: folders in the zip are dropped, so nothing lands outside Subs.
                var name = Path.GetFileName(entry.FullName);
                if (name.Length == 0 || !SubtitleExtensions.Contains(Path.GetExtension(name))) continue;
                // A season pack: only this episode's subtitles.
                if (!SubtitleMatching.FitsEpisode(videoPath, name)) continue;
                entry.ExtractToFile(FreeName(subs, name));
                written++;
            }
            return written;
        }, ct);

    /// <summary>"Subs" next to a movie; "Subs\<file name>" next to an episode.</summary>
    public string SubsFolder(string videoPath)
    {
        var subs = Path.Combine(Path.GetDirectoryName(videoPath)!, SubsFolderName);
        return SubtitleMatching.EpisodeCode(Path.GetFileName(videoPath)) is null
            ? subs
            : Path.Combine(subs, Path.GetFileNameWithoutExtension(videoPath));
    }

    /// <summary>"Movie.srt", or "Movie (2).srt" etc. when taken (several releases often use the same name).</summary>
    private static string FreeName(string folder, string name)
    {
        var path = Path.Combine(folder, name);
        for (var n = 2; File.Exists(path); n++)
            path = Path.Combine(folder, $"{Path.GetFileNameWithoutExtension(name)} ({n}){Path.GetExtension(name)}");
        return path;
    }
}
