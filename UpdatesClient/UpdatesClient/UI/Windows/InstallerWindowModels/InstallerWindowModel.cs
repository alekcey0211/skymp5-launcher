using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UpdatesClient.UI.Windows.InstallerWindowModels
{
    public class InstallerWindowModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string pathToSkyrim;
        private string skyrimVersion;
        private string skseVersion;
        private bool canInstall;

        public bool CanInstall
        {
            get { return canInstall; }
            set { canInstall = value; OnPropertyChanged(); }
        }


        public string SKSEVersion
        {
            get { return skseVersion; }
            set { skseVersion = value; OnPropertyChanged(); }
        }

        public string SkyrimVersion
        {
            get { return skyrimVersion; }
            set { skyrimVersion = value; OnPropertyChanged(); }
        }

        public string PathToSkyrim
        {
            get { return pathToSkyrim; }
            set { pathToSkyrim = value; OnPropertyChanged(); }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
