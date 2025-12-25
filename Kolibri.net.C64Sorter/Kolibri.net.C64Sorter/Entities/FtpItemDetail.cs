 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kolibri.net.Common.Images;
using Kolibri.net.Common.Utilities;

namespace Kolibri.net.C64Sorter.Entities
{
    public class FtpItemDetail
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        // Add an IsDirectory property based on your parsing logic
        public bool IsDirectory { get; set; }
        // You might need more properties based on your FTP server's LIST command response
     
    }
}
