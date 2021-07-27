using System.Windows;
using System.Windows.Input;
using UpdatesClient.Modules;
using UpdatesClient.Modules.Configs;
using UpdatesClient.Modules.Notifications;
using UpdatesClient.UI.Pages.MainWindow;
using UpdatesClient.UI.Pages.MainWindow.Models;

namespace UpdatesClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowModel WindowModel;
        private ServerList ServerList;

        public MainWindow()
        {
            InitializeComponent();
            userButton.LogoutBtn.Click += LogOut_Click;

            WindowModel = new MainWindowModel(Settings.UserName);
            wind.DataContext = WindowModel;

            ServerList = new ServerList();
            content.Content = ServerList;

            wind.Loaded += Wind_Loaded;
        }

        private void Wind_Loaded(object sender, RoutedEventArgs e)
        {
            WindowModel.Width = stackUserPanel.ActualWidth;
            NotifyController.Init(setNewNotification);
            ServerList.PostInit();
            ModulesManager.PostInitModules();
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            //TODO: аннулирование токена

            Settings.UserId = 0;
            Settings.UserName = "";
            Settings.UserToken = "";
            Settings.Save();

            new Modules.SelfUpdater.SplashScreen().Show();
            Close();
        }

        private void openSettings(object sender, RoutedEventArgs e)
        {
            if (WindowModel.IsOpenSettings)
            {
                settingsPanel.Init();
            }
            else
            {
                settingsPanel.Save();
                ServerList.PostInit();
            }
        }

        public void setNewNotification()
        {
            WindowModel.HasNewNotification = true;
        }

        private void wind_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            NotifyController.Save();
            Settings.Save();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowModel.OpenNotifications = false;
        }
    }
}
