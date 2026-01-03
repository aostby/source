 
using Kolibri.net.Common.Images;
using Kolibri.net.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kolibri.net.C64Sorter.Entities
{
    [JsonSerializable(typeof(FileItemDetail))]
    public class FileItemDetail
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; }
        [JsonPropertyName("Description")]
        public string Description { get; set; }
        [JsonPropertyName("FullPath")]
        public string FullPath { get; set; }
        [JsonIgnore]
        public Image Icon { get; set; }
    }
}
