using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UpdatesClient.Core;
using UpdatesClient.Modules.Configs;
using UpdatesClient.Modules.Debugger;
using UpdatesClient.Modules.GameManager.Helpers;

namespace UpdatesClient.Modules.GameManager
{
    public class GameLauncher
    {
        public static bool Runing { get; private set; }

        private static Process GameProcess = new Process();
        private static readonly ProcessStartInfo StartInfo = new ProcessStartInfo();

        public static async Task StopGame()
        {
            try
            {
                if (GameProcess != null && !GameProcess.HasExited)
                {
                    GameProcess.Kill();
                    await Task.Run(() => GameProcess.WaitForExit());
                }
            }
            catch { }
            await KillProcess();
        }

        public static void EnableDebug()
        {
            string dir = $"{Settings.PathToSkyrim}\\Data\\SKSE\\";
            string path = $"{dir}SKSE.ini";
            IO.CreateDirectory(dir);
            if (!File.Exists(path)) File.Create(path).Close();

            IniFile iniFile = new IniFile(path);
            iniFile.WriteINI("DEBUG", "WriteMiniDumps", "1");
        }

        private static async Task KillProcess()
        {
            Process[] SkyrimPlatformCEFs = Process.GetProcessesByName("SkyrimPlatformCEF");
            for (int i = 0; i < SkyrimPlatformCEFs.Length; i++)
            {
                try
                {
                    int tr = 0;
                    while (!SkyrimPlatformCEFs[i].HasExited && tr++ < 5)
                    {
                        SkyrimPlatformCEFs[i].Kill();
                        await Task.Delay(250);
                    }
                }
                catch (Win32Exception)
                {
                    try
                    {
                        if (!SkyrimPlatformCEFs[i].HasExited)
                            ProcessKiller.KillProcess((IntPtr)SkyrimPlatformCEFs[i].Id);
                    }
                    catch (Exception e)
                    {
                        Logger.Error("StartGame_Killer_SkyrimPlatformCEF", e);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error("StartGame_KillSkyrimPlatformCEF", e);
                }
            }
        }

        public static async Task<bool> StartGame()
        {
            EnableDebug();

            StartInfo.FileName = $"{Settings.PathToSkyrim}\\skse64_loader.exe";
            StartInfo.WorkingDirectory = $"{Settings.PathToSkyrim}";
            StartInfo.UseShellExecute = false;
            StartInfo.CreateNoWindow = true;
            StartInfo.Verb = "runas";

            StartInfo.Domain = AppDomain.CurrentDomain.FriendlyName;

            GameProcess.StartInfo = StartInfo;

            Runing = true;
            GameProcess.Start();
            Logger.ReportMetricaEvent("StartedGame");

            int ParentPID = GameProcess.Id;

            await Task.Run(() => GameProcess.WaitForExit());

            foreach (var p in ProcessExtensions.FindChildrenProcesses(ParentPID))
            {
                GameProcess = p;
            }
            Microsoft.Win32.SafeHandles.SafeProcessHandle sh = GameProcess.SafeHandle;
            if (!GameProcess.HasExited) await Task.Run(() => GameProcess.WaitForExit());

            Logger.ReportMetricaEvent("ExitedGame");

            await Task.Delay(1000);
            await KillProcess();

            Runing = false;

            return GameProcess.ExitCode != 0;
        }

    }
}
