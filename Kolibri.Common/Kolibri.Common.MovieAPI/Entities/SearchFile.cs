using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kolibri.Common.Utilities;
using OMDbApiNet.Model;
namespace Kolibri.Common.MovieAPI.Entities
{
    public class SearchFile
    { 
        public string Name { get; set; }
        public FileInfo FilePath { get; set; }
        public DirectoryInfo Path { get { return FilePath.Directory; } }       

        public string FileSize { get { return FileUtilities.GetFilesize(FilePath.Length); } }
        public bool Exists { get { return FilePath.Exists; } }

    }
}
