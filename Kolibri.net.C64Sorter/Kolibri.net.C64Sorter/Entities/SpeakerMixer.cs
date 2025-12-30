using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolibri.net.C64Sorter.Entities
{
   
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Speaker
    {
        [JsonProperty("Speaker Mixer")]
        public SpeakerMixer SpeakerMixer { get; set; }
        public List<object> errors { get; set; }
    }

    public class SpeakerEnable
    {
        public string current { get; set; }
        public List<string> values { get; set; }
        public string @default { get; set; }
    }

    public class SpeakerMixer
    {
        [JsonProperty("Speaker Enable")]
        public SpeakerEnable SpeakerEnable { get; set; }
    }

}
