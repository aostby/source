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
    private Item _item;

    [BsonIgnoreAttribute]
    public string Title { get; private set; }
    [BsonIgnoreAttribute]
    public string Year { get; private set; }
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
    [Obsolete("Do not use this field")]
    public TvShow TvShow { get; set; }
    public Item Item
    {
        get { return _item; }
        set { _item = value; try { Title = _item.Title; Year = Item.Year; } catch (Exception) { } }
    }
    public List<Episode> EpisodeList { get; set; } = new List<Episode>();
    public List<KolibriSeason> SeasonList { get; set; } = new List<KolibriSeason>();
    
    [BsonIgnoreAttribute]
    public List<Episode> GetEpisodes()
    {

        if (EpisodeList.Count <= 0)
        {
            var liste = SeasonList.SelectMany(x => x.Episodes).ToList();  
            foreach (SeasonEpisode ep in liste)
            {
                if (ep != null)
                {
                    Episode episode = ep.DeepCopy<Episode>();
                    if (string.IsNullOrWhiteSpace(episode.SeriesId)) { episode.SeriesId = Item.ImdbId; }
                    EpisodeList.Add(episode);
                }
            }
         
        }
        return EpisodeList.OrderBy(x=>x.Released).ToList(); 
    }
    public void SetSeasonRelated<T>(T obj) { throw new NotImplementedException("Sesongrelatert.... Finn ut hvordan først"); }
}
