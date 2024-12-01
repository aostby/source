 
using Kolibri.net.Common.Dal;

namespace Kolibri.SilverScreen
{
    public partial class TESTForm : Form
    {
        private LiteDBController _LITEDB;
        private OMDBController _OMDB;

        public TESTForm()
        {
            InitializeComponent();
        }

        private async void button1_ClickAsync(object sender, EventArgs e)
        {

            try
            {
                var moviePath = @"C:\TEMP\1951";
                _LITEDB = new LiteDBController(false, true);

                var omdbkey = OMDBController.GetOmdbKey(false);
                _OMDB = new OMDBController(omdbkey, _LITEDB);

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }

        }
    }
}
