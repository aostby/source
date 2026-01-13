

using Kolibri.net.C64Sorter.Controllers;
using Kolibri.net.C64Sorter.Entities;
using LibVLCSharp.Shared;
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

        private async void Init()
        { string settings = string.Empty;
            try
            {
                  settings = await _client.ConfigurationGetCurrentStreamSettings();
                if (!string.IsNullOrEmpty(settings))
                {
                    Image icon = new Icon(Path.Combine(UltmateEliteClient.ResourcesPath.FullName, $"{toolStripDropDownButtonToggleVideo.Tag.ToString()}.ico")).ToBitmap();
                    toolStripDropDownButtonToggleVideo.Width = icon.Width + 15;
                    toolStripDropDownButtonToggleVideo.BackgroundImage = icon;
                    toolStripDropDownButtonToggleVideo.Tag = "Disabled";//This is a toggle, so we mean Enabled after calling the method. 
                    toolStripDropDownButtonToggleVideo_Click(toolStripDropDownButtonToggleVideo, null);                    

                }
                else 
                {
                    toolStripDropDownButtonToggleVideo.Enabled = false;
                    throw new EntryPointNotFoundException("'Stream VIC to' cannot be empty. Please configure 'Data Streams' settings.");
                }
            }
            catch (Exception ex)
            {
                SetStatusLabel($"Error {ex.GetType().Name} {ex.Message}");
            }
            if(false)// (!string.IsNullOrEmpty(settings))
            {
                try
                {
                    Core.Initialize(); // Initialize LibVLC binaries

                    // _libVLC = new LibVLC(enableDebugLogs: true, options: new string[] { "--demux=mp4v", "--rawvid-fps=9" });
                    //  _libVLC = new LibVLC(enableDebugLogs: true, options: new string[] { "--demux=mpegts", "--rawvid-fps=9" });

                    _libVLC = new LibVLC(enableDebugLogs: true, options: new string[] { "--rawvid-fps=9" }); // Disable hardware decoding
                    _libVLC.Log += (sender, e) => SetStatusLabel($"[VLC] {e.Message}");

                    _mediaPlayer = new MediaPlayer(_libVLC);
                    videoview.MediaPlayer = _mediaPlayer;
                }
                catch (Exception ex)
                {

                    SetStatusLabel($"Error {ex.GetType().Name} {ex.Message}");
                }
            }
        }
        public void SetStatusLabel(string statusText)
        {
            try { toolStripStatusLabelStatus.Text = statusText; }
            catch (Exception) { }
        }
        private void startStreamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // Use '@' to listen on the specified port
                Uri mrl = new Uri($"udp://{"192.168.5.65"}:11002");// $"udp://192.168.5.65:11000";// "udp://@:11000";
                SetStatusLabel($"Starting media from {mrl}");

                // Use :network-caching to give the player time to fill its buffer (in ms)
                // Use :demux=h264 or :demux=mpegts if you know the specific format
                string[] options = new[] { ":network-caching=300", ":clock-jitter=0" };
                options = new string[] { "--demux=mpegts", "--rawvid-fps=9" };
                options = new string[] { "--demux=mpegts", "--rawvid-fps=9" };

                using (var media = new Media(_libVLC, mrl, options: options))
                {
                    _mediaPlayer.Play(media);
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
                    case "Disabled": btn.Tag = "Enabled"; command = "start";  break;
                    default: btn.Tag = "Enabled"; break;
                }
                toolStripDropDownButtonToggleVideo.BackgroundImage = new Icon(Path.Combine(UltmateEliteClient.ResourcesPath.FullName, $"{btn.Tag.ToString()}.ico")).ToBitmap();


                string conf= await _client.ConfigurationGetCurrentStreamSettings();
                string destinationIp = conf.Split(":").FirstOrDefault();
                string port = conf.Split(":").LastOrDefault();
                var test =    ApiUrls.StartStream(_ue2Logon.Hostname, StreamType.Video, conf);
                test = ApiUrls.ToLocalPath(test);
                //var url = $"v1/Streams/VIC Stream/Send to...:{command}?ip={destinationIp}?port={port}";                
                var url = $"v1/Streams/{(HttpUtility.UrlEncode("VIC Stream"))}/{ (HttpUtility.UrlEncode("Send to..."))}/{(HttpUtility.UrlEncode( conf))}";
                var success = await _client.PutUrl( test);
                  url = $"/v1/streams/audio:{command}?ip={destinationIp}&port={port}";
           //       json = await _client.PutUrl(url);
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
    }
}
