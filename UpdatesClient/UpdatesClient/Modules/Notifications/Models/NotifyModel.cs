using System;
using UpdatesClient.Modules.Notifications.Enums;

namespace UpdatesClient.Modules.Notifications.Models
{
    public struct NotifyModel
    {
        public string Text { get; set; }
        public string Color { get; set; }
        public NotificationType Type { get; set; }
        public DateTime DateTime { get; set; }

        public NotifyModel(string text, string color, NotificationType type) : this()
        {
            Text = text;
            Color = color;
            Type = type;
            DateTime = DateTime.Now;
        }

        public NotifyModel(Exception exception) : this()
        {
            Text = exception.Message;
            Color = null;
            Type = NotificationType.Text;
            DateTime = DateTime.Now;
        }
    }
}
