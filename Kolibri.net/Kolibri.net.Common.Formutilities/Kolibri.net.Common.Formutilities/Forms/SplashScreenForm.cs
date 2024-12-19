using Kolibri.net.Common.Utilities;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Kolibri.net.Common.FormUtilities.Forms
{
    public partial class SplashScreenForm : Form
    {
     //   delegate void StringParameterDelegate(string Text);
        delegate void StringParameterTypeDelegate(string Text, TypeOfMessage tom);
        delegate void SplashShowCloseDelegate();

        /// <summary>
        /// To ensure splash screen is closed using the API and not by keyboard or any other things
        /// </summary>
        bool CloseSplashScreenFlag = false;

        /// <summary>
        /// Base constructor
        /// </summary>
        public SplashScreenForm(Image img = null)
        {
            try
            {


                InitializeComponent();
                pictureBox1.Size = this.Size;
                this.label1.Parent = this.pictureBox1;
                this.label1.BackColor = Color.White;
                label1.ForeColor = Color.CornflowerBlue;

                if (img != null)
                {
                    pictureBox1.Image = img;
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom; 
                }
                else
                {
                    try
                    {
                        pictureBox1.Image = ImageUtilities.GetImageFromUrl(@"https://media.licdn.com/mpr/mpr/shrink_200_200/AAEAAQAAAAAAAAQJAAAAJDk3ZjBkODdjLTI2OTAtNDliZi05ZGU4LWUyZTFkZjg3NzMxNw.png");
                    }
                    catch (Exception ex)
                    { string kammar = ex.Message; }
                } 
                label1.BringToFront();

                this.progressBar1.Parent = this.pictureBox1;
                // this.progressBar1.BackColor = Color.Transparent;
                progressBar1.Visible = true;
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// Displays the splashscreen
        /// </summary>
        public void ShowSplashScreen()
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                BeginInvoke(new SplashShowCloseDelegate(ShowSplashScreen));
                return;
            }
            else
            {
                this.Show();
                Application.Run(this);
            }
        }

        /// <summary>
        /// Closes the SplashScreen
        /// </summary>
        public void CloseSplashScreen()
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                BeginInvoke(new SplashShowCloseDelegate(CloseSplashScreen));
                return;
            }
            CloseSplashScreenFlag = true;
            this.Close();
        }

        /// <summary>
        /// Update text in default green color of success message
        /// </summary>
        /// <param name="Text">Message</param>
        public void UpdateStatusText(string Text)
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                //BeginInvoke(new StringParameterDelegate(UpdateStatusText), new object[] { Text });
                StringParameterTypeDelegate update = new StringParameterTypeDelegate(UpdateStatusText);
                this.Invoke(update, new object[] { Text, TypeOfMessage.Success });
                return;
            }
            else
            {
                label1.Text = Text;
                label1.BringToFront();
                label1.Refresh();
            }
        }


        /// <summary>
        /// Update text with message color defined as green/yellow/red/ for success/warning/failure
        /// </summary>
        /// <param name="Text">Message</param>
        /// <param name="tom">Type of Message</param>
        public void UpdateStatusText(string Text, TypeOfMessage tom)
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                // BeginInvoke(new StringParameterTypeDelegate(UpdateStatusText), new object[] { Text, tom });

                StringParameterTypeDelegate update = new StringParameterTypeDelegate(UpdateStatusText);
                this.Invoke(update, new object[] { Text, tom });

                return;
            }
            else
            {
                // Must be on the UI thread if we've got this far     
                
              //  label1.Visible = true;
              //  label1.BringToFront();
                switch (tom)
                {
                    case TypeOfMessage.Error:
                        label1.ForeColor = Color.Red;
                        break;
                    case TypeOfMessage.Warning:
                        label1.ForeColor = Color.Orange;
                        break;
                    case TypeOfMessage.Success:
                        label1.ForeColor = Color.CornflowerBlue;
                        break;
                    default:
                        break;
                }
                label1.Text = Text;
                label1.BringToFront();
                label1.Refresh();
            }
        }

        /// <summary>
        /// Prevents the closing of form other than by calling the CloseSplashScreen function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SplashForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CloseSplashScreenFlag == false)
                e.Cancel = true;
        }
        

    }
}
