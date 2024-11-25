using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolibri.Common.MovieAPI.Controller
{
    public class AccessConfig
    {
        public static string TMDBkey { get { return GetSettings(nameof(TMDBkey)); } set { SetSetting(nameof(TMDBkey), value); } }

        public static string TMCBConfig { get { return GetSettings(nameof(TMCBConfig)); } set { SetSetting(nameof(TMCBConfig), value); } }
        public static string OMDBkey { get { return GetSettings(nameof(OMDBkey)); } set { SetSetting(nameof(OMDBkey), value); } }
        public static string LastImageCacheConnectionString { get { return GetSettings(nameof(LastImageCacheConnectionString)); } set { SetSetting(nameof(LastImageCacheConnectionString), value); } }
        public static string LastOMDBConnectionString { get { return GetSettings(nameof(LastOMDBConnectionString)); } set { SetSetting(nameof(LastOMDBConnectionString), value); } }
       // public static string LastSeriesDBConnectionString { get { return GetSettings(nameof(LastSeriesDBConnectionString)); } set { SetSetting(nameof(LastSeriesDBConnectionString), value); } }

        private static string GetSettings(string key) {
            switch (key)
            {
                case "TMDBkey": return Properties.Settings.Default.TMDBkey; break;
                case "TMCBConfig": return Properties.Settings.Default.TMCBConfig; break;
                case "OMDBkey": return Properties.Settings.Default.OMDBkey; break;
                case "LastImageCacheConnectionString": return Properties.Settings.Default.LastImageCacheConnectionString; break;
                case "LastOMDBConnectionString": return Properties.Settings.Default.LastOMDBConnectionString; break;
                case "LastSeriesDBConnectionString": return Properties.Settings.Default.LastOMDBConnectionString; break;
                default:
                    throw new InvalidOperationException
                        ($"Properties.Settings.Default.SettingsKey {key} does not exist. Valid keys are {Properties.Settings.Default}");
            }
        }

        public static void SetSetting(string key, string value)
        {
            switch (key)
            {
                case "TMDBkey": Properties.Settings.Default.TMDBkey = value; break;
                case "TMCBConfig": Properties.Settings.Default.TMCBConfig = value; ; break;
                case "OMDBkey": Properties.Settings.Default.OMDBkey = value; ; break;
                case "LastImageCacheConnectionString": Properties.Settings.Default.LastImageCacheConnectionString = value; ; break;
                case "LastOMDBConnectionString": Properties.Settings.Default.LastOMDBConnectionString = value; ; break;
                case "LastSeriesDBConnectionString": Properties.Settings.Default.LastOMDBConnectionString = value; ; break;
                default:
                    throw new InvalidOperationException
                        ($"Properties.Settings.Default.SettingsKey {key} does not exist. Valid keys are {Properties.Settings.Default}");
            }
            Properties.Settings.Default.Save();
        }


    }
}
