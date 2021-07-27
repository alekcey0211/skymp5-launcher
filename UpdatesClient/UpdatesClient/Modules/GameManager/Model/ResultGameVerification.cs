using System;
using System.Collections.Generic;
using UpdatesClient.Modules.GameManager.Enums;

namespace UpdatesClient.Modules.GameManager.Model
{
    public struct ResultGameVerification
    {
        public bool IsGameFound { get; set; }
        public bool IsSKSEFound { get; set; }
        public bool IsModFound { get; set; }
        public bool IsRuFixConsoleFound { get; set; }

        public bool IsGameSafe { get; set; }
        public bool IsSKSESafe { get; set; }

        public bool NeedInstall { get; set; }

        public Dictionary<string, FileState> UnSafeGameFilesDictionary { get; set; }
        public Dictionary<string, FileState> UnSafeSKSEFilesDictionary { get; set; }

        public Version GameVersion { get; set; }
        public Version SKSEVersion { get; set; }
    }
}
