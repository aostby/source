using Kolibri.net.Common.Images;
using Kolibri.net.Common.Utilities;
using OMDbApiNet.Model;
using System.ComponentModel;
using System.Data;

namespace Kolibri.net.SilverScreen.Forms
{
    public partial class TreeViewItemsForm : Form
    {
        private ContextMenuStrip _cms;
        private List<OMDbApiNet.Model.Item> _items;
        private string groupBoxOrderbyText = "Order by";
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Item CurrentItem { get; set; }


        public event EventHandler CurrentItemChanged;

        protected virtual void OnCurrentItemChanged(EventArgs e)
        {
            EventHandler handler = CurrentItemChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }


        [Obsolete("Exists for DesignerSerializationVisibility purposes only")]
        public TreeViewItemsForm()
        {
            InitializeComponent();
        }


        public TreeViewItemsForm(List<OMDbApiNet.Model.Item> items)
        {
            InitializeComponent();
            _items = items;
            Init();
            BuildTree();
        }

        private void Init()
        {
            _cms = new ContextMenuStrip();
            _cms.Items.Add("Expand all", null, (s, e) =>
            {
                treeView1.ExpandAll();
            });
            _cms.Items.Add("Collapse all", null, (s, e) =>
            {
                treeView1.CollapseAll();
            });

            groupBoxOrderbyText = groupBoxOrder.Text;
            treeView1.ImageList = imageListIcons;
            try
            {
                var defaultIcon = Icons.GetFolderIcon();
                imageListIcons.Images.Add("default", defaultIcon);
                imageListIcons.Images.Add(nameof(MessageBoxIcon.Question).ToLower(), SystemIcons.Question);
                List<string> common = MovieUtilites.MoviesCommonFileExt(true);
                foreach (var key in common)
                {
                    try
                    {
                        if (!treeView1.ImageList.Images.ContainsKey(key))
                        {
                            var icon = Icons.IconFromExtension(key, Icons.SystemIconSize.Small);
                            if (icon != null && !treeView1.ImageList.Images.ContainsKey(key))
                            {
                                imageListIcons.Images.Add(key, icon);
                            }
                        }
                    }
                    catch (Exception ex) { }
                }
            }
            catch (Exception)
            { }
        }

        private void Radio_CheckedChanged(object sender, EventArgs e)
        {
            BuildTree();
            treeView1.CollapseAll();
        }

        private void BuildTree()
        {
            groupBoxOrder.Text = $"{groupBoxOrderbyText} (Items count: {_items.Count})";
            treeView1.Nodes.Clear();
            treeView1.BeginUpdate();
            treeView1.ShowNodeToolTips = true;

            if (_items == null || !_items.Any())
                return;

            if (radioButtonRating.Checked)
                BuildByRating();
            else if (radioButtonYear.Checked)
                BuildByYear();
            else if (radioButtonGenre.Checked)
                BuildByGenre();
            else if (radioButtonTitle.Checked)
                BuildByTitle();

            else if (radioButtonActor.Checked)
                BuildByActor();

            treeView1.EndUpdate();
        }

        private void BuildByActor()
        {

            var actoreDict = new Dictionary<string, List<OMDbApiNet.Model.Item>>();

            foreach (var item in _items)
            {
                if (string.IsNullOrWhiteSpace(item.Actors))
                    continue;

                var act = item.Actors.Split(',');

                foreach (var g in act)
                {
                    var genre = g.Trim();

                    if (!actoreDict.ContainsKey(genre))
                        actoreDict[genre] = new List<OMDbApiNet.Model.Item>();

                    actoreDict[genre].Add(item);
                }
            }

            foreach (var kvp in actoreDict.OrderBy(g => g.Key))
            {
                TreeNode parent = new TreeNode(kvp.Key);

                foreach (var item in kvp.Value.OrderBy(t => t.Title))
                {
                    parent.Nodes.Add(new TreeNode($"{item.Title} ({item.Year})")
                    {
                        Tag = item,
                        ImageKey = Path.GetExtension($"{item.TomatoUrl}".ToLower()),
                        ToolTipText = $"{item.TomatoUrl}"
                    });
                }

                treeView1.Nodes.Add(parent);
            }
        }

        private void BuildByTitle()
        {
            var groups = _items
                .Where(i => !string.IsNullOrEmpty(i.Title))
                .GroupBy(i => char.ToUpper(i.Title[0]))
                .OrderBy(g => g.Key);

            foreach (var group in groups)
            {
                TreeNode parent = new TreeNode(group.Key.ToString());
                parent.ImageKey = "default";

                foreach (var item in group.OrderBy(i => i.Title))
                {
                    var imgKey = Path.GetExtension($"{item.TomatoUrl}".ToLower());
                    string info = GetTooltipInfo(item);

                    var node = new TreeNode($"{item.Title} ({item.Year})")
                    {
                        Tag = item,
                        ImageKey = imgKey,
                        ToolTipText = info
                    };
                    node.ImageKey = imgKey;
                    parent.Nodes.Add(node);
                }

                treeView1.Nodes.Add(parent);
            }
        }

        private string GetTooltipInfo(Item item)
        {
            string ret = $"Item Details: {item.Title} ({item.Year})\r\n" +
              "----------------\r\n" +
              $"- Rating: {item.ImdbRating}\r\n" +
              $"- Year: {item.Year}\r\n" +
              $"- Genre: {item.Genre}\r\n" +
              $"- URL: {item.TomatoUrl}";
            return ret;
        }

        private void BuildByGenre()
        {
            var genreDict = new Dictionary<string, List<OMDbApiNet.Model.Item>>();

            foreach (var item in _items)
            {
                if (string.IsNullOrWhiteSpace(item.Genre))
                    continue;

                var genres = item.Genre.Split(',');

                foreach (var g in genres)
                {
                    var genre = g.Trim();

                    if (!genreDict.ContainsKey(genre))
                        genreDict[genre] = new List<OMDbApiNet.Model.Item>();

                    genreDict[genre].Add(item);
                }
            }

            foreach (var kvp in genreDict.OrderBy(g => g.Key))
            {
                TreeNode parent = new TreeNode(kvp.Key);

                foreach (var item in kvp.Value.OrderBy(t => t.Title))
                {
 string info = GetTooltipInfo(item);
                    var node = new TreeNode($"{item.Title} ({item.Year})")
                    { 
                        Tag = item,
                        ImageKey = Path.GetExtension($"{item.TomatoUrl}".ToLower()),
                        ToolTipText = $"{info}"
                    };

                    parent.Nodes.Add(node);
                }

                treeView1.Nodes.Add(parent);
            }
        }
        private void BuildByYear()
        {
            var groups = _items
                .GroupBy(i => i.Year ?? "Unknown")
                .OrderByDescending(g => g.Key);

            foreach (var group in groups)
            {
                TreeNode parent = new TreeNode(group.Key);

                foreach (var item in group.OrderBy(t => t.Title))
                {
                    string info = GetTooltipInfo(item);
                    parent.Nodes.Add(new TreeNode($"{item.Title} ({item.Year})")
                    {
                        Tag = item,
                        ImageKey = Path.GetExtension($"{item.TomatoUrl}".ToLower()),
                        ToolTipText = $"{info}"
                    });
                }

                treeView1.Nodes.Add(parent);
            }
        }
        private void BuildByRating()
        {
            var groups = _items
                .GroupBy(i => $"{i.ImdbRating}".Split('.').FirstOrDefault() ?? "N/A")
                .OrderByDescending(g => g.Key);

            foreach (var group in groups)
            {
                TreeNode parent = new TreeNode($"Rating: {group.Key.Split('.').FirstOrDefault()}");

                foreach (var item in group.OrderByDescending(r => r.ImdbRating))
                {
                    string info = GetTooltipInfo(item);
                    parent.Nodes.Add(new TreeNode($"{item.Title} ({item.Year}) - {item.ImdbRating}")
                    {
                        Tag = item,
                        ImageKey = Path.GetExtension($"{item.TomatoUrl}".ToLower())
                    ,
                        ToolTipText = $"{info}"
                    });
                }

                treeView1.Nodes.Add(parent);
            }
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            string url = string.Empty;
            TreeNode clickedNode = e.Node;
            try
            {
                // Perform your desired action with the clicked node
                if (clickedNode != null)
                {
                    CurrentItem = clickedNode.Tag as Item;
                    if (CurrentItem != null)
                    {
                        OnCurrentItemChanged(EventArgs.Empty);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                var contr = treeView1;

                if (e.Button == MouseButtons.Right)
                {
                    _cms.Show(treeView1, new System.Drawing.Point(e.X, e.Y));
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
 