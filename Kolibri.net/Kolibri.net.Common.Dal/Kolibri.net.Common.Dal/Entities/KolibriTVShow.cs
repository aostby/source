using Kolibri.net.Common.Utilities.Extensions;
using LiteDB;
using OMDbApiNet.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMDbLib.Objects.TvShows;

namespace Kolibri.net.Common.Dal.Entities;

public class KolibriTVShow : Interfaces.KolibriMedia
{
    private string _title;

    [BsonIgnoreAttribute]
    public string Title { get => _title; set => _title = value; }
    
    [BsonIgnoreAttribute]
    public string Path { get
        {
            string ret = string.Empty;
            try
            { 
                if (Item != null && !string.IsNullOrEmpty(Item.TomatoUrl))
                {
                    ret= Item.TomatoUrl;
                }
            }
            catch (Exception ex)
            {
                ret= string.Empty;
            }
            return ret;
        }    
    }

    public TvShow TvShow { get; set; }
    public Item Item { get; set; }
    public List<KolibriSeasonEpisode> EpisodeList { get; set; } = new List<KolibriSeasonEpisode>();
    public List<KolibriSeason> SeasonList { get; set; } = new List<KolibriSeason>();
    
    [BsonIgnoreAttribute]
    public List<KolibriSeasonEpisode> GetEpisodes()
    {

        if (EpisodeList.Count <= 0)
        {
            var liste = SeasonList.SelectMany(x => x.Episodes).ToList();  
            foreach (SeasonEpisode ep in liste)
            {
                if (ep != null)
                {
                    KolibriSeasonEpisode episode = ep.DeepCopy<KolibriSeasonEpisode>();
                    if (string.IsNullOrWhiteSpace(episode.SeriesId)) { episode.SeriesId = Item.ImdbId; }
                    EpisodeList.Add(episode);
                }
            }
         
        }
        return EpisodeList.OrderBy(x=>x.Released).ToList(); 
    }
}
