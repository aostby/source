using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Kolibri.Data.Connection;

namespace Kolibri.Common.Utilities
{
    /// <summary>
    /// Klasse for å kunne gi tilbakemelding på oppkobling for Tidregistrering
    /// </summary>
    public class SelfTest
    {
        public static string OdbcSelfTest(string applikasjon,  DbConnection dbConnection)
        {
            string ret = string.Empty;
            string visTekst = string.Empty;
            visTekst =
            "Oversikt over oppkobling for " + applikasjon + ": \r\n" +
            "Programmet kjører fra:\t " + System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + Environment.NewLine +
            "DbConnectionType:\t" + dbConnection.DbType.ToString() + "\r\n" +
            "DbConnectionString:\t " + dbConnection.DbConnectionString.Replace(dbConnection.DbPassword, "") + "\r\n" +
            "DbType:\t" + dbConnection.DbType.ToString() + "\r\n" + Environment.NewLine +
            "DbDataSource finnes i systemet: " + SjekkODBC(dbConnection.DbConnectionString);
            ret = visTekst;
            return ret;
        }

        private static bool SjekkODBC(string odbcconnectionstring)
        {

            bool ret = false;
            try
            {
                string odbcnavn = string.Empty;
                string[] connectionarray = odbcnavn.ToUpper().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string item in connectionarray)
                {
                    if (item.StartsWith("DSN", StringComparison.OrdinalIgnoreCase))
                    {
                        odbcnavn = item.Substring(item.IndexOf("=") + 1);

                    }
                }

                ODBCDSN odbc = ODBCManager.GetSystemDSN(odbcnavn);
                foreach (ODBCDSN item in ODBCManager.GetSystemDSNList())
                {
                    if (odbc != null && item != null && odbc.GetDSNName().Equals(item.GetDSNName()))
                    {
                        ret = true;
                        break;
                    }
                }
            }
            catch (Exception)
            {
                ret = false;
            }

            return ret;
        }

        public static string GetEnvironmentDetails()
        {
            return ObjektUtilities.GenerellBeskrivelse(new SystemInfo());

        }
    }
}
