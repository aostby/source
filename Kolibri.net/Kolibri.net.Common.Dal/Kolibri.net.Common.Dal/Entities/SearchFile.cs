using Kolibri.net.Common.Utilities;
namespace Kolibri.net.Common.Dal.Entities
{
    public class SearchFile
    { 
        public string Name { get; set; }
        public int Year { get; set; }
        public FileInfo FilePath { get; set; }
        public DirectoryInfo Path { get { return FilePath.Directory; } }       

        public string FileSize { get { return FileUtilities.GetFilesize(FilePath.Length); } }
        public bool Exists { get { return FilePath.Exists; } }

    }
}
