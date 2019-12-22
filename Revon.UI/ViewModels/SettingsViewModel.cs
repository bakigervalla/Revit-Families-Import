using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revon.UI.ViewModels
{
    public class SettingsViewModel : PropertyChangedBase
    {

        #region declarations

        private string _fileURL = Properties.Settings.Default.FileURL;
        public string FileURL { get => _fileURL; set { _fileURL = value; Properties.Settings.Default.FileURL = value; NotifyOfPropertyChange(() => FileURL); } }

        private string _thumbnailURL = Properties.Settings.Default.ThumbnailURL;
        public string ThumbnailURL { get => _thumbnailURL; set { _thumbnailURL = value; Properties.Settings.Default.ThumbnailURL = value; NotifyOfPropertyChange(() => ThumbnailURL); } }
        
        #endregion

        ShellViewModel _shell;
        public SettingsViewModel(ShellViewModel shell)
        {
            _shell = shell;
        }
    }
}
