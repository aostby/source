
using Kolibri.net.C64Sorter.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kolibri.net.C64Sorter.Forms
{
    public partial class VolumeControlForm : Form
    {
        UltmateEliteClient _client;
        public VolumeControlForm(UltmateEliteClient client)
        {
            InitializeComponent();
            _client = client;
            Init();
        }
        private async void Init() {
            // var conf = await _client.GetConfigs();
            // var mixer = await _client.GetConfigs("Speaker Mixer");///Vol UltiSid 1  Vol UltiSid 1
            // mixer = await _client.GetConfigs("Speaker Mixer/Vol UltiSid 1");///Vol UltiSid 1  Vol UltiSid 1
            try
            {
            var res = await _client.GetVersionAsync();
                
                this.Text = $"{_client.ClientName} Volume control (ReST API Version: {res.version})";
            }
            catch (Exception ex)
            {
                this.Text = $"{_client.ClientName} Volume control ({ex.Message})";
            }

            toolTip1.SetToolTip(buttonEnable, $"{buttonEnable.Text} speaker on UE2 device {_client.ClientName}");
            toolTip1.SetToolTip(buttonDisable, $"{buttonDisable.Text} speaker on UE2 device {_client.ClientName}");

        }
        private async void trackBar1_Scroll(object sender, EventArgs e)
        {
            try
            {
                int NewVolume = trackBar1.Value;// ((ushort.MaxValue / 10) * trackWave.Value);
                toolTip1.SetToolTip(trackBar1, NewVolume.ToString());
                var success = await _client.ConfigurationVolumeLevel(NewVolume);

               
            }
            catch (Exception ex)
            { }
        }

        private async void toggleSound_Click(object sender, EventArgs e)
        {
            try
            {
                await _client.ConfigurationSpeakerEnable(((Button)sender).Text);
            }
            catch (Exception ex) { }
        }
    }
}
