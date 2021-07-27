using System.Windows;
using System.Windows.Controls;
using UpdatesClient.Modules;

namespace UpdatesClient.UI.Controllers
{
    /// <summary>
    /// Логика взаимодействия для MainButton.xaml
    /// </summary>
    public partial class MainButton : UserControl
    {
        public event RoutedEventHandler Click;

        public MainButton()
        {
            InitializeComponent();
            ModulesManager.RegObject(progressBar);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(sender, e);
        }
    }
}
