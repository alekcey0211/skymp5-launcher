using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UpdatesClient.Core;
using UpdatesClient.Modules.Recovery.Models;
using UpdatesClient.Modules.SelfUpdater;

namespace UpdatesClient.Modules.Recovery
{
    public class GameCleaner
    {
        public static async Task CreateGameManifest(string pathToGame)
        {
            if (File.Exists($"{pathToGame}\\SkyrimSE.exe"))
            {
                string vers = FileVersionInfo.GetVersionInfo($"{pathToGame}\\SkyrimSE.exe")?.FileVersion;
                if (!string.IsNullOrEmpty(vers))
                {
                    GameManifestModel model = new GameManifestModel
                    {
                        Version = "1.0",
                        GameVersion = vers
                    };

                    await Task.Run(() =>
                    {
                        IO.RecursiveHandleFile(pathToGame, (file) =>
                        {
                            if (file != null && !new FileInfo(file).Attributes.HasFlag(FileAttributes.ReparsePoint))
                            {
                                string path = file?.Replace(pathToGame, "");
                                model.Files.Add(path, 0);
                            }
                        });
                    });

                    File.WriteAllText($"{pathToGame}\\game.manifest.json", JsonConvert.SerializeObject(model));
                }

            }
        }
    }
}
