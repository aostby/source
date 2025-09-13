using com.sun.tools.corba.se.idl;
using com.sun.xml.@internal.bind.v2.model.core;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.Utilities.Extensions;
using LiteDB;
using OMDbApiNet.Model;
using System.Collections;
using System.Security.Policy;
using TMDbLib.Objects.TvShows;
using Season = OMDbApiNet.Model.Season;


namespace Kolibri.net.Common.Dal.Controller
{
    //Må være versjon 5.0.1.5 for å fungere
    public class LiteDBController : IDisposable
    {
        private bool ExclusiveConnection { get; set; }

        private LiteDatabase _liteDB;
        public ConnectionString ConnectionString;
        //internal Hashtable _ht = new Hashtable();

        public LiteDBController(FileInfo dbPath, bool exclusiveAccess = false, bool readOnly = false, bool upgrade=true)
        {

            ExclusiveConnection = exclusiveAccess;
            //  readOnly = false;
            ConnectionString = new ConnectionString()
            {
                Connection = ExclusiveConnection ? ConnectionType.Direct : ConnectionType.Shared,
                ReadOnly = readOnly,
                Upgrade = upgrade,
                Password = null,
                Filename = dbPath.FullName

            };
            _liteDB = new LiteDatabase(ConnectionString);
        }
 
        public void Dispose()
        {

            try
            {
                _liteDB.Dispose();
            }
            catch (Exception)
            {
            }
        }

        #region Item table funcitonss
        public async Task< IEnumerable<Item>> FindAllItems(string type = null)
        {
            IEnumerable<Item> items = null;

            if (!string.IsNullOrEmpty(type))
            {
                if (type.Equals("movies", StringComparison.OrdinalIgnoreCase))
                {
                    type = "movie";

                    items = _liteDB.GetCollection<Item>("Item").Find(x => x.Type.Equals(type, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.Title);
                }
                else
                {
                    items = _liteDB.GetCollection<Item>("Item").FindAll();
                }
            }
            else {
                items=  _liteDB.GetCollection<Item>("Item").FindAll();
            } 
            return items;   
            }
        public Item FindItem(string imdbId)
        {
            try
            {
                var ret = _liteDB.GetCollection<Item>("Item")
                                         .Find(x => x.ImdbId == imdbId).FirstOrDefault();
                return ret;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public Item FindItemByTitle(string title, int year, string type = null)
        {
            if (type != null)
            {
                var list = FindAllItems(type).GetAwaiter().GetResult().ToList();
                return list.FindAll(x => x.Title == title.Trim() && x.Year.StartsWith(year.ToString())).FirstOrDefault();
            }
            if (year < 1800)
            {
                return FindItemByTitle(title).First();
            }
            else
            {
                var ret = _liteDB.GetCollection<Item>("Item")
                    .Find(x => x.Title == title.Trim() && x.Year == year.ToString()).FirstOrDefault();
                return ret;
            }
        }
        public IEnumerable<Item> FindItemByTitle(string searchTerm)
        {
            StringComparison comp = StringComparison.Ordinal;

            var ret = _liteDB.GetCollection<Item>("Item")
               .Find(x => x.Title.ToUpper().Equals(searchTerm.ToUpper()));
            if (ret != null)
                return ret;
            //if not found, return the most likely one
            ret = _liteDB.GetCollection<Item>("Item")
               .Find(x => x.Title.ToUpper().Contains(searchTerm.ToUpper()));
            return ret;
        }
        public IEnumerable<Item> FindItemByGenre(string genre)
        {
            return _liteDB.GetCollection<Item>("Item")
                .Find(x => x.Genre.Contains(genre));
        }

        public IEnumerable<Item> GetAllItemsTypes()
        {
            var list = _liteDB.GetCollection<Item>("Item").FindAll().ToList();
            var typeList = list.Select(o => o.Type).Distinct();
            return list;
        }
        public IEnumerable<Item> GetAllItemsByType(string searchTerm)
        {
            StringComparison comp = StringComparison.Ordinal;

            var temp = _liteDB.GetCollection<Item>("Item");
            var list = temp.Find(x => x.Type.ToUpper().Contains(searchTerm.ToUpper()));
            return list;
        }
        public IEnumerable<Item> FindItemsByType(OMDbApiNet.OmdbType type)
        {
            return GetAllItemsByType(type.ToString());
        }

        public IEnumerable<Item> FindItemByGenreNew(string genre)
        {
            // This way is more efficient
            if (_liteDB.GetCollection<Item>().Count(Query.Contains("Genre", genre)) > 0)
            {
                return _liteDB.GetCollection<Item>().Find(Query.Contains("Genre", genre));
            }
            return null;

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
                _liteDB.GetCollection<Item>("Item")
                    .Insert(item.ImdbId, item);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
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
            return _liteDB.GetCollection<Item>("Item")
                .Update(movie.ImdbId, movie);
        }
        public bool Delete(Item item)
        {
            return DeleteItem(item.ImdbId) >= 1;
        }
            public int DeleteItem(string id)
        {
            try
            {
                var col = _liteDB.GetCollection<Item>();
                //col.DeleteMany("1=1");
                col.Delete(id);

                var nokko = _liteDB.GetCollection<FileItem>();
                nokko.Delete(id);
                return 1;
            }
            catch (Exception)
            {

                return -1;
            }


        }

        #region series
        #region TvEpisode - TMDB
        public TvEpisode FindTvEpisode(string imdbId)
        {
            try
            {
                return _liteDB.GetCollection<TvEpisode>("TvEpisode")
                            .Find(x => x.ExternalIds.ImdbId == imdbId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool Insert(TvEpisode ep)
        {
            try
            {
                _liteDB.GetCollection<TvEpisode>("TvEpisode")
                    .Insert(ep.ExternalIds.ImdbId.GetHashCode(), ep);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool Upsert(TvEpisode ep)
        {
            if (ep == null) return false;

            try
            {
                Insert(ep);
            }
            catch (Exception ex)
            {
                try
                {
                    Update(ep);
                }
                catch (Exception exu)
                {
                    return false;
                }
            }
            return true;
        }
        public bool Update(TvEpisode ep)
        {
            try
            {
                _liteDB.GetCollection<TvEpisode>("TvEpisode")
                    .Update(ep.ExternalIds.Id, ep);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region TvShow - TMDB
        /// <summary>
        /// Finner TMDB TvShow. Mye detaljer om show, lite om episoder.
        /// </summary>
        /// <param name="imdbid"></param>
        /// <returns></returns>
        public TvShow FindTvShow(string imdbid)
        {
            try
            {
                var list = _liteDB.GetCollection<TvShow>("TvShow").FindAll().ToList();
                var tmp = list.ToList();
                foreach (var item in tmp)
                {
                    if (item.ExternalIds != null && item.ExternalIds.ImdbId != null && item.ExternalIds.ImdbId.ToLower().Equals(imdbid.ToLower()))
                    {
                        return item;
                        break;
                    }
                }
                return null;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public TvShow FindTvShowByTitle(string title)
        {
            try
            {
                var list = _liteDB.GetCollection<TvShow>("TvShow").FindAll().ToList();
                var tmp = list.ToList();
                foreach (var item in tmp)
                {
                    if(item.Name.ToLower().Equals(title.ToLower()))   
                        return item;    
                }
                return null;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool Insert(TvShow tv)
        {
            try
            {
                _liteDB.GetCollection<TvShow>("TvShow")
                    .Insert(tv.Id, tv);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool Upsert(TvShow tv)
        {
            if (tv == null) return false;

            try
            {
                if (!Insert(tv))
                    Update(tv);
            }
            catch (Exception ex)
            {
                try
                {
                    Update(tv);
                }
                catch (Exception exu)
                {
                    return false;
                }
            }
            return true;
        }
        public bool Update(TvShow tv)
        {
            try
            {
                _liteDB.GetCollection<TvShow>("TvShow")
                    .Update(tv.Id, tv);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region SearchTvSeason - TMDB
        #endregion

        #region Season - OMDB
        public Season FindSeason(string seriesName, string seasonnumber)
        {
            try
            {
                return _liteDB.GetCollection<Season>("Season")
             .Find(x => x.Title.ToLower().Contains(seriesName.ToLower()) && x.SeasonNumber.Equals(seasonnumber)).FirstOrDefault();

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool Update(Season season)
        {
            return _liteDB.GetCollection<Season>("Season")
                .Update(season.Title + season.SeasonNumber, season);
        }
        public bool Insert(Season season)
        {
            _liteDB.GetCollection<Season>("Season")
                .Insert(season.Title + season.SeasonNumber, season);
            return true;
        }
        public bool Upsert(Season season)
        {
            if (season == null) return false;

            try
            {
             if(!   Insert(season))
                    Update(season);
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
        #endregion

        #region KolibriSeason - OMDB
        public KolibriSeason FindSeason(string seriesImdbId, int seasonNumber)
        {
            try
            {
                string id = $"{seriesImdbId}_{seasonNumber}";

                var collection = _liteDB.GetCollection<KolibriSeason>("KolibriSeason");
                //rs = collection.FindOne(Query.EQ("_id", id));
                return  collection.FindById(id);

                //   return _liteDB.GetCollection<KolibriSeason>("KolibriSeason")
                //.Find(x => x.SeriesId.Equals(seriesImdbId)&&x.SeasonNumber == seasonNumber.ToString()).FirstOrDefault();

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool Update(KolibriSeason season)
        {
            string id = $"{season.SeriesId}_{season.SeasonNumber}";
            return _liteDB.GetCollection<KolibriSeason>("KolibriSeason")
                .Update(id, season);
        }
        public bool Insert(KolibriSeason season)
        {
            string id = $"{season.SeriesId}_{season.SeasonNumber}";
            _liteDB.GetCollection<KolibriSeason>("KolibriSeason")
                .Insert(id, season);
            return true;
        }
        public bool Upsert(KolibriSeason season)
        {
            if (season == null) return false;
            try
            {
                if (!Insert(season))
                    Update(season);
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

        public int Delete(KolibriSeason season )
        {
            string id = $"{season.SeriesId}_{season.SeasonNumber}";
            try
            {
                var col = _liteDB.GetCollection<KolibriSeason>();
                //col.DeleteMany("1=1");
                var ret= col.Delete(id);
                return Convert.ToInt32(ret);

           
            }
            catch (Exception)
            {

                return -1;
            }


        }

        #endregion


        #region SeasonEpisode - OMDB
        [Obsolete($"Benytt {nameof(Episode)} istedet")]
        public SeasonEpisode FindSeasonEpisode(string imdbId)
        {if (string.IsNullOrWhiteSpace(imdbId)) return null;
            try
            {
                int hash = imdbId.GetHashCode();
                

                string id = $"{imdbId}";

                var collection = _liteDB.GetCollection<SeasonEpisode>("SeasonEpisode");
                //rs = collection.FindOne(Query.EQ("_id", id));
                var ret = collection.FindById(id);
              
                return ret;


                //return _liteDB.GetCollection<SeasonEpisode>("SeasonEpisode")
                //            .Find(x => x.ImdbId == imdbId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [Obsolete($"Benytt {nameof(Episode)} istedet")]
        public List<SeasonEpisode> FindAllSeasonEpisodes()
        {
            try
            {
                return _liteDB.GetCollection<SeasonEpisode>("SeasonEpisode").FindAll().ToList();

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [Obsolete($"Benytt {nameof(Episode)} istedet")]
        public SeasonEpisode FindSeasonEpisode(string seriesName, string season, string episode)
        {
            
            try
            {//en må prøve å finne sesong og lete seg frem.

                if (_liteDB.GetCollection<Season>().Count(Query.Contains("Title", seriesName)) > 0)
                {
                    var ses = _liteDB.GetCollection<Season>().Find(x => x.Title.ToLower() == seriesName.ToLower()
                    && x.SeasonNumber == season).FirstOrDefault();

                    var se =  ses.Episodes.Find(x => x.Episode == episode);
                    if (!string.IsNullOrEmpty(se.ImdbId))
                    {
                        return FindSeasonEpisode(se.ImdbId);
                    }
                    else
                    {
                        var ret = FindAllSeasonEpisodes().FirstOrDefault(x => x.Episode == se.Episode && x.Released == se.Released && x.Title == se.Title);
                        if (ret == null) ret = se;
                        return ret;
                    }
                }
                return null;

            }
            catch (Exception ex)
            {
                return null;
            }

        }
        [Obsolete($"Benytt {nameof(Episode)} istedet")]
        public bool Insert(SeasonEpisode ep)
        {
            try
            {
                _liteDB.GetCollection<SeasonEpisode>("SeasonEpisode")
                    .Insert(ep.ImdbId, ep);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        [Obsolete($"Benytt {nameof(Episode)} istedet")]
        public bool Update(SeasonEpisode ep)
        {
            try
            {
                _liteDB.GetCollection<SeasonEpisode>("SeasonEpisode")
                    .Update(ep.ImdbId, ep);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        [Obsolete($"Benytt {nameof(Episode)} istedet")]
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
        #endregion

        #region Episode - OMDB
        /// <summary>
        /// Episoder kan finnes via imdbId
        /// </summary>
        /// <param name="imdbId"></param>
        /// <returns></returns>
        public Episode GetEpisode(string imdbId)
        {
            try
            {
                //var ret = _liteDB.GetCollection<Episode>("Episode")
                //                  .Find(x => x.ImdbId == imdbId).FirstOrDefault();

                var collection = _liteDB.GetCollection<Episode>("Episode");
                //rs = collection.FindOne(Query.EQ("_id", id));
                var ret = collection.FindById(imdbId);
             
                return ret;



                return ret;
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
                _liteDB.GetCollection<Episode>("Episode")
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
                _liteDB.GetCollection<Episode>("Episode")
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
        #endregion


        static string HashString(string text, string salt = "")
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
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
                    .Replace("-", string.Empty);

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
                    ret.Add(movie.Result);
            };
            return ret;
        }

        public IEnumerable<FileItem> FindAllFileItems()
        {
            var col = _liteDB.GetCollection<FileItem>("FileItem");
            List<FileItem> ret = new List<FileItem>();
            ret = col.Find(Query.GT("FullName", "-1")).ToList();
            return ret;
        }
        public IEnumerable<FileItem> FindAllFileItems(DirectoryInfo dirInfo)
        {
            var col = _liteDB.GetCollection<FileItem>("FileItem");
            List<FileItem> ret = new List<FileItem>();

            ret = col.Find(Query.StartsWith("FullName", dirInfo.FullName)).ToList();
            return ret;
        }
        public async Task<FileItem> FindFile(string imdbId)
        {
            FileItem ret;

            ret = _liteDB.GetCollection<FileItem>("FileItem")
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
            var ret= _liteDB.GetCollection<FileItem>("FileItem")
                .Find(x => x.FullName == file.FullName).FirstOrDefault(); 
     
            return ret;
        }

        /// <summary>
        /// Upsert - Insert først, dersom den feiler, oppdaterer den record'en for angitt imdbid
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool Upsert(FileItem file, bool checkPath=false)
        {
            if (checkPath)
            {
                if (!file.ItemFileInfo.Exists)
                { return false; }
            }
            try
            {
                _liteDB.GetCollection<FileItem>("FileItem")
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
                catch (Exception exf)
                { return false; }

                return false;
            }
        }

        public bool Update(FileItem file)
        {
            if (file.ImdbId == null) { return false; }

            return _liteDB.GetCollection<FileItem>("FileItem")
                .Update(file.ImdbId, file);
        }

        public int Delete(FileItem file)
        {
            try
            {
                var nokko = _liteDB.GetCollection<FileItem>();
                nokko.Delete(file.ImdbId);
                return 1;
            }
            catch (Exception)
            {

                return -1;
            }
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

        #region TMDB movie 

        public IEnumerable<TMDbLib.Objects.Movies.Movie> FindAllMovies()
        {
            return _liteDB.GetCollection<TMDbLib.Objects.Movies.Movie>("Movie")
                .FindAll();
        }
        public TMDbLib.Objects.Movies.Movie FindMovieByTitle(string title, int year)
        {
            return _liteDB.GetCollection<TMDbLib.Objects.Movies.Movie>("Movie")
                .Find(x => x.Title == title.Trim() && x.ReleaseDate.GetValueOrDefault().Year == year).FirstOrDefault();
        }

        public TMDbLib.Objects.Movies.Movie FindMovie(string id, bool isImdbId = false)
        {
            if (isImdbId)
            {
                return _liteDB.GetCollection<TMDbLib.Objects.Movies.Movie>("Movie")
                    .Find(x => x.ImdbId == id).FirstOrDefault();

            }
            else
            {

                return _liteDB.GetCollection<TMDbLib.Objects.Movies.Movie>("Movie")
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
                _liteDB.GetCollection<TMDbLib.Objects.Movies.Movie>("Movie")
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
            return _liteDB.GetCollection<TMDbLib.Objects.Movies.Movie>("Movie")
                .Update(movie.Id, movie);
        }

        public int DeleteMovie(string id)
        {
            try
            {
                var col = _liteDB.GetCollection<TMDbLib.Objects.Movies.Movie>();
                //col.DeleteMany("1=1");
                return Convert.ToInt32(col.Delete(id.ToInt32()));


            }
            catch (Exception)
            {

                return -1;
            }
        }


        #endregion

        #region wishlist
        public async void WishListAdd(WatchList movie)
        {   try
            { 
                    if (string.IsNullOrEmpty(movie.FilePath))
                    {

                        var t = await FindFile(movie.ImdbId);
                        var path = t.FullName;



                        if (File.Exists(path))
                        {
                            movie.FilePath = new FileInfo(path).Directory.FullName;
                        }
                    }

                    if (!Directory.Exists(movie.FilePath))
                    {

                        movie.FilePath = null;
                    } 

                _liteDB.GetCollection<WatchList>("WatchList")
                    .Insert(movie.ImdbId, movie);
                //   return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        
        }
        public async Task< bool> WishListUpsert(WatchList movie)
        {
            try
            {

                if (string.IsNullOrEmpty(movie.FilePath))
                {

                    var t = await FindFile(movie.ImdbId);
                    var path = t.FullName;
                    if (File.Exists(path))
                    {
                        movie.FilePath = new FileInfo(path).Directory.FullName;
                    }
                }

                if (!Directory.Exists(movie.FilePath))
                {

                    movie.FilePath = null;
                }
            }
            catch (Exception)
            { }


            return _liteDB.GetCollection<WatchList>("WatchList")
                       .Update(movie.ImdbId, movie);
        }

        public int DeleteWishListItem(string id)
        {
            try
            {
                var nokko = _liteDB.GetCollection<WatchList>();
                nokko.Delete(id);
                return 1;
            }
            catch (Exception)
            {

                return -1;
            }
        }


        public List<WatchList> WishListFindAll(string type = null, string watchListName = null)
        {
            //if (!string.IsNullOrEmpty(type))
            //{
            //    var list = _liteDB.GetCollection<WatchList>("WatchList").Find(x => x.Type.Equals(type, StringComparison.OrdinalIgnoreCase));
            //    return list;
            //}
            //else

            var temp = _liteDB.GetCollection<WatchList>("WatchList").FindAll().ToList();
            var list = temp.ToList();
            if (!string.IsNullOrEmpty(type))
            {
                list = list.Where(x => x.Type.Equals(type, StringComparison.OrdinalIgnoreCase)).ToList();
            }
              
            if (!string.IsNullOrEmpty(watchListName))
            {
                list = list.Where(x => x.WatchListName.Equals(watchListName, StringComparison.OrdinalIgnoreCase)).ToList();
            }
       
            return list.ToList();

        }
        public WatchList WishListGetItemByID(string id)
        {
            WatchList ret = null;

            return _liteDB.GetCollection<WatchList>("WatchList")
                      .Find(x => x.ImdbId == id).FirstOrDefault();
        }

        public bool AddToWishList(Item movie)
        {
            try
            {
                _liteDB.GetCollection<Item>("WishList")
                    .Insert(movie);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
        #region UserSettings
        public UserSettings GetUserSettings()
        {
            UserSettings ret = null;
            try
            {
                ret = _liteDB.GetCollection<UserSettings>("UserSettings")
                        .Find(x => x.UserName == Environment.UserName).First();
                if (ret == null)
                { throw new Exception("UserSettings not found!"); }
                return ret;
            }
            catch (Exception ex)
            {
                ret = new UserSettings();
            }
            return ret;
        }


        public bool Upsert(UserSettings userSettings)
        {
            bool ret = Insert(userSettings);
            if (!ret) ret = Update(userSettings);
            return ret;
        }
        public bool Update(UserSettings userSettings)
        {
            try
            {
                _liteDB.GetCollection<UserSettings>("UserSettings")
                    .Update(userSettings.UserName, userSettings);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Insert(UserSettings userSettings)
        {
            try
            {
                _liteDB.GetCollection<UserSettings>("UserSettings")
                    .Insert(userSettings.UserName, userSettings);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion
    }
}