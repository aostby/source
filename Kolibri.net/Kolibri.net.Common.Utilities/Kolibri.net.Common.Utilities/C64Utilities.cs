using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolibri.net.Common.Utilities
{
    public class C64Utilities
    {
        public static List<string> C64CommonFileExt(bool withPunctuation = false)
        {
            var ret = new List<string>() { "PRG","D64","T64",/*"P00",*/"D71", "D81","CRT","TAP",/*"Oth",*/"G64","X64","ZIP","SID"/*,"CVT"*/ };

            if (withPunctuation)
                ret = ret.Select(r => string.Concat('.', r)).ToList();

            return ret.Distinct().ToList();
        }

    }
}
