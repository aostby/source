using Kolibri.Kino.Core;
using Kolibri.Kino.KMDb;
using Kolibri.Kino.SilverScreenCopy;

namespace Kolibri.Kino.Tests;

public class ImageInfoTests
{
    [Fact]
    public void Png_and_gif_sizes_are_read_from_the_header()
    {
        byte[] png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0, 0, 0, 13, 0x49, 0x48, 0x44, 0x52, 0, 0, 1, 0xF4, 0, 0, 2, 0xEE];
        Assert.Equal((500, 750), ImageInfo.Size(png));
        byte[] gif = [0x47, 0x49, 0x46, 0x38, 0x39, 0x61, 0x80, 0x07, 0x38, 0x04];
        Assert.Equal((1920, 1080), ImageInfo.Size(gif));
    }

    [Fact]
    public void Jpeg_size_comes_from_the_start_of_frame()
    {
        // SOI, an APP0 segment of 16 bytes, then SOF0 with height 1080 and width 1920.
        byte[] jpeg = [0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, .. new byte[14], 0xFF, 0xC0, 0x00, 0x11, 0x08, 0x04, 0x38, 0x07, 0x80, 0x03];
        Assert.Equal((1920, 1080), ImageInfo.Size(jpeg));
    }

    [Fact]
    public void Unknown_or_cut_off_images_have_no_size()
    {
        Assert.Null(ImageInfo.Size([1, 2, 3]));
        Assert.Null(ImageInfo.Size([0x89, 0x50, 0x4E, 0x47]));
    }

    [Theory]
    [InlineData(500, 750, "poster")]
    [InlineData(1920, 1080, "backdrop")]
    public void Shape_tells_poster_from_backdrop(int w, int h, string type) => Assert.Equal(type, ImageInfo.TypeFromShape(w, h));
}

public class ImageClassifierTests
{
    private static readonly ImageClassifier Classifier = new(null!, TextWriter.Null);

    [Fact]
    public void Kino_keys_tell_title_and_type()
    {
        Assert.Equal(new ImageMeta("tt0133093", "poster", Source: "kino"), Classifier.Classify("tt0133093", null));
        Assert.Equal(new ImageMeta("tt0903747", "still", 1, 2, Source: "kino"), Classifier.Classify("tt0903747_S01E02", null));
        Assert.Equal(new ImageMeta(null, "profile", TmdbId: 287, Source: "tmdb"), Classifier.Classify("tmdb-person-287", null));
        Assert.Equal("icon", Classifier.Classify("buttonSimilar", null).Type);
    }

    [Fact]
    public void Unknown_addresses_are_typed_by_shape()
    {
        var meta = Classifier.Classify("https://image.tmdb.org/t/p/w500/unknown.jpg", (1920, 1080));
        Assert.Equal((null, "backdrop", "tmdb"), (meta.ImdbId, meta.Type, meta.Source));
    }

    [Theory]
    [InlineData("https://image.tmdb.org/t/p/w500/AbC.jpg", "tmdb:abc.jpg")]
    [InlineData("https://image.tmdb.org/t/p/original/AbC.jpg", "tmdb:abc.jpg")]
    [InlineData("/AbC.jpg", "tmdb:abc.jpg")]
    [InlineData("http://HTPC:32400/library/metadata/1/thumb/2?X-Plex-Token=secret", "http://htpc:32400/library/metadata/1/thumb/2")]
    public void Addresses_match_whatever_the_size_or_token(string url, string key) => Assert.Equal(key, ImageClassifier.AddressKey(url));

    [Theory]
    [InlineData("https://image.tmdb.org/t/p/w200/x.jpg", "tmdb")]
    [InlineData("http://HTPC:32400/library/metadata/1/thumb/2", "plex")]
    [InlineData("https://m.media-amazon.com/images/M/x.jpg", "omdb")]
    [InlineData("https://assets.fanart.tv/fanart/movies/1/x.jpg", "fanart")]
    [InlineData("https://www.platekompaniet.no/x.jpg", "web")]
    public void Sources_are_told_from_the_address(string url, string source) => Assert.Equal(source, ImageClassifier.SourceOf(url));
}

public class KodiImportTests
{
    [Theory]
    [InlineData("13_Hours__The_Secret_Soldiers_of_Benghazi_2016", "13 Hours: The Secret Soldiers of Benghazi", "2016")]
    [InlineData("'71_2014", "'71", "2014")]
    [InlineData("The_Matrix_1999", "The Matrix", "1999")]
    public void File_names_match_title_and_year(string fileName, string title, string year) =>
        Assert.Equal(KodiImport.NameKey(title + year), KodiImport.NameKey(fileName));

    [Theory]
    [InlineData("fanart", "backdrop")]
    [InlineData("poster", "poster")]
    [InlineData("landscape", "landscape")]
    [InlineData("clearlogo", "clearlogo")]
    public void Kodi_art_names_become_image_types(string kodi, string type) => Assert.Equal(type, KodiImport.TypeOf(kodi));

    [Fact]
    public void Movies_are_read_with_their_imdb_id_and_art()
    {
        var file = Path.Combine(Path.GetTempPath(), $"videodb-{Guid.NewGuid():N}.xml");
        File.WriteAllText(file, """
            <?xml version="1.0" encoding="UTF-8" standalone="yes" ?>
            <videodb><version>1</version>
              <movie><title>Dr. No</title><year>1962</year><uniqueid type="tmdb">646</uniqueid><uniqueid type="imdb">tt0055928</uniqueid>
                <art><fanart>https://image.tmdb.org/t/p/original/a.jpg</fanart><poster>https://assets.fanart.tv/p.jpg</poster></art></movie>
              <movie><title>No id</title><year>2000</year><uniqueid type="tmdb">1</uniqueid></movie>
              <tvshow><title>A show</title><uniqueid type="imdb">tt1</uniqueid></tvshow>
            </videodb>
            """);
        try
        {
            var movie = Assert.Single(KodiImport.ReadMovies(file));
            Assert.Equal(("Dr. No", "1962", "tt0055928"), (movie.Title, movie.Year, movie.ImdbId));
            Assert.Equal("https://image.tmdb.org/t/p/original/a.jpg", movie.Art["fanart"]);
        }
        finally
        {
            File.Delete(file);
        }
    }

    [Fact]
    public void The_export_date_comes_from_the_folder_name() =>
        Assert.Equal(new DateTime(2024, 10, 4), KodiImport.ExportDate(@"\\HTPC\Shared\Kodi_Cache\Excalibur\kodi_videodb_2024-10-04"));
}

public class ArtKeyTests
{
    [Fact]
    public void Episode_keys_mean_the_episodes_still()
    {
        Assert.Equal(("tt0903747", "still", 1, 2), PosterService.ArtKey("tt0903747_S01E02"));
        Assert.Equal(("tt0133093", "poster", (int?)null, (int?)null), PosterService.ArtKey("tt0133093"));
    }
}
