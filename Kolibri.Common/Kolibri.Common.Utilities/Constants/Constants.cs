 
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
 
using System.Xml.Linq;

namespace Kolibri.Common.Utilities.Constants
{
    public partial class Constants
    {
        private static string _bizTalkAdGroup;

        #region FilePaths
        /// <summary>
        /// Returnerer den vanligste filstien for testfiler
        /// </summary>
        /// <returns></returns>
        public static DirectoryInfo GetTestFilerPath()
        {
            return GetHemitbiblTestFilerPath();
        }
        /// <summary>
        /// hemitbibl testdirectory
        /// </summary>
        /// <returns></returns>
        public static DirectoryInfo GetHemitbiblTestFilerPath()
        {
            return new DirectoryInfo(@"\\hemitbibl\Applikasjoner\Biztalk\Forvaltning\Testfiler");
        }
        /// <summary>
        /// hemitbrukere (gammel) testfilerpath
        /// </summary>
        /// <returns></returns>
        [Obsolete("Fremdeles i bruk, men hemitbrukere er byttet til hemitbibl i 2018")]
        public static DirectoryInfo GetHemitbrukereTestFilerPath()
        {
            return new DirectoryInfo(@"\\hemitbrukere\felleshemit\Anvendelse\Biztalk\Testfiler");
        }
        
        public static DirectoryInfo ToolsPath {
            get { return new DirectoryInfo(@"\\hemitbibl\Applikasjoner\Biztalk\Forvaltning\Hemit_Leveranser2016\Tools"); }
        }
        #endregion


        public enum BizTalkEnvironment
        {
            Prod,
            ProdTest,
            Test,
            Devl,
            LocalDevl
        }

        public static List<string> ComputerNames
        {
            get
            {//Test, ProdTest, produksjon
                List<String> ret = new List<string>() { "ahappbizt01", "ahappbizt02", "nmbiz2013pt01", "fsbiz2013pt01", "nmbiz2013p01", "fsbiz2013p01" };

                //Utiviklermaskiner
                for (int i = 0; i < 10; i++)
                {
                    ret.Insert(0, "nmbizutvp" + i.ToString().PadRight(2, '0'));
                }
                ret.Sort();
                /* "nmbizutvp01", "nmbizutvp02", "nmbizutvp03", "nmbizutvp04", "nmbizutvp05", "nmbizutvp06",*/

                return ret;
            }
        }
        public static List<string> AppNames
        {
            get
            {
                FileInfo info = new FileInfo(Path.Combine(Constants.ToolsPath.FullName, @"BizTalk.Tools.SSOStorageBTDF\BizTalk.Tools.SSOStorageBTDF.exe.config"));

                string apps = string.Empty;
                if (info.Exists)
                {
                    try
                    {
                        XDocument xDoc = XDocument.Load(info.FullName);
                        XElement ele = XUtilities.GetElement(xDoc.Root, "setting", "name", "SSOApplications");
                        apps = ele.Value;
                    }
                    catch (Exception)
                    { apps = "Hemit.BizTalk.FileMove;Hemit.BizTalk.IHR;Hemit.BizTalk.Dossier;Hemit.BizTalk.Lab.SymPathy;Hemit.BizTalk.OpPlan.ReportToDL;Hemit.BizTalk.PASEvent.Endobase;Hemit.BizTalk.PASEvent.MBA;Hemit.BizTalk.PatientRegistry.HL7v3;Hemit.BizTalk.NHN.FolkeregisterToPAS;Hemit.BizTalk.DL.DokImport;Hemit.BizTalk.ERP.PAS;Hemit.BizTalk.eResept;TestAppenMin;Hemit.BizTalk.DeploymentTest;Hemit.BizTalk.AnIn.Booking;SP.IHR;Hemit.BizTalk.PAS.FindPatient.HL7v2;Hemit.BizTalk.DL.iECGImport;Hemit.BizTalk.PASEvent.Booking;Hemit.BizTalk.PASEvent.iECG;Hemit.BizTalk.PASEvent.Receiver;Hemit.BizTalk.TDocToUniqueEHandel;Hemit.BizTalk.NHN.WS;Hemit.BizTalk.AnIn.Sammendragsrapport;Hemit.BizTalk.Apotek.DeltaSwisslog;Hemit.BizTalk.OpPlan.Publisher;Hemit.BizTalk.SonyVisualisering;Hemit.BizTalk.Kunnskapssenteret;Hemit.BizTalk.Rtg.Svar;Hemit.BizTalk.MottakOgDekrypter;Hemit.BizTalk.DL.EksterneMeldingerInn;Hemit.BizTalk.DL.EksterneMeldingerUt;Hemit.BizTalk.ERP.Ansattgrunnlag;Hemit.BizTalk.ERP.EHF;Hemit.BizTalk.ERP.SAP.Ansatt;Hemit.BizTalk.ERP.SAP.Bank;Hemit.BizTalk.ERP.SAP.Hovedbok;Hemit.BizTalk.ERP.SAP.Inkassator;Hemit.BizTalk.ERP.SAP.Kunde;Hemit.BizTalk.ERP.SAP.Salg;Hemit.BizTalk.ERP.SAP.RESH;Hemit.BizTalk.ERP.SAP.Utstyr;Hemit.BizTalk.ESB.HarBorgerFrikort;Hemit.BizTalk.Felles.ApplikasjonskvitteringRev2;Hemit.BizTalk.PASEvent.HarBorgerFrikort;Hemit.BizTalk.Felles.Meldingsmottak;Hemit.BizTalk.Felles.DigitalPost;Hemit.BizTalk.Felles.PASEvent.Publisher;Hemit.BizTalk.Felles.PASEvent.BKM;Hemit.BizTalk.Felles.PASEvent.RUS;Hemit.BizTalk.Felles.PASEvent.TR;Hemit.BizTalk.Felles.PASEvent.AL;Hemit.BizTalk.Felles.PASEvent.KR;Hemit.BizTalk.Felles.PASEvent.MO;Hemit.BizTalk.Felles.PASEvent.NA;Hemit.BizTalk.Felles.PASEvent.VO;Hemit.BizTalk.Felles.PASEvent.LE;Hemit.BizTalk.Felles.PasProxy;Hemit.BizTalk.Felles.Toolbox;Hemit.BizTalk.Kjernejournal;Hemit.BizTalk.KrypterOgSend;Hemit.BizTalk.KrypterOgSendBKM;Hemit.BizTalk.Lab.Mbio;Hemit.BizTalk.Lab.KKL;Hemit.BizTalk.Natus;Hemit.BizTalk.NHN.FellesMottak;Hemit.BizTalk.NPR.Rapportering;Hemit.BizTalk.PASEvent.Forum;Hemit.Pas.Kristiansund;Hemit.Pas.Levanger;Hemit.Pas.Molde;Hemit.Pas.Namsos;Hemit.Pas.Orkanger;Hemit.Pas.Roros;Hemit.Pas.StOlav;Hemit.Pas.Volda;Hemit.Pas.Alesund;Hemit.BizTalk.Rtg.Rekvisisjon;Hemit.BizTalk.Rtg.Svar.Intern;Hemit.BizTalk.ERP.SAP.Import;Hemit.BizTalk.Brreg.ws;Hemit.BizTalk.ERP.Hovedboksgrunnlag;Hemit.BizTalk.ERP.Ordregrunnlag;"; }
                }
                List<string> ret = new List<string>(apps.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                return ret;
            }
        }

        public static DataTable SettingsTable
        {
            get
            {
                DataTable ret = null;

                FileInfo info = new FileInfo(Path.Combine(Constants.ToolsPath.FullName, @"BizDploy\BizDploy.exe.config"));
                DataSet ds = new DataSet();

                try
                {
                    ds = new DataSet();
                    ds.ReadXml(info.FullName);

                    ret = ds.Tables["setting"];
                }
                catch (Exception)
                {
                    ret = GetHardcodedSettings();
                }
                return ret;
            }
        }

        public static Dictionary<string, string> SettingsDic
        {
            get
            {
                DataTable table = new DataView(SettingsTable, "Name <> '' AND Value <>'' ", "", DataViewRowState.CurrentRows).ToTable(true, "Name", "Value");
                return table.AsEnumerable().ToDictionary<DataRow, string, string>(row => row.Field<string>("Name"), row => row.Field<string>("Value"));
            }
        }
        public static string GetConnectionString(string machineName)
        {
            var dic = SettingsDic;
            if (dic.ContainsKey("Connectionstring_" + machineName.ToLower()))
                return dic["Connectionstring_" + machineName.ToLower()];
            else return string.Empty;
        }

        public static string GetToolBoxConnectionString(string key)
        {
            string ret = string.Empty;
            try
            {
                var dic = GetToolBoxConnectionStrings();
                ret = $"{new DataView(dic.ToDataTable(), $"Key like '%{key}%'", "", DataViewRowState.CurrentRows).ToTable().AsEnumerable().FirstOrDefault()["Value"]}";
            }
            catch (Exception)
            { }
            return ret;
        }

        public static Dictionary<string, string> GetToolBoxConnectionStrings()
        {throw
            new NotImplementedException();

            //Dictionary<string, string> ret = null;
            //try
            //{
            //    var test = null;
            //    ret = new DataView(test.ToDataTable(), "Key like '%connectionstring%'", "", DataViewRowState.CurrentRows).ToTable().AsEnumerable()
            //         .ToDictionary<DataRow, string, string>(row => row[0].ToString(),
            //                                           row => row[1].ToString());
            //}
            //catch (Exception)
            //{ }

            //return ret;
        }


        public static bool CheckUser(WindowsPrincipal user)
        {
            try
            {
                if (user.IsInRole("G_App_BizTalk_Server_Administrators") || user.IsInRole(_bizTalkAdGroup))
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                //    MessageBox.Show(ex.Message);
                //  EventLog.WriteEntry("BizDploy", ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
                return false;
            }
        }

        public static BizTalkEnvironment GetEnvironment()
        {
            var dic = SettingsDic;

            try
            {
                if (dic["Servers_" + BizTalkEnvironment.Prod].Contains(Environment.MachineName))
                {
                    _bizTalkAdGroup = "G_App_BizTalk_Server_Administrators";
                    return BizTalkEnvironment.Prod;
                }
                if (dic["Servers_" + BizTalkEnvironment.ProdTest].Contains(Environment.MachineName))
                {
                    _bizTalkAdGroup = "G_App_BizTalk_Server_Administrators_Prodtest";
                    return BizTalkEnvironment.ProdTest;
                }
                if (dic["Servers_" + BizTalkEnvironment.Test].Contains(Environment.MachineName))
                {
                    _bizTalkAdGroup = "G_App_BizTalk_Server_Administrators_Test";
                    return BizTalkEnvironment.Test;
                }
                if (dic["Servers_" + BizTalkEnvironment.Devl].Contains(Environment.MachineName))
                {
                    _bizTalkAdGroup = "G_App_BizTalk_Server_Administrators_Utvikling";
                    return BizTalkEnvironment.Devl;
                }
                _bizTalkAdGroup = "Domain Users";
                return BizTalkEnvironment.LocalDevl;

            }
            catch (Exception ex)
            {
                //   MessageBox.Show(ex.Message);
                //   EventLog.WriteEntry("BizDploy", ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
                return BizTalkEnvironment.LocalDevl;
            }
        }
         
        private static DataTable GetHardcodedSettings()
        {
            #region settings
            string m_settingsXML = @"<settings>
<setting><name>AdminGroupName_LocalDevl</name> <value>helsemn\btsadmutv</value></setting>
<setting><name>AdminGroupName_Prod</name> <value>HELSEMN\L_App_BizTalk_SSO_Administrators</value></setting>
<setting><name>AdminGroupName_ProdTest</name> <value>HELSEMN\L_App_BizTalk_SSO_Administrators_Prodtest</value></setting>
<setting><name>AdminGroupName_Test</name> <value>HELSEMN\L_App_BizTalk_SSO_Administrators_Test</value></setting>
<setting><name>Connectionstring_Devl</name> <value>Data Source=trdbbiztalkt01;Initial Catalog=utvbizt01_BizTalkMgmtDb;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False</value></setting>
<setting><name>Connectionstring_Prod</name> <value>Data Source=sql2k12habizp01\sql2k12habizp01;Initial Catalog=BizTalkMgmtDb;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False</value></setting>
<setting><name>Connectionstring_ProdTest</name> <value>Data Source=sql2k12k3pt01\sql2k12k3pt01;Initial Catalog=BizTalkMgmtDb;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False</value></setting>
<setting><name>Connectionstring_Test</name> <value>Data Source=SQL2K12K3T01\SQL2K12K3T01;Initial Catalog=BizTalkMgmtDb;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False</value></setting>
<setting><name>Connectionstring_nmbizutvp01</name> <value>Data Source=nmbizutvdbp01;Initial Catalog=jogr_BizTalkMgmtDb;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False</value></setting>
<setting><name>Connectionstring_nmbizutvp02</name> <value>Data Source=nmbizutvdbp01;Initial Catalog=nmbizutvp02_BizTalkMgmtDb;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False</value></setting>
<setting><name>Connectionstring_nmbizutvp03</name> <value>Data Source=nmbizutvdbp01;Initial Catalog=nmbizutvp03_BizTalkMgmtDb;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False</value></setting>
<setting><name>Connectionstring_nmbizutvp04</name> <value>Data Source=nmbizutvdbp01;Initial Catalog=nmbizutvp04_BizTalkMgmtDb;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False</value></setting>
<setting><name>Connectionstring_nmbizutvp05</name> <value>Data Source=nmbizutvdbp01;Initial Catalog=nmbizutvp05_BizTalkMgmtDb;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False</value></setting>
<setting><name>Connectionstring_nmbizutvp06</name> <value>Data Source=nmbizutvdbp01;Initial Catalog=nmbizutvp06_BizTalkMgmtDb;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False</value></setting>
<setting><name>Connectionstring_nmbizutvp07</name> <value>Data Source=nmbizutvdbp01;Initial Catalog=nmbizutvp07_BizTalkMgmtDb;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False</value></setting>
<setting><name>Connectionstring_nmbizutvp08</name> <value>Data Source=nmbizutvdbp01;Initial Catalog=nmbizutvp08_BizTalkMgmtDb;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False</value></setting>
<setting><name>Connectionstring_nmbizutvp09</name> <value>Data Source=nmbizutvdbp01;Initial Catalog=nmbizutvp09_BizTalkMgmtDb;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False</value></setting>
<setting><name>Connectionstring_nmutvbiztalkp01</name> <value>Data Source=trdbbiztalkt01;Initial Catalog=erithu_BizTalkMgmtDb;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False</value></setting>
<setting><name>Connectionstring_trutvbizannaar</name> <value>Data Source=trdbbiztalkt01;Initial Catalog=annaar_BizTalkMgmtDb;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False</value></setting>
<setting><name>Connectionstring_trutvbizjasvend</name> <value>Data Source=trdbbiztalkt01;Initial Catalog=jasvend_BizTalkMgmtDb;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False</value></setting>
<setting><name>Connectionstring_trutvbizjogr</name> <value>Data Source=trdbbiztalkt01;Initial Catalog=jogr_BizTalkMgmtDb;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False</value></setting>
<setting><name>Connectionstring_trutvbizsurkat</name> <value>Data Source=trdbbiztalkt01;Initial Catalog=surkat_BizTalkMgmtDb;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False</value></setting>
<setting><name>Connectionstring_trutvbiztorvei</name> <value>Data Source=trdbbiztalkt01;Initial Catalog=torvei_BizTalkMgmtDb;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False</value></setting>
<setting><name>Path_Packages</name> <value>\\hemitbibl\Applikasjoner\Biztalk\Forvaltning\Hemit_Leveranser2013\Releaser\BizDploy</value></setting>
<setting><name>Path_Releases</name> <value>\\hemitbibl\Applikasjoner\Biztalk\Forvaltning\Hemit_Leveranser2013</value></setting>
<setting><name>ReleaseFolderFilter</name> <value>Denne verdien er ikke i bruk enda.</value></setting>
<setting><name>Servers_Devl</name> <value>TRAPPUTVBIZT01</value></setting>
<setting><name>Servers_Prod</name> <value>NMBIZ2013P01;FSBIZ2013P01</value></setting>
<setting><name>Servers_ProdTest</name> <value>NMBIZ2013PT01;FSBIZ2013PT01</value></setting>
<setting><name>Servers_Test</name> <value>AHAPPBIZT01;AHAPPBIZT02</value></setting>
<setting><name>Setting</name> <value /></setting>
<setting><name>UpdateNotificationFile</name> <value>\\hemitbibl\Applikasjoner\Biztalk\Forvaltning\Hemit_Leveranser2013\Tools\BizDploy\</value></setting>
<setting><name>UserGroupName_LocalDevl</name> <value>helsemn\btsadmutv</value></setting>
<setting><name>UserGroupName_Prod</name> <value>HELSEMN\L_App_BizTalk_SSO_Affiliate_Administrators</value></setting>
<setting><name>UserGroupName_ProdTest</name> <value>HELSEMN\L_App_BizTalk_SSO_Affiliate_Administrators_Prodtest</value></setting>
<setting><name>UserGroupName_Test</name> <value>HELSEMN\L_App_BizTalk_SSO_Affiliate_Administrators_Test</value></setting>
</settings>";
            #endregion

            DataSet ds = new DataSet();
            using (MemoryStream str = new MemoryStream(Encoding.UTF8.GetBytes(m_settingsXML)))
            {
                ds.ReadXml(str);
            }
            return ds.Tables["setting"];
        }



        /* Fra MaintLogDB */

        public enum HospitalId
        {
            /// <summary>
            /// Trondheim
            /// </summary>
            TR,
            /// <summary>
            /// Orkdal
            /// </summary>
            OR,

            RO,
            /// <summary>
            /// Namsos
            /// </summary>
            NA,
            /// <summary>
            /// Levanger
            /// </summary>
            LE,
            /// <summary>
            /// Kristiansund
            /// </summary>
            KR,
            /// <summary>
            /// Molde
            /// </summary>
            MO,
            /// <summary>
            /// Ålesund
            /// </summary>
            AL,
            //Volda
            VO
        }


        public static Constants.HospitalId CSLSectionToHospitalId(string sCSLSection)
        {
            Constants.HospitalId sPAS = (Constants.HospitalId)Enum.Parse(typeof(Constants.HospitalId), sCSLSection.Substring(0, 2));
            return sPAS;
        }
    }
}