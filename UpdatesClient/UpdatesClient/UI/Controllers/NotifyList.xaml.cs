using System.Windows.Controls;
using UpdatesClient.Modules.Notifications;

namespace UpdatesClient.UI.Controllers
{
    /// <summary>
    /// Логика взаимодействия для NotifyList.xaml
    /// </summary>
    public partial class NotifyList : UserControl
    {
        public static NotifyList NotifyPanel;

        public NotifyList()
        {
            InitializeComponent();
            //if (NotifyPanel != null) throw new Exception("Допускается один элемент");
            NotifyPanel = this;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NotifyController.CloseAll();
        }
    }
}
