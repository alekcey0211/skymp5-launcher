using System.Collections.Generic;
using UpdatesClient.Core.Helpers;

namespace UpdatesClient.Modules.ModsManager.Models
{
    public class ModsModel : IJsonSaver
    {
        public List<string> Mods { get; set; } = new List<string>();
        public List<string> EnabledMods { get; set; } = new List<string>();
    }
}
