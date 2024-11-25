using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kolibri.Common.FormUtilities
{
    public class DefaultIcon
    {
        //mehhh http://stackoverflow.com/questions/5174604/where-to-find-the-default-winforms-icon-in-windows
        private static Icon _defaultFormIcon;
        public static Icon DefaultFormIcon
        {
            get
            {
                if (_defaultFormIcon == null)
                    _defaultFormIcon = (Icon)typeof(Form).
                        GetProperty("DefaultIcon", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetValue(null, null);

                return _defaultFormIcon;
            }
        }

        public static void SetDefaultIcon()
        {

            var icon = Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location);
            typeof(Form)
                .GetField("defaultIcon", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                .SetValue(null, icon);
        }
    }

    public static class FormExtensions
    {
        internal static void GetIconIfDefault(this Form dest, Form source)
        {
            if (dest.Icon == DefaultIcon.DefaultFormIcon)
                dest.Icon = source.Icon;
        }

    }
}