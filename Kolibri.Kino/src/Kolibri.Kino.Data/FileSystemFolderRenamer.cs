using Kolibri.Kino.Core.Abstractions;

namespace Kolibri.Kino.Data;

public sealed class FileSystemFolderRenamer : IFolderRenamer
{
    public Task<string> RenameAsync(string folder, string newName, CancellationToken ct = default) =>
        Task.Run(() =>
        {
            var trimmed = folder.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var parent = Path.GetDirectoryName(trimmed) ?? throw new IOException($"{folder} has no parent folder.");
            if (newName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) throw new IOException($"\"{newName}\" is not a valid folder name.");

            var target = Path.Combine(parent, newName);
            if (string.Equals(target, trimmed, StringComparison.Ordinal)) return target;
            // A change of case only: Windows needs a detour through another name.
            if (string.Equals(target, trimmed, StringComparison.OrdinalIgnoreCase))
            {
                var temp = Path.Combine(parent, $"{newName}.kino-rename");
                Directory.Move(trimmed, temp);
                Directory.Move(temp, target);
                return target;
            }
            if (Directory.Exists(target) || File.Exists(target)) throw new IOException($"{target} already exists.");
            Directory.Move(trimmed, target);
            return target;
        }, ct);
}
