 
using Kolibri.Common.Utilities.Settings;
using Kolibri.Common.FormUtilities.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
 
namespace Kolibri.Common.FormUtilities.Forms
{
    public partial class PropertyGridForm : Form
    {//http://stackoverflow.com/questions/5211987/showing-the-attributes-of-dynamic-types-of-objects-on-propertygrid
        public PropertyGridForm(ISettings setting)//Kolibri.Utilities.Settings.ISettings obj)
        {
            InitializeComponent();
            propertyGrid1.SelectedObject = setting;
            this.Text = string.Format("Edit properties for {0}", setting.GetType().Name);
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            ((propertyGrid1.SelectedObject) as Settings).Save();
        }
    }
}
