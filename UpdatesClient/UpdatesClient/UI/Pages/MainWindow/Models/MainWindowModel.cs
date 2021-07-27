using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace UpdatesClient.UI.Pages.MainWindow.Models
{
    public class MainWindowModel : INotifyPropertyChanged
    {
        private string userName;
        private bool isOpenSettings;
        private double marginNotifyRight;
        private bool openNotifications;
        private bool hasNewNotification;

        public bool HasNewNotification
        {
            get { return hasNewNotification; }
            set { hasNewNotification = value; OnPropertyChanged(); OnPropertyChanged("ShowNewNotification"); }
        }

        public Visibility ShowNewNotification { get => hasNewNotification ? Visibility.Visible : Visibility.Collapsed; }

        public bool OpenNotifications
        {
            get { return openNotifications; }
            set
            {
                openNotifications = value;
                OnPropertyChanged();
                OnPropertyChanged("ShowNotificationsBg");
                if (openNotifications) HasNewNotification = false;
            }
        }

        public Visibility ShowNotificationsBg { get => openNotifications ? Visibility.Visible : Visibility.Collapsed; }

        public Thickness MarginNotify
        {
            get { return new Thickness(0, 50, 175 + marginNotifyRight - 42, 0); }
        }

        public double Width
        {
            get => marginNotifyRight;
            set { marginNotifyRight = value; OnPropertyChanged("MarginNotify"); }
        }

        public bool IsOpenSettings
        {
            get { return isOpenSettings; }
            set { isOpenSettings = value; OnPropertyChanged(); }
        }

        public string UserName
        {
            get { return userName; }
            set { userName = value; OnPropertyChanged(); }
        }

        public MainWindowModel(string userName)
        {
            UserName = userName;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
