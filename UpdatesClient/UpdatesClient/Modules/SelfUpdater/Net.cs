using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UpdatesClient.Core;
using UpdatesClient.Modules.Configs;

namespace UpdatesClient.Modules.SelfUpdater
{
    public static class Net
    {
        static Net()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
        }

#if (DEBUG)
        internal static string PROTOCOL = "http://";

        private static readonly string OwnDomain = $@"skymp.local";
        private static readonly string SubDomainS001 = $@"resource.{OwnDomain}";

        private static readonly string FolderLauncher = $@"launcher";
#elif (BETA)
        internal static string PROTOCOL = "https://";

        private static readonly string OwnDomain = $@"skymp.skyrez.su";
        private static readonly string SubDomainS001 = $@"{OwnDomain}/resources";

        private static readonly string FolderLauncher = $@"appBeta";
#else
        internal static string PROTOCOL = "https://";

        private static readonly string OwnDomain = $@"skymp.skyrez.su";
        private static readonly string SubDomainS001 = $@"{OwnDomain}/resources";

        private static readonly string FolderLauncher = $@"app";
#endif

        internal const string LauncherName = "UpdatesClient.exe";
        internal static string AddressToLauncher = $@"{PROTOCOL}{SubDomainS001}/{FolderLauncher}/";

        private static Task<string> Request(string url, string data)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.3; Win64; x64; rv:74.0) Gecko/20100101 Firefox/74.0";
            req.ContentType = "application/x-www-form-urlencoded;";
            if (data != null)
                using (var sw = new StreamWriter(req.GetRequestStream())) sw.Write($"{data}");

            using (var sr = new StreamReader(req.GetResponse().GetResponseStream())) return sr.ReadToEndAsync();
        }

        internal static async Task<string> GetLauncherHash()
        {
            if (NetworkSettings.ByPass)
            {
                Version version = new Version(EnvParams.VersionFile);
                Version nVersion = new Version(NetworkSettings.ByPassVers);

                if (nVersion > version)
                {
                    AddressToLauncher = NetworkSettings.ByPassAddr;
                }
            } 
            string[] req = (await Request($"{AddressToLauncher}", null)).Split('|');
            if (req[0] == "OK") return req[1];
            return null;
        }
    }
}
