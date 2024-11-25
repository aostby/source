using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace Kolibri.Common.Utilities.Controller
{ 

        // Based on: http://kjappsms.no/ssn.aspx
        //	 and: https://www.diskusjon.no/index.php?showtopic=738727


        /// <summary>
        /// Class which checks the norwegian birth number.
        /// <para></para>
        /// Also D-nummer og H-nummer is recognized if the parameters allow this.
        /// <para></para>
        /// If it is necessary to respond to the reasons why a number is recognized as false,
        /// you must use the static Action delegates.
        /// <para></para>
        /// The ctor only loads the birth number into a local field.
        /// Use the check method to start the check.
        /// <para></para>
        /// The result of the Check is a boolean.
        /// <para></para>
        /// Other informations can be getted by some properties with get accessor.
        /// </summary>
        public class NorwegianBirthNumberChecker
        {
            public static Action<string> OnWrongCharacters { get; set; }
            public static Action<string> OnWrongLength { get; set; }
            public static Action<string> OnWrongDate { get; set; }
            public static Action<string> OnYearLess1900 { get; set; }
            public static Action<string> OnWrongCheckNumber1 { get; set; }
            public static Action<string> OnWrongCheckNumber2 { get; set; }
            public static Action<string> OnHNumberRecognized { get; set; }
            public static Action<string> OnDNumberRecognized { get; set; }
            public static Action<string> OnFemaleRecognized { get; set; }
            public static Action<string> OnMaleRecognized { get; set; }

            private readonly string _fnr;
            private string _day;
            private string _month;
            private string _year;
            private string _individual;
            private string _k1;
            private string _k2;
            private int _d1;
            private int _d2;
            private int _m1;
            private int _m2;
            private int _y1;
            private int _y2;
            private int _i1;
            private int _i2;
            private int _i3;
            private int _k1Calculated = -1;
            /// <summary>
            /// True if the birth number belongs to a woman.
            /// </summary>
            public bool IsFemale { get; private set; }
            /// <summary>
            /// True if this is a D-Nummer.
            /// </summary>
            public bool IsDNr { get; private set; }
            /// <summary>
            /// True if this is a H-Nummer.
            /// </summary>
            public bool IsHNr { get; private set; }
        public DateTime BirthDate { get {
                if (_year == null) { CreateParts(); CreateDigits(); }
                DateTime ret;
                if (IsDNr) {
                 
                    ret = new DateTime(Millenium() + Convert.ToInt32(_year), Convert.ToInt32(_month), Convert.ToInt32(_day) - 40);

                }

                else ret = new DateTime(Millenium()+Convert.ToInt32( _year), Convert.ToInt32(_month), Convert.ToInt32(_day));
                return ret;
            } }

        /// <summary>
        /// 1 viss mann, 0 viss kvinne.
        /// </summary>
        public int Sex
        {
            get
            {
             //   return IsFemale ? 0 : 1;
               return Convert.ToInt32(   ((Convert.ToInt64(_fnr) % 1000) / 100 % 2)); //1 viss mann, 0 viss kvinne.
               // int tall = _fnr.Substring(8, 1).ToInt().GetValueOrDefault();
               // tall = tall % 2;
               // return tall;
               // //Partall er hunnkjønn, oddetall betyr hankjønn

            }
        }


        /// <summary>
        /// Constructor, loads the parameter into a local field.
        /// </summary>
        /// <param name="foedselsnummer"></param>
        public NorwegianBirthNumberChecker(string foedselsnummer)
            {
                _fnr = foedselsnummer;
            }
            /// <summary>
            /// Starts the check of the loaded birth number.
            /// </summary>
            /// <param name="allowDNumber">
            /// True: D numbers will be adjusted to do a right date check.
            /// False: This method returns false if a D number is recognized.
            /// </param>
            /// <param name="allowHNumber">
            /// True: H numbers will be adjusted to do a right date check.
            /// False: This method returns false if a H number is recognized.
            /// </param>
            /// <returns></returns>
            public bool Check(bool allowDNumber, bool allowHNumber)
            {
                if (!CheckNumbers())
                {
                    if (OnWrongCharacters != null) OnWrongCharacters(_fnr);
                    return false;
                }
                if (!CheckLength())
                {
                    if (OnWrongLength != null) OnWrongLength(_fnr);
                    return false;
                }
                CreateParts();
                CreateDigits();
                if (!CheckValidDate(allowDNumber, allowHNumber))
                {
                    if (OnWrongDate != null) OnWrongDate(_fnr);
                    return false;
                }
                if (!CheckYearGreater1900())
                {
                    if (OnYearLess1900 != null) OnYearLess1900(_fnr);
                    // NO RETURN! Fnr can be ok!
                }
                // Recognize sex
                IsFemale = _i3 % 2 == 0;
                if (IsFemale)
                {
                    if (OnFemaleRecognized != null) OnFemaleRecognized(_fnr);
                }
                else
                {
                    if (OnMaleRecognized != null) OnMaleRecognized(_fnr);
                }
                // Check the check digits k1 and k2
                if (!CheckCheckNumber1())
                {
                    if (OnWrongCheckNumber1 != null) OnWrongCheckNumber1(_fnr);
                    return false;
                }
                if (!CheckCheckNumber2())
                {
                    if (OnWrongCheckNumber2 != null) OnWrongCheckNumber2(_fnr);
                    return false;
                }
                return true;
            }
         
            private void CreateParts()
            {
                _day = _fnr.Substring(0, 2);
                _month = _fnr.Substring(2, 2);
                _year = _fnr.Substring(4, 2);
                _individual = _fnr.Substring(6, 3);
                _k1 = _fnr.Substring(9, 1);
                _k2 = _fnr.Substring(10, 1);
            }
            private void CreateDigits()
            {
                _d1 = Convert.ToInt32(_day.Substring(0, 1));
                _d2 = Convert.ToInt32(_day.Substring(1, 1));
                _m1 = Convert.ToInt32(_month.Substring(0, 1));
                _m2 = Convert.ToInt32(_month.Substring(1, 1));
                _y1 = Convert.ToInt32(_year.Substring(0, 1));
                _y2 = Convert.ToInt32(_year.Substring(1, 1));
                _i1 = Convert.ToInt32(_individual.Substring(0, 1));
                _i2 = Convert.ToInt32(_individual.Substring(1, 1));
                _i3 = Convert.ToInt32(_individual.Substring(2, 1));
            }
            private bool CheckNumbers()
            {
                long fnr;
                return Int64.TryParse(_fnr, out fnr);
            }
            private bool CheckLength()
            {
                return _fnr.Length == 11;
            }
            private bool CheckValidDate(bool allowDNumber, bool allowHNumber)
            {
                string date = _fnr.Substring(0, 6);
                if (_fnr[0] > '3')
                {
                    IsDNr = true;
                    if (OnDNumberRecognized != null) OnDNumberRecognized(_fnr);
                    if (allowDNumber)
                        date = (_fnr[0] - '4') + _fnr.Substring(1, 5);
                }
                if (_month[0] > '3')
                {
                    IsHNr = true;
                    if (OnHNumberRecognized != null) OnHNumberRecognized(_fnr);
                    if (allowHNumber)
                        date = (_day + (_month[0] - '4') + _month[1] + _year);
                }
                // Both D and H number isn't allowed
                if (IsDNr && IsHNr) return false;
                DateTime tmp;
                return DateTime.TryParseExact(date, "ddMMyy",
                                              System.Globalization.CultureInfo.InvariantCulture,
                                              System.Globalization.DateTimeStyles.None,
                                              out tmp);
            }
            private bool CheckYearGreater1900()
            {
                // this check is only valid for real fnr
                if (IsDNr || IsHNr) return true;
                // dersom individnummeret er mellom 500 og 750 er vedkommende født mellom 1855 og 1899
                int i = Convert.ToInt32(_individual);
                return !(i >= 500 && i < 750);
            }
        private int Millenium() {
            /*
            Individnummer År i fødselsdato Født
500 - 749 > 54 1855 - 1899
000 - 499   1900 - 1999
900 - 999 * > 39 * 1940 - 1999 *
500 - 999 < 40 2000 - 2039
*/



            if (TestRange(500, 749)) return 1800;
            if (TestRange(000 , 499)) return 1900;
            if (TestRange(500, 999)) return 2000;

            return 0;
        }
        bool TestRange( int bottom, int top)
        {
            int numberToCheck = Convert.ToInt32(_individual);
            return (numberToCheck >= bottom && numberToCheck <= top);
        }


        private bool CheckCheckNumber1()
            {
                // no check for H numbers
                if (IsHNr) return true;
                // Calculate k1 (first check digit)
                _k1Calculated = 11 - (((3 * _d1) + (7 * _d2) + (6 * _m1) + (1 * _m2) + (8 * _y1) + (9 * _y2) + (4 * _i1) + (5 * _i2) + (2 * _i3)) % 11);
                _k1Calculated = (_k1Calculated == 11 ? 0 : _k1Calculated);
                // k1 cannot be 10
                if (_k1Calculated == 10) return false;
                // Is calculated k1 the same as k1 in _fnr?
                return _k1Calculated == Convert.ToInt32(_k1);
            }
            private bool CheckCheckNumber2()
            {
                // no check for H numbers
                if (IsHNr) return true;
                if (_k1Calculated == -1)
                {
                    // Program error: the order in the public Check method is wrong!
                    throw new Exception("Error in FoedselsNummerChecker: " +
                                        "The CheckCheckNumber1 method must be called BEFORE CheckCheckNumber2 starts! " +
                                        "This is a program error. Most possibly the order in the Check method is wrong.");
                }
                // Calculate k2 (second check digit)
                int k2Calculated = 11 - (((5 * _d1) + (4 * _d2) + (3 * _m1) + (2 * _m2) + (7 * _y1) + (6 * _y2) + (5 * _i1) + (4 * _i2) + (3 * _i3) + (2 * _k1Calculated)) % 11);
                k2Calculated = (k2Calculated == 11 ? 0 : k2Calculated);
                // Fnr with k2 = 10 is invalid
                if (k2Calculated == 10) return false;
                // Is calculated k2 the same as k2 in _fnr?
                return k2Calculated == Convert.ToInt32(_k2);
            }
        }
    }