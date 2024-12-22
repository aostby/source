using Kolibri.net.Common.Utilities;
using Kolibri.net.Common.Utilities.Extensions;
using Newtonsoft.Json;
using OMDbApiNet.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Caching;

using System.Text;
using System.Threading.Tasks;
using TMDbLib.Objects.People;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;

namespace Kolibri.net.Common.Dal.Controller
{
    public class SeriesCache
    {
        private FileInfo _seasonEpDicPath, _seasonDicPath;
     
        private Dictionary<string, SeasonEpisode> _seasonEpDic = new Dictionary<string, SeasonEpisode>();
        private Dictionary<string, Season> _seasonDic = new Dictionary<string, Season>();

        public SeriesCache(DirectoryInfo cachePath)
        {
            if (!cachePath.Exists) cachePath.Create();
            
            try
            {
                _seasonEpDicPath = new FileInfo(Path.Combine(cachePath.FullName, "SeasonEpisodes.js"));
                if (_seasonEpDicPath.Exists)
                    _seasonEpDic = JsonConvert.DeserializeObject< Dictionary<string, SeasonEpisode>>(File.ReadAllText(_seasonEpDicPath.FullName));
            }
            catch (Exception) { }

            try
            {
                _seasonDicPath = new FileInfo(Path.Combine(cachePath.FullName, "Seasons.js"));
                if (_seasonDicPath.Exists)
                    _seasonDic = JsonConvert.DeserializeObject<Dictionary<string, Season>>(File.ReadAllText(_seasonDicPath.FullName));
            }
            catch (Exception) { } 
        }

        public void Add(string showTitle, int season , SeasonEpisode sEp)
        {
            string id = $"{showTitle}{season}{sEp.Episode.ToInt32()}";
            _seasonEpDic[id] = sEp;
        }
        public void Add(string showTitle,  Season season)
        {
            string id = $"{showTitle}{season.SeasonNumber.ToInt32()}";
            _seasonDic[id] = season;
        } 

        public List<Season> Get(string showTitle, int season )
        {
            List<Season> ret = new List<Season>();
            string id = $"{showTitle}{season}";
            foreach (var item in _seasonDic.Keys)
            {
                if (item.StartsWith(id, StringComparison.OrdinalIgnoreCase))
                {
                    ret.Add(_seasonDic[item]);
                }
            }
            return ret;
        }

        public   SeasonEpisode  Get(string seriesTitle, int season, int episode)
        {
            try
            {
  string id = $"{seriesTitle}{season}{episode}";
            if (_seasonEpDic.Keys.Contains(id, StringComparer.OrdinalIgnoreCase))
                return _seasonEpDic[id];
            else return null;
            }
            catch (Exception)
            {

                return null;
            }
          
        }
        public bool Save()
        {
            bool ret = true;
            try
            {
                if (_seasonEpDic.Count() > 0)
                    File.WriteAllText(_seasonEpDicPath.FullName, JsonConvert.SerializeObject(_seasonEpDic));

                if (_seasonDic.Count() > 0)
                    File.WriteAllText(_seasonDicPath.FullName, JsonConvert.SerializeObject(_seasonDic)); 
            }
            catch (Exception)
            {
                ret = false;
            }
            return ret;
        } 

        public   TvSeason TVSeasonCacheGet(string showTitle, int season)
        {  string CacheKey = $"TVSeason_{showTitle}_{season}";

            //Returns null if the string does not exist, prevents a race condition where the cache invalidates between the contains check and the retreival.
            var ret = MemoryCache.Default.Get(CacheKey, null) as TvSeason;
            return ret;
        }
        public   void TVSeasonCacheSet(string showTitle, TvSeason season)
        {
            if (season == null) return;

            object cacheLock = new object();
            string CacheKey = $"TVSeason_{showTitle}_{season.SeasonNumber}";

            //Returns null if the string does not exist, prevents a race condition where the cache invalidates between the contains check and the retreival.
            var ret = MemoryCache.Default.Get(CacheKey, null) as TvSeason;

            if (ret != null)
            {
                bool exist = true;
                return;
            }

            lock (cacheLock)
            {
                //Check to see if anyone wrote to the cache while we where waiting our turn to write the new value.
                ret = MemoryCache.Default.Get(CacheKey, null) as TvSeason;

                if (ret != null)
                {
                    return;
                }

                //The value still did not exist so we now write it in to the cache.
               
                CacheItemPolicy cip = new CacheItemPolicy()
                {
                    AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(20))
                };
                MemoryCache.Default.Set(CacheKey, season, cip); 
            }
        }

        public SearchTv SearchTvCacheGet(string showTitle)
        {
            string CacheKey = $"SearchTv_{showTitle}";

            //Returns null if the string does not exist, prevents a race condition where the cache invalidates between the contains check and the retreival.
            var ret = MemoryCache.Default.Get(CacheKey, null) as SearchTv;
            return ret;
        }

        public void TVSeasonCacheSet(SearchTv searchTv)
        {
            if (searchTv == null) return;

            object cacheLock = new object();
            string CacheKey = $"SearchTv_{searchTv.OriginalName}";

            //Returns null if the string does not exist, prevents a race condition where the cache invalidates between the contains check and the retreival.
            var ret = MemoryCache.Default.Get(CacheKey, null) as SearchTv;

            if (ret != null)
            { 
                return;
            }

            lock (cacheLock)
            {
                //Check to see if anyone wrote to the cache while we where waiting our turn to write the new value.
                ret = MemoryCache.Default.Get(CacheKey, null) as SearchTv;

                if (ret != null)
                {
                    return;

                }

                //The value still did not exist so we now write it in to the cache.

                CacheItemPolicy cip = new CacheItemPolicy()
                {
                    AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(20))
                };
                MemoryCache.Default.Set(CacheKey, searchTv, cip);
            }
        }
    }
}