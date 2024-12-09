namespace Kolibri.net.Common.Dal.Entities
{
    public class FileItem
    {
        public FileItem(string imdbid, string fullFileName)
        {
            ImdbId = imdbid;
            FullName = fullFileName;
        }

        /// <summary>
        /// References the imdb id
        /// </summary>
        public string ImdbId { get; set; }
        /// <summary>
        /// References the path the file is found at
        /// </summary>
        public string FullName { get; set; }
    }
    
}
