using System.Windows.Controls;

namespace UpdatesClient.UI.Controllers
{
    /// <summary>
    /// Логика взаимодействия для InfoBottomPanel.xaml
    /// </summary>
    public partial class InfoBottomPanel : UserControl
    {
        public string Text { get => textBlock.Text; set => textBlock.Text = value; }

        public InfoBottomPanel()
        {
            InitializeComponent();
        }
    }
}
