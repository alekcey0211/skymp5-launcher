using System;
using System.Collections.Generic;
using UpdatesClient.Core;
using UpdatesClient.Modules.Configs;
using UpdatesClient.Modules.Debugger;
using UpdatesClient.Modules.Downloader;
using UpdatesClient.Modules.Downloader.UI;

namespace UpdatesClient.Modules
{
    public static class ModulesManager
    {
        private static readonly Dictionary<string, object> objects = new Dictionary<string, object>();

        public static void PreInitModules()
        {
            Settings.Load();
            Logger.Init(new Version(EnvParams.VersionFile));
            NetworkSettings.Init();
        }

        public static void InitModules()
        {

        }

        public static void PostInitModules()
        {
            DownloadManager.PostInit(GetObject<ProgressBar>());
        }

        public static T GetObject<T>()
        {
            string key = typeof(T).Name;
            if (objects.TryGetValue(key, out object o))
                return (T)o;
            else return default;
        }

        public static void RegObject(object o)
        {
            string key = o.GetType().Name;
            if (!objects.ContainsKey(key)) objects.Add(key, o);
            else objects[key] = o;
        }
    }
}
