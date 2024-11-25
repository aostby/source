using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace Kolibri.Common.Utilities.Settings
{
    /// <summary>
    /// Class used by Startpanel and DataDigger to keep settings saved to file
    /// </summary>
    [Serializable()]
    public class AppSettings : ISettings
    {
        public AppSettings()
        {
            m_WMIusername = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\')[1];
        }

        [NonSerializedAttribute]
        private System.String m_dbconnectionstring;
        [NonSerializedAttribute]
        private System.String m_WMIusername;

        private System.String m_startupPath;
        private System.String m_fullfilsti;
        private DateTime m_dato;        
        private bool m_pollDb;
        private System.String m_parametere;
        private System.String m_dsn;
        private System.String m_uid;
        private System.String m_pwd;

        private System.Drawing.Size m_size;

        [DescriptionAttribute("Sti uten filnavn en vil applikasjonen skal starte fra.\n Alle undermapper listes, men kun de med .exe filer benyttes.")]
        public System.String StartupPath
        {
            get { return m_startupPath; }
            set { m_startupPath = value; }
        }
                [DescriptionAttribute("Sist brukte dsn (datasource name). \nLagres og benyttes til neste pålogg.")]
        [Category("ODBC")]
        public System.String Dsn
        {
            get { return m_dsn; }
            set { m_dsn = value; }
        }
        [DescriptionAttribute("Brukernavn. \nLagres og benyttes til neste pålogg.")]
        [Category("ODBC")]
        public System.String Uid
        {
            get { return m_uid; }
            set { m_uid = value; }
        }

        [Category("ODBC")]
        [DescriptionAttribute("Passord. \nLagres kryptert og benyttes til neste pålogg.")]
        public System.String Pwd
        {
            get { return m_pwd;}
            set { m_pwd = value; }
        }
        [DescriptionAttribute("Full sti til applikasjonen sist startet, inkludert filetternavn.")]
        [Category("Sist startet")]
        public System.String Fullfilsti
        {
            get { return m_fullfilsti; }
            set { m_fullfilsti = value; }
        }

        [DescriptionAttribute("Dato en sist startet en fil via denne applikasjonen.")]
        [Category("Sist startet")]
        public System.DateTime Dato
        {
            get { return m_dato; }
            set { m_dato = value; }
        }
        [DescriptionAttribute("Dersom du ønsker å starte applikasjonene med parametere, vil disse kunne lagres og benyttes om igjen. ")]
        [Category("Sist startet")]
        public System.Boolean PollDb
        {
            get { return m_pollDb; }
            set { m_pollDb = value; }
        }
        [DescriptionAttribute("Folder applikasjonen sist startet ligger i.")]
        [Category("Sist startet")]
        public System.String Parametere
        {
            get { return m_parametere; }
            set { m_parametere = value; }
        }

        [DescriptionAttribute("Databasetilkoblings streng. Genereres utfra DSN, UID og PWD")]
        [Category("ODBC")]
        public System.String DbConnectionString
        {
            get { return "DSN=" + Dsn + ";UID=" + Uid + ";PWD=" + Pwd + ";"; }
        }

        [CategoryAttribute("WindowSize"), DescriptionAttribute("Lagrer preferert størrelse av dette applikasjonsvinduet")]
        public System.Drawing.Size WindowSize
        {
            get { return m_size; }
            set { m_size = value; }
        }

        public bool Load()
        {
            string usersettings = Application.StartupPath + "\\settings\\" + m_WMIusername + "_settings.xml";
            XmlSerializer serializer = new XmlSerializer(typeof(AppSettings));
            AppSettings retVal = default(AppSettings);
            TextReader reader = default(TextReader);
            bool fileNotFound = false;

            try
            {
                reader = new StreamReader(usersettings);
            }
            catch (Exception ex)
            {
                // Take the defaults
                fileNotFound = true;
            }


            PropertyInfo[] allClassProperties = this.GetType().GetProperties();
            retVal = new AppSettings();

            if (fileNotFound)
            {
                WindowSize = new System.Drawing.Size(600, 600);
                foreach (PropertyInfo propertyInfo in allClassProperties)
                {
                    if (propertyInfo.PropertyType.Equals(typeof(Boolean)))
                        propertyInfo.SetValue(this, false, null);
                    else if (propertyInfo.Name.Equals("Dato"))
                        m_dato = DateTime.Now;
                    else if (propertyInfo.Name.Equals("DbConnectionString"))
                    { }
                    else if (propertyInfo.Name != "WindowSize")
                        propertyInfo.SetValue(this, "unknown", null);
                }
            }
            else
            {
                //Read it from the file
                retVal = (AppSettings)serializer.Deserialize(reader);

                foreach (PropertyInfo propertyInfo in allClassProperties)
                {if(!propertyInfo.Name.Equals("DbConnectionString"))
                    propertyInfo.SetValue(this, retVal.GetType().GetProperty(propertyInfo.Name).GetValue(retVal, null), null);
                }
                reader.Close();
              //TODO:  Pwd = CEncryption.Decrypt(Pwd);
            }
            return !fileNotFound;
        }
       
        public bool Save()
        {
            string usersettings = Application.StartupPath + "\\settings\\" + m_WMIusername + "_settings.xml";

            if (!Directory.Exists(usersettings))
                Directory.CreateDirectory(Directory.GetParent(usersettings).ToString());

            bool ret = false;
            try
            {
              //TODO:  Pwd = CEncryption.Encrypt(Pwd);
                XmlSerializer serializer = new XmlSerializer(typeof(AppSettings));
                TextWriter writer = new StreamWriter(usersettings);
                serializer.Serialize(writer, this);
                writer.Close();
            //TODO:    Pwd = CEncryption.Decrypt(Pwd);
                ret = true;
            }
            catch (Exception ex)
            {
            }
            return ret;
        }


    }
}


