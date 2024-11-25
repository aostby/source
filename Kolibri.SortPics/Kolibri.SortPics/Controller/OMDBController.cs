using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kolibri.Utilities.Extensions;
using Newtonsoft.Json;
using OMDbApiNet.Model;
namespace SortPics.Controller
{
    public class OMDBController
    {
        private string _apikey;
        public OMDbApiNet.OmdbType type = OMDbApiNet.OmdbType.Movie;
        private OMDbApiNet.OmdbClient _client;
        private LiteDBController _LITEDB;
        /// <summary>
        ///  Insert your api key here. You can get one on http://www.omdbapi.com/
        /// </summary>
        /// <param name="apikey">omdbapi key</param>
        public OMDBController(string apikey, LiteDBController contr = null)
        {
            if (string.IsNullOrEmpty(apikey))
                throw new Exception("API key for OMDB is null. This is not allowed. Please obtain an API key from OMDb API at omdbapi.com");
            this._apikey = apikey;
            type = OMDbApiNet.OmdbType.Movie;

            _client = new OMDbApiNet.OmdbClient(apikey);
            _LITEDB = contr;

        }

        public Item GetMovieByIMDBTitle(string title, int year)
        {
            Item ret = null;
            try
            {
                var query = OMDbApiNet.Utilities.QueryBuilder.GetItemByTitleQuery(title, type, year, false);
                ret = _client.GetItemByTitle(title, year, false);
            }
            catch (Exception ex)
            {
                ret = null;
            }
            return ret;
        }

        internal static string GetOmdbKey(bool obtain = false)
        {
            string ret=string.Empty;
            if (!obtain)
            {
                ret = Properties.Settings.Default.OMDBkey; 
            }
            if (!String.IsNullOrWhiteSpace(ret)) return ret;
            else
            {
                bool ok = Kolibri.FormUtilities.InputDialogs.InputBox("Omdb key required, please submit one, or cancel to obtain it.", "Please submit your omdb key", ref ret) == System.Windows.Forms.DialogResult.OK;
                if (ok)
                {
                    if (!string.IsNullOrEmpty(ret))
                    {
                        Properties.Settings.Default.OMDBkey = ret;
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
            Properties.Settings.Default.OMDBkey = ret;
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

            OMDbApiNet.Model.Item ret = null;
            try
            {
                if (!string.IsNullOrEmpty(imdbId))
                {

                    if (_LITEDB != null)
                        ret = _LITEDB.FindItem(imdbId);
                    if (ret == null)
                    {
                        try
                        {
                            ret = _client.GetItemById(imdbId, false);
                            if (insert && ret != null && _LITEDB != null)
                            {
                                string title = ret.Title;
                                _LITEDB.Insert(ret);
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
        
        /// <summary>
        /// Plukk Item vha ImdbId, alle typer media
        /// </summary>
        /// <param name="imdbId"></param>
        /// <returns></returns>
        public Item GetItemByImdbId(string imdbId) {
            try
            {
                return _client.GetItemById(imdbId, true);
            }
            catch (Exception)
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
            { return null; }
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

        internal Season SeriesByImdbId(string imdbId, string seasonNumber)
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
        internal Season SeriesBySeason(string series, string season)
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

        internal Episode SeriesEpisode(string series, string season, string episode)
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
    }
}
