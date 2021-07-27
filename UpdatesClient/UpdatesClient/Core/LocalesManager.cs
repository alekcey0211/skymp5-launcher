using System.Diagnostics;
using System.IO;
using UpdatesClient.Modules.Configs;

using Res = UpdatesClient.Properties.Resources;

namespace UpdatesClient.Core
{
    public static class LocalesManager
    {
        private const string LastLocaleVersion = "3.0.0.2";
        private static readonly string[] locales = { "ru-RU" };

        public static string GetPathToLocaleLib(string locale)
        {
            return $"{DefaultPaths.PathToLocal}{locale}\\UpdatesClient.resources.dll";
        }

        public static bool ExistLocale(string locale)
        {
            return Directory.Exists($"{DefaultPaths.PathToLocal}{locale}\\")
                && File.Exists(GetPathToLocaleLib(locale));
        }

        public static bool CheckResxLocales()
        {
            bool res = false;
            foreach (string locale in locales)
            {
                string path = $"{DefaultPaths.PathToLocal}\\{locale}";
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                string pathToFile = $"{path}\\UpdatesClient.resources.dll";

                string vers = null;
                if (File.Exists(pathToFile))
                {
                    vers = FileVersionInfo.GetVersionInfo(pathToFile)?.FileVersion;
                }
                if (string.IsNullOrEmpty(vers) || LastLocaleVersion != vers)
                {
                    UnpackResxLocale(locale, pathToFile);
                    res = true;
                }
            }
            return res;
        }

        private static void UnpackResxLocale(string locale, string destPath)
        {
            try
            {
                byte[] bytes = (byte[])Res.ResourceManager.GetObject($"UpdatesClient_{locale}_resources");

                IO.FileSetNormalAttribute(destPath);

                File.WriteAllBytes(destPath, bytes);
            }
            catch { }
        }
    }
}
