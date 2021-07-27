using System;
using System.Windows.Controls;
using System.Windows.Media;
using UpdatesClient.Modules.Notifications.Models;

namespace UpdatesClient.Modules.Notifications.UI
{
    /// <summary>
    /// Логика взаимодействия для PopupNotify.xaml
    /// </summary>
    public partial class PopupNotify : UserControl
    {
        public event EventHandler ClickClose;
        public readonly NotifyModel notifyModel;

        public PopupNotify(NotifyModel model)
        {
            InitializeComponent();
            notifyModel = model;

            description.Text = model.Text;
            time.Text = model.DateTime.ToString("HH:mm");

            if (model.Color != null)
            {
                description.Foreground = new BrushConverter().ConvertFromString(model.Color) as SolidColorBrush;
            }

            closeBtn.Click += (s, e) => ClickClose?.Invoke(this, e);
        }
    }
}
