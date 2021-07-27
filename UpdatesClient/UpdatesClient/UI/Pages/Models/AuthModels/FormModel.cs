using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace UpdatesClient.UI.Pages.Models.AuthModels
{
    public class FormModel : INotifyPropertyChanged
    {
        private View currentView;
        private AuthModel authModel;
        private RegModel regModel;
        private RecPswrdModel recPswrdModel;

        public event PropertyChangedEventHandler PropertyChanged;

        public enum View
        {
            Loading,
            SignIn,
            SignUp,
            Recov
        }

        public View CurrentView
        {
            get { return currentView; }
            set { currentView = value; OnPropertyChanged("AuthPanel"); OnPropertyChanged("RegPanel"); OnPropertyChanged("RecPanel"); }
        }

        public AuthModel AuthModel
        {
            get { return authModel; }
            set { authModel = value; OnPropertyChanged("AuthModel"); }
        }

        public RegModel RegModel
        {
            get { return regModel; }
            set { regModel = value; OnPropertyChanged("RegModel"); }
        }

        public RecPswrdModel RecPswrdModel
        {
            get { return recPswrdModel; }
            set { recPswrdModel = value; OnPropertyChanged("RecPswrdModel"); }
        }

        public Visibility AuthPanel { get => CurrentView == View.SignIn ? Visibility.Visible : Visibility.Collapsed; }
        public Visibility RegPanel { get => CurrentView == View.SignUp ? Visibility.Visible : Visibility.Collapsed; }
        public Visibility RecPanel { get => CurrentView == View.Recov ? Visibility.Visible : Visibility.Collapsed; }

        //private void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
