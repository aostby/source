﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kolibri.Common.MovieAPI.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.4.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string TMDBkey {
            get {
                return ((string)(this["TMDBkey"]));
            }
            set {
                this["TMDBkey"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"{""images"":{""base_url"":""http://image.tmdb.org/t/p/"",""secure_base_url"":""https://image.tmdb.org/t/p/"",""backdrop_sizes"":[""w300"",""w780"",""w1280"",""original""],""logo_sizes"":[""w45"",""w92"",""w154"",""w185"",""w300"",""w500"",""original""],""poster_sizes"":[""w92"",""w154"",""w185"",""w342"",""w500"",""w780"",""original""],""profile_sizes"":[""w45"",""w185"",""h632"",""original""],""still_sizes"":[""w92"",""w185"",""w300"",""original""]},""change_keys"":[""adult"",""air_date"",""also_known_as"",""alternative_titles"",""biography"",""birthday"",""budget"",""cast"",""certifications"",""character_names"",""created_by"",""crew"",""deathday"",""episode"",""episode_number"",""episode_run_time"",""freebase_id"",""freebase_mid"",""general"",""genres"",""guest_stars"",""homepage"",""images"",""imdb_id"",""languages"",""name"",""network"",""origin_country"",""original_name"",""original_title"",""overview"",""parts"",""place_of_birth"",""plot_keywords"",""production_code"",""production_companies"",""production_countries"",""releases"",""revenue"",""runtime"",""season"",""season_number"",""season_regular"",""spoken_languages"",""status"",""tagline"",""title"",""translations"",""tvdb_id"",""tvrage_id"",""type"",""video"",""videos""]}")]
        public string TMCBConfig {
            get {
                return ((string)(this["TMCBConfig"]));
            }
            set {
                this["TMCBConfig"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string OMDBkey {
            get {
                return ((string)(this["OMDBkey"]));
            }
            set {
                this["OMDBkey"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string LastImageCacheConnectionString {
            get {
                return ((string)(this["LastImageCacheConnectionString"]));
            }
            set {
                this["LastImageCacheConnectionString"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("./OMDB/OMDB.db")]
        public string LastOMDBConnectionString {
            get {
                return ((string)(this["LastOMDBConnectionString"]));
            }
            set {
                this["LastOMDBConnectionString"] = value;
            }
        }
    }
}
