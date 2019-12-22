using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Revon.UI.ViewModels
{

    public partial class ImportViewModel : PropertyChangedBase
    {

        #region declarations

        private string _fileURL;
        public string FileURL { get => Properties.Settings.Default.FileURL; }

        private string _thumbnailURL;
        public string ThumbnailURL { get => _thumbnailURL; set { _thumbnailURL = value; Properties.Settings.Default.ThumbnailURL = value; NotifyOfPropertyChange(() => ThumbnailURL); } }

        private string _progress;
        public string Progress { get => _progress; set { _progress = value; NotifyOfPropertyChange(() => Progress); } }

        #endregion

        ShellViewModel _shell;
        public ImportViewModel(ShellViewModel shell)
        {
            _shell = shell;
        }

        public async Task ImportFamilyFile()
        {
            Uri ur = new Uri(FileURL);
            using (WebClient client = new WebClient())
            {

                client.DownloadFileCompleted += client_DownloadFileCompleted;
                client.DownloadFileAsync(ur, $@"{FileLocations.AssemblyPath}\{Path.GetFileName(FileURL)}");
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(delegate (object sender, DownloadProgressChangedEventArgs e)
                {
                    // update the UI
                    Progress = $"Download status: {e.ProgressPercentage}%.";
                    System.Windows.Forms.Application.DoEvents();
                });

            }

        }
        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {

        }

        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {

            Progress = $"Import completed.";
            
            FamilyRequestHandler.familyFile = $@"{FileLocations.AssemblyPath}\{Path.GetFileName(FileURL)}";
            
            _shell.MakeRequest(RequestId.Create);
        }

    }
}
