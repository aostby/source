﻿using Newtonsoft.Json;

namespace Kolibri.net.Common.Utilities.Extensions
{
    public static class StringExt
    {   

        public static Int32 ToInt32(this string number)
        {
            if (string.IsNullOrEmpty(number)) return 0;

            number = number.ToLower().Replace("s", string.Empty);
            number = number.ToLower().Replace("S", string.Empty);
            number = number.ToLower().Replace("e", string.Empty);
            number = number.ToLower().Replace("E", string.Empty);

            if (number.IsNumeric())
            {

                return Int32.Parse(
                    number,
                    System.Globalization.NumberStyles.Integer,
                    System.Globalization.CultureInfo.CurrentCulture.NumberFormat);
            }
            else throw new Exception($"Verdien {number} er ikke ett heltall!");
        }
        //public static bool IsNumeric(this string value)
        //{
        //    return value.All(char.IsNumber);
        //}
        public static bool IsNumeric(this object Expression)
        {
            double retNum;

            bool isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        public static string JsonSerializeObject<T>(this T obj, bool format = false) where T : class
        {
            string text = JsonConvert.SerializeObject(obj, (format? Formatting.Indented:Formatting.None));

            return text;
        }

    }
}
