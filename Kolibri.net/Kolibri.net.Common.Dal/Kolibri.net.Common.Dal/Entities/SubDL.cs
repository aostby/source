using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolibri.net.Common.Dal.Entities
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Result
    {
        public int sd_id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string imdb_id { get; set; }
        public int tmdb_id { get; set; }
        public DateTime? first_air_date { get; set; }
        public DateTime? release_date { get; set; }
        public int year { get; set; }
    }

    public class SubDL
    {
        public bool status { get; set; }
        public List<Result> results { get; set; }
        public List<Subtitle> subtitles { get; set; }
    }

    public class Subtitle
    {
        public string release_name { get; set; }
        public string name { get; set; }
        public string lang { get; set; }
        public string author { get; set; }
        public string url { get; set; }
        public string subtitlePage { get; set; }
        public int? season { get; set; }
        public int? episode { get; set; }
        public string language { get; set; }
        public bool hi { get; set; }
    }


}
