using System;
using System.IO;
using System.Threading.Tasks;
using UpdatesClient.Core;
using UpdatesClient.Modules.Debugger;

namespace UpdatesClient.Modules.SelfUpdater
{
    public static class Updater
    {
        public static readonly string PathToFileUpdate = $"{Path.GetDirectoryName(EnvParams.PathToFile)}\\{EnvParams.NameOfFileWithoutExtension}.update.exe";

        private static string fileHash;
        private static string masterHash;
        private static SplashScreen SplashWindow;

        private static string MasterHash { get => masterHash; set => masterHash = value?.ToUpper()?.Trim(); }
        private static string FileHash { get => fileHash; set => fileHash = value?.ToUpper()?.Trim(); }

        public static async Task Init(SplashScreen splashWindow)
        {
            SplashWindow = splashWindow;

            MasterHash = await Net.GetLauncherHash();
            FileHash = GetFileHash(EnvParams.PathToFile);

            if (string.IsNullOrEmpty(MasterHash)) throw new Exception("MasterHash is empty");
            if (string.IsNullOrEmpty(FileHash)) throw new Exception("FileHash is empty");
        }

        private static string GetFileHash(string path)
        {
            if (!File.Exists(path)) return null;
            return Hashing.GetMD5FromFile(File.OpenRead(path));
        }

        //!======================================
        public static bool UpdateAvailable()
        {
            return MasterHash != FileHash;
        }

        public static bool Update()
        {
            string pathToUpdateFile = $"{Path.GetDirectoryName(EnvParams.PathToFile)}\\{EnvParams.NameOfFileWithoutExtension}.update.exe";
            if (File.Exists(pathToUpdateFile))
            {
                try
                {
                    File.SetAttributes(pathToUpdateFile, FileAttributes.Normal);
                }
                catch (Exception e)
                {
                    Logger.Error("UpdateSetAttr", e);
                }
                File.Delete(pathToUpdateFile);
            }

            Downloader downloader = new Downloader(Net.AddressToLauncher + Net.LauncherName, pathToUpdateFile)
            {
                IsHidden = true
            };
            downloader.DownloadChanged += SplashWindow.SetProgress;

            bool downloaded = downloader.Download();

            return downloaded && MasterHash == GetFileHash($"{EnvParams.NameOfFileWithoutExtension}.update.exe");
        }
    }
}
