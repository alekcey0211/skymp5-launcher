using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UpdatesClient.UI.Pages.MainWindow.Models
{
    public class SettingsPanelModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string skyrimPath;
        private string[] locales;
        private int selectedLocale;
        private bool expFunctions;

        private bool canDisabledSKSE;
        private bool canDisabledMods;

        private bool disabledSKSE;
        private bool disabledMods;

        private string selfVersion;

        public string SelfVersion
        {
            get { return selfVersion; }
            set { selfVersion = value; OnPropertyChanged(); }
        }

        public bool CanDisabledSKSE
        {
            get { return canDisabledSKSE; }
            set { canDisabledSKSE = value; OnPropertyChanged(); }
        }

        public bool DisabledMods
        {
            get { return disabledMods; }
            set { disabledMods = value; OnPropertyChanged(); }
        }

        public bool CanDisabledMods
        {
            get { return !disabledSKSE && canDisabledMods; }
            set { canDisabledMods = value; OnPropertyChanged(); }
        }

        public bool DisabledSKSE
        {
            get { return disabledSKSE; }
            set { disabledSKSE = value; OnPropertyChanged(); OnPropertyChanged("CanDisabledMods"); }
        }

        public bool ExpFunctions
        {
            get { return expFunctions; }
            set { expFunctions = value; OnPropertyChanged(); }
        }


        public int SelectedLocale
        {
            get { return selectedLocale; }
            set { selectedLocale = value; OnPropertyChanged(); }
        }

        public string[] Locales
        {
            get { return locales; }
            set { locales = value; OnPropertyChanged(); }
        }

        public string SkyrimPath
        {
            get { return skyrimPath; }
            set { skyrimPath = value; OnPropertyChanged(); }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
