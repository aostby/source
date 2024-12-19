using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolibri.net.SilverScreen.IMDBForms
{
    public class Top100IMDb
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("rank")]
        public string Rank { get; set; }
        
        [JsonProperty("Genre")]
        public string Genre { get; set; }

        [JsonProperty("ImdbId")]
        public string ImdbId { get; set; }

    


    }
}
