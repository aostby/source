using System.Reflection;
using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using LiteDB;
using Microsoft.Extensions.Options;

namespace Kolibri.Kino.Data;

/// <summary>
/// The "UserSettings" document for the current Windows user (_id = user name, or Kino:SettingsUser), shared with SilverScreen.
/// </summary>
/// <remarks>
/// Saving merges into the stored document: fields Kino doesn't know are kept, and settings left empty are
/// removed rather than written as empty, so SilverScreen's own defaults apply to them.
/// </remarks>
public sealed class LiteDbSettingsStore : ISettingsStore
{
    private readonly ILiteCollection<BsonDocument> _documents;
    private readonly string? _dbPath;
    private readonly string _user;
    private readonly Lock _lock = new();
    private UserSettings? _current;

    public LiteDbSettingsStore(KinoDatabase db, IOptions<KinoOptions> options)
        : this(db, options.Value.LiteDbPath, options.Value.ResolveSettingsUser())
    {
    }

    /// <remarks>Internal so dependency injection only sees the IOptions constructor (an optional parameter made both look usable).</remarks>
    internal LiteDbSettingsStore(KinoDatabase db, string? dbPath = null, string? user = null)
    {
        _documents = db.Database.GetCollection("UserSettings");
        _dbPath = dbPath;
        _user = string.IsNullOrWhiteSpace(user) ? Environment.UserName : user;
    }

    public event EventHandler? Changed;

    public UserSettings Current
    {
        get
        {
            lock (_lock) return _current ??= Load();
        }
    }

    public Task SaveAsync(UserSettings settings, CancellationToken ct = default) =>
        Task.Run(() =>
        {
            var incoming = BsonMapper.Global.ToDocument(Normalized(settings));
            var stored = _documents.FindById(_user) ?? new BsonDocument();
            ApplyKnownFields(stored, incoming, typeof(UserSettings));
            stored["_id"] = _user;
            stored["UserName"] = _user;
            _documents.Upsert(stored);

            lock (_lock) _current = Load();
            Changed?.Invoke(this, EventArgs.Empty);
        }, ct);

    private UserSettings Load()
    {
        var doc = _documents.FindById(_user);
        var settings = doc is null ? new UserSettings() : BsonMapper.Global.ToObject<UserSettings>(doc);
        settings.UserName = _user;
        settings.UserFilePaths ??= new UserSettings.FilePaths();
        // Missing or empty keys mean the shared defaults, so a new or old database works at once.
        if (string.IsNullOrWhiteSpace(settings.OMDBkey)) settings.OMDBkey = UserSettings.DefaultOmdbKey;
        if (string.IsNullOrWhiteSpace(settings.TMDBkey)) settings.TMDBkey = UserSettings.DefaultTmdbKey;
        if (string.IsNullOrWhiteSpace(settings.IMDbDataFiles)) settings.IMDbDataFiles = UserSettings.DefaultImdbDataFiles;
        if (!string.IsNullOrWhiteSpace(_dbPath)) settings.LiteDBFilePath = _dbPath;
        return settings;
    }

    /// <summary>Empty strings become null (not set), so they are removed instead of stored.</summary>
    private static UserSettings Normalized(UserSettings settings)
    {
        var copy = settings.Clone();
        TrimStrings(copy);
        TrimStrings(copy.UserFilePaths);
        return copy;

        static void TrimStrings(object target)
        {
            foreach (var p in target.GetType().GetProperties().Where(p => p.PropertyType == typeof(string) && p.CanWrite))
                p.SetValue(target, (p.GetValue(target) as string)?.Trim() is { Length: > 0 } value ? value : null);
        }
    }

    /// <summary>Copies the properties of <paramref name="type"/> from <paramref name="incoming"/>; unknown fields in <paramref name="stored"/> stay.</summary>
    private static void ApplyKnownFields(BsonDocument stored, BsonDocument incoming, Type type)
    {
        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite))
        {
            var name = property.Name;
            if (!incoming.TryGetValue(name, out var value) || value.IsNull)
            {
                stored.Remove(name);
            }
            else if (value.IsDocument && property.PropertyType.IsClass && property.PropertyType != typeof(string))
            {
                var nested = stored.TryGetValue(name, out var existing) && existing.IsDocument ? existing.AsDocument : new BsonDocument();
                ApplyKnownFields(nested, value.AsDocument, property.PropertyType);
                stored[name] = nested;
            }
            else
            {
                stored[name] = value;
            }
        }
    }
}
