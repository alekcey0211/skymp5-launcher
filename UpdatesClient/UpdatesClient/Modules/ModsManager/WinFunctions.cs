using System;
using System.Runtime.InteropServices;
using UpdatesClient.Modules.ModsManager.Enums;

namespace UpdatesClient.Modules.ModsManager
{
    public static class WinFunctions
    {
        [DllImport("kernel32.dll")]
        public static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags);

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool CreateHardLink(string lpFileName, string lpExistingFileName, IntPtr lpSecurityAttributes);
    }
}
