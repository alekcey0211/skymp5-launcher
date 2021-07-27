using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UpdatesClient.Modules.Debugger;
using UpdatesClient.Modules.Notifications;

namespace UpdatesClient.Modules.Downloader
{
    public class Downloader
    {
        public delegate void DownloaderStateHandler(long donwloaded, long size, double prDown);
        public event DownloaderStateHandler DownloadChanged;

        public delegate void DownloadedStateHandler(string DestinationFile);
        public event DownloadedStateHandler DownloadComplete;

        private long iFileSize = 0;
        public int iBufferSize = 1024;

        public bool Downloading = false;
        public string sDestinationPath = $"\\";
        public string sInternetPath;

        public Downloader(string DestinationPath, string InternetPath)
        {
            sDestinationPath = DestinationPath;
            sInternetPath = InternetPath;
            iBufferSize *= 10;
        }

        public async void StartAsync()
        {
            string path = Path.GetDirectoryName(sDestinationPath);
            if (!string.IsNullOrEmpty(path) && !Directory.Exists(path)) Directory.CreateDirectory(path);

            await Task.Run(() => StartDown());

            DownloadComplete?.Invoke(sDestinationPath);
        }

        public async Task<bool> StartSync()
        {
            string path = Path.GetDirectoryName(sDestinationPath);
            if (!string.IsNullOrEmpty(path) && !Directory.Exists(path)) Directory.CreateDirectory(path);

            await Task.Run(() => StartDown());

            DownloadComplete?.Invoke(sDestinationPath);
            return sDestinationPath != null;
        }

        private void StartDown()
        {
            try
            {
                if (File.Exists(sDestinationPath) && File.GetAttributes(sDestinationPath) != FileAttributes.Normal)
                    File.SetAttributes(sDestinationPath, FileAttributes.Normal);

                Downloading = true;
                DownloadFile();
            }
            catch (WebException we)
            {
                sDestinationPath = null;
                NotifyController.Show(we);
            }
            catch (SocketException se)
            {
                sDestinationPath = null;
                NotifyController.Show(se);
            }
            catch (UnauthorizedAccessException uae)
            {
                sDestinationPath = null;
                Dictionary<string, string> tags = new Dictionary<string, string>(1)
                {
                    { "FullPath", sDestinationPath }
                };
                Logger.Error("Downloader_UAE", uae, tags);
            }
            catch (Exception e)
            {
                if (e.HResult == -2146232800)
                {
                    NotifyController.Show(e.Message);
                }
                sDestinationPath = null;
                Logger.Error("Downloader", e);
            }
            finally
            {
                Downloading = false;
            }
        }

        private void DownloadFile()
        {
            HttpWebRequest hwRq = (HttpWebRequest)HttpWebRequest.Create(new Uri(sInternetPath));
            hwRq.Timeout = 30000;
            hwRq.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            hwRq.UserAgent = "Mozilla/5.0 (Windows NT 6.3; Win64; x64; rv:74.0) Gecko/20100101 Firefox/74.0";

            using (FileStream saveFileStream = new FileStream(sDestinationPath, FileMode.Create, FileAccess.Write))
            {
                using (HttpWebResponse hwRes = (HttpWebResponse)hwRq.GetResponse())
                {
                    using (Stream smRespStream = hwRes.GetResponseStream())
                    {
                        iFileSize = hwRes.ContentLength;
                        int iByteSize;
                        byte[] downBuffer = new byte[iBufferSize];

                        while ((iByteSize = smRespStream.Read(downBuffer, 0, downBuffer.Length)) > 0)
                        {
                            saveFileStream.Write(downBuffer, 0, iByteSize);
                            DownloadChanged?.Invoke(saveFileStream.Length, iFileSize, 0);
                        }
                    }
                }
            }
        }
    }
}
