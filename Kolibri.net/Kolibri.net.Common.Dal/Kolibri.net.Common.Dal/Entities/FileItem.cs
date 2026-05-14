using LiteDB;

namespace Kolibri.net.Common.Dal.Entities
{   public class FileItem
    {
        private FileInfo _fileInfo;
        private Lazy<FileInfo> _lazyFileInfo; 

        [Obsolete("Constructor only, do not use")]
        public FileItem() { }
        public FileItem(string imdbid, string fullFileName)
        {
            ImdbId = imdbid;
            FullName = fullFileName;
            _lazyFileInfo = new Lazy<FileInfo>(() => new FileInfo(FullName));
        }

        /// <summary>
        /// References the imdb id
        /// </summary>
        public string ImdbId { get; set; }
        /// <summary>
        /// References the path the file is found at
        /// </summary>
        public string FullName { get; set; }

                /// <summary>
                /// Helper attribute, same as FullName, but as a FileInfo object. Ignored by database
                /// </summary>
                [BsonIgnoreAttribute]
        public FileInfo ItemFileInfo
        {
            get
            {
                if (_lazyFileInfo == null )
                {
                    _lazyFileInfo = new Lazy<FileInfo>(() => new FileInfo(FullName));
                }

                return _lazyFileInfo.Value;
            }
        }

    }
    
}
