using Kolibri.Utilities;
using LiteDB;
using MoviesFromImdb.Entities;
using OMDbApiNet.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MoviesFromImdb.Controller
{ 
    public class LiteDBController
        {
            private bool _seriesDB = false;
            private bool ExclusiveConnection { get; set; }

            private LiteDatabase _liteOMDB;
            public ConnectionString ConnectionString;

            public string LastBConnectionString
            {
                get
                {
                    if (_seriesDB)
                        return Properties.Settings.Default.LastSeriesDBConnectionString;
                    else
                        return Properties.Settings.Default.LastOMDBConnectionString;
                }
            }

            private string DefaultFilePath
            {
                get
                {
                return @"C:\inetpub\wwwroot\TMDB\OMDB.db";
                    //FileInfo filepath = new FileInfo(Path.Combine(Application.StartupPath, "OMDB", "OMDB.db"));
                    //if (_seriesDB) filepath = new FileInfo(Path.Combine(Application.StartupPath, "OMDB", "series.db"));
                    //if (!File.Exists(filepath.FullName))
                    //    filepath.Directory.Create();
                    //return filepath.FullName;
                }
            }

            public LiteDBController(bool exclusiveAccess = false, bool readOnly = true, bool seriesDB = false)
            {
                _seriesDB = seriesDB;
                ExclusiveConnection = exclusiveAccess;
                readOnly = false;
                ConnectionString = new ConnectionString()
                {
                    Connection = ExclusiveConnection ? ConnectionType.Direct : ConnectionType.Shared,
                    ReadOnly = readOnly,
                    Upgrade = false,
                    Password = null,
                    Filename = DefaultFilePath,
                };
                try
                {
                    if (!string.IsNullOrEmpty(LastBConnectionString))
                    {
                        if (File.Exists(LastBConnectionString))
                        {
                            ConnectionString.Filename = LastBConnectionString;
                        }
                    }
                    else
                        ConnectionString.Filename = DefaultFilePath;
                }
                catch (Exception) { }

                try
                {
                    if (_seriesDB)
                        Properties.Settings.Default.LastSeriesDBConnectionString = ConnectionString.Filename;
                    else
                        Properties.Settings.Default.LastOMDBConnectionString = ConnectionString.Filename;
                    Properties.Settings.Default.Save();
                }
                catch (Exception) { }

                _liteOMDB = new LiteDatabase(ConnectionString);
            }
            //public void LiteDBController_old(bool exclusiveAccess = false)
            //{
            //    #region dette veriker

            //    if (exclusiveAccess)
            //        _liteOMDB = new LiteDatabase(DefaultFilePath);
            //    else
            //        _liteOMDB = new LiteDatabase(new ConnectionString(DefaultFilePath) { Connection = ConnectionType.Shared });
            //    return;

            //    #endregion
            //}
            public void Dispose()
            {

                try
                {
                    _liteOMDB.Dispose();
                }
                catch (Exception)
                {
                }
            }

            #region Item table funcitons
            public IEnumerable<Item> FindAllItems(string type = null)
            {
                if (!string.IsNullOrEmpty(type))
                {
                    var list = _liteOMDB.GetCollection<Item>("Item").Find(x => x.Type.Equals(type, StringComparison.OrdinalIgnoreCase));
                    return list;
                }
                else
                    return _liteOMDB.GetCollection<Item>("Item")
                        .FindAll();
            }

            public Item FindItemByTitle(string title, int year)
            {
                return _liteOMDB.GetCollection<Item>("Item")
                    .Find(x => x.Title == title.Trim() && x.Year == year.ToString()).FirstOrDefault();
            }
            public IEnumerable<Item> FindItemByTitle(string searchTerm)
            {
                StringComparison comp = StringComparison.Ordinal;

                return _liteOMDB.GetCollection<Item>("Item")
                    .Find(x => x.Title.ToUpper().Contains(searchTerm.ToUpper()));
            }
            public IEnumerable<Item> FindItemByGenre(string genre)
            {
                return _liteOMDB.GetCollection<Item>("Item")
                    .Find(x => x.Genre.Contains(genre));
            }
            public IEnumerable<Item> FindItemByGenreNew(string genre)
            {
                // This way is more efficient
                if (_liteOMDB.GetCollection<Item>().Count(Query.Contains("Genre", genre)) > 0)
                {
                    return _liteOMDB.GetCollection<Item>().Find(Query.Contains("Genre", genre));
                }
                return null;

            }

            public Item FindItem(string imdbId)
            {
                try
                {
                    var ret = _liteOMDB.GetCollection<Item>("Item")
                                             .Find(x => x.ImdbId == imdbId).FirstOrDefault();
                    return ret;
                }
                catch (Exception ex)
                {
                    return null;
                }

            }

            /// <summary>
            /// Insert a movie or series title
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public bool Insert(Item item)
            {
                try
                {
                    _liteOMDB.GetCollection<Item>("Item")
                        .Insert(item.ImdbId, item);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            public IEnumerable<Item> GetAllItemsTypes()
            {
                var list = _liteOMDB.GetCollection<Item>("Item").FindAll().ToList();
                var typeList = list.Select(o => o.Type).Distinct();
                return list;
            }
            public IEnumerable<Item> GetAllItemsByType(string searchTerm)
            {
                StringComparison comp = StringComparison.Ordinal;

                var temp = _liteOMDB.GetCollection<Item>("Item");
                var list = temp.Find(x => x.Type.ToUpper().Contains(searchTerm.ToUpper()));
                return list;
            }


            public bool Upsert(Item item)
            {
                if (!Insert(item))
                    try
                    {
                        Update(item);
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                return true;
            }

            public bool Update(Item movie)
            {
                return _liteOMDB.GetCollection<Item>("Item")
                    .Update(movie.ImdbId, movie);
            }

            public int DeleteItem(string id)
            {
                try
                {
                    var col = _liteOMDB.GetCollection<Item>();
                    //col.DeleteMany("1=1");
                    col.Delete(id);

                    var nokko = _liteOMDB.GetCollection<FileItem>();
                    nokko.Delete(id);
                    return 1;
                }
                catch (Exception)
                {

                    return -1;
                }


            }

            #region series

            //internal Season FindSeason(string seriesName, int seasonNumber)
            //{
            //    try
            //    {
            //   return _liteOMDB.GetCollection<Season>("Season") 
            //       .Find(x => x.Title.ToLower().Contains(seriesName.ToLower()) && x.SeasonNumber.Equals(seasonNumber)).FirstOrDefault();
            //    }
            //    catch (Exception ex)
            //    {
            //        return null;
            //    }
            //}


            internal Season FindSeason(string seriesName, string seasonnumber)
            {
                try
                {
                    return _liteOMDB.GetCollection<Season>("Season")
                 .Find(x => x.Title.ToLower().Contains(seriesName.ToLower()) && x.SeasonNumber.Equals(seasonnumber)).FirstOrDefault();

                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            public bool Update(Season season)
            {
                return _liteOMDB.GetCollection<Season>("Season")
                    .Update(season.Title + season.SeasonNumber, season);
            }
            public bool Insert(Season season)
            {
                _liteOMDB.GetCollection<Season>("Season")
                    .Insert(season.Title + season.SeasonNumber, season);
                return true;
            }

            public bool Upsert(Season season)
            {
                if (season == null) return false;

                try
                {
                    Insert(season);
                }
                catch (Exception ex)
                {
                    try
                    {
                        Update(season);
                    }
                    catch (Exception exu)
                    {
                        return false;
                    }
                }
                return true;
            }

            public bool Insert(SeasonEpisode ep)
            {
                try
                {
                    _liteOMDB.GetCollection<SeasonEpisode>("SeasonEpisode")
                        .Insert(ep.ImdbId, ep);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            public bool Update(SeasonEpisode ep)
            {
                try
                {
                    _liteOMDB.GetCollection<SeasonEpisode>("SeasonEpisode")
                        .Update(ep.ImdbId, ep);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            public bool Upsert(SeasonEpisode ep)
            {
                bool ret = false;
                if (!string.IsNullOrEmpty(ep.ImdbId))
                {
                    ret = Insert(ep);
                    if (!ret) ret = Update(ep);
                }


                return ret;
            }

            public SeasonEpisode FindSeasonEpisode(string imdbId)
            {
                try
                {
                    return _liteOMDB.GetCollection<SeasonEpisode>("SeasonEpisode")
                                .Find(x => x.ImdbId == imdbId).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            public SeasonEpisode FindSeasonEpisode(string seriesName, string season, string episode)
            {
                string id = HashString($"{seriesName}{season}".ToUpper());
                try
                {//en må prøve å finne sesong og lete seg frem.

                    if (_liteOMDB.GetCollection<Season>().Count(Query.Contains("Title", seriesName)) > 0)
                    {
                        var ses = _liteOMDB.GetCollection<Season>().Find(x => x.Title.ToLower() == seriesName.ToLower()
                        && x.SeasonNumber == season).FirstOrDefault();

                        return ses.Episodes.Find(x => x.Episode == episode);
                    }
                    return null;

                }
                catch (Exception ex)
                {
                    return null;
                }

            }

            public bool Insert(Episode ep)
            {
                try
                {
                    _liteOMDB.GetCollection<Episode>("Episode")
                        .Insert(ep.ImdbId, ep);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            public bool Update(Episode ep)
            {
                try
                {
                    _liteOMDB.GetCollection<Episode>("Episode")
                        .Update(ep.ImdbId, ep);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            public bool Upsert(Episode ep)
            {
                bool ret = Insert(ep);
                if (!ret) ret = Update(ep);
                return ret;
            }

            public Episode GetEpisode(string imdbId)
            {
                try
                {
                    var ret = _liteOMDB.GetCollection<Episode>("SeasonEpisode")
                                      .Find(x => x.ImdbId == imdbId).FirstOrDefault();
                    return ret;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            static string HashString(string text, string salt = "")
            {
                if (String.IsNullOrEmpty(text))
                {
                    return String.Empty;
                }

                // Uses SHA256 to create the hash
                using (var sha = new System.Security.Cryptography.SHA256Managed())
                {
                    // Convert the string to a byte array first, to be processed
                    byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(text + salt);
                    byte[] hashBytes = sha.ComputeHash(textBytes);

                    // Convert back to a string, removing the '-' that BitConverter adds
                    string hash = BitConverter
                        .ToString(hashBytes)
                        .Replace("-", String.Empty);

                    return hash;
                }
            }
            #endregion



            #endregion

            #region FileItem functions

            public IEnumerable<FileItem> FindFileItemByGenre(string genre)
            {
                List<FileItem> ret = new List<FileItem>();
                foreach (var item in FindItemByGenre(genre))
                {
                    var movie = FindFile(item.ImdbId);
                    if (movie != null)
                        ret.Add(movie);
                };
                return ret;
            }

            public IEnumerable<FileItem> FindAllFileItems()
            {
                var col = _liteOMDB.GetCollection<FileItem>("FileItem");
                List<FileItem> ret = new List<FileItem>();
                ret = col.Find(Query.GT("FullName", "-1")).ToList();
                return ret;
            }
            public IEnumerable<FileItem> FindAllFileItems(DirectoryInfo dirInfo)
            {
                var col = _liteOMDB.GetCollection<FileItem>("FileItem");
                List<FileItem> ret = new List<FileItem>();

                ret = col.Find(Query.StartsWith("FullName", dirInfo.FullName)).ToList();
                return ret;
            }
            public FileItem FindFile(string imdbId)
            {
                FileItem ret;

                ret = _liteOMDB.GetCollection<FileItem>("FileItem")
                    .Find(x => x.ImdbId == imdbId).FirstOrDefault();

                if (ret == null)
                {
                    var tmp = FindItem(imdbId);
                    if (tmp != null && tmp.TomatoUrl != null)
                    {
                        FileItem item = new FileItem(imdbId, tmp.TomatoUrl);
                        return item;
                    }
                }
                return ret;

            }
            public FileItem FindByFileName(FileInfo file)
            {
                return _liteOMDB.GetCollection<FileItem>("FileItem")
                    .Find(x => x.FullName == file.FullName).FirstOrDefault();
            }

            /// <summary>
            /// Upsert - Insert først, dersom den feiler, oppdaterer den record'en for angitt imdbid
            /// </summary>
            /// <param name="file"></param>
            /// <returns></returns>
            public bool Upsert(FileItem file)
            {
                try
                {
                    _liteOMDB.GetCollection<FileItem>("FileItem")
                        .Insert(file.ImdbId, file);
                    return true;
                }
                catch (Exception ex)
                {
                    try
                    {
                        Update(file);
                        return true;
                    }
                    catch (Exception)
                    { return false; }

                    return false;
                }
            }

            public bool Update(FileItem file)
            {
                return _liteOMDB.GetCollection<FileItem>("FileItem")
                    .Update(file.ImdbId, file);
            }

            #endregion



            #region wishlist
            public bool WishListAdd(MoviesFromImdb.Entities.WatchList movie)
            {
                try
                {
                    _liteOMDB.GetCollection<WatchList>("WatchList")
                        .Insert(movie.ImdbId, movie);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        public bool WishListUpsert(WatchList movie) 
        {
            return _liteOMDB.GetCollection< WatchList>("WatchList")
                       .Update(movie.ImdbId, movie);
        }

        public int DeleteWishListItem(string id)
        {
            try
            {
                var nokko = _liteOMDB.GetCollection<WatchList>();
                nokko.Delete(id);
                return 1;
            }
            catch (Exception)
            {

                return -1;
            }
        }


        public IEnumerable<WatchList> WishListFindAll(string type = null)
        {
            if (!string.IsNullOrEmpty(type))
            {
                var list = _liteOMDB.GetCollection<WatchList>("WatchList").Find(x => x.Type.Equals(type, StringComparison.OrdinalIgnoreCase));
                return list;
            }
            else
                return _liteOMDB.GetCollection<WatchList>("WatchList")
                    .FindAll();
        }
        public WatchList WishListGetItemByID(string id)
        {
            WatchList ret = null;

            return _liteOMDB.GetCollection<WatchList>("WatchList")
                      .Find(x => x.ImdbId == id).FirstOrDefault();
        }
        #endregion

        #region examples
        //public LiteDbItemService(ILiteDbContext liteDbContext)
        //{
        //    _liteDb = liteDbContext.Database;
        //}  


        /*
         public class Index
    {
        public string FileId { get; set; }
        public string AgreementId { get; set; }
        public string AccountNumber { get; set; }
        public string SortCode { get; set; }
        public string FilePath { get; set; }
    }
           var col = _db.GetCollection<Index>("index");
List<Index> query = new List<Index>();

switch (searchCriteria)
{
   case "File ID":
       query = col.Find(Query.Contains("FileId", searchText)).ToList();
       break;
    case "Agreement ID":
       query = col.Find(Query.Contains("AgreementId", searchText)).ToList();
       break;
   case "Account Number":
       query = col.Find(Query.Contains("AccountNumber", searchText)).ToList();
       break;
   case "Sort Code":
       query = col.Find(Query.Contains("SortCode", searchText)).ToList();
       break;
   case "Account Number & Sort Code":
       query = col.Find(Query.And(Query.EQ("AccountNumber", searchText), Query.EQ("SortCode", searchTextOpt))).ToList();
       break;
   default:
       break;*/
        #endregion

        #region classes



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
            #endregion

            #region TMDB movie 

            public IEnumerable<TMDbLib.Objects.Movies.Movie> FindAllMovies()
            {
                return _liteOMDB.GetCollection<TMDbLib.Objects.Movies.Movie>("Movie")
                    .FindAll();
            }
            public TMDbLib.Objects.Movies.Movie FindMovieByTitle(string title, int year)
            {
                return _liteOMDB.GetCollection<TMDbLib.Objects.Movies.Movie>("Movie")
                    .Find(x => x.Title == title.Trim() && x.ReleaseDate.GetValueOrDefault().Year == year).FirstOrDefault();
            }

            public TMDbLib.Objects.Movies.Movie FindMovie(string id, bool isImdbId = false)
            {

                if (isImdbId)
                {

                    return _liteOMDB.GetCollection<TMDbLib.Objects.Movies.Movie>("Movie")
                        .Find(x => x.ImdbId == id).FirstOrDefault();

                }
                else
                {

                    return _liteOMDB.GetCollection<TMDbLib.Objects.Movies.Movie>("Movie")
                        .Find(x => x.Id == id.ToInt32()).FirstOrDefault();
                }
            }

            public bool Upsert(TMDbLib.Objects.Movies.Movie movie)
            {
                try
                {
                    Insert(movie);

                }
                catch (Exception ex)
                {
                    try
                    {
                        Update(movie);
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
                return true;
            }

            public bool Insert(TMDbLib.Objects.Movies.Movie movie)
            {
                try
                {
                    _liteOMDB.GetCollection<TMDbLib.Objects.Movies.Movie>("Movie")
                        .Insert(movie.Id, movie);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            public bool Update(TMDbLib.Objects.Movies.Movie movie)
            {
                return _liteOMDB.GetCollection<TMDbLib.Objects.Movies.Movie>("Movie")
                    .Update(movie.Id, movie);
            }




            public int DeleteMovie(string id)
            {
                try
                {
                    var col = _liteOMDB.GetCollection<TMDbLib.Objects.Movies.Movie>();
                    //col.DeleteMany("1=1");
                    return Convert.ToInt32(col.Delete(id.ToInt32()));


                }
                catch (Exception)
                {

                    return -1;
                }
            }


            #endregion
        }
    }