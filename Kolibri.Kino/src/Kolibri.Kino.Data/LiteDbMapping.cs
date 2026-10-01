using Kolibri.Kino.Core.Models;
using LiteDB;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Data;

/// <summary>
/// Builds LiteDB's mappings for Kino's stored classes once, before any parallel use.
/// </summary>
/// <remarks>
/// BsonMapper.Global builds a class's mapping on first use and is not thread-safe while doing it: two threads
/// storing an Item for the first time at once can make one see a half-built mapping
/// ("Collection was modified; enumeration operation may not execute"). Warming up under a lock avoids that.
/// </remarks>
public static class LiteDbMapping
{
    private static readonly Lock Gate = new();
    private static bool _done;

    public static void WarmUp()
    {
        lock (Gate)
        {
            if (_done) return;
            Map(new Item());
            Map(new WatchListItem());
            Map(new FileItem());
            Map(new UserSettings());
            _done = true;
        }
    }

    /// <summary>Both directions, so reading builds nothing new either.</summary>
    private static void Map<T>(T sample) => BsonMapper.Global.ToObject<T>(BsonMapper.Global.ToDocument(sample));
}
