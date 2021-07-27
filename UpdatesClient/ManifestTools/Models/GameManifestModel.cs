using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManifestTools.Models
{
    public class GameManifestModel
    {
        public string Version { get; set; }
        public Dictionary<string, string> Files { get; set; }
    }
}
