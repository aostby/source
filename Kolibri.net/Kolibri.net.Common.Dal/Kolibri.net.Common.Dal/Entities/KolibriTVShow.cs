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
    public string Title { get => _title; set => _title = value; }
    public TvShow TvShow { get; set; }
    public Item Item { get; set; }
    public List<Episode> EpisodeList { get; set; } = new List<Episode>();
    public List<KolibriSeason> SeasonList { get; set; } = new List<KolibriSeason>();
    

}
