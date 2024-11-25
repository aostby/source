using System;
using System.ComponentModel.Composition;
using System.Windows.Forms;

namespace Kolibri.Common.PlugIn
{
    public interface IFormPlugin  :IContainerControl
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
          void SetForm(Form mdiParent); 
    }
}