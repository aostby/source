using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.Validation;
using Kolibri.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Kolibri.Common.FormUtilities.Forms
{
    public partial class TransformFHIRForm : Form
    {
        private ProfileValidator _profileValidator;
        private DirectoryInfo m_dirStructureDefinitions, m_dirExampleFiles;

        public TransformFHIRForm()
        {
            InitializeComponent();

            m_dirExampleFiles = new DirectoryInfo(@"\\hemitbrukere\felleshemit\Anvendelse\Biztalk\Testfiler\DIS\ExampleFiles");

            m_dirStructureDefinitions = new DirectoryInfo(@"\\hemitbrukere\felleshemit\Anvendelse\Biztalk\Testfiler\DIS\StructureDefinitions");
            InitPath(m_dirStructureDefinitions);
        }
        private void InitPath(DirectoryInfo path)
        {
            List<DirectoryInfo> infos = path.GetDirectories().ToList();
            if (infos.Count == 0)
                infos.Add(path);
            comboBox1.Items.AddRange(infos.ToArray());
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "FullPath";
            comboBox1.SelectedIndex = 0;

            linkLabelPath.Text = path.Name;
            linkLabelPath.Tag = path.FullName;
            m_dirStructureDefinitions = path;
        }

        private void SetPath(DirectoryInfo path)
        {
            //var assembly = Assembly.GetExecutingAssembly();
            //var location = new Uri(assembly.GetName().CodeBase);
            //var directoryInfo = new FileInfo(location.AbsolutePath).Directory;
            //var herre = Directory.GetCurrentDirectory(); 

            var jall = new DirectorySource(path.FullName, includeSubdirectories: true);

            var cachedResolver = new CachedResolver(jall);
            var naaa = cachedResolver.Source;

            var coreSource = new CachedResolver(ZipSource.CreateValidationSource());
            var combinedSource = new MultiResolver(cachedResolver, coreSource);
            var settings = new ValidationSettings
            {
                EnableXsdValidation = true,
                GenerateSnapshot = true,
                Trace = true,
                ResourceResolver = combinedSource,
                ResolveExteralReferences = true,
                SkipConstraintValidation = false
            };
            var validator = new Validator(settings);
            _profileValidator = new ProfileValidator(validator);


            ReadFromResx();
        }

        private void SaveToResx()
        {
            using (ResXResourceWriter resx = new ResXResourceWriter(@".\Config.resx"))
            {
                resx.AddResource("InputFileLocation", textBox1.Text);
            }
        }
        private void ReadFromResx()
        {
            /*   using (ResXResourceReader resx = new ResXResourceReader(@".\Config.resx"))
                   foreach (DictionaryEntry d in resx)
                       if (d.Key.Equals("InputFileLocation"))
                           textBox1.Text = (string)d.Value;*/



            FileInfo[] infors = m_dirExampleFiles.GetFiles("*.xml", SearchOption.TopDirectoryOnly);

            if (infors.Length == 0) throw new FileNotFoundException(m_dirExampleFiles.FullName);
            else textBox1.Text = infors.FirstOrDefault().FullName;

        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            label2.Text = DateTime.UtcNow.ToLocalTime().ToString("s");

            try
            {
                if (File.Exists(textBox1.Text))
                {
                    try
                    {

                        XDocument xdoc = XDocument.Load(textBox1.Text);
                        object fhirMessage;
                        switch (xdoc.Root.Name.LocalName.ToString())
                        {
                            case "Appointment":
                                fhirMessage = new FhirXmlParser().Parse<Appointment>(xdoc.ToString());
                                break;
                            case "ReferralRequest":
                                fhirMessage = new FhirXmlParser().Parse<ReferralRequest>(xdoc.ToString());
                                break;
                            case "DocumentReference":
                                fhirMessage = new FhirXmlParser().Parse<DocumentReference>(xdoc.ToString());
                                break;
                            case "Patient":
                                fhirMessage = new FhirXmlParser().Parse<Patient>(xdoc.ToString());
                                break;
                            case "Organization":
                                fhirMessage = new FhirXmlParser().Parse<Organization>(xdoc.ToString());
                                break;
                            default:
                                fhirMessage = new FhirXmlParser().Parse<Bundle>(xdoc.ToString());
                                break;
                        }

                        var validResource = _profileValidator.Validate(xdoc.CreateReader(), true);
                        rtfResult.Text = (FhirSerializer.SerializeToXml(validResource));
                    }
                    catch (System.Exception exp)
                    {
                        string errorMessage = exp.Message;
                        if (exp.InnerException != null)
                            errorMessage += Environment.NewLine + exp.InnerException;
                        if (exp.StackTrace != null)
                            errorMessage += Environment.NewLine + Environment.NewLine + exp.StackTrace;
                        rtfResult.Text = errorMessage;
                    }
                }
                if (File.Exists(textBox1.Text))
                {
                    SaveToResx();

                    XDocument outcome = XDocument.Parse(rtfResult.Text);
                    FileInfo info = FileUtilities.GetTempFile();
                    outcome.Save(info.FullName);

                     FormUtilities.Controller.TransFormController.ShowFiles(info, Controller.TransFormController.GetXSLTPath("GenericHTML01_attributes"));
                }
                else
                    rtfResult.Text = "No file in location";
            }
            catch (Exception ex)
            {
                rtfResult.Text = ex.Message;
            }

        }

        private void btnUpFile_Click(object sender, EventArgs e)
        {
            string directoryPath = Path.GetDirectoryName(textBox1.Text);
            if (Directory.Exists(directoryPath))
            {
                List<string> fileList = Directory.GetFiles(directoryPath).ToList<string>();
                fileList.Sort();
                if (fileList.Contains(textBox1.Text))
                {
                    int index = fileList.FindIndex(x => x.Equals(textBox1.Text));
                    if (index > 0)
                        index--;
                    textBox1.Text = fileList[index];
                }
                else
                    textBox1.Text = fileList[0];
            }
        }

        private void btnDownFile_Click(object sender, EventArgs e)
        {
            string directoryPath = Path.GetDirectoryName(textBox1.Text);
            if (Directory.Exists(directoryPath))
            {
                List<string> fileList = Directory.GetFiles(directoryPath).ToList<string>();
                fileList.Sort();
                if (fileList.Contains(textBox1.Text))
                {
                    int index = fileList.FindIndex(x => x.Equals(textBox1.Text));
                    if (index < (fileList.Count - 1))
                        index++;
                    textBox1.Text = fileList[index];
                }
                else
                    textBox1.Text = fileList[(fileList.Count - 1)];
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ComboBox box = sender as ComboBox;
                var dirinfo = box.SelectedItem as DirectoryInfo;
                SetPath(dirinfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                XDocument doc = XDocument.Load(textBox1.Text);

                linkLabelFHIRType.Text = string.Format(@"https://www.hl7.org/fhir/{0}.html", doc.Root.Name.LocalName);
            }
            catch (Exception)
            { }
        }

        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string text = string.Empty;
            try
            {
                var lab = (sender as LinkLabel);
                if (lab.Tag != null)
                { text = lab.Tag.ToString(); }
                else
                {
                    text = lab.Text;
                }
                Process.Start(text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (sender.Equals(buttonOpen))
            {
                if (Directory.Exists(m_dirStructureDefinitions.FullName))
                    dlg.SelectedPath = m_dirStructureDefinitions.FullName;
                else
                    dlg.SelectedPath = Path.GetDirectoryName(textBox1.Text);
            }
            else if (sender.Equals(buttonOpenFile))
            {
                dlg.SelectedPath = Path.GetDirectoryName(textBox1.Text);
            }

            SendKeys.Send("{TAB}{TAB}{RIGHT}");  // <<-- Workaround - SendKeys
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                if (sender.Equals(buttonOpen))
                {
                    InitPath(new DirectoryInfo(dlg.SelectedPath));
                }
                else if (sender.Equals(buttonOpenFile))
                {
                    m_dirExampleFiles = new DirectoryInfo(dlg.SelectedPath);
                    SetPath(m_dirExampleFiles);

                }
        }
    }

    public class ProfileValidator
    {
        private static Validator _validator;
        public ProfileValidator(Validator validator)
        {
            if (_validator == null)
            {
                _validator = validator;
            };
        }

        public OperationOutcome Validate(XmlReader reader, bool onlyErrors)
        {
            var result = _validator.Validate(reader);
            if (!onlyErrors)
            {
                return result;
            }
            var invalidItems = (from item in result.Issue
                                let error = item.Severity != null && item.Severity.Value == OperationOutcome.IssueSeverity.Error
                                where error
                                select item).ToList();

            result.Issue = invalidItems;
            return result;
        }
    }
}
