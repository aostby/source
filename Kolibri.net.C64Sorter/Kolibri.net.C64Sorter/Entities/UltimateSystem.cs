using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kolibri.net.C64Sorter.Entities
{    [JsonSerializable(typeof(UltimateSystem))]
    public class UltimateSystem
    {
        [JsonPropertyName("product")]
        public string Product { get; set; }

        [JsonPropertyName("hostname")]
        public string Hostname { get; set; }

        [JsonPropertyName("firmware_version")]
        public string FirmwareVersion { get; set; }

        [JsonPropertyName("fpga_version")]
        public string FPGAVersion { get; set; }

        [JsonPropertyName("core_version")]
        public string CoreVersion { get; set; }

        [JsonPropertyName("unique_id")]
        public string Serial { get; set; }
    }
}
 