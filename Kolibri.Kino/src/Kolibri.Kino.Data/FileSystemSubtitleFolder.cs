using System.IO.Compression;
using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.Data;

/// <summary>
/// SilverScreen's subtitle layout: Movie.srt next to Movie.mkv, or downloaded subtitles in a "Subs" folder next to it.
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

        var subs = Path.Combine(folder, SubsFolderName);
        var files = Directory.Exists(subs)
            ? Directory.EnumerateFiles(subs, "*", SearchOption.AllDirectories).Count(f => SubtitleExtensions.Contains(Path.GetExtension(f)))
            : 0;
        if (files > 0) return new(SubtitleState.SubsFolder, subs, files);

        return new(Path.GetExtension(videoPath).Equals(".mkv", StringComparison.OrdinalIgnoreCase)
            ? SubtitleState.NoneMaybeBuiltIn
            : SubtitleState.None);
    }

    public string? CopyMatchFromSubs(string videoPath)
    {
        var folder = Path.GetDirectoryName(videoPath)!;
        var subs = Path.Combine(folder, SubsFolderName);
        var target = Path.Combine(folder, Path.GetFileNameWithoutExtension(videoPath) + ".srt");
        if (!Directory.Exists(subs) || File.Exists(target)) return null;

        var match = SubtitleMatching.BestMatch(videoPath, Directory.EnumerateFiles(subs, "*.srt", SearchOption.AllDirectories));
        if (match is null) return null;
        File.Copy(match, target);
        return target;
    }

    public IReadOnlyList<SubtitleFile> GetSubsFiles(string videoPath)
    {
        var subs = Path.Combine(Path.GetDirectoryName(videoPath)!, SubsFolderName);
        return Directory.Exists(subs)
            ? Directory.EnumerateFiles(subs, "*.srt", SearchOption.AllDirectories)
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
            var subs = Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(videoPath)!, SubsFolderName)).FullName;
            using var archive = new ZipArchive(new MemoryStream(zip), ZipArchiveMode.Read);
            var written = 0;
            foreach (var entry in archive.Entries)
            {
                ct.ThrowIfCancellationRequested();
                // Only the file name: folders in the zip are dropped, so nothing lands outside Subs.
                var name = Path.GetFileName(entry.FullName);
                if (name.Length == 0 || !SubtitleExtensions.Contains(Path.GetExtension(name))) continue;
                entry.ExtractToFile(FreeName(subs, name));
                written++;
            }
            return written;
        }, ct);

    /// <summary>"Movie.srt", or "Movie (2).srt" etc. when taken (several releases often use the same name).</summary>
    private static string FreeName(string folder, string name)
    {
        var path = Path.Combine(folder, name);
        for (var n = 2; File.Exists(path); n++)
            path = Path.Combine(folder, $"{Path.GetFileNameWithoutExtension(name)} ({n}){Path.GetExtension(name)}");
        return path;
    }
}
