using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace UpdatesClient.Core
{
    public static class EnvParams
    {
        public static readonly string PathToFile = Assembly.GetExecutingAssembly().Location;
        public static readonly string NameOfFileWithoutExtension = Path.GetFileNameWithoutExtension(PathToFile);

        public static readonly string VersionFile = FileVersionInfo.GetVersionInfo(PathToFile)?.FileVersion ?? "0.0.0.0";
        public static readonly string VersionAssembly = Assembly.GetExecutingAssembly().GetName().Version.ToString() ?? "0.0.0.0";
    }
}
