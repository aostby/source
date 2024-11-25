using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace Kolibri.Common.Utilities.Settings
{
    [Serializable()]
    public class FTPSettings : ISettings
    {
        private enum ProtocolType { FTP, sFTP };

        public FTPSettings()
        {
            m_WMIusername = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\')[1];
        }

        [NonSerializedAttribute]
        internal System.String m_WMIusername;
      
        private System.String m_host;
        private System.String m_port;
        private System.String m_subfolder;

        private System.String m_uid;
        private System.String m_pwd;

        //private System.Drawing.Size m_size;
        private System.Boolean m_useVpn;
        private System.String m_vpnuid;
        private System.String m_vpnpwd;

        //The syntax of FTP URLs According to the specification of URL formats, RFC 1738, an FTP URL is of the form
        //ftp://user:password@host:port/path

        [Category("Setup")]
        [DescriptionAttribute("Host/IP filen skal sendes til")]
        public System.String Host
        {
            get { return m_host; }
            set { m_host = value; }
        }

        [Category("Setup")]
        [DescriptionAttribute("Port som skal benyttes (hvis annet enn default)")]
        public System.String Port
        {
            get { return m_port; }
            set { m_port = value; }
        }
        [Category("Setup")]
        [DescriptionAttribute("Mappestruktur å lete opp")]
        public System.String Subfolder
        {
            get { return m_subfolder; }
            set { m_subfolder = value; }
        }

        [DescriptionAttribute("Brukernavn for pålogg.")]
        [Category("Credentials")]
        public string Brukernavn
        {
            get { return m_uid; }
            set { m_uid = value; }
        }
        [DescriptionAttribute("Passord for pålogg.")]
        [Category("Credentials")]
        public System.String Passord
        {
            get { return m_pwd; }
            set { m_pwd = value; }
        }


        [CategoryAttribute("VPN"), DescriptionAttribute("Angående tunnellering av oppkobling, ikke benytt feltene om det ikke berører din bedrift")]
        public System.Boolean UseVpn
        {
            get { return m_useVpn; }
            set { m_useVpn = value; }
        }

        [DescriptionAttribute("Brukernavn for pålogg vpn.")]
        [Category("VPN")]
        public System.String VpnBrukernavn
        {
            get { return m_vpnuid; }
            set { m_vpnuid = value; }
        }

        [DescriptionAttribute("Passord for pålogg vpn.")]
        [Category("VPN")]
        public System.String VpnPassord
        {
            get { return m_vpnpwd; }
            set { m_vpnpwd = value; }
        }

        //[CategoryAttribute("WindowSize"), DescriptionAttribute("Lagrer preferert størrelse av dette applikasjonsvinduet")]
        //public System.Drawing.Size WindowSize
        //{
        //    get { return m_size; }
        //    set { m_size = value; }
        //}

        public bool Load()
        {
            FTPSettings retVal = default(FTPSettings);

            PropertyInfo[] allClassProperties = this.GetType().GetProperties();
            retVal = new FTPSettings();
             
            string usersettings = Path.GetTempPath() + m_WMIusername + "_" + this.GetType().Name + ".xml";


            if (!File.Exists(usersettings))
            {
                // WindowSize = new System.Drawing.Size(600, 600);
                foreach (PropertyInfo propertyInfo in allClassProperties)
                {
                    if (propertyInfo.PropertyType.Equals(typeof(Boolean)))
                        propertyInfo.SetValue(this, false, null);
                    else if (propertyInfo.PropertyType.Equals(typeof(String)))
                        propertyInfo.SetValue(this, "", null);
                    else if (propertyInfo.Name.Equals("DbConnectionString"))
                    { }
                    else if (propertyInfo.Name != "WindowSize")
                        propertyInfo.SetValue(this, "600;600", null);
                }
            }
            else
            {
                //Read it from the file
                string xml = FileUtilities.ReadTextFile(usersettings);
           //TODO:     xml = Utilities.Crypto.DecryptStringAES(xml, "16777216");
                retVal = XMLUtilities.SerializationHelper<FTPSettings>.DeserializeObject(xml);

                foreach (PropertyInfo propertyInfo in allClassProperties)
                {
                    if (!propertyInfo.Name.Equals("DbConnectionString"))
                        propertyInfo.SetValue(this, retVal.GetType().GetProperty(propertyInfo.Name).GetValue(retVal, null), null);
                }
            }
            return File.Exists(usersettings);
        }

        public bool Save()
        {
            string usersettings = Path.GetTempPath() + m_WMIusername + "_" + this.GetType().Name + ".xml";

            bool ret = false;
            try
            {
                string xml = XMLUtilities.SerializationHelper<FTPSettings>.SerializeObject(this);
               //TODO:  xml = Utilities.Crypto.EncryptStringAES(xml, "16777216");
                FileUtilities.WriteStringToFile(xml, usersettings);
                ret = true;
            }
            catch (Exception ex)
            {
            }
            return ret;
        }
    }
}


