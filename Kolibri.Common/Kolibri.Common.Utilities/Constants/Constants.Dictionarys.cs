using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolibri.Common.Utilities.Constants
{
    public partial class Constants
    {
        public static Dictionary<string, DirectoryInfo> DicUtvikling { get { return m_dicUtvikling; } }
        public static Dictionary<string, DirectoryInfo> DicTest { get { return m_dicTest; } }
        public static  Dictionary<string, DirectoryInfo> DicTools { get { return m_dicTools; } }
        public static Dictionary<string, FileInfo> DicHemitHER
        {
            get
            {
                Dictionary<string, FileInfo> ret = m_dicHemitHER;
                try
                {
                    FileInfo info = new FileInfo(@"\\hemitbrukere\felleshemit\Anvendelse\Biztalk\Utvikling\asostb-he\HemitHER20\MaintLogDB.exe");
                    if (info.Exists) { ret.Add("HemitHER2.0", info); }
                }
                catch (Exception) { }
                return ret;
            }
        }
        public static Dictionary<string, Uri> DicAdresseRegisteret { get { return Constants.m_dicAdresseRegisteret; } }

        #region dictionarys
        #region miljø
        private static Dictionary<string, Uri> m_dicMDM = new Dictionary<string, Uri>(){
        {"PROD",new Uri(@"http://p.helsemn.no/Kodes/Index")},
        {"TEST",new Uri(@"http://mdmwebt.helsemn.no/Kodes/Index")}, 
        {"PRODTEST",new Uri(@"http://mdmwebpt.helsemn.no/Kodes/Index")},
        {"UTVIKLING",new Uri(@"http://mdmwebutv.helsemn.no/Kodes/Index")}, 
        };
        #endregion
        
        #region tools
        private static Dictionary<string, DirectoryInfo> m_dicTools = new Dictionary<string, DirectoryInfo>()
        { 
            {"BizTalk Support Tools",new DirectoryInfo(@"C:\Program Files (x86)\Microsoft BizTalk Server 2013 R2\SDK\Utilities\Support Tools\")}, 
            {"Verktoy", new DirectoryInfo(@"\\hemitbrukere\felleshemit\Anvendelse\Biztalk\Verktoy")},// @"\\hemitbrukere\felleshemit\Anvendelse\Biztalk\Hemit_Leveranser2010\Tools") },/*@"\\hemitbrukere\felleshemit\Anvendelse\Biztalk\Verktoy") */
            { "Tools",  Constants.ToolsPath },
            {"Installed", new DirectoryInfo(@"C:\Program Files (x86)\Microsoft BizTalk Server 2016")},
           };
        #endregion
        
        #region utvikling
        private static Dictionary<string, DirectoryInfo> m_dicUtvikling = new Dictionary<string, DirectoryInfo>()
        {
          {"Utvikling", new DirectoryInfo( @"\\hemitbrukere\felleshemit\Anvendelse\Biztalk\Utvikling")},
        };
        #endregion
        
        #region test og dokumentasjon
        private static Dictionary<string, DirectoryInfo> m_dicTest = new Dictionary<string, DirectoryInfo>() 
        { 
             {"Testbibliotek", new DirectoryInfo(Path.Combine(Constants.GetHemitbrukereTestFilerPath().FullName,  @"\Testbibliotek"))},            //{ "Biztalk TestFiler", new DirectoryInfo(@"\\hemitbrukere\felleshemit\Anvendelse\Biztalk\Testfiler") },         
             {"Dokumentasjon",   new DirectoryInfo(@"\\hemitbrukere\felleshemit\Anvendelse\Biztalk\Dokumentasjon")},
        };
        #endregion
        
        #region HemitHER
        private static Dictionary<string, FileInfo> m_dicHemitHER = new Dictionary<string, FileInfo>(){
           {"PROD", new FileInfo(@"\\hemitbrukere\felleshemit\Anvendelse\Biztalk\DRIFT\HemitHER\MaintLogDB.exe")},
            {"PRODTEST", new FileInfo(@"\\hemitbrukere\felleshemit\Anvendelse\Biztalk\DRIFT\HemitHER - PRODTEST\MaintLogDB.exe")}, 
            {"PRODTEST2", new FileInfo(@"\\hemitbrukere\felleshemit\Anvendelse\Biztalk\DRIFT\HemitHER - PRODTEST2\MaintLogDB.exe")},
          //  {"TEST", new FileInfo(@"\\hemitbrukere\felleshemit\Anvendelse\Biztalk\DRIFT\HemitHER - TEST\MaintLogDB.exe")},
            {"TEST", new FileInfo(@"\\hemitbrukere\felleshemit\Anvendelse\Biztalk\DRIFT\HemitHER - TEST\MaintLogDB_20.exe")},
            {"UTVIKLING", new FileInfo(@"\\hemitbrukere\felleshemit\Anvendelse\Biztalk\Utvikling\HemitHER - UTVIKLING\MaintLogDB.exe")},
        };
        #endregion

        #region        AdresseRegisteret
        private static Dictionary<string, Uri> m_dicAdresseRegisteret = new Dictionary<string, Uri>()
        { 
            {"AR_Prod", new Uri(@"https://register.nhn.no/Ar/")},
            {"AR_Test" , new Uri(@"https://register.test.nhn.no/Ar")}
        };
        #endregion

        #endregion
    }
}