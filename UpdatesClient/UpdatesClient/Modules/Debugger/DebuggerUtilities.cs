using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UpdatesClient.Core;
using UpdatesClient.Modules.Configs;

namespace UpdatesClient.Modules.Debugger
{
    public static class DebuggerUtilities
    {
        public static readonly string pathToDmps = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\My Games\Skyrim Special Edition\SKSE\Crashdumps\";

        public static async Task ReportDmp()
        {
            if (!Directory.Exists(pathToDmps)) return;
            try
            {
                DateTime dt = ModVersion.LastDmpReported;
                string fileName = "";
                foreach (FileSystemInfo fileSI in new DirectoryInfo(pathToDmps).GetFileSystemInfos())
                {
                    if (fileSI.Extension == ".dmp")
                    {
                        if (dt < fileSI.CreationTime)
                        {
                            dt = fileSI.CreationTime;
                            fileName = fileSI.Name;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(fileName))
                {
                    if (await Net.ReportDmp(pathToDmps + fileName))
                        Logger.ReportMetricaEvent("CrashReported");
                    else Logger.ReportMetricaEvent("CantReport");
                    ModVersion.LastDmpReported = dt;
                    ModVersion.Save();

                    await Task.Delay(3000);
                    File.Delete(pathToDmps + fileName);
                }
            }
            catch (Exception e)
            {
                if (!(e is WebException) && (e is SocketException))
                    Logger.Error("ReportDmp", e);
            }
        }
    }
}
