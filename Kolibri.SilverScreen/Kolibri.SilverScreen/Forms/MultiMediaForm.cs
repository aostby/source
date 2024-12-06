using com.sun.org.apache.bcel.@internal.generic;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kolibri.SilverScreen.Forms
{
    public partial class MultiMediaForm : Form
    {
        public enum MultimediaType { Movies, Series, Audio, Pictures }

        Kolibri.net.Common.Dal.LiteDBController _liteDB;
        MultimediaType _type;
        private readonly UserSettings _settings;
        private IEnumerable<FileItem> _files;

        public MultiMediaForm(MultimediaType type, UserSettings settings)
        {
            InitializeComponent();
            _type = type;
            this._settings = settings;
            this.Text = $"{_type.ToString()}";
            _liteDB = new net.Common.Dal.LiteDBController(new FileInfo(settings.LiteDBFilePath), false, false);
            Init();

        }

        private void Init()
        {
            _files = new List<FileItem>();
            this.Text = $"{_type.ToString()} - {_settings.LiteDBFilePath}";
            var path=string.Empty;
            switch (_type)
            {
                case MultimediaType.Movies:
                    path = _settings.UserFilePaths.MoviesSourcePath; break;
                case MultimediaType.Series: break;
                case MultimediaType.Audio: break;
                case MultimediaType.Pictures: break;
                default: path = _settings.UserFilePaths.MoviesSourcePath; break;
            }
            textBoxSource.Text = path;
            _files = _liteDB.FindAllFileItems(new DirectoryInfo(path)).ToList();
            labelNumItems.Text = $"{_files.Count()} files found";

        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            var dInfo=FileUtilities.LetOppMappe(_settings.UserFilePaths.MoviesSourcePath, $"Let opp mappe ({_type})");
            if (dInfo != null)
            {
                _settings.UserFilePaths.MoviesSourcePath = dInfo.FullName;
                if (dInfo.Exists)
                { _settings.UserFilePaths.MoviesSourcePath = dInfo.FullName;
                    _liteDB.Upsert(_settings);
                    textBoxSource.Text = dInfo.FullName;
                    Init();
                }
            }
        }
    }
}
