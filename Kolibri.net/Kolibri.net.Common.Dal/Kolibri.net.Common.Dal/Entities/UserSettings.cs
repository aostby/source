namespace Kolibri.net.Common.Dal.Entities
{
    public  class UserSettings
    {
        public UserSettings()        { }
        public UserSettings(string liteDBFilePath)
        {
            UserName = Environment.UserName;
            LiteDBFilePath = liteDBFilePath;
        }
        public string LiteDBFilePath { get; set; }  
        public string UserName { get; set; } = Environment.UserName;
        public string OMDBkey { get; set; } = null;
        public string TMDBkey { get; set; } = null;
        public string SUBDLkey { get; set; } = null;
        public FilePaths UserFilePaths { get; set; }=new FilePaths();

        public void Save()
        {
            try
            {

            }
            catch (Exception)
            {
                  
            }
        }
    }
    public class FilePaths
    {
        public string MoviesSourcePath { get; set; } = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments );
        public string MoviesDestinationPath { get; set; } = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public string SeriesDestination { get; set; } = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public string SeriesSourcePath { get; set; } = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public string SeriesForm_Search { get; set; } = System.Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

        public string PicturesSourcePath { get; set; } = System.Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        public string PicturesDestination { get; set; } = System.Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        public string BeerXMLPath { get; set; } = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public string BeerWordPressExportPath { get; set; } = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    }


}