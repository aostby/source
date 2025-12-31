using Kolibri.net.C64Sorter.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;



namespace Kolibri.net.C64Sorter.Forms
{
    public partial class VideoStreamForm : Form
    {// Inside your Form class
        private LibVLC _libVLC;
        private MediaPlayer _mediaPlayer;
        private UE2LogOn _hostname;

        public VideoStreamForm()
        {
            InitializeComponent();
        }

        public VideoStreamForm(UE2LogOn hostname)
        {
            InitializeComponent();
            this._hostname = hostname;
            Init();
        }

        private void Init()
        {
            try
            {
                Core.Initialize(); // Initialize LibVLC binaries

                // _libVLC = new LibVLC(enableDebugLogs: true, options: new string[] { "--demux=mp4v", "--rawvid-fps=9" });
                //  _libVLC = new LibVLC(enableDebugLogs: true, options: new string[] { "--demux=mpegts", "--rawvid-fps=9" });

                _libVLC = new LibVLC(enableDebugLogs: true, options:new string[] { "--rawvid-fps=9" }); // Disable hardware decoding
                _libVLC.Log += (sender, e) =>  SetStatusLabel($"[VLC] {e.Message}");

                _mediaPlayer = new MediaPlayer(_libVLC);
                videoview.MediaPlayer = _mediaPlayer;
            }
            catch (Exception ex)
            {

                SetStatusLabel($"Error {ex.GetType().Name} {ex.Message}");
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

                using (var media = new Media(_libVLC, mrl, options:options))
                {
                    _mediaPlayer.Play(media);
                }
            }
            catch (Exception ex)
            {  SetStatusLabel($"Error {ex.GetType().Name} {ex.Message}");
            }
        }
    }
}
