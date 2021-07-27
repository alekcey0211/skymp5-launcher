using System;
using System.Windows;
using System.Windows.Controls;

namespace UpdatesClient.UI.Controllers
{
    /// <summary>
    /// Логика взаимодействия для OnlineButton.xaml
    /// </summary>
    public partial class OnlineButton : UserControl
    {
        public event EventHandler Click;

        public OnlineButton()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(sender, e);
        }
    }
}
