using Newtonsoft.Json;
using Security.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using UpdatesClient.Core;
using UpdatesClient.Core.Enums;
using UpdatesClient.Modules.Configs.Models;
using UpdatesClient.Modules.Debugger;

namespace UpdatesClient.Modules.Configs
{
    internal static class Settings
    {
        private static SettingsFileModel model;

        public static bool Loaded { get; private set; } = false;

        #region Skyrim
        public static string PathToSkyrim { get => model.PathToSkyrim; set => model.PathToSkyrim = value; }
        public static string PathToSkympClientSettings => $"{PathToSkyrim}\\Data\\Platform\\Plugins\\skymp5-client-settings.txt";
        public static string PathToSkyrimTmp { get => PathToSkyrim + "\\tmp\\"; }
        public static string PathToSkyrimMods { get => PathToSkyrim + "\\Mods\\"; }
        #endregion

        #region Launcher
        public static string LastVersion { get => model.LastVersion; private set => model.LastVersion = value; }
        public static int LastServerID { get => model.LastServerID ?? -1; set => model.LastServerID = value; }
        public static List<int> FavoriteServers { get => model.FavoriteServers; }
        public static Locales Locale { get => model.Locale; set => model.Locale = value; }
        public static bool? ExperimentalFunctions { get => model.ExperimentalFunctions; set => model.ExperimentalFunctions = value; }
        #endregion

        #region User
        public static int UserId { get => model.UserId ?? -1; set => model.UserId = value; }
        public static string UserName { get; set; }
        public static bool RememberMe { get; set; } = true;
        private static SecureString userToken;
        public static string UserToken
        {
            get => RememberMe ? model.UserToken : userToken;
            set { if (RememberMe) model.UserToken = value; else userToken = value; }
        }
        #endregion


        internal static void Load()
        {
            try
            {
                if (File.Exists(DefaultPaths.PathToSettingsFile))
                {
                    model = JsonConvert.DeserializeObject<SettingsFileModel>(File.ReadAllText(DefaultPaths.PathToSettingsFile));
                }
                else
                {
                    model = new SettingsFileModel();
                }
            }
            catch (Exception e)
            {
                Logger.Error("Settings_Load", e);
            }

            if (model == null) model = new SettingsFileModel();
            
            Loaded = true;
        }
        internal static void Save()
        {
            try
            {
                IO.CreateDirectory(DefaultPaths.PathToLocal);
                File.WriteAllText(DefaultPaths.PathToSettingsFile, JsonConvert.SerializeObject(model));
            }
            catch (Exception e)
            {
                Logger.Error("Settings_Save", e);
            }
        }
        internal static void Reset()
        {
            try
            {
                if (File.Exists(DefaultPaths.PathToSettingsFile)) File.Delete(DefaultPaths.PathToSettingsFile);
                model = new SettingsFileModel();
            }
            catch (Exception e)
            {
                Logger.Error("Settings_Reset", e);
            }
            Loaded = true;
        }
    }
}
