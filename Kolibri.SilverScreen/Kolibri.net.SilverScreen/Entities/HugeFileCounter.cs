using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolibri.net.SilverScreen.Entities
{
    public class HugeFileCounter
    {
        public long totalCount { get; set; }
        public long currentCount { get; set; }
        public FileInfo file { get; set; }  

        public string Name{ get{ return Path.GetFileNameWithoutExtension(file.FullName); } }
    }
}
