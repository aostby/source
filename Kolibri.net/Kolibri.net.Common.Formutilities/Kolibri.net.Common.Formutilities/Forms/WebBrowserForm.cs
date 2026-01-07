using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
      using Microsoft.Web.WebView2.WinForms;
using System.Windows.Forms;

namespace Kolibri.net.Common.Formutilities.Forms
{
    public partial class WebBrowserForm : Form
    {
        public WebBrowserForm(string url)
        {
            InitializeComponent();
            InitializeWebView2(url);
        } 

        public async void InitializeWebView2(string url)
        {
            // Ensure the CoreWebView2 is initialized
            await webView21.EnsureCoreWebView2Async(null);

            // Navigate to a URL
            webView21.Source = new System.Uri(url);
        }
    }


}
 