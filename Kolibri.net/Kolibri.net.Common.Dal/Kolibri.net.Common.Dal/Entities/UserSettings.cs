using Kolibri.net.Common.Dal.Controller;
using System.ComponentModel;

namespace Kolibri.net.Common.Dal.Entities
{
    public class UserSettings
    {
        public UserSettings() { }
        public UserSettings(string liteDBFilePath)
        {
            UserName = Environment.UserName;
            LiteDBFilePath = liteDBFilePath;
        }
        [BrowsableAttribute(true)]
        [Description(nameof(LiteDBFilePath))]
        [DisplayName(nameof(LiteDBFilePath))]
        public string LiteDBFilePath { get; set; }

        [BrowsableAttribute(true)]
        [ReadOnly(true)]
        [Description(nameof(UserName))]
        [DisplayName(nameof(UserName))]
        public string UserName { get; set; } = Environment.UserName;

        [BrowsableAttribute(true)]
        [Description(nameof(OMDBkey))]
        [DisplayName(nameof(OMDBkey))]
        public string OMDBkey { get; set; } = "be7b1bec";

        [BrowsableAttribute(true)]
        [Description(nameof(TMDBkey))]
        [DisplayName(nameof(TMDBkey))]
        public string TMDBkey { get; set; } = null;

        [BrowsableAttribute(true)]
        [Description(nameof(SUBDLkey))]
        [DisplayName(nameof(SUBDLkey))]
        public string SUBDLkey { get; set; } = null;

        [BrowsableAttribute(true)]
        [Description(nameof(UserFilePaths))]
        [DisplayName(nameof(UserFilePaths))]
        public FilePaths UserFilePaths { get; set; } = new FilePaths();

        public void Save()
        {
            using (LiteDBController contr = new LiteDBController(new FileInfo(LiteDBFilePath), false, false))
            {
                if (!contr.Upsert(this)) { throw new Exception($"Could not save {this.UserName}'s {this.GetType().Name}"); }
            }
        }
        public class FilePaths
        {
            [BrowsableAttribute(true)]
            [Description(nameof(MoviesSourcePath))]
            [DisplayName(nameof(MoviesSourcePath))]
            public string MoviesSourcePath { get; set; } = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            [BrowsableAttribute(true)]
            [Description(nameof(MoviesDestinationPath))]
            [DisplayName(nameof(MoviesDestinationPath))]
            public string MoviesDestinationPath { get; set; } = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            [BrowsableAttribute(true)]
            [Description(nameof(SeriesDestination))]
            [DisplayName(nameof(SeriesDestination))]
            public string SeriesDestination { get; set; } = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            [BrowsableAttribute(true)]
            [Description(nameof(SeriesSourcePath))]
            [DisplayName(nameof(SeriesSourcePath))]
            public string SeriesSourcePath { get; set; } = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
           
            //[BrowsableAttribute(true)]
            //[Description(nameof(SeriesForm_Search))]
            //[DisplayName(nameof(SeriesForm_Search))]
            //public string SeriesForm_Search { get; set; } = "Tulsa";


            [BrowsableAttribute(true)]
            [Description(nameof(PicturesSourcePath))]
            [DisplayName(nameof(PicturesSourcePath))]
            public string PicturesSourcePath { get; set; } = System.Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            [BrowsableAttribute(true)]
            [Description(nameof(PicturesDestination))]
            [DisplayName(nameof(PicturesDestination))]
            public string PicturesDestination { get; set; } = System.Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            //[BrowsableAttribute(true)]
            //[Description(nameof(BeerXMLPath))]
            //[DisplayName(nameof(BeerXMLPath))]
            //public string BeerXMLPath { get; set; } = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            //[BrowsableAttribute(true)]
            //[Description(nameof(BeerWordPressExportPath))]
            //[DisplayName(nameof(BeerWordPressExportPath))]
            //public string BeerWordPressExportPath { get; set; } = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
    }
}