namespace Kolibri.Kino.Tests;

/// <summary>A throwaway LiteDB file path, deleted on dispose. Tests never touch the real library.</summary>
public sealed class TempDatabase : IDisposable
{
    public string Path { get; } = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"kino-test-{Guid.NewGuid():N}.db");

    public void Dispose()
    {
        foreach (var file in new[] { Path, Path.Replace(".db", "-log.db") })
            if (File.Exists(file)) File.Delete(file);
    }
}
