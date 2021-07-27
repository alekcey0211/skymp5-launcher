using System;
using UpdatesClient.Modules.Notifications.Enums;

namespace UpdatesClient.Modules.Notifications.Models
{
    public class WNotifyModel
    {
        public int Id { get; set; }

        public DateTime DateBegin { get; set; }

        public DateTime DateEnd { get; set; }

        public NotificationType Type { get; set; }

        public string Color { get; set; }

        public string Text { get; set; }

        public object[] Parameters { get; set; }
    }
}
