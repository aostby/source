using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolibri.net.C64Sorter.Entities
{
    public class Configs
    {
        public List<string> categories { get; set; }
        public List<object> errors { get; set; }
    }

    public class Sections
    {

        public string CatagoryName { get; set; }
        public string SectionName { get; set; }
        public List<string> AllowedValues{ get; set; }
    }

}
 
