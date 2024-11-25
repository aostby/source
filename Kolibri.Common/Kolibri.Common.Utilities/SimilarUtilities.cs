using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuzzySharp;
namespace Kolibri.Common.Utilities
{
    public class SimilarUtilities
    {
        public static string ExtractOne(string word, List<string> process)
        {

            var temp = Process.ExtractOne(word, process);
            return temp.Value;
            //Example Process.ExtractOne("cowboys", new[] { "Atlanta Falcons", "New York Jets", "New York Giants", "Dallas Cowboys" })            (string: Dallas Cowboys, score: 90, index: 3)
        }
    }
}