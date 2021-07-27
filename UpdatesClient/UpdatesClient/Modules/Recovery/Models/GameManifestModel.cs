using System.Collections.Generic;

namespace UpdatesClient.Modules.Recovery.Models
{
    public class GameManifestModel
    {
        public string Version { get; set; }
        public string GameVersion { get; set; }
        public Dictionary<string, uint> Files { get; set; } = new Dictionary<string, uint>();
    }
}
