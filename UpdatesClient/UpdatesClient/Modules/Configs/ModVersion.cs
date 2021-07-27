using Newtonsoft.Json;
using System;
using System.IO;
using UpdatesClient.Modules.Configs.Models;
using UpdatesClient.Modules.Debugger;
using UpdatesClient.Modules.Notifications;
using Res = UpdatesClient.Properties.Resources;

namespace UpdatesClient.Modules.Configs
{
    internal class ModVersion
    {
        private static readonly object sync = new object();
        private static ModVersionModel model = new ModVersionModel();

        public static DateTime LastDmpReported
        {
            get { return model.LastDmpReported ?? default; }
            set { model.LastDmpReported = value; }
        }

        public static bool SKSEDisabled { get => model.SKSEDisabled; set => model.SKSEDisabled = value; }
        public static bool ModsDisabled { get => model.ModsDisabled; set => model.ModsDisabled = value; }

        internal static void Load()
        {
            try
            {
                if (string.IsNullOrEmpty(Settings.PathToSkyrim)) return;

                string path = $"{Settings.PathToSkyrim}\\version.json";
                if (File.Exists(path))
                {
                    lock (sync)
                        model = JsonConvert.DeserializeObject<ModVersionModel>(File.ReadAllText(path));
                }
                else
                {
                    model = new ModVersionModel();
                }
            }
            catch (Exception e)
            {
                Logger.Error("Version_Load", e);
            }
            if (model == null) model = new ModVersionModel();
        }
        internal static void Save()
        {
            try
            {
                if (string.IsNullOrEmpty(Settings.PathToSkyrim)) return;
                string path = $"{Settings.PathToSkyrim}\\version.json";

                if (File.Exists(path) && File.GetAttributes(path) != FileAttributes.Normal) File.SetAttributes(path, FileAttributes.Normal);

                lock (sync)
                    File.WriteAllText(path, JsonConvert.SerializeObject(model));
            }
            catch (Exception e)
            {
                NotifyController.Show(Res.ErrorSaveFileBusy);
                Logger.Error("Version_Save", e);
            }
        }
        internal static void Reset()
        {
            try
            {
                if (string.IsNullOrEmpty(Settings.PathToSkyrim)) return;
                string path = $"{Settings.PathToSkyrim}\\version.json";
                lock (sync)
                    if (File.Exists(path)) File.Delete(path);
                model = new ModVersionModel();
            }
            catch (Exception e)
            {
                Logger.Error("Version_Reset", e);
            }
        }
    }
}
