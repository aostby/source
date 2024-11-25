using RandomNameGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolibri.Common.Utilities.Controller
{
    public class RandomNameGeneratorController
    {
        public static string GetRandomName(string kjonn)
        {
            Gender gender;
            if (kjonn.ToUpper().StartsWith("M")
                || kjonn.ToUpper().StartsWith("G"))
                gender = Gender.Male;

            else if (kjonn.ToUpper().StartsWith("F")
              || kjonn.ToUpper().StartsWith("K")
              || kjonn.ToUpper().StartsWith("J"))
                gender = Gender.Female;
            else
                throw new Exception(kjonn + " - Gender not spesified - kjønn finnes ikke. " + System.Reflection.MethodBase.GetCurrentMethod().Name);

            return GetRandomName(gender);
        }
        private static string GetRandomName(Gender gender)
        {
            string first = RandomNameGenerator.NameGenerator.GenerateFirstName(gender);
            string last = RandomNameGenerator.NameGenerator.GenerateLastName();
            return string.Format("{0} {1}", first, last);
        }
    }
}
