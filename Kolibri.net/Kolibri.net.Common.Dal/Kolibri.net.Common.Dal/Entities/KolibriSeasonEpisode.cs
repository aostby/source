﻿using Newtonsoft.Json;
using OMDbApiNet.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolibri.net.Common.Dal.Entities
{
    /// <summary>
    /// OMDB har ikke lenke til serie i Season, så denne klassen brukes istedet.
    /// </summary>
    [Obsolete("Episode dekker dette", true)]
    public class KolibriSeasonEpisode : SeasonEpisode
    {
        public string SeriesId { get; set; }
        public string Season { get; set; }
    }


    //public class KolibriSeason
    // {
    //     [JsonProperty("SeriesTitle")]
    //     public string SeriesTitle { get; set; }
    //     [JsonProperty("SeriesImdbId")]
    //     public string SeriesImdbId { get; set; }

    //     [JsonProperty("Title")]
    //     public string Title { get; set; }

    //     [JsonProperty("Season")]
    //     public string SeasonNumber { get; set; }

    //     [JsonProperty("totalSeasons")]
    //     public string TotalSeasons { get; set; }

    //     [JsonProperty("Episodes")]
    //     public List<SeasonEpisode> Episodes { get; set; }

    //     [JsonProperty("Response")]
    //     public string Response { get; set; }

    //     [JsonProperty("Error")]
    //     public string Error { get; set; }
    // }
}
