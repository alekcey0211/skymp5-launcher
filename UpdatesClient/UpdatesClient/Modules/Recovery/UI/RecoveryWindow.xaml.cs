using System.Windows;
using System.Windows.Input;

namespace UpdatesClient.Modules.Recovery.UI
{
    /// <summary>
    /// Логика взаимодействия для RecoveryWindow.xaml
    /// </summary>
    public partial class RecoveryWindow : Window
    {
        public RecoveryWindow()
        {
            InitializeComponent();
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
