using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolibri.XMLValidator.Entities
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class ActiveProfileSession
    {
        public string name { get; set; }
        public string hydrometerId { get; set; }
        public DateTime startDate { get; set; }
        public DateTime modifiedOn { get; set; }
        public string modifiedBy { get; set; }
        public string id { get; set; }
        public bool deleted { get; set; }
        public DateTime createdOn { get; set; }
        public string createdBy { get; set; }
    }

    public class Profile
    {
        public double temperature { get; set; }
        public double gravity { get; set; }
        public string firmwareVersion { get; set; }
        public bool isLatestFirmware { get; set; }
        public double battery { get; set; }
        public ActiveProfileSession activeProfileSession { get; set; }
        public DateTime lastActivityTime { get; set; }
        public int rssi { get; set; }
        public string name { get; set; }
        public string macAddress { get; set; }
        public string deviceType { get; set; }
        public bool active { get; set; }
        public bool disabled { get; set; }
        public DateTime modifiedOn { get; set; }
        public string modifiedBy { get; set; }
        public string id { get; set; }
        public bool deleted { get; set; }
        public DateTime createdOn { get; set; }
        public string createdBy { get; set; }
        public List<Telemetry> telemetry { get; set; }
        public string url { get; set; }
    }

    public class Telemetry
    {
        public double temperature { get; set; }
        public double gravity { get; set; }
        public int gravityVelocity { get; set; }
        public double battery { get; set; }
        public string version { get; set; }
        public DateTime createdOn { get; set; }
        public string macAddress { get; set; }
        public int rssi { get; set; }
    }


}
