using MySqlConnector;

namespace Kolibri.Kino.SilverScreenCopy;

/// <summary>
/// The image cache table (Image) beyond LiteDB's _id/Added/Data: which title an image belongs to and what it is, so
/// images are found by IMDb id and type instead of by the key Kino happened to use (an id, or the address it came from).
/// </summary>
public static class ImageTable
{
    public const string Name = "Image";

    /// <summary>The kinds of image: posters and backdrops (TMDb, Kino), episode stills, people, and Kodi/fanart.tv's art.</summary>
    public static readonly IReadOnlyList<string> Types =
        ["poster", "backdrop", "still", "profile", "avatar", "landscape", "thumb", "clearlogo", "clearart", "banner", "discart", "keyart", "characterart", "icon"];

    /// <summary>Adds the metadata columns and their index when they are missing (the copy from LiteDB has only _id, Added, Data).</summary>
    public static async Task EnsureColumnsAsync(MySqlConnection connection, CancellationToken ct = default)
    {
        await using var cmd = connection.CreateCommand();
        cmd.CommandText = """
            ALTER TABLE `Image`
              ADD COLUMN IF NOT EXISTS `ImdbId` VARCHAR(20) CHARACTER SET ascii COLLATE ascii_bin NULL COMMENT 'the title (a series for episode stills)',
              ADD COLUMN IF NOT EXISTS `ImageType` VARCHAR(20) CHARACTER SET ascii COLLATE ascii_bin NULL COMMENT 'poster, backdrop, still, profile, avatar, landscape, thumb, clearlogo, clearart, banner, discart, keyart, characterart, icon',
              ADD COLUMN IF NOT EXISTS `Season` SMALLINT NULL COMMENT 'episode stills and season posters',
              ADD COLUMN IF NOT EXISTS `Episode` SMALLINT NULL,
              ADD COLUMN IF NOT EXISTS `TmdbId` INT NULL COMMENT 'people, and titles known only by TMDb id',
              ADD COLUMN IF NOT EXISTS `Source` VARCHAR(20) CHARACTER SET ascii COLLATE ascii_bin NULL COMMENT 'kino, kodi, tmdb, plex, omdb, web',
              ADD COLUMN IF NOT EXISTS `SourceUrl` TEXT NULL COMMENT 'where the image came from (Plex addresses without the token)',
              ADD COLUMN IF NOT EXISTS `Width` INT NULL,
              ADD COLUMN IF NOT EXISTS `Height` INT NULL,
              ADD INDEX IF NOT EXISTS `ix_imdb_type` (`ImdbId`, `ImageType`, `Season`, `Episode`)
            """;
        await cmd.ExecuteNonQueryAsync(ct);
    }
}
