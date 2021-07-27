using Newtonsoft.Json;
using Security;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UpdatesClient.Core;
using UpdatesClient.Modules.SelfUpdater.Models;

namespace UpdatesClient.Modules.SelfUpdater
{
    internal static class Security
    {
        internal static VersionStatus Status = new VersionStatus();
        internal static string UID;

        internal static bool CheckEnvironment()
        {
            Task<bool> checking = CheckVersion();
            checking.Wait();
            if (!checking.Result)
            {
                MessageBox.Show(Encoding.UTF8.GetString(Convert.FromBase64String("VGhlIHZlcnNpb24gaXMgcmV2b2tlZA=="))
                    + "\n" + Encoding.UTF8.GetString(Convert.FromBase64String("UGxlYXNlIGRvd25sb2FkIHRoZSBuZXcgdmVyc2lvbiBmcm9tIHNreW1wLmlv")),
                    Encoding.UTF8.GetString(Convert.FromBase64String("SXMgbm90IGEgYnVn")), MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            UID = Hashing.GetMD5FromText(SystemFunctions.GetHWID());
            AesEncoder.Init();
#if (DEBUG)

#elif (BETA)
            if (!CheckInjection()) return false;
#else
            if (!CheckInjection()) return false;
#endif
            return true;
        }

#pragma warning disable IDE0051 // Удалите неиспользуемые закрытые члены
        private static bool CheckInjection()
#pragma warning restore IDE0051 // Удалите неиспользуемые закрытые члены
        {
            if (WinFunctions.GetModuleHandle("SbieDll.dll") != IntPtr.Zero) return false;
            return true;
        }

        private static async Task<bool> CheckVersion()
        {
            try
            {
                string jsn = await Core.Net.Request($"{Core.Net.URL_ApiLauncher}CheckVersion/{EnvParams.VersionFile}", "POST", false, null);
                Status = JsonConvert.DeserializeObject<VersionStatus>(jsn);
            }
            catch
            {
                Status.Block = false;
            }

            return !(Status.Block && Status.Full);
        }
    }
}
