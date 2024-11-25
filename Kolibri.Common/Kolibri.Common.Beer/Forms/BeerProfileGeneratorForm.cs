using Kolibri.Common.Utilities.Extensions;
using Kolibri.XMLValidator.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Kolibri.Common.Beer.Forms
{
    public partial class BeerProfileGeneratorForm : Form
    {
        private Dictionary<string, string> storyDic;
        private Dictionary<string, string> dateDic;
        private List<Profile> _profiles;
        private XDocument _wordPressXDoc;
        public BeerProfileGeneratorForm(XDocument wordpressLinksXDoc)
        {
            InitializeComponent();
            _wordPressXDoc = wordpressLinksXDoc;
            Init();
        }
        public BeerProfileGeneratorForm(FileInfo wordpressLinks)
        {
            InitializeComponent();
            _wordPressXDoc = XDocument.Load(wordpressLinks.FullName);
            Init();
        }

        private void Init()
        {
            storyDic = new Dictionary<string, string>();
            var template = JsonConvert.DeserializeObject<Profile>(File.ReadAllText(@"C:\Users\asoes\source\repos\Kolibri.SortPics\Kolibri.SortPics\Profile_Template.dat"));
            _profiles = new List<Profile>();
            try
            {
                foreach (XElement element in _wordPressXDoc.Root.GetElement("channel").GetElements("link").Reverse())
                {
                    //kun bryggelogg!
                    if (element.Parent.GetElement("category").Attribute("nicename").Value != "bryggelogg") continue;
                    Profile profile = template.CloneJson<Profile>();
                    profile.activeProfileSession.name = element.Parent.GetElement("title").Value;
                    profile.url = element.Value;
                    profile.gravity = GetGravityFromContent(element.Parent.ToString());
                    profile.createdOn = element.Parent.GetElementValue("pubDate").ToDateTime(); ;
                    profile.activeProfileSession.startDate = element.Parent.GetElementValue("pubDate").ToDateTime();
                    profile.activeProfileSession.createdOn = profile.activeProfileSession.startDate;

                    var teleStart = profile.telemetry[0].CloneJson();
                    teleStart.gravity = profile.gravity;
                    teleStart.createdOn = profile.createdOn;

                    Random rnd = new Random();
                    var teleStop = profile.telemetry[profile.telemetry.Count - 1].CloneJson();
                            teleStop.gravity = GetGravityFromContent(element.Parent.ToString(), 1004.2, "FG"); ;
                    teleStop.createdOn = profile.createdOn.AddDays(rnd.Next(10, 20));
                    profile.telemetry = new List<Telemetry>
                    { teleStart, teleStop };
                    _profiles.Add(profile);
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message, ex.GetType().Name);
            }
            if (_profiles.Count > 0)
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.RootFolder = Environment.SpecialFolder.MyComputer;
                fbd.Description = $"Velg mappe for {_profiles.Count} profiler"; 
                fbd.SelectedPath = @"D:\Temp";              
                
                    //  SendKeys.SendWait("{TAB}{TAB}{DOWN}{UP}");
                SendKeys.SendWait("{TAB}{TAB}{RIGHT}");
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    var dirInfo = new DirectoryInfo(fbd.SelectedPath);

                    foreach (Profile profile in _profiles)
                    {
                        try
                        {
                            FileInfo profileFile = new FileInfo(Path.Combine(dirInfo.FullName, $"Profile_{profile.activeProfileSession.name}.dat"));
                            File.WriteAllText(profileFile.FullName, Newtonsoft.Json.JsonConvert.SerializeObject(profile));
                        }
                        catch (Exception ex) { }
                    }
                    this.DialogResult = DialogResult.OK;
                }
                else this.DialogResult = DialogResult.Cancel;
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        private double GetGravityFromContent(string content, double defaultValue = 0, string sgType="OG")
        {
            double ret = defaultValue;
            if (sgType == "OG" && defaultValue == 0) ret = 1044.2;
            if ((sgType == "FG"||sgType=="SG") && defaultValue == 0) ret = 1004.2;

            if(string.IsNullOrEmpty(content)) { return ret; }
            try
            {
                if (content.Contains("Svarte katta i svarte natta - sider")) { }

                if (content.IndexOf(sgType, StringComparison.OrdinalIgnoreCase )>=1)
                {
                        List<string> list = content.Split(' ').ToList();
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].Trim().ToUpper().Contains(sgType)) 
                        {string test = list[i];
                            string gravity = list[i+1].Trim();
                                    Regex reg = new Regex(@"\d+");
                            MatchCollection matchedAuthors = reg.Matches(gravity);
                            if (matchedAuthors.Count > 0)
                            {
                                double tall;
                                if (Double.TryParse(matchedAuthors[0].Value, out tall))
                                { ret = tall; break; }
                            }
                        }
                    }
                     
                }
            }
            catch (Exception)
            {
            }
            return ret;
        }
    }
}