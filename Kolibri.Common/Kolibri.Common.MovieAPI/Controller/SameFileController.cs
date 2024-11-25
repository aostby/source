using Kolibri.Common.MovieAPI.Entities;
using Kolibri.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kolibri.Common.MovieAPI.Controller
{
    public class SameFileController
    {
        public DirectoryInfo DirInfo { get; }

        List<SearchFile> _currentList;

        public SameFileController(DirectoryInfo dirInfo)
        {
            DirInfo = dirInfo;
        }
        public List<SearchFile> SearchForMovies()
        {
            _currentList = new List<SearchFile>();
            DataTable resultTable = null;

            List<string> common = Kolibri.Common.Utilities.FileUtilities.MoviesCommonFileExt();
            var searchStr = "*." + string.Join("|*.", common);
            var list = Kolibri.Common.Utilities.FileUtilities.GetFiles(DirInfo, searchStr, true);

            if (list.Count() < 1) return _currentList;
            var count = 0;
            foreach (var file in list)
            {
                _currentList.Add(new SearchFile { Name = Utilities.MovieUtilites.GetMovieTitle(file.Name), FilePath = new FileInfo(file.FullName) });
            }
            return _currentList;
        }

        public List<SearchFile> GetDupes()
        {
            if (_currentList == null || _currentList.Count < 1) SearchForMovies();

            //https://stackoverflow.com/questions/57749071/using-linq-to-get-all-object-with-the-same-value-property

            var ret = _currentList
           .GroupBy(x => x.Name)        // Group by name
           .Where(g => g.Count() > 1)   // Select only groups having duplicates
           .SelectMany(g => g);         // Ungroup (flatten) the groups

            return ret.ToList();
        }

        public List<SearchFile> GetDupesByFolder()
        {
            if (_currentList == null || _currentList.Count < 1) SearchForMovies();

            //https://stackoverflow.com/questions/57749071/using-linq-to-get-all-object-with-the-same-value-property

            var ret = _currentList
           .GroupBy(x => x.Path.FullName)        // Group by name
           .Where(g => g.Count() > 1)   // Select only groups having duplicates
           .SelectMany(g => g);         // Ungroup (flatten) the groups

            return ret.ToList();
        }

    }
}