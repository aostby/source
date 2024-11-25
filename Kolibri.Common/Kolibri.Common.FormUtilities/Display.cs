using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kolibri.Common.FormUtilities
{
    public class Display
    {
        public static void DisplayError(System.Exception ex)
        {
            DisplayError(ex.Message);
        }

        public static void DisplayError(string message)
        {
            MessageBox.Show(message, Assembly.GetCallingAssembly().GetName().Name, MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }

        public static void DisplayMessage(string message)
        {
            MessageBox.Show(message, Assembly.GetCallingAssembly().GetName().Name, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        public static DialogResult DisplayQuestion(string message)
        {
            return MessageBox.Show(message, Assembly.GetCallingAssembly().GetName().Name, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
        }

        public static DialogResult DisplayWarning(string message)
        {
            return MessageBox.Show(message, Assembly.GetCallingAssembly().GetName().Name, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
        }
    }
}