using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.Utilities;
using Newtonsoft.Json;
using OMDbApiNet.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using TMDbLib.Objects.TvShows;

namespace Kolibri.net.Common.Dal.Controller
{
    public class SubDLSubtitleController
    {
        private const string API_DOC = "https://subdl.com/api-doc";

        private   string baseurl = "https://api.subdl.com/api/v1/subtitles";
        public string apikey { get; set; }
        private   string languages = "NO,EN";

      //  string exampleQuery = $"https://api.subdl.com/api/v1/subtitles?api_key={apikey}&film_name=Inception&type=movie&languages=EN";

        private UserSettings _settings;

        public SubDLSubtitleController(UserSettings settings) 
        {
            _settings = settings;
            apikey = _settings.SUBDLkey;
        }

        public   SubDL SearchByIMDBid(string imdbid)
        { 
            SubDL ret = null;
            if (string.IsNullOrEmpty(imdbid)) { return ret; }

            var queryParams = new NameValueCollection()
        {
            { "api_key", apikey }, {"languages", languages }, { "imdb_id", imdbid }
        };

            string url = baseurl + StringUtilities.ToQueryString(queryParams);
              ret = Execute(  url);
            return ret;
        }
        public   SubDL SearchByNameAndType(string name, string year, string type = "tv")
        {
            SubDL ret = null;
            if (string.IsNullOrEmpty(name)) { return ret; }

            var queryParams = new NameValueCollection()
            {
            { "api_key", apikey }, {"languages", languages }, {"year", year } ,{ "type", type } 
            };
            if (type.Equals("tv"))
                {
                queryParams.Add("title", name);
                //string requestUrl = $"{baseUrl}?title={showTitle}&season={season}&episode={episode}";
            }
            else
            {
                queryParams.Add("film_name", name);
            }

            //  https://api.subdl.com/api/v1/subtitles?api_key=2gHHygdT4rS_uTtd11AlLun6f-eYjWgU&languages=NO%2CEN&year=2019&type=tv&film_name=See
            //  https://api.subdl.com/api/v1/subtitles?api_key=2gHHygdT4rS_uTtd11AlLun6f-eYjWgU&languages=NO%2CEN&film_name=See&type=tv

            string url = baseurl + StringUtilities.ToQueryString(queryParams);
            ret = Execute(url);
            return ret;
        }

        public   SubDL SearchBySD_ID(string sd_id)
        {
            SubDL ret = null;
            if (string.IsNullOrEmpty(sd_id)) { return ret; }

            var queryParams = new NameValueCollection()
        {
            { "api_key", apikey }, {"languages", languages }, { "sd_id", sd_id }
        };

            string url = baseurl + StringUtilities.ToQueryString(queryParams);
            ret = Execute(url);
            return ret;
        }

        public   SubDL SearchByFileName(string filename)
        {
            SubDL ret = null;

            if (string.IsNullOrEmpty(filename)) { return ret; }

            var queryParams = new NameValueCollection()
        {
            { "api_key", apikey }, {"languages", languages }, { "file_name", filename }
        };

            string url = baseurl + StringUtilities.ToQueryString(queryParams);
              ret = Execute(url);
            return ret;
        }

        private   SubDL Execute(  string url)
        {
            SubDL ret = null;
            try
            {
                string res = HTMLUtilities.GetHTML(new System.Uri(url));
                var temp = JsonConvert.DeserializeObject<SubDL>(res);
                ret = temp;
                if (temp != null && temp.status == true && temp.subtitles.Count > 0)
                {
                    temp.subtitles = temp.subtitles.OrderByDescending(c => c.language).ToList();
                }

            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return ret;
        }

        public   SubDL SearchByItemValues(Item mmi)
        {
            SubDL ret = null;

            if (mmi.Equals(null)) { return ret; }
             
            var queryParams = new NameValueCollection()
        {
            { "api_key", apikey }, {"languages", languages }, 
                { "year", mmi.Year },//`year` (optional): Release year of the movie or TV show.
                { "type",mmi.Type },//`type` (optional): Type of the content, either `movie` or `tv`.
                { "film_name",mmi.Title },
        };

            


            string url = baseurl + StringUtilities.ToQueryString(queryParams);
            ret = Execute(url);
            return ret;
        }
    }
}
