

using Kolibri.net.C64Sorter.Controllers;
using Kolibri.net.C64Sorter.Entities;
using Kolibri.net.Common.Utilities;
using LibVLCSharp.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using static sun.security.ec.ECDSAOperations;

namespace Kolibri.net.C64Sorter.Forms
{
    public partial class VideoStreamForm : Form
    {// Inside your Form class
        private LibVLC _libVLC;
        private MediaPlayer _mediaPlayer;
        private UE2LogOn _ue2Logon;
        private UltmateEliteClient _client = null;

        public VideoStreamForm()
        {
            InitializeComponent();
        }

        public VideoStreamForm(UE2LogOn ue2Logon)
        {
            InitializeComponent();
            this._ue2Logon = ue2Logon;
            _client = new UltmateEliteClient(ue2Logon.Hostname);
            Init();
        }
        public void SetStatusLabel(string statusText)
        {
            try { toolStripStatusLabelStatus.Text = statusText; }
            catch (Exception) { }
        }
        private async void Init()
        {
            string settings = string.Empty;
            try
            {
                settings = await _client.ConfigurationGetCurrentStreamSettings("video");
                if (!string.IsNullOrEmpty(settings))
                {
                    Image icon = new Icon(Path.Combine(UltmateEliteClient.ResourcesPath.FullName, $"{toolStripDropDownButtonToggleVideo.Tag.ToString()}.ico")).ToBitmap();
                    toolStripDropDownButtonToggleVideo.Width = icon.Width + 5;
                    toolStripDropDownButtonToggleVideo.Image = icon;
                    toolStripDropDownButtonToggleVideo.Tag = "Disabled";//This is a toggle, so we mean Enabled after calling the method. 
                    toolStripDropDownButtonToggleVideo_Click(toolStripDropDownButtonToggleVideo, null);

                }
                else
                {
                    toolStripDropDownButtonToggleVideo.Enabled = false;
                    throw new EntryPointNotFoundException("'Stream VIC to' cannot be empty. Please configure 'Data Streams' settings.");
                }

                try
                {



                    var json = Path.Combine(UltmateEliteClient.ResourcesPath.FullName, $"tools.json");
                    if (File.Exists(json))
                    {
                        dynamic data = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(json)); 
                        
                        foreach (var item in data)
                        {
                            FileInfo info = new FileInfo($"{item.FullPath}");
                            if (info.Exists && (info.Name.EndsWith("U64-Streamer.exe") || info.Name.EndsWith("Ultimate64Manager-Win.exe")))
                            {
                                buttonOpenStreamWindow.Tag = info;
                                buttonOpenStreamWindow.BackgroundImage = (Image)Icon.ExtractAssociatedIcon(info.FullName).ToBitmap();
                                if (info.Name.Equals("U64-Streamer.exe")) break;
                                labelOpenStreamWindow.Text = $"{item.Description}";
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    buttonOpenStreamWindow.Enabled = false;
                    SetStatusLabel($"Error initalizing - Check tools.json setting");
                }




            }
            catch (Exception ex)
            {
                SetStatusLabel($"Error {ex.GetType().Name} {ex.Message}");
            }
        }


        private async void toolStripDropDownButtonToggleVideo_Click(object sender, EventArgs e)
        {
            try
            {
                ToolStripDropDownButton btn = ((ToolStripDropDownButton)sender);
                var res = btn.Tag.ToString();
                var command = "start";

                switch (res)
                {
                    case "Enabled": btn.Tag = "Disabled"; command = "stop"; break;
                    case "Disabled": btn.Tag = "Enabled"; command = "start"; break;
                    default: btn.Tag = "Enabled"; break;
                }
                toolStripDropDownButtonToggleVideo.Image = new Icon(Path.Combine(UltmateEliteClient.ResourcesPath.FullName, $"{btn.Tag.ToString()}.ico")).ToBitmap();

                string type = "video";
                string conf = await _client.ConfigurationGetCurrentStreamSettings(type);

                var url = $"v1/streams/{type}:{command}?ip={conf}";
                if (command.Equals("stop"))
                {
                    url = $"v1/streams/{type}:{command}";
                }
                var success = await _client.PutUrl(url);
                String message = $"{type} {command}: {success}.";
                if (success)
                {
                    type = "audio";
                    conf = await _client.ConfigurationGetCurrentStreamSettings(type);
                    url = $"v1/streams/{type}:{command}?ip={conf}";
                    if (command.Equals("stop"))
                    {
                        url = $"v1/streams/{type}:{command}";
                    }
                    success = await _client.PutUrl(url);
                    message += $" {type} {command}: {success}.";
                }
                SetStatusLabel(message);

            }
            catch (HttpRequestException hex)
            {
                MessageBox.Show(hex.Message, hex.GetType().Name);
                SetStatusLabel($"Streams failed: {hex.Message}");
            }
            catch (AggregateException aex)
            {
                MessageBox.Show(aex.Message, aex.GetType().Name);
                SetStatusLabel($"Streams failed: {aex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
                SetStatusLabel($"Streams failed: {ex.Message}");
            }
        }

        private void buttonOpenStreamWindow_Click(object sender, EventArgs e)
        {
            try
            {
                FileUtilities.Start(((sender as Button).Tag as FileInfo).FullName);
            }
            catch (Exception ex)
            {
                SetStatusLabel($"Error {ex.GetType().Name} {ex.Message}");
            }
        }
    }
}