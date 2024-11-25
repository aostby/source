using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Kolibri.Common.Utilities
{
    public class DateTimeUtilities
    {
        /// <summary>
        /// 
        /// </summary>
        public enum DatoFormat
        {
            /// <summary>
            /// Datoformat yyyyMMdd
            /// </summary>
            yyyyMMdd,
            /// <summary>
            /// Datoformat yyyy-MM-dd
            /// </summary>
            yyyy_MM_dd,
            /// <summary>
            /// Datoformat yyyyMMddHHmm
            /// </summary>
            yyyyMMddHHmm,
            /// <summary>
            /// Datoformat yyyyMMddHHmmss
            /// </summary>
            yyyyMMddHHmmss,
            /// <summary>
            /// Datoformat ddMMyy
            /// </summary>
            ddMMyy,
            /// <summary>
            /// Datoformat ddMMyyyy
            /// </summary>
            ddMMyyyy,
            /// <summary>
            /// Datoformat ddMMyyyyHHmm
            /// </summary>
            ddMMyyyyHHmm,
            /// <summary>
            /// Datoformat dd.MM.yyyy
            /// </summary>
            dd_MM_yyyy,
            /// <summary>
            /// Datoformat dd.MM.yyyy HH:mm
            /// </summary>
            dd_MM_yyyy_HH_mm,
            /// <summary>
            /// Datoformat dd.MM.yyyy HH:mm:ss
            /// </summary>
            dd_MM_yyyy_HH_mm_ss,
            /// <summary>
            /// Datoformat HH:mm
            /// </summary>
            HH_mm,
            /// <summary>
            /// Datoformat HH:mm:ss
            /// </summary>
            HH_mm_ss,
            /// <summary>
            /// Datoformat HHmm
            /// </summary>
            HHmm,
            /// <summary>
            /// Datoformat HHmmss
            /// </summary>
            HHmmss

        }
        /// <summary>
        /// Sjekk om oppgitt dato er gyldig ut fra oppgitt datoformat og returner dato som DateTime.
        /// </summary>
        /// <param name="dato">Dato som skal sjekkes</param>
        /// <param name="datoFormat">Format det skal sjekkes mot.</param>
        /// <param name="dateTime">Dato på DateTime format.</param>
        /// <returns>True dersom dato på riktig format; ellers, false.</returns>
        public static bool StringTilDateTime(string dato, DatoFormat datoFormat, out DateTime dateTime)
        {
            bool ret = false;
            dateTime = new DateTime();

            try
            {
                ret = !string.IsNullOrEmpty(dato);

                if (ret)
                {
                    // Sjekker lengde
                    switch (datoFormat)
                    {
                        case DatoFormat.HHmm:
                            ret = dato.Length == 4;
                            break;
                        case DatoFormat.HH_mm:
                            ret = dato.Length == 5;
                            break;
                        case DatoFormat.ddMMyy:
                        case DatoFormat.HHmmss:
                            ret = dato.Length == 6;
                            break;
                        case DatoFormat.yyyyMMdd:
                        case DatoFormat.ddMMyyyy:
                        case DatoFormat.HH_mm_ss:
                            ret = dato.Length == 8;
                            break;
                        case DatoFormat.dd_MM_yyyy:
                        case DatoFormat.yyyy_MM_dd:
                            ret = dato.Length == 10;
                            break;
                        case DatoFormat.yyyyMMddHHmm:
                            ret = dato.Length == 12;
                            break;
                        case DatoFormat.ddMMyyyyHHmm:
                            break;
                        case DatoFormat.yyyyMMddHHmmss:

                            ret = dato.Length == 14;
                            break;
                        case DatoFormat.dd_MM_yyyy_HH_mm:
                            ret = dato.Length == 16;
                            break;
                        case DatoFormat.dd_MM_yyyy_HH_mm_ss:
                            ret = dato.Length == 19;
                            break;
                        default:
                            ret = false;
                            break;
                    }
                }

                // Sjekk innhold
                if (ret)
                {
                    switch (datoFormat)
                    {
                        case DatoFormat.yyyyMMdd:
                            ret = DateTime.TryParse(DatoSkilletegnDbDato(dato), out dateTime);
                            break;
                        case DatoFormat.yyyyMMddHHmm:
                            ret = DateTime.TryParse(dato.Substring(6, 2) + "." + dato.Substring(4, 2) + "." + dato.Substring(0, 4) + " " + dato.Substring(8, 2) + ":" + dato.Substring(10, 2), out dateTime);
                            break;
                        case DatoFormat.yyyyMMddHHmmss:
                            ret = DateTime.TryParse(dato.Substring(6, 2) + "." + dato.Substring(4, 2) + "." + dato.Substring(0, 4) + " " + dato.Substring(8, 2) + ":" + dato.Substring(10, 2) + ":" + dato.Substring(12, 2), out dateTime);
                            break;
                        case DatoFormat.ddMMyy:
                            ret = DateTime.TryParse(DatoSkilletegn(dato), out dateTime);
                            break;
                        case DatoFormat.ddMMyyyy:
                            ret = DateTime.TryParse(DatoSkilletegn(dato), out dateTime);
                            break;
                        case DatoFormat.ddMMyyyyHHmm:
                            ret = DateTime.TryParse(dato.Substring(0, 2) + "." + dato.Substring(2, 2) + "." + dato.Substring(4, 4) + " " + dato.Substring(8, 2) + ":" + dato.Substring(10, 2), out dateTime);
                            break;
                        case DatoFormat.dd_MM_yyyy:
                        case DatoFormat.yyyy_MM_dd:
                            ret = DateTime.TryParse(dato, out dateTime);
                            break;
                        case DatoFormat.dd_MM_yyyy_HH_mm:
                            ret = DateTime.TryParse(dato, out dateTime);
                            break;
                        case DatoFormat.dd_MM_yyyy_HH_mm_ss:
                            ret = DateTime.TryParse(dato, out dateTime);
                            break;
                        case DatoFormat.HH_mm:
                            ret = DateTime.TryParse(dato, out dateTime);
                            break;
                        case DatoFormat.HH_mm_ss:
                            ret = DateTime.TryParse(dato, out dateTime);
                            break;
                        case DatoFormat.HHmm:
                            ret = DateTime.TryParse(dato.Substring(0, 2) + ":" + dato.Substring(2, 2), out dateTime);
                            break;
                        case DatoFormat.HHmmss:
                            ret = DateTime.TryParse(dato.Substring(0, 2) + ":" + dato.Substring(2, 2) + ":" + dato.Substring(4, 2), out dateTime);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                ret = false;
            }

            return ret;
        }
        public static string DatoSkilletegnDbDato(string dato)
        {
            string sTmp = dato;
            try
            {
                if (dato != null)
                {
                    if (dato.Length == 8)
                    {
                        sTmp = dato.Substring(6, 2) + "." + dato.Substring(4, 2) + "." + dato.Substring(0, 4);
                    }
                }
            }
            catch (Exception e) { } return sTmp;
        }
        /// <summary>
        /// Setter inn . som skilletegn i datoen
        /// Innparameter er DDMMYYYY
        /// Utparameter er DD.MM.YYYY
        /// </summary>
        /// <param name="dato">DDMMYYYY</param>
        /// <returns>DD.MM.YYYY</returns>
        public static string DatoSkilletegn(string dato)
        {
            string tmp = "";
            try
            {
                if (dato.Length == 8)
                {
                    //Substring starter på 0 derfor de rare tallene					
                    tmp = dato.Substring(0, 2) + "." + dato.Substring(2, 2) + "." + dato.Substring(4, 4);
                }
                else if (dato.Length == 6)
                {
                    tmp = dato.Substring(0, 2) + "." + dato.Substring(2, 2) + "." + dato.Substring(4, 2);
                }
            }
            catch (Exception e) { } return tmp;
        }
        public static string MaanedNavn(int maaned)
        {
            string ret = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(maaned);

            return ret;
        }

        public static int MaanedNr(string maaned)
        {
            int ret = 0;
            string[] monthnames = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
            // Thread.CurrentThread.CurrentCulture.DateTimeFormat.MonthNames
            //  if(CultureInfo.CurrentCulture.DateTimeFormat.MonthNames.Contains(maaned.ToLower()))
            // ret = Array.IndexOf(CultureInfo.CurrentCulture.DateTimeFormat.MonthNames, maaned.ToLower());
            ret = Array.IndexOf(monthnames, maaned.Trim().ToLower());


            return ret + 1;
        }

        public static string TimeSpanToText(TimeSpan duration)
        {
            TimeSpan t = duration;
            string answer = string.Format("{0:D2}:{1:D2}:{2:D2}",//:{3:D3}ms",
                            t.Hours,
                            t.Minutes,
                            t.Seconds
                //,t.Milliseconds
                            );
            return answer;

        }

        public static string TimeSpanToText(DateTime start, DateTime slutt)
        {
            string ret = string.Empty;
            try
            {
                ret = TimeSpanToText(slutt.Subtract(start), true);
            }
            catch (Exception)
            {
            }
            return ret;
        }

        public static string TimeSpanToText(TimeSpan time_span, bool whole_seconds)
        {
            string txt = "";

            if (time_span.Days > 0)
            {
                txt += ", " + time_span.Days.ToString() + " dager";
                time_span = time_span.Subtract(new TimeSpan(time_span.Days, 0, 0, 0));
            }
            if (time_span.Hours > 0)
            {
                txt += ", " + time_span.Hours.ToString() + " timer";
                time_span = time_span.Subtract(new TimeSpan(0, time_span.Hours, 0, 0));
            }
            if (time_span.Minutes > 0)
            {
                txt += ", " + time_span.Minutes.ToString() + " " + "minutter";
                time_span = time_span.Subtract(new TimeSpan(0, 0, time_span.Minutes, 0));
            }

            if (whole_seconds)
            {
                // Display only whole seconds.
                if (time_span.Seconds > 0)
                {
                    txt += ", " + time_span.Seconds.ToString() + " " + "sekunder";
                }
            }
            else
            {
                // Display fractional seconds.
                txt += ", " + time_span.TotalSeconds.ToString() + " " + "sekunder";
            }

            // Remove the leading ", ".
            if (txt.Length > 0)
                txt = txt.Substring(2);

            // Return the result.
            return txt;
        }

        public static string FormatMinutter(int antMin)
        {
            int timer = antMin / 60;
            double minutter = antMin % 60;
            string min = "" + minutter;
            if (min.Length == 1)
                min = min.Insert(0, "0");

            return "" + timer + ":" + min;
        }

        public static string ReplaceDate(string instring, DatoFormat oldFormat, DatoFormat newFormat)
        {
            string ret;
            string[] temp = instring.Split(new char[] { ' ' });
            for (int i = 0; i < temp.Length; i++)
            {
                string item = temp[i];
                if (!string.IsNullOrEmpty(item))
                {
                    DateTime dt = new DateTime();
                    if (StringTilDateTime(item, oldFormat, out dt))
                    {
                        switch (newFormat)
                        {
                            case DatoFormat.yyyyMMdd:
                            case DatoFormat.yyyy_MM_dd:
                            case DatoFormat.yyyyMMddHHmm:
                            case DatoFormat.yyyyMMddHHmmss:
                            case DatoFormat.ddMMyy:
                            case DatoFormat.ddMMyyyyHHmm:
                            case DatoFormat.ddMMyyyy:
                                temp[i] = dt.ToString(newFormat.ToString().Replace("_", "-"));
                                break;

                            case DatoFormat.dd_MM_yyyy:
                                temp[i] = dt.ToString(newFormat.ToString().Replace("_", "."));
                                break;
                            case DatoFormat.dd_MM_yyyy_HH_mm:
                                temp[i] = dt.ToString("dd.MM.yyyy HH:mm");
                                break;
                            case DatoFormat.dd_MM_yyyy_HH_mm_ss:
                                temp[i] = dt.ToString("dd.MM.yyyy HH:mm:ss");
                                break;

                            case DatoFormat.HH_mm:
                            case DatoFormat.HH_mm_ss:
                            case DatoFormat.HHmm:
                            case DatoFormat.HHmmss:
                                temp[i] = dt.ToString(newFormat.ToString().Replace("_", ":"));
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            ret = string.Join(" ", temp);
            return ret;
        }

    }
}

