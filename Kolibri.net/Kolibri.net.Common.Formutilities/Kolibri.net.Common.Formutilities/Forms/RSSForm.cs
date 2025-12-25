using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Kolibri.net.Common.Formutilities.Forms
{
    public partial class RSSForm : Form
    {
       

        private string _url = string.Empty;
        public RSSForm(string url)
        {
            InitializeComponent();
            _url = url;
            PopulateRssList(url);
        }
        

        
        public async  void PopulateRssList(string url)
        {
            await webView21.EnsureCoreWebView2Async();
            listView1.Items.Clear();
            listView1.View = View.Details;
            listView1.HeaderStyle = ColumnHeaderStyle.None;
            listView1.Columns.Add("FeedItems", listView1.Width - 25); // -25 for scrollbar
            listView1.ShowItemToolTips = true;

            using (XmlReader reader = XmlReader.Create(url))
            {
                SyndicationFeed feed = SyndicationFeed.Load(reader);

                // 1. Set the Channel info at the top
                this.Text = $"{feed.Title.Text} - {(feed.Description?.Text ?? "No description provided.")}";
                
                // 2. Clear and populate the news items below
                listView1.Items.Clear();
                listView1.BeginUpdate(); // Prevents flickering during mass update

                foreach (SyndicationItem item in feed.Items)
                {
                    // Main item (Title)
                    ListViewItem lvi = new ListViewItem(item.Title.Text);

                    // Sub-items (Date and Summary) shown under/beside based on View mode
                    lvi.SubItems.Add(item.PublishDate.ToString("MMM dd, yyyy"));
                    lvi.SubItems.Add(item.Summary?.Text ?? "Click to read more...");
                    lvi.Tag = item;

                    // Assign the description to show on hover
                    lvi.ToolTipText = item.Summary?.Text ?? "No description available.";

                    listView1.Items.Add(lvi);
                }
                listView1.EndUpdate();
            }
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var rssItem = (SyndicationItem)listView1.SelectedItems[0].Tag;
                // Now you have access to rssItem.Summary, rssItem.Links, etc.
           
            webView21.CoreWebView2.Navigate(rssItem.Links.FirstOrDefault()?.Uri.ToString());
        } }
    }

}
