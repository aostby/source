using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Kolibri.Common.FormUtilities.Forms;

namespace Kolibri.Common.FormUtilities
{
    /// <summary>
    /// Defined types of messages: Success/Warning/Error.
    /// </summary>
    public enum TypeOfMessage
    {
        Success,
        Warning,
        Error,
    }
    /// <summary>
    /// Initiate instance of SplashScreen
    /// </summary>
    public class SplashScreen
    {
        static Forms.SplashScreenForm sf = null;
        static Image _img = null;



        public static void Splash(string melding, int ms, Image img = null)
        {
            _img = img; 
            Splash(melding, ms, TypeOfMessage.Success);
        }

        public static void Splash(string melding, int ms, TypeOfMessage meldingsType)
        {
            Thread splashthread = new Thread(new ThreadStart(SplashScreen.ShowSplashScreen));
            splashthread.IsBackground = true;
            splashthread.Start();

            //Gi tråden ett sekund til å få igang formen først (funnet ut gjennom prøv og feil)
             SplashScreen.UdpateStatusText(melding);
             Thread.Sleep(100);
             SplashScreen.UdpateStatusText(melding);
             Thread.Sleep(100);
             SplashScreen.UdpateStatusText(melding);
             Thread.Sleep(100);

            SplashScreen.UpdateStatusText(melding, meldingsType);
            Thread.Sleep(ms);
            SplashScreen.CloseSplashScreen();
        }

        /// <summary>
        /// Displays the splashscreen
        /// </summary>
        private static void ShowSplashScreen()
        {
            if (sf == null)
            {if (_img != null)
                    sf = new Forms.SplashScreenForm(_img);
                else
                    sf = new Forms.SplashScreenForm();
                sf.ShowSplashScreen();
            }
        }

        /// <summary>
        /// Closes the SplashScreen
        /// </summary>
        private static void CloseSplashScreen()
        {
            if (sf != null)
            {
                sf.CloseSplashScreen();
                sf = null;
            }
        }

        /// <summary>
        /// Update text in default green color of success message
        /// </summary>
        /// <param name="Text">Message</param>
        private static void UdpateStatusText(string Text)
        {
            if (sf != null)
                sf.UpdateStatusText(Text);

        }

        /// <summary>
        /// Update text with message color defined as green/yellow/red/ for success/warning/failure
        /// </summary>
        /// <param name="Text">Message</param>
        /// <param name="tom">Type of Message</param>
        private static void UpdateStatusText(string Text, TypeOfMessage tom)
        {
            if (sf != null)
                sf.UpdateStatusText(Text, tom);
        } 
        
    }
}
