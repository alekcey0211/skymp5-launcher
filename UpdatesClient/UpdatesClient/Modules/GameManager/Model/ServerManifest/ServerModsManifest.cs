using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UpdatesClient.Modules.GameManager.Models.ServerManifest
{
    public class ServerModsManifest
    {
        [JsonProperty("mods")]
        public List<ServerModModel> Mods { get; set; } = new List<ServerModModel>();

        [JsonProperty("versionMajor")]
        public int VersionMajor { get; set; } = -1;

        [JsonProperty("loadOrder")]
        public List<string> LoadOrder { get; set; } = new List<string>();

        public Dictionary<string, List<(string, uint)>> GetMods()
        {
            List<string> WhiteList = ModsManager.Mods.WhiteListMods;

            List<string> mods = new List<string>(16);
            foreach (string mod in LoadOrder)
            {
                string modName = Path.GetFileNameWithoutExtension(mod);
                if (!mods.Contains(modName) && !WhiteList.Contains(modName)) mods.Add(modName);
            }

            Dictionary<string, List<(string, uint)>> files = new Dictionary<string, List<(string, uint)>>(32);
            foreach (string mod in mods)
            {
                files.Add(mod,
                    Mods.FindAll(m => Path.GetFileNameWithoutExtension(m.FileName) == mod).Select(s => (s.FileName, (uint)s.CRC32)).ToList());
            }

            return files;
        }
    }
}
