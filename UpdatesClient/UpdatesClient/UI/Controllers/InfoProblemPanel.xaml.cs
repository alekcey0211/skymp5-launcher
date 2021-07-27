using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UpdatesClient.Modules.Configs;

namespace UpdatesClient.UI.Controllers
{
    /// <summary>
    /// Логика взаимодействия для InfoProblemPanel.xaml
    /// </summary>
    public class InfoProblemModel : INotifyPropertyChanged
    {
        private bool showProblem;

        private string textProblem;

        public string TextProblem
        {
            get { return textProblem; }
            set { textProblem = value; OnPropertyChanged(); }
        }

        public Visibility VisibilityProblem
        {
            get { return showProblem ? Visibility.Visible : Visibility.Collapsed; }
        }

        public bool ShowProblem
        {
            get { return showProblem; }
            set { showProblem = value; OnPropertyChanged(); OnPropertyChanged(nameof(VisibilityProblem)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
    public partial class InfoProblemPanel : UserControl
    {
        private const double Time = 150;

        DoubleAnimation aOpen = new DoubleAnimation()
        {
            Duration = TimeSpan.FromMilliseconds(Time)
        };

        DoubleAnimation aClose = new DoubleAnimation()
        {
            To = 0,
            Duration = TimeSpan.FromMilliseconds(Time)
        };

        public InfoProblemPanel()
        {
            InitializeComponent();
            InfoProblemModel model = new InfoProblemModel()
            {
                ShowProblem = NetworkSettings.ProblemShow,
                TextProblem = NetworkSettings.ProblemText
            };

            infoPanel.DataContext = model;

            text.Loaded += InfoPanel_Loaded;

            
        }

        private void InfoPanel_Loaded(object sender, RoutedEventArgs e)
        {
            aOpen.To = text.ActualWidth;
            text.MaxWidth = 0;
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            text.BeginAnimation(TextBlock.MaxWidthProperty, aOpen);
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            text.BeginAnimation(TextBlock.MaxWidthProperty, aClose);
        }
    }
}
