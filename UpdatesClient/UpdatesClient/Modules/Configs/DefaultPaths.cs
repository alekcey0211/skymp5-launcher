using System;
using System.Diagnostics;
using System.Reflection;

namespace UpdatesClient.Modules.Configs
{
    public static class DefaultPaths
    {
        private static readonly string VersionFile = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
        private static readonly string VersionAssembly = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        #region Paths
        public static readonly string PathToLocal = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\UpdatesClient\\";
        public static readonly string PathToLocalSkyrim = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Skyrim Special Edition\\";
        public static readonly string PathToLocalTmp = $"{PathToLocal}tmp\\";
        public static readonly string PathToLocalDlls = $"{PathToLocal}dlls\\";
        public static readonly string PathToSettingsFile = $"{PathToLocal}{VersionAssembly}.json";
        public static readonly string PathToSavedServerList = $"{PathToLocalTmp}\\servers.json";
        #endregion

    }
}
