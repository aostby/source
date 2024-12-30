using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Kolibri.net.Common.Utilities
{
    public class StringUtilities
    {
        #region constants
        private static readonly string[] BADSQLWORDS = new string[]{
           "--",
"ALTER",
"ALTER_USER",
"CASCADE", 
/*"CREATE", */
"CREATE_ROLE",
"CREATE_USER", 
/*"DELDATA", */
"DELETE",
"DROP",
"DROP_ANY_ROLE",
"DROP_USER",
"EXEC",
"EXEC",
"EXECUTE",
"EXECUTE",
"GRANT",
"GRANT_ANY_PRIVILEGE",
"GRANT_ANY_ROLE",
"INSERT",
"UPDATE",
"WRITE"
      };
        private static readonly string[] RESERVEDSQLWORDS = new string[]{
          "ABSOLUTE",
"ACTION",
"ADD",
"AFTERHAVING",
"ALL",
"ALLOCATE",
"ALPHAUP",
"ALTER",
"ALTER_USER",
"AND",
"ANY",
"ARE",
"AS",
"ASC",
"ASSERTION",
"AT",
"AUTHORIZATION",
"AVG",
"BEGIN",
"BEGTRANS",
"BETWEEN",
"BIT",
"BIT_LENGTH",
"BOTH",
"BY",
"CASCADE",
"CASE",
"CAST",
"CATALOG",
"CHAR",
"CHARACTER",
"CHARACTER_LENGTH",
"CHAR_LENGTH",
"CHECK",
"CHECKPRIV",
"CLOSE",
"COALESCE",
"COBOL",
"COLLATE",
"COLLATION",
"COLUMN",
"COMMIT",
"CONNECT",
"CONNECTION",
"CONSTRAINT",
"CONSTRAINTS",
"CONTINUE",
"CONVERT",
"CORRESPONDING",
"COUNT",
"CREATE",
"CREATE_ROLE",
"CREATE_USER",
"CROSS",
"CURRENT",
"CURRENT_DATE",
"CURRENT_TIME",
"CURRENT_TIMESTAMP",
"CURRENT_USER",
"CURSOR",
"DATE",
"DAY",
"DBUGFULL",
"DEALLOCATE",
"DEC",
"DECIMAL",
"DECLARE",
"DEFAULT",
"DEFERRABLE",
"DEFERRED",
"DELDATA",
"DELETE",
"DESC",
"DESCRIBE",
"DESCRIPTION",
"DESCRIPTOR",
"DIAGNOSTICS",
"DISCONNECT",
"DISTINCT",
"DOMAIN",
"DOUBLE",
"DROP",
"DROP_ANY_ROLE",
"DROP_USER",
"ELSE",
"END",
"ENDEXEC",
"ESCAPE",
"EXACT",
"EXCEPT",
"EXCEPTION",
"EXEC",
"EXECUTE",
"EXISTS",
"EXTERNAL",
"EXTERNAL",
"EXTRACT",
"FALSE",
"FETCH",
"FILE",
"FILE",
"FIRST",
"FLOAT",
"FOR",
"FOREACH",
"FOREIGN",
"FORTRAN",
"FOUND",
"FROM",
"FULL",
"FULL",
"GET",
"GLOBAL",
"GO",
"GOTO",
"GRANT",
"GRANT_ANY_PRIVILEGE",
"GRANT_ANY_ROLE",
"GROUP",
"HAVING",
"HOUR",
"IDENTITY",
"IMMEDIATE",
"IN",
"INDICATOR",
"INITIALLY",
"INNER",
"INORDER",
"INPUT",
"INSENSITIVE",
"INSERT",
"INT",
"INTEGER",
"INTERNAL",
"INTERSECT",
"INTERVAL",
"INTEXT",
"INTO",
"INTRANS",
"INTRANSACTION",
"IS",
"ISOLATION",
"JOIN",
"KEY",
"LANGUAGE",
"LAST",
"LEADING",
"LEFT",
"LEVEL",
"LIKE",
"LOCAL",
"LOWER",
"MATCH",
"MAX",
"MCODE",
"MIN",
"MINUTE",
"MODULE",
"MONTH",
"NAMES",
"NATIONAL",
"NATURAL",
"NCHAR",
"NEXT",
"NO",
"NOCHECK",
"NODELDATA",
"NOINDEX",
"NOLOCK",
"NOT",
"NOTRIGGER",
"NULL",
"NULLIF",
"NUMERIC",
"NUMROWS",
"OCTET_LENGTH",
"ODBCOUT",
"OF",
"ON",
"ONLY",
"OPEN",
"OPTION",
"OR",
"ORDER",
"OUTER",
"OUTPUT",
"OVERLAPS",
"PAD",
"PARTIAL",
"PASCAL",
"PLI",
"POSITION",
"PRECISION",
"PREPARE",
"PRESERVE",
"PRIMARY",
"PRIOR",
"PRIVILEGES",
"PROCEDURE",
"PUBLIC",
"READ",
"REAL",
"REFERENCES",
"RELATIVE",
"RESTRICT",
"REVOKE",
"RIGHT",
"ROLE",
"ROLLBACK",
"ROUTINE",
"ROWCOUNT",
"ROWS",
"SCHEMA",
"SCROLL",
"SECOND",
"SECTION",
"SELECT",
"SESSION_USER",
"SET",
"SIZE",
"SMALLINT",
"SOME",
"SPACE",
"SQL",
"SQLCODE",
"SQLERROR",
"SQLSTATE",
"STARTSWITH",
"STRING",
"SUBSTRING",
"SUM",
"SYSTEM_USER",
"TABLE",
"TEMPORARY",
"THEN",
"THRESHOLD",
"TIME",
"TIMESTAMP",
"TIMEZONE_HOUR",
"TIMEZONE_MINUTE",
"TO",
"TRAILING",
"TRANSACTION",
"TRANSLATE",
"TRANSLATION",
"TRIM",
"TRUE",
"UNION",
"UNIQUE",
"UNKNOWN",
"UPDATE",
"UPPER",
"UPPER",
"USAGE",
"USER",
"USING",
"VALUE",
"VALUES",
"VARCHAR",
"VARYING",
"VIEW",
"WHEN",
"WHENEVER",
"WHERE",
"WITH",
"WORK",
"WRITE",
"YEAR",
"ZONE" };
        #endregion
   
        public static List<int> GetNumbersInString(string sentence)
        {
            List<int> ret = new List<int>();

            //
            // Get all digit sequence as strings.
            //
            string[] digits = Regex.Split(sentence, @"\D+");
            //
            // Now we have each number string.
            //
            foreach (string value in digits)
            {
                //
                // Parse the value to get the number.
                //
                int number;
                if (int.TryParse(value, out number))
                {
                    ret.Add(number);
                }
            }
            return ret;
        }

        public static string[] AllToUpper(string[] param)
        {
            return Array.ConvertAll<string, string>(param, delegate (string s) { return s.ToUpper(); });
        }
        /// <summary>
        /// Remove all instanses of given item from a stringarray
        /// </summary>
        /// <param name="array">arrray to remove item from </param>
        /// <param name="item">item to be removeduplicates</param>
        /// <returns>array without item in it</returns>
        public static string[] RemoveItem(string[] array, string item)
        {
            ArrayList ret = new ArrayList();
            ret.AddRange(array);
            while (ret.IndexOf(item) >= 0)
            {
                ret.RemoveAt(ret.IndexOf(item));
                ret.TrimToSize();
            }

            return ret.ToArray(typeof(string)) as string[];
        }
        public static string[] RemoveDuplicates(string[] s)
        {
            HashSet<string> set = new HashSet<string>(s);
            string[] result = new string[set.Count];
            set.CopyTo(result); return result;
        }

        /// <summary>
        /// Fjerner forbanna irriterende tegn fra en string.
        /// </summary>
        /// <param name="s">strengen som må renses fra de føkkings forbanna irriterende kontrolltegnenede som ikke betyr noe, men som gjør deg sprø</param>
        /// <returns>string uten forbanna irriterende tegn</returns>
        public static string RemoveControlChars(string s)
        {
            return Regex.Replace(s, @"[^\u0000-\u007F]", "");
        }

        public static string FormaterTelefonnr(string telefonr)
        {
            string ret = telefonr;
            try
            {
                double erTall = 0;
                if (Double.TryParse(telefonr, out erTall))
                {
                    if (telefonr.Length == 8)
                    {
                        if (telefonr.StartsWith("4") || telefonr.StartsWith("8") || telefonr.StartsWith("9"))
                            ret = String.Format("{0:### ## ###}", Convert.ToInt32(erTall));
                        else
                            ret = String.Format("{0:## ## ## ##}", Convert.ToInt32(erTall));
                    }
                    else
                    {
                        ret = String.Format("{0:## ## ## ## ## ## ## ##}", erTall);

                        if (telefonr.StartsWith("+00") || telefonr.StartsWith("00"))
                            ret = "00" + ret;

                        if (telefonr.StartsWith("+"))
                            ret = "+" + ret;
                    }
                }
            }
            catch (Exception ex) { }
            ret = ret.Replace("  ", " ");
            return ret.Trim();
        }

        /// <summary>
        /// Returns a copy of this System.String with first letter of each word in uppercase.
        /// </summary>
        /// <param name="s">The string to convert</param>
        /// <returns>Converted string</returns>
        public static string FirstToUpper(string text)
        {
            string ret = "";

            if (text != null)
            {
                try
                {
                    char[] a = text.ToCharArray();
                    bool setNextCharToUpper = true;

                    foreach (char c in a)
                    {
                        if (setNextCharToUpper)
                        {
                            ret += Char.ToUpper(c);
                            setNextCharToUpper = false;
                        }
                        else
                            ret += Char.ToLower(c);

                        if (c == ' ' || c == '-')
                            setNextCharToUpper = true;
                    }
                }
                catch (Exception ex)
                {

                }
            }

            return ret;
        }

        /// <summary>
        /// Deler en string med CamelCasing og returnerer en string med ord
        /// </summary>
        /// <param name="text"></param>
        /// <returns>Converted string</returns>
        public static string FirstToUpperCamelCasing(string text)
        {
            string ret = string.Empty;
            try
            {
                ret = System.Text.RegularExpressions.Regex.Replace(
                    text,
                    "([A-Z])",
                    " $1",
                    System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
            }

            catch (Exception)
            {
            }
            return ret;
        }

        /// <summary>
        /// Deler en string med CamelCasing og returnerer en string med ord
        /// </summary>
        /// <param name="text">string som skal deles</param>
        /// /// <param name="text">tegn/string som skal erstatte spesielle tegn</param>
        /// <returns>Converted string</returns>
        public static string FirstToUpperCamelCasing(string text, string replaceSpecialChar)
        {
            string ret = FirstToUpperCamelCasing(text);

            if (!String.IsNullOrEmpty(ret) && !string.IsNullOrEmpty(replaceSpecialChar))
            {
                string[] specialChars = new string[] { "_" };
                foreach (string item in specialChars)
                {
                    ret = ret.Replace(item, replaceSpecialChar);
                }
            }
            return ret;
        }
        public static double FirstNumberInString(string tekst)
        {
            double ret = 00;
            string[] temp = tekst.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (temp != null && temp.Length > 0)
            {
                for (int i = 0; i < temp.Length; i++)
                {
                    if (Double.TryParse(temp[i], out ret))
                    {
                        if (ret > 0)
                        {
                            if (i > 0)
                            {
                                if (temp[i - 1].Trim() == "-")
                                    ret = ret * -1;
                            }
                        }
                    }
                }

            }
            return ret;
        }

        /// <summary>
        /// Metode som tar inn en string og forsøker å returnere etternavn og fornavn 
        /// Antar at etternavn er oppgitt først dersom komma ikke er med.
        /// </summary>
        /// <param name="p"></param>
        /// <returns>Array - 2 langt, etternavn fornavn  </returns>
        public static string[] NavnToArray(string p)
        {
            string[] ret = new string[] { "", "" };
            char[] split = new char[] { ' ' };
            if (p.Contains(","))
                split = new char[] { ',' };

            string[] navnArr = p.Split(split, StringSplitOptions.RemoveEmptyEntries);
            if (navnArr != null && navnArr.Length >= 2)
            {
                try
                {
                    ret[0] = navnArr[0];
                    for (int i = 1; i < navnArr.Length; i++)
                    {
                        ret[1] += navnArr[i] + " ";
                    }
                }
                catch (Exception) { }

            }
            ret[1] = ret[1].Trim();
            return ret;
        }

        /// <summary>
        /// Hent ett subarray av ett array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string[] SubArray(string[] data, int index, int length)
        {
            string[] result = new string[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static bool IsValidEmail(string strIn)
        {
            bool ret = false;
            if (!string.IsNullOrEmpty(strIn))
            {
                // Return true if strIn is in valid e-mail format.
                ret = Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");

            }
            return ret;
        }

        public static bool IsValidIBAN(string strIn)
        {
            bool ret = false;
            string regEx = @"[a-zA-Z]{2}[0-9]{2}[a-zA-Z0-9]{4}[0-9]{7}([a-zA-Z0-9]?){0,16}";
            if (!string.IsNullOrEmpty(strIn))
            {
                // Return true if strIn is in valid e-mail format.
                ret = Regex.IsMatch(strIn.Replace("-", ""), regEx);

            }
            return ret;

        }

     

        #region String Functions
        public static System.Text.Encoding Encoding
        {
            get
            {
                return System.Text.Encoding.GetEncoding("iso-8859-15");
            }
        }

        public static byte[] GetBytes(string msg)
        {
            return Encoding.GetBytes(msg);
        }

        public static string GetString(byte[] bytes)
        {
            return Encoding.GetString(bytes);
        }

        public static string GetString(byte[] bytes, int index, int count)
        {
            return Encoding.GetString(bytes, index, count);
        }
        #endregion

        /// <summary>
        /// Preventing SQL injection attacks using C#.NET 
        /// http://forum.coolwebawards.com/threads/12-Preventing-SQL-injection-attacks-using-C-NET
        /// Vet ikke om dette er noe særlig,,,,, finn noe annet.
        /// </summary>
        /// <param name="theValue"></param>
        /// <param name="theLevel"></param>
        /// <returns></returns>
        public static string SafeSqlLiteral(System.Object theValue, System.Object theLevel)
        {

            // Written by user CWA, CoolWebAwards.com Forums. 2 February 2010
            // http://forum.coolwebawards.com/threads/12-Preventing-SQL-injection-attacks-using-C-NET

            // intLevel represent how thorough the value will be checked for dangerous code
            // intLevel (1) - Do just the basic. This level will already counter most of the SQL injection attacks
            // intLevel (2) -   (non breaking space) will be added to most words used in SQL queries to prevent unauthorized access to the database. Safe to be printed back into HTML code. Don't use for usernames or passwords

            string strValue = (string)theValue;
            int intLevel = (int)theLevel;

            if (strValue != null)
            {
                if (intLevel > 0)
                {
                    strValue = strValue.Replace("'", "''"); // Most important one! This line alone can prevent most injection attacks
                    strValue = strValue.Replace("--", "");
                    strValue = strValue.Replace("[", "[[]");
                    strValue = strValue.Replace("%", "[%]");
                }
                if (intLevel > 1)
                {
                    string[] myArray = new string[] { "xp_ ", "update ", "insert ", "select ", "drop ", "alter ", "create ", "rename ", "delete ", "replace " };
                    int i = 0;
                    int i2 = 0;
                    int intLenghtLeft = 0;
                    for (i = 0; i < myArray.Length; i++)
                    {
                        string strWord = myArray[i];
                        Regex rx = new Regex(strWord, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                        MatchCollection matches = rx.Matches(strValue);
                        i2 = 0;
                        foreach (Match match in matches)
                        {
                            GroupCollection groups = match.Groups;
                            intLenghtLeft = groups[0].Index + myArray[i].Length + i2;
                            strValue = strValue.Substring(0, intLenghtLeft - 1) + "&nbsp;" + strValue.Substring(strValue.Length - (strValue.Length - intLenghtLeft), strValue.Length - intLenghtLeft);
                            i2 += 5;
                        }
                    }
                }
                return strValue;
            }
            else
            {
                return strValue;
            }
        }

        public static bool ContainsBadReservedSQLWord(string strValue)
        {
            bool ret = false;

            string[] myArray = BADSQLWORDS;
            int i = 0;
            int i2 = 0;
            int intLenghtLeft = 0;
            for (i = 0; i < myArray.Length; i++)
            {
                string strWord = myArray[i];
                Regex rx = new Regex(strWord, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                MatchCollection matches = rx.Matches(strValue);
                i2 = 0;
                foreach (Match match in matches)
                {
                    ret = true;
                    break;
                }
                if (ret)
                    break;
            }

            if (ret)
                ret = ret && !strValue.Contains("#"); // hindrer spørringer med temptabeller å ikke bli utført

            return ret;
        }


        /// <summary>
        /// Contains approximate string matching 
        /// Compute the distance between two strings.
        /// </summary>
        public static int LevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }


        #region web stringutils

        /* Example Usage
        var queryParams = new NameValueCollection()
        {
            { "x", "1" },
            { "y", "2" },
            { "foo", "bar" },
            { "foo", "baz" },
            { "special chars", "? = &" },
        };

        string url = "http://example.com/stuff" + ToQueryString(queryParams);*/
        public static string ToQueryString(NameValueCollection nvc)
        {
            StringBuilder sb = new StringBuilder("?");

            bool first = true;

            foreach (string key in nvc.AllKeys)
            {
                //foreach (string value in nvc.GetValues(key))
                //{
                    if (!first)
                    {
                        sb.Append("&");
                    }

                    sb.AppendFormat("{0}={1}", Uri.EscapeDataString(key), Uri.EscapeDataString(nvc.GetValues(key).FirstOrDefault().ToString()));

                    first = false;
      //          }
            }

            return sb.ToString();
        }
        #endregion

        public static string ConvertToPascalCase(string snakeCase)
        {
            if (string.IsNullOrEmpty(snakeCase))
                return snakeCase;

            var words = snakeCase.Split('_');
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                {
                    words[i] = char.ToUpper(words[i][0], CultureInfo.InvariantCulture) + words[i].Substring(1);
                }
            }
            return string.Join(string.Empty, words);
        }
    }
}