using System;
using System.IO;

namespace Kolibri.net.C64Sorter.Controllers
{
    class Config
    {
        

        public string Hostname { get; set; }
        public readonly int Port = 64;

        
        public Config()
        {
            
        }

        public Config(string host, int port=64)
        {
            this.Hostname = host;
            this.Port = port;
        }
         
    }
}
