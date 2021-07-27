using System.IO;
using UpdatesClient.Core;
using UpdatesClient.Modules.Configs;

namespace UpdatesClient.Modules.GameManager.Helpers
{
    public static class GameCleaner
    {
        private static readonly string[] DirectoriesSKSE = new string[] { "src", "Data\\Scripts" };
        private static readonly string[] FilesSKSE = new string[] { "skse64_1_5_97.dll", "skse64_loader.exe", "skse64_readme.txt",
            "skse64_steam_loader.dll", "skse64_whatsnew.txt" };

        private static readonly string[] Directories = new string[] { "tmp", "Data\\Interface", "Data\\meshes", "Data\\Platform",
            "Data\\ShaderCache", "Data\\SKSE", "Data\\textures" };
        private static readonly string[] Files = new string[] { "README.txt", "version.json", "Data\\FarmSystem.esp" };

        public static void Clear(bool full = false)
        {
            foreach (string path in Directories)
            {
                try
                {
                    IO.RemoveDirectory(Settings.PathToSkyrim + "\\" + path);
                }
                catch { }
            }

            foreach (string path in Files)
            {
                try
                {
                    File.Delete(Settings.PathToSkyrim + "\\" + path);
                }
                catch { }
            }

            if (full)
            {
                foreach (string path in DirectoriesSKSE)
                {
                    try
                    {
                        IO.RemoveDirectory(Settings.PathToSkyrim + "\\" + path);
                    }
                    catch { }
                }

                foreach (string path in FilesSKSE)
                {
                    try
                    {
                        File.Delete(Settings.PathToSkyrim + "\\" + path);
                    }
                    catch { }
                }
            }
        }
    }
}
