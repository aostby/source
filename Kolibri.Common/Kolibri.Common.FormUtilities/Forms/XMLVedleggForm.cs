
using Kolibri.Common.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Kolibri.Common.FormUtilities.Forms
{
    public partial class XMLVedleggForm : Form
    {
        private bool m_CloseForm = false;
        private XmlDocument m_xmlDoc;

        private List<string> m_imageMimeTypes = new List<string>() { 
                    "image/gif",
                    "image/jpeg",
                    "image/png",
                    "application/x-shockwave-flash",
                    "image/psd",
                    "image/bmp",
                    "image/tiff",
                    "image/jp2",
                    "application/octet-stream",
                    "application/x-shockwave-flash",
                    "image/iff",
                    "image/vnd.wap.wbmp",
                    "image/xbm",
                    "image/vnd.microsoft.icon"
                    };

        public bool CloseForm { get { return m_CloseForm; } }
        public XMLVedleggForm(XmlDocument doc)
        {
            InitializeComponent();
            m_xmlDoc = doc;
            Init();
        }

        private void Init()
        {
            XDocument xDoc = XDocument.Load(new XmlNodeReader(m_xmlDoc));
            this.Text = xDoc.Root.Name.LocalName;
            XElement refDoc = GetElement(xDoc.Root, "RefDoc");
            if (refDoc != null) { VedleggKITH(refDoc); return; }

            refDoc = GetElement(xDoc.Root, "EmbeddedDocumentBinaryObject");
            if (refDoc != null) { VedleggEHF(refDoc); return; }

            refDoc = GetElement(xDoc.Root, "TiffImageFirstPage");
            if (refDoc != null) { VedleggGenericShow(refDoc); return; }
            refDoc = GetElement(xDoc.Root, "DocumentData");
            if (refDoc != null) {
                var type = GetElement(xDoc.Root, "FileType");
                if(type==null)
                    type = GetElement(GetElement(xDoc.Root, "MessagingType"), "Text");
                switch (type.Value)
                {
                    case "Brev":type.Value = "pdf";break;
                    case "SMS": type.Value = "txt"; break;
                    default:
                        break;
                }

                ShowVedlegg(refDoc, type.Value); return; }


            if (refDoc == null)
                throw new Exception("Ingen vedlegg funnet for dokument Root: " + this.Text);
        }

        private void VedleggGenericShow(XElement refDoc)
        {
            if (refDoc != null)
            {
                panel1.Controls.Clear();
                ShowPictureBox(refDoc.Value);
            }
        }
        private void ShowVedlegg(XElement refDoc, string mimetype)
        {
            byte[] sPDFDecoded = Convert.FromBase64String(refDoc.Value);

            var ext = MimeTypes.FindExtension(mimetype);
            if (ext == null)
                ext = mimetype;// Utilities.MimeTypes.Get(mimetype);
            FileInfo info = FileUtilities.GetTempFile(ext);
            File.WriteAllBytes(info.FullName, sPDFDecoded);

            var uri = new System.Uri(info.FullName);

            if (info.Extension.EndsWith("pdf"))
            {
                string embedded = string.Format(@"<html>
                        <body>
                            <object data=""{0}"" type=""application/pdf"">
                                <embed src=""{0}"" type=""application/pdf"" />
                            </object>
                        </body>
                        </html>", info.FullName);
                string htmlPath = info.FullName + ".html";

                File.WriteAllText(htmlPath, embedded);
                uri = new System.Uri(htmlPath);

                Process.Start(info.FullName);m_CloseForm = true;
                return;
            }

            panel1.Controls.Clear();

            WebBrowser box = new WebBrowser();
            box.Navigate(info.FullName);
            box.Dock = DockStyle.Fill;
            panel1.Controls.Add(box);

            //}
        }
        private void VedleggEHF(XElement refDoc)
        {
            byte[] sPDFDecoded = Convert.FromBase64String(refDoc.Value);

            var jall = refDoc.Attribute("mimeCode").Value;

            var ext = MimeTypes.FindExtension(jall);

            FileInfo info = FileUtilities.GetTempFile(ext);
            File.WriteAllBytes(info.FullName, sPDFDecoded);
            //    Process.Start(info.FullName);

            panel1.Controls.Clear();

            WebBrowser box = new WebBrowser();
            box.Navigate(info.FullName);
            box.Dock = DockStyle.Fill;
            panel1.Controls.Add(box);


        }
       private void VedleggKITH(XElement refDoc)
        {
            if (refDoc != null)
            {
                panel1.Controls.Clear();
                string mimeType = GetElement(refDoc, "MimeType").Value;
                if (m_imageMimeTypes.Contains(mimeType.ToLower()))
                {
                    ShowPictureBox(GetElement(refDoc, "Base64Container").Value);
                }
                else
                {//Eh, hva er planen her mon tro? - fiks om det blir nødvendig
                    FileInfo temp = FileUtilities.GetTempFile(MimeTypes.FindExtension(mimeType));
                    Process.Start(temp.FullName);
                }
            }
        } 

        private XElement GetElement(XElement startNode, string localName)
        {
            try
            {
                return startNode.Descendants().FirstOrDefault(p => p.Name.LocalName == localName);
            }
            catch (Exception)
            {
                return null;
            }

        }
        private void ShowPictureBox(string base64Value)
        {
            Image img = ImageUtilities.Base64ToImage(base64Value);
            PictureBox box = new PictureBox();
            box.Image = img;
            box.Dock = DockStyle.Fill;
            box.SizeMode = PictureBoxSizeMode.StretchImage;
            panel1.Controls.Add(box);
        }
     
    }
}
