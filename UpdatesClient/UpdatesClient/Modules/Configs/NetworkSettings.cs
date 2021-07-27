using Newtonsoft.Json;
using System;
using UpdatesClient.Core;
using UpdatesClient.Modules.Configs.Models;
using UpdatesClient.Modules.Debugger;

namespace UpdatesClient.Modules.Configs
{
    internal static class NetworkSettings
    {
        private static NetworkSettingsModel model = new NetworkSettingsModel();
        public static bool Loaded { get; private set; } = false;

        public static bool ReportDmp { get => model.ReportDmp; }
        public static string OfficialServerAdress { get => model.OfficialServerAdress; }
        public static bool EnableAntiCheat { get => model.EnableAntiCheat; }
        public static bool CompatibilityMode { get => model.CompatibilityMode; }
        public static string Banners { get => model.Banners; }

        public static bool ProblemShow { get => model.ProblemShow; }
        public static string ProblemText { get => model.ProblemText; }

        public static bool ByPass { get => model.ByPass; }
        public static string ByPassVers { get => model.ByPassVers;  }
        public static string ByPassAddr { get => model.ByPassAddr;  }

        public static async void Init()
        {
            try
            {
                if (!Loaded)
                {
                    string jsn = await Net.Request(Net.URL_ApiLauncher + "GetParameters", "POST", false, null);
                    model = JsonConvert.DeserializeObject<NetworkSettingsModel>(jsn);
                    Loaded = true;
                }
            }
            catch (Exception e)
            {
                Logger.Error("NetSettigsInit", e);
            }
        }
    }
}
