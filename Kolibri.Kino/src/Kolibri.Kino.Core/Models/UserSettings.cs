using System.ComponentModel;

namespace Kolibri.Kino.Core.Models;

/// <summary>
/// The user's settings, stored in the database's "UserSettings" collection (_id = Windows user name) and shared
/// with SilverScreen (Kolibri.net.Common.Dal.Entities.UserSettings). Edited in the Settings window.
/// </summary>
/// <remarks>
/// OMDb and TMDb default to the public keys SilverScreen ships with, so a new database works at once.
/// They are shared by everyone using them (OMDb's daily limit is shared too), so the UI asks for your own keys
/// while a default is in use (<see cref="UsesDefaultKeys"/>). A key cleared in Settings falls back to the default.
/// </remarks>
public sealed class UserSettings
{
    /// <summary>Public OMDb key from SilverScreen (Kolibri.net.Common.Dal.Entities.UserSettings).</summary>
    public const string DefaultOmdbKey = "be7b1bec";

    /// <summary>Public TMDb key from SilverScreen (Kolibri.net.Common.Dal.Entities.UserSettings).</summary>
    public const string DefaultTmdbKey = "c6b31d1cdad6a56a23f0c913e2482a31";

    /// <summary>IMDb's official download address for its data sets (https://developer.imdb.com/non-commercial-datasets/).</summary>
    public const string DefaultImdbDataFiles = "https://datasets.imdbws.com/";

    [Category("General"), ReadOnly(true), Description("Windows user these settings belong to.")]
    public string UserName { get; set; } = Environment.UserName;

    [Category("General"), DisplayName("LiteDB file"),
     Description("The library database: a file, or a folder (then SilverScreen.db in it). If the folder has no database, " +
                 "a new, empty one is made there. Used after a restart. The first default is Kino:LiteDbPath in appsettings.json.")]
    public string? LiteDBFilePath { get; set; }

    [Category("General"), DisplayName("Favorite watchlist"), Description("The watchlist opened by default.")]
    public string? FavoriteWatchList { get; set; } = "MyMovies";

    [Category("Movie services"), DisplayName("OMDb key"),
     Description("API key for omdbapi.com (details, posters, search). Get your own free key at https://www.omdbapi.com/apikey.aspx; " +
                 "the default key is shared by everyone using it. Empty means the default.")]
    public string? OMDBkey { get; set; } = DefaultOmdbKey;

    [Category("Movie services"), DisplayName("TMDb key"),
     Description("API key for themoviedb.org (episode details, finding IMDb ids in scans). Get your own free key at " +
                 "https://www.themoviedb.org/settings/api; the default key is shared. Empty means the default.")]
    public string? TMDBkey { get; set; } = DefaultTmdbKey;

    /// <summary>True while the OMDb or TMDb key is one of the shared defaults. (A method, so it isn't stored.)</summary>
    public bool UsesDefaultKeys() =>
        string.IsNullOrWhiteSpace(OMDBkey) || OMDBkey.Trim() == DefaultOmdbKey
        || string.IsNullOrWhiteSpace(TMDBkey) || TMDBkey.Trim() == DefaultTmdbKey;

    [Category("Movie services"), DisplayName("SubDL key"),
     Description("API key for subdl.com: downloads subtitles from the details window (and in SilverScreen). " +
                 "Get your own free key at https://subdl.com (see Help, Prerequisites).")]
    public string? SUBDLkey { get; set; }

    /// <summary>The languages SilverScreen asked SubDL for.</summary>
    public const string DefaultSubtitleLanguages = "NO,EN";

    [Category("Movie services"), DisplayName("Subtitle languages"),
     Description("Languages to download from SubDL, as codes separated by commas, e.g. NO,EN or NO,SV,DA,EN. Empty means NO,EN.")]
    public string? SubtitleLanguages { get; set; } = DefaultSubtitleLanguages;

    [Category("Movie services"), DisplayName("IMDb data files"),
     Description("Where IMDb publishes its free data sets (title.basics.tsv.gz and so on). The default is IMDb's official " +
                 "address; you only need to change it if IMDb moves them. Empty means the default.")]
    public string? IMDbDataFiles { get; set; } = DefaultImdbDataFiles;

    [Category("Plex"), DisplayName("Plex token (X-Plex-Token)"),
     Description("Your Plex token. With a token, scans match files against Plex before TMDb/OMDb, and watchlists can be copied to Plex playlists.")]
    [PasswordPropertyText(true)]
    public string? XPlexToken { get; set; }

    [Category("Plex"), DisplayName("Plex server name"), Description("Host name of the Plex server on your network, e.g. HTPC. Kino connects to http://<name>:32400.")]
    public string? XPlexServerName { get; set; }

    [Category("Other"), DisplayName("SQL connection"), Description("Used by SilverScreen's IMDb data import.")]
    public string? DefaultConnection { get; set; }

    [Category("Folders"), DisplayName("Folders"), TypeConverter(typeof(ExpandableObjectConverter))]
    public FilePaths UserFilePaths { get; set; } = new();

    /// <summary>The Plex base URL, or null when no server name is set. (A method, so it isn't stored.)</summary>
    public string? GetPlexBaseUrl() => string.IsNullOrWhiteSpace(XPlexServerName) ? null : $"http://{XPlexServerName.Trim()}:32400";

    public UserSettings Clone()
    {
        var copy = (UserSettings)MemberwiseClone();
        copy.UserFilePaths = UserFilePaths.Clone();
        return copy;
    }

    public sealed class FilePaths
    {
        [DisplayName("Movies"), Description("Folder with your movies (the Local movies window starts here).")]
        public string? MoviesSourcePath { get; set; }

        [DisplayName("Movies destination")]
        public string? MoviesDestinationPath { get; set; }

        [DisplayName("Series")]
        public string? SeriesSourcePath { get; set; }

        [DisplayName("Series destination")]
        public string? SeriesDestination { get; set; }

        [DisplayName("Pictures")]
        public string? PicturesSourcePath { get; set; }

        [DisplayName("Pictures destination")]
        public string? PicturesDestination { get; set; }

        public FilePaths Clone() => (FilePaths)MemberwiseClone();

        public override string ToString() => MoviesSourcePath ?? "(not set)";
    }
}
