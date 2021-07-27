using System.Windows;
using System.Windows.Controls;

namespace UpdatesClient.Modules.SelfUpdater
{
    /// <summary>
    /// Логика взаимодействия для SelectLanguage.xaml
    /// </summary>
    public partial class SelectLanguage : Window
    {
        public string LanguageBase = null;
        private string languageBase = "en-US";


        public SelectLanguage()
        {
            InitializeComponent();
        }

        private void SelectLang(object sender, RoutedEventArgs e)
        {
            ruEff.Opacity = 0;
            enEff.Opacity = 0;

            switch (((Button)sender).Name)
            {
                case "ru":
                    languageBase = "ru-RU";
                    ruEff.Opacity = 1;

                    text.Text = "Выберите ваш язык";
                    _continue.Content = "ПРОДОЛЖИТЬ";
                    break;
                case "en":
                    languageBase = "en-US";
                    enEff.Opacity = 1;

                    text.Text = "Choose your language";
                    _continue.Content = "CONTINUE";
                    break;
                default:
                    break;
            }
        }

        private void _continue_Click(object sender, RoutedEventArgs e)
        {
            LanguageBase = languageBase;
            Close();
        }
    }
}
