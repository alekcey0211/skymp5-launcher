using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace UpdatesClient.UI.Pages.Models.AuthModels
{
    public class AuthModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string error;

        public string Error
        {
            get { return error; }
            set { error = value; OnPropertyChanged(); OnPropertyChanged("ShowError"); }
        }

        public Visibility ShowError { get => !string.IsNullOrEmpty(Error) ? Visibility.Visible : Visibility.Collapsed; }

        public string Email { get; set; }
        public bool RememberMe { get; set; }

        public AuthModel(string email, bool rememberMe)
        {
            Email = email;
            RememberMe = rememberMe;
        }

        public AuthModel()
        {
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
