using System;

namespace UpdatesClient.Modules.Configs.Models
{
    public class ModVersionModel
    {
        public DateTime? LastDmpReported { get; set; }
        public bool SKSEDisabled { get; set; }
        public bool ModsDisabled { get; set; }

        public ModVersionModel()
        {
            LastDmpReported = new DateTime();
        }
    }
}
