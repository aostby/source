using Kolibri.Common.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
 using Kolibri.Common.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace Kolibri.SortPictures.Forms
{
 
        public partial class SchrinkImagesForm : MALSourceDestinationForm
        {
            Panel _customPanel;
            Panel _panelButtons;
            string[] _filefilter;
            public SchrinkImagesForm()
            {
                InitializeComponent();
                Init();
            }

            private void Init()
            {
                List<string> tall = (new int[] { 100, 90, 80, 70, 60, 50, 40, 30, 20, 10 }).Select(i => i.ToString()).ToList();
                ComboBox percent = new ComboBox() { Name = "comboBoxPercent" };
                percent.Items.AddRange(tall.ToArray());
                percent.SelectedIndex = 3;
                FlowLayoutPanel flp = new FlowLayoutPanel() { Width = 400, FlowDirection = FlowDirection.LeftToRight };
                flp.Controls.Add(new Label() { AutoSize = true, Text = "Kompressjonsprosent: " });
                flp.Controls.Add(percent);

                _customPanel = (Panel)this.Controls.Find("panelCustom", false).FirstOrDefault();
                _customPanel.Controls.Add(flp);

                Button button = new Button() { Text = "Lag HTML" };
                button.Click += new EventHandler(htmlbutton_Clicked);
                _panelButtons = (Panel)this.Controls.Find("flowLayoutPanelButtons", true).FirstOrDefault();
                _panelButtons.Controls.Add(button);

                _filefilter = FileUtilities.PictureFileExt().ToArray();

            }

            private void htmlbutton_Clicked(object sender, EventArgs e)
            {
                StringBuilder builder = new StringBuilder();


                DirectoryInfo dest = new DirectoryInfo(this.Controls.Find("textBoxDestination", false).FirstOrDefault().Text);
                FileInfo destination = new FileInfo(Path.Combine(dest.FullName, "allPics.html"));

                foreach (var item in FileUtilities.HentFiler(dest, _filefilter))
                {
                    Image img = Image.FromFile(item.FullName);
                    var size = ImageUtilities.AspectSize(img);


                    builder.Append($@"<img src=""{item.Name}""  height=""{250}"" alt=""{item.Name}"" title=""{item.Name}""/>");

                    // builder.Append($@"<img src=""{item.Name}""  height=""{250}"" alt=""{item.Name}"" title=""{item.Name}""onmouseover=""this.width = '450'; "" onmouseout= ""this.width = '250' ""        />");
                    // builder.Append($@"<img src=""{item.Name}""  height=""{(int) img.Height/2}""       />");
                    // builder.Append($@"<img src=""{item.Name}""  height=""{(int)Size.Height / 2}"" onmouseover=""this.width = '{Size.Width}'; "" onmouseout= ""this.height = '{(int)Size.Height / 2}' "" />");
                    // builder.Append($@"<img src=""{item.Name}""  height=""{(int)size.Height / 2}""   onmouseover=""this.width='{ Size.Width }'; this.height='{Size.Height}'"" onmouseout=""this.width='{(int)size.Width / 0.5}'; this.height='{(int)size.Height / 0.5}'"" />") ;
                }

                FileUtilities.WriteStringToFile(builder, destination.FullName);
                FileUtilities.Start(destination.FullName);
            }

            private void SchrinkImagesForm_Load(object sender, EventArgs e)
            {
                try
                {
                    var controls = this.Controls.Find("panelCustom", false);
                    Panel panel = (Panel)controls.FirstOrDefault();


                }
                catch (Exception)
                {
                }
            }

            public override void buttonExecute_Click(object sender, EventArgs e)
            {
                try
                {

                    DirectoryInfo source = new DirectoryInfo(this.Controls.Find("textBoxSource", false).FirstOrDefault().Text);
                    DirectoryInfo dest = new DirectoryInfo(this.Controls.Find("textBoxDestination", false).FirstOrDefault().Text);
                    if (!dest.Exists) dest.Create();
                    var collection = Kolibri.Common.Utilities.FileUtilities.HentFiler(source, _filefilter);
                    foreach (var item in collection)
                    {
                        Image img = Image.FromFile(item.FullName);
                        int percent = Int32.Parse(this.Controls.Find("comboBoxPercent", true).FirstOrDefault().Text);
                        FileInfo destination = new FileInfo(Path.Combine(dest.FullName, item.Name));

                        Kolibri.Common.Utilities.ImageUtilities.ReduceJpegSize(destination.FullName, img, percent);

                    }
                }
                catch (Exception ex)
                { string msg = ex.Message; SetLabelText(ex.Message); }
            }
        }
    }
