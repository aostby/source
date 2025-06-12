using com.sun.org.apache.bcel.@internal.generic;
using MovieFileLibrary;
using System.Data;
using System.Text.RegularExpressions;
using System.Xml.Schema;

namespace Kolibri.net.Common.Utilities
{
    public class MovieUtilites
    {
        private static DataTable MovieLinks
        {
            get
            {
                DataTable ret = DataSetUtilities.ColumnNamesToDataTable("url", "name", "comment").Tables[0];
                ret.Rows.Add("https://www.imdb.com/chart/moviemeter/", "MovieMeter", "");
                ret.Rows.Add("https://www.imdb.com/search/title/?genres=war&title_type=feature&explore=genres", "Top 50 War movies", "");
                ret.Rows.Add($"https://www.moviefone.com/movies/{DateTime.Now.Year}/war/", "Moveifone war", "");
                ret.Rows.Add("https://yts.mx/", "YTS", "");
                ret.Rows.Add("https://www.boxofficemojo.com/genre/sg2564681985/?ref_=bo_gs_table_139", "BoxOfficeMojo War", "");
                ret.Rows.Add("https://www.filmsite.org/greatwarfilms.html", "Filmsite", "");

                ret.Rows.Add("https://top10.netflix.com/", "Top 10 NetFlix");
                return ret;
            }
        }
        public static Dictionary<string, string> MovieLinksDic
        {
            get
            {
                return DataSetUtilities.DataTableToDictionary(MovieLinks, "name", "url");
            }
        }
        public static List<string> YearList
        {
            get
            {
                List<string> decades = new List<string>();
                var years = Enumerable.Range(1907, DateTime.Now.Year - 1907 + 1).ToList();
                int first = years.FirstOrDefault();
                foreach (var year in years)
                {
                    if (year % 10 == 0)
                    {
                        string range = $"{first}-{year}";
                        decades.Add(range);
                        first = year;
                    }
                }
                //manually add an interval of interest closer to today
                decades.Add($"{DateTime.Now.AddYears(-5).Year}-{DateTime.Now.Year}");

                //manually add an interval of interest
                decades.Add("1900-1940");

                //add last five years
                //   first = DateTime.Now.Year - 5;
                years = Enumerable.Range(first, DateTime.Now.Year - first + 1).ToList();
                decades.AddRange(years.Select(i => i.ToString()).OrderByDescending(i => i).ToList());



                //Sort the list
                var ret = decades.OrderByDescending(i => i).ToList();
                ret.Insert(0, $"{(DateTime.Now.Year / 10) * 10}-{DateTime.Now.Year}");


                //       List<string> ret = new List<string>() { "", "2022", "2021", "2020", "2019", "2020-2029", "2015-2019", "2010-2014", "2000-2009", "1990-1999", "1980-1989", "1970-1979", "1900-1969" };

                return ret;
            }
        }
        public static List<string> GenreList
        {
            get
            {
                return new List<string>() { "", "Action", "Adventure", "Animation", "Biography", "Comedy", "Crime", "Documentary", "Drama", "Family", "Fantasy", "Film-Noir", "History", "Horror", "Music", "Musical", "Mystery", "Romance", "Sci-Fi", "Sport", "Thriller", "War", "Western", };
            }
        }


        /// <summary>
        /// Metode som henter år fra oppgitt filstreng. 
        /// Dersom det blir funnet flere årstall i filstrengen, blir det største året foretrukket
        /// </summary>
        /// <param name="directoryName">directory eller fil en vil finne ett årstall fra</param>
        /// <returns></returns>
        public static int GetYear(string directoryName)
        {
            int year = 1;
            try
            {
                string dir = directoryName;
                if (dir.Contains(@"\"))
                    dir = directoryName.Substring(directoryName.LastIndexOf(@"\"));

                string pattern = @"\d{4}";
                Regex regex = new Regex(pattern);
                Match match = regex.Match(dir);
                if (match.Success)
                {
                    year = Convert.ToInt32((match.Value));
                }
                if (directoryName.Contains($"({year})"))
                    return year;
                //else if (directoryName.Contains($"[{year}]"))                    return year;

                var k = Regex.Matches(dir, pattern);
                if (k.Count > 1)
                {
                    Match[] matches = k
                        .Cast<Match>()
                        .ToArray();

                    var biggest = matches.Select(i => Int32.Parse(i.ToString())).ToList().Max();
                    var smallest = matches.Select(i => Int32.Parse(i.ToString())).ToList().Min();

                    if (directoryName.Contains($"({biggest})"))
                        return biggest;
                    if (directoryName.Contains($"({smallest})"))
                        return smallest;

                        if (Enumerable.Range(1900, DateTime.Now.AddYears(3).Year).Contains(biggest))
                        year = biggest;

                    if ((biggest - smallest) < 4)
                        year = smallest; // 2012 utgitt i 2009


                    else if (Enumerable.Range(1900, DateTime.Now.AddYears(3).Year).Contains(smallest))
                        year = smallest;
                    else if (biggest > year && biggest < 2040)
                    { year = biggest; }
                }
            }
            catch (Exception ex)
            {
                year = 1;
            }
            return year;
        }

        #region Filmnavn fra filnavn
        // Create an instance of the MovieDetector class.
        private static MovieDetector movieDetector = null;
        private static void Init()
        {
            if (movieDetector == null)
                movieDetector = new MovieDetector();
        }
        /// <summary>
        /// Finn tittelen til en film fra ett filnavn
        /// https://github.com/moviecollection/movie-file-library
        /// </summary>
        /// <param name="filePath">filnavn uten fulstendig sti</param>
        /// <returns>navnet på filmen</returns>
        public static string GetMovieTitle(string filePath)
        {
            Init();
            string ret = string.Empty;
            try
            {
                // Call GetInfo to process the filename.
                MovieFileLibrary.MovieFile movieFile = movieDetector.GetInfo(filePath);
                if (movieFile.IsSuccess)
                    ret = movieFile.Title;
                else ret = GetMovieTitleLight(filePath);
            }
            catch (Exception)
            { }
            return ret;
        }

        public static MovieFile DetectMovieFile(FileInfo filePath)
        {
            Init();
            MovieFile ret = null;
            try
            {
                // Call GetInfo to process the filename.
                MovieFileLibrary.MovieFile movieFile = movieDetector.GetInfo(filePath.FullName);
                if (movieFile.IsSuccess)
                    ret = movieFile;
                
            }
            catch (Exception)
            { ret = null; }
            return ret;
        }


        /// <summary>
        /// https://github.com/mebjas/movie-name-extractor
        /// </summary>
        /// <param name="movieTitleToFetch"></param>
        /// <returns></returns>
        private static string GetMovieTitleLight(string movieTitleToFetch)
        {
            string ret = GetMovieTitleprepp(movieTitleToFetch).Replace(".", " ").Trim(); ;

            int pos = ret.IndexOf($"{GetYear(ret)}");
            if (pos > 0)
                ret = ret.Substring(0, pos);
            var regExstring = @"/\[.*?\]|\(.*?\)/";
            ret = Regex.Replace(ret, regExstring, "");
            regExstring = @"/s(\d{1,2})e(\d{1,3})|(\d{1,2})x(\d{1,3})/";
            ret = Regex.Replace(ret, regExstring, "");

            return ret.Trim();

        }

        private static string GetMovieTitleprepp(string movieTitleToFetch)
        {

            var regExstring = @"([\[].*?[\]]|[\(].*?[\)])";
            var ret = Regex.Replace(movieTitleToFetch, regExstring, "");
            return ret.Trim();

        }
        #endregion

        //
        // Summary:
        //     Video file extensions.
        public static readonly List<string> FiltersVideoVlc = new List<string>()
        {
        "*.3g2", "*.3gp", "*.3gp2", "*.3gpp", "*.amv", "*.asf", "*.avi", "*.bik", "*.bin", "*.divx",
        "*.drc", "*.dv", "*.f4v", "*.flv", "*.gvi", "*.gfx", "*.iso", "*.m1v", "*.m2v", "*.m2t",
        "*.m2ts", "*.m4v", "*.mkv", "*.mov", "*.mp2", "*.mp2v", "*.mp4", "*.mp4v", "*.mpe", "*.mpeg",
        "*.mpeg1", "*.mpeg2", "*.mpeg4", "*.mpg", "*.mpv2", "*.mts", "*.mtv", "*.mxf", "*.mxg", "*.nsv",
        "*.nuv", "*.ogm", "*.ogv", "*.ogx", "*.ps", "*.rec", "*.rm", "*.rmvb", "*.rpl", "*.thp",
        "*.tod", "*.ts", "*.tts", "*.txd", "*.vob", "*.vro", "*.webm", "*.wm", "*.wmv", "*.wtv",
        "*.xesc"
        };
        public static List<string> MoviesCommonFileExt(bool withPunctuation = false)
        {
            var ret = new List<string>() { "avi", "mkv", "mp4", "mpg", "mpeg", "ts" };

            if (withPunctuation)
                ret = ret.Select(r => string.Concat('.', r)).ToList();

            return ret.Distinct().ToList();
        }
        public static List<string> MoviesFileExt(bool withPunctuation = false)
        {
            var ret= new List<string>() { 
                "3g2", "3gp", "amv", "asf", "avi", "drc", "flv", "flv", "flv", "f4v",
                "f4p", "f4a", "f4b", "gif", "gifv", "m4v", "mkv", "mng", "mov", "qt", 
                "mp4", "m4p", "m4v", "mpg", "mp2", "mpeg", "mpe", "mpv", "mpg", "mpeg", 
                "m2v", "MTS", "M2TS", "TS", "mxf", "nsv", "ogv", "ogg", "rm", "rmvb", 
                "roq", "svi", "vob", "webm", "wmv", "yuv", "ts" };
            if (withPunctuation)
                ret = ret.Select(r => string.Concat('.', r)).ToList();

            return ret.Distinct().ToList();
        }
        public static List<string> MulitpartFilter()
        {
            var filterList = new List<string>() { "CD", ".PART", " PART", "Disk0", "Extra", "@__thumb" };
            return filterList;
        }
    }
}
