using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MoviesFromImdb
{
    public partial class FriendsWatchlist : Form
    {
        public FriendsWatchlist()
        {
            InitializeComponent();
        }

        public FriendsWatchlist(DataSet result)
        {
            InitializeComponent();
            gridFriendsWatchlist.DataSource = result.Tables[0];
        }
    }
}
