

using Kolibri.net.Common.Dal;

namespace Kolibri.SilverScreen
{
    public partial class Form1 : Form
    {
        LiteDBController _LITEDB;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            try
            {

                _LITEDB = new LiteDBController(false, true);

                throw new NotImplementedException(System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }

        }
    }
}
