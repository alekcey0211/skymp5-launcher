using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace UpdatesClient.Modules.GameManager.Helpers
{
    public class ProcessKiller
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, IntPtr dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool TerminateProcess(int hProcess, uint uExitCode);

        private const uint PROCESS_TERMINATE = 0x1;

        public static void KillProcess(IntPtr PID)
        {
            IntPtr hProcess = OpenProcess(PROCESS_TERMINATE, false, PID);

            if (hProcess == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            if (!TerminateProcess((int)hProcess, 0))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            CloseHandle(hProcess);
        }
    }
}
