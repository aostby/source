using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using java.util;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.FormUtilities.Forms;
using Kolibri.net.Common.Utilities.Extensions;
using Newtonsoft.Json;
using OMDbApiNet;
using OMDbApiNet.Model;

namespace Kolibri.net.Common.Dal.Controller
{
    public class OMDBController:IDisposable
    {
        private string _apikey;
        //public OMDbApiNet.OmdbType _type = OMDbApiNet.OmdbType.Movie;
        private OMDbApiNet.OmdbClient _client;
        private LiteDBController _liteDB;
        /// <summary>
        ///  Insert your api key here. You can get one on http://www.omdbapi.com/
        /// </summary>
        /// <param name="apikey">omdbapi key</param>
        public OMDBController(string apikey, LiteDBController contr = null)
        {
            if (string.IsNullOrEmpty(apikey))
                throw new Exception("API key for OMDB is null. This is not allowed. Please obtain an API key from OMDb API at omdbapi.com");
            _apikey = apikey;
            //_type = OMDbApiNet.OmdbType.Movie;

            _client = new OMDbApiNet.OmdbClient(apikey);
            _liteDB = contr;

        }

        public Item GetMovieByIMDBTitle(string title, int year )
        {
            Item ret = null;
            try
            {
                var query = OMDbApiNet.Utilities.QueryBuilder.GetItemByTitleQuery(title, OmdbType.Movie, year, false);
                ret = _client.GetItemByTitle(title, year, false);
            }
            catch (Exception ex)
            {
              
                ret = null;
            }
            return ret;
        }

        public Item GetSeriesByTitle(string title, int? year)
        {
            Item ret = null;
            try
            {
                var query = OMDbApiNet.Utilities.QueryBuilder.GetItemByTitleQuery(title, OmdbType.Series,year, false);
                ret = _client.GetItemByTitle(title, year, false);
            }
            catch (Exception ex)
            {

                ret = null;
            }
            return ret;
        }

        public string GetOmdbKey(bool obtain = false, bool replace = false)
        {
            string ret = string.Empty;

            UserSettings settings = _liteDB.GetUserSettings();
            if (!obtain)
            {
                ret = settings.OMDBkey;
                if (!string.IsNullOrEmpty(ret) && replace)
                {
                    if (MessageBox.Show($"{ret} - value found. Do you wish to type in a different one?)", System.Reflection.MethodBase.GetCurrentMethod().Name, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        settings.OMDBkey = string.Empty;
                        ret = string.Empty;
                        GetOmdbKey();
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(ret)) return ret;
            else
            {
                bool ok = InputDialogs.InputBox("Omdb key required, please submit one, or cancel to obtain it.", "Please submit your omdb key", ref ret) == DialogResult.OK;
                if (ok)
                {
                    if (!string.IsNullOrEmpty(ret))
                    {
                        settings.OMDBkey = ret;
                        return ret;
                    }
                    {
                        GetOmdbKey();
                    }
                }
                else
                {
                    if (MessageBox.Show("No OMDB key found. Do you want to obtain one?\n\rUsually it's free :)", "OMDB key missing", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    { Process.Start("https://www.omdbapi.com/apikey.aspx"); }
                    else
                    {
                        ret = string.Empty;
                        throw new Exception("Weeeeelll without key, and one wont obtain key even though one is needed....");
                    }
                }
            }
            settings.OMDBkey = ret;
            _liteDB.Upsert(settings);
            return ret;
        }
        /// <summary>
        /// Hennt en film fra db. Dersom DB ikke har filmen, kan den hentes ut vha OMDB klient. Insert setter filmen inn i DB om den blir funnet online
        /// </summary>
        /// <param name="imdbId">IMDB id for filmen</param>
        /// <param name="insert"> Insert setter filmen inn i DB om den blir funnet online</param>
        /// <returns></returns>
        public Item GetMovieByIMDBid(string imdbId, bool insert = false)
        {

            
            Item ret = null;
            try
            {
                if (!string.IsNullOrEmpty(imdbId))
                {

                    if (_liteDB != null)
                    {
                        ret = _liteDB.FindItem(imdbId);
                        if (ret == null)
                            ret = _client.GetItemById(imdbId);
                    }
                    if (ret == null)
                    {
                        try
                        {
                            
                            if (insert && ret != null && _liteDB != null)
                            {
                                string title = ret.Title;
                                _liteDB.Upsert(ret);
                            }
                        }
                        catch (Exception ex)
                        {
                            string message = ex.Message;

                        }
                    }
                }
            }
            catch (Exception ex)
            { }
            return ret;
        }

        public Item GetItemById(int id, bool fullplot = false)
        {

            return _client.GetItemById(id.ToString(), fullplot);
        }


        /// <summary>
        /// Plukk Item vha ImdbId, alle typer media
        /// </summary>
        /// <param name="imdbId"></param>
        /// <returns></returns>
        public Item GetItemByImdbId(string imdbId)
        {
            try
            {
                return _client.GetItemById(imdbId, true);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<SearchItem> GetByTitle(string title, OMDbApiNet.OmdbType type, int pages = 1)
        {
            try
            {
                var test = _client.GetSearchList(title, type, pages);
                return test.SearchResults;
            }
            catch (Exception ex)
            { return new List<SearchItem> (); }
        }

        public List<SearchItem> GetByTitle(string title, int year, OMDbApiNet.OmdbType type, int pages = 1)
        {
            try
            {
                var test = _client.GetSearchList(year, title, type, pages);

                return test.SearchResults;
            }
            catch (Exception ex)

            {
                return null;
            }

        }

        public Season SeasonByImdbId(string imdbId, string seasonNumber)
        {
            try
            {
                return _client.GetSeasonBySeriesId(imdbId, seasonNumber.ToInt().GetValueOrDefault());
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public Season SeriesBySeason(string series, string season)
        {

            try
            {
                int s = season.Replace("S", string.Empty).Replace("s", string.Empty).ToInt().GetValueOrDefault();

                var test = _client.GetSeasonBySeriesTitle(series, s);

                return test;
            }
            catch (Exception ex)

            {
                return null;
            }
        }

        public Episode SeriesEpisode(string series, string season, string episode)
        {

            try
            {
                int s = season.Replace("S", string.Empty).Replace("s", string.Empty).ToInt().GetValueOrDefault();
                int e = episode.Replace("S", string.Empty).Replace("s", string.Empty).ToInt().GetValueOrDefault();

                var test = _client.GetEpisodeBySeriesTitle(series, s, e);

                return test;
            }
            catch (Exception ex)

            {
                return null;
            }
        }

        internal Episode SeriesEpisode(string imdbid, int season, int episode)
        {
            return _client.GetEpisodeBySeriesId(imdbid, season, episode);
        }

        public void Dispose()
        {
            try {
                if (_liteDB != null)
                {
                    //  _liteDB.Dispose();
                    GC.Collect();
                }
            } catch (Exception) { }
        }
    }
}
