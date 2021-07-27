using System;

namespace UpdatesClient.Modules.Downloader.Models
{
    public class DownloadModel
    {
        public string Url { get; }
        public string DestinationPath { get; }

        public Action PostAction { get; }
        public string PostActionDescription { get; }

        public bool Performed { get; set; } = false;
        public bool Success { get; set; } = false;

        public DownloadModel(string url, string destinationPath, Action action, string postActionDescription)
        {
            Url = url;
            DestinationPath = destinationPath;
            PostAction = action;
            PostActionDescription = postActionDescription;
        }
    }
}
