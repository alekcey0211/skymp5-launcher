using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Threading;
using UpdatesClient.Modules.Debugger;
using UpdatesClient.Modules.Downloader.Models;
using UpdatesClient.Modules.Downloader.UI;

namespace UpdatesClient.Modules.Downloader
{
    public static class DownloadManager
    {
        private const int MaxTries = 3;

        private static ProgressBar ProgressBar;
        private static Dispatcher Dispatcher;

        private static readonly object sync = new object();
        private static readonly Queue<DownloadModel> Downloads = new Queue<DownloadModel>();

        public static void PostInit(ProgressBar progressBar)
        {
            ProgressBar = progressBar;
            Dispatcher = Dispatcher.CurrentDispatcher;
            Worker();
        }

        public static async Task<bool> DownloadFile(string destinationPath, string url, Action action, string postActionDescription)
        {
            DownloadModel model = new DownloadModel(url, destinationPath, action, postActionDescription);

            lock (sync) Downloads.Enqueue(model);

            while (!model.Performed) await Task.Delay(500);
            return model.Success;
        }

        private static async void Worker()
        {
            while (true)
            {
                if (Downloads.Count != 0)
                {
                    DownloadModel model;
                    lock (sync) model = Downloads.Dequeue();

                    bool success = false;

                    try
                    {
                        success = await Download(model);
                    }
                    catch (Exception e)
                    {
                        Logger.Error("DownloadManager", e);
                    }

                    model.Success = success;
                    model.Performed = true;
                }
                else
                {
                    await Task.Delay(100);
                }
            }
        }

        private static async Task<bool> Download(DownloadModel model, int c = 0)
        {
            ProgressBar.Init(false);

            Downloader downloader = new Downloader(model.DestinationPath, model.Url);
            downloader.DownloadChanged += Downloader_DownloadChanged;

            ProgressBar.Start();

            bool ok = await downloader.StartSync();
            downloader.DownloadChanged -= Downloader_DownloadChanged;

            ProgressBar.Stop();

            if (ok && model.PostAction != null)
            {
                ProgressBar.Init(true, model.PostActionDescription);
                await Task.Run(() => model.PostAction.Invoke());
            }

            if (!ok && c < MaxTries) return await Download(model, ++c);
            return ok;
        }

        private static void Downloader_DownloadChanged(long downloaded, long size, double prDown)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Invoker)delegate
            {
                try
                {
                    ProgressBar.Size = size;
                    ProgressBar.Update(downloaded);
                }
                catch { }
            });
        }

    }
}
