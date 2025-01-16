using OMDbApiNet.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolibri.net.Common.Dal.Entities
{
    public class ItemSearch
    {
        public Item[] Search { get; set; }
        public int totalResults
        {
            get
            {
                try
                {
                    return Search.Count();

                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }
        public string Response { get; set; }
    }
}
