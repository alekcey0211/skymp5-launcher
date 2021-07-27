using System;
using System.Runtime.InteropServices;

namespace UpdatesClient.Modules.SelfUpdater
{
    internal static class WinFunctions
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        public const int HWND_BROADCAST = 0xffff;
        public static readonly int WM_SHOWME = RegisterWindowMessage("WM_SHOWME");
        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);
        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr handle, int cmdShow);
        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(IntPtr handle);
    }
}
