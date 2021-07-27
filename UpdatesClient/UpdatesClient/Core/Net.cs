using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UpdatesClient.Modules.Configs;
using UpdatesClient.Modules.Debugger;

namespace UpdatesClient.Core
{
    public class Net
    {
        public const string URL_Version = "https://skymp.io/api/latest_version";
        public const string URL_SKSELink = "https://skymp.io/api/skse_link/{VERSION}";
        public const string URL_ModLink = "https://skymp.io/api/skymp_link/{VERSION}";

        public const string URL_CrashDmp = "https://skymp.io/updates/api/crashes.php";
        public const string URL_CrashDmpSec = "https://skymp.skyrez.su/api/crashes.php";
        public const string URL_SERVERS = "https://skymp.io/api/servers";

        public const string URL_Lib = "https://skymp.io/updates/libs/7z.dll";
        public const string URL_Mod_RuFix = "https://skymp.io/updates/mods/SSERuFixConsole.zip";

        public const string URL_ApiLauncher = "https://skymplauncher.skyrez.su/api/v1/";

        private static readonly HttpClient http = new HttpClient();

        static Net()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }

        public static async Task<string> GetLastestVersion()
        {
            string result = await Request($"{URL_Version}", "GET", false, null);
            return result;
        }

        public static async Task<(string, string)> GetUrlToClient()
        {
            string ver = await GetLastestVersion();
            string link = await Request(URL_ModLink.Replace("{VERSION}", ver), "GET", false, null);
            return (link, ver);
        }

        public static async Task<string> GetUrlToSKSE()
        {
            string ver = await GetLastestVersion();
            string link = await Request(URL_SKSELink.Replace("{VERSION}", ver), "GET", false, null);
            return link;
        }

        public static async Task<bool> ReportDmp(string pathToFile)
        {
            if (NetworkSettings.ReportDmp)
            {
                string req = await UploadRequest(URL_CrashDmpSec, null, pathToFile, "crashdmp", "application/x-dmp");
                if (req != "OK") Logger.Error("ReportDmp_Net", new Exception(req));
                return req == "OK";
            }
            return true;
        }


        public static async Task<string> Request(string url, string method, bool auth, string data)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = method;
            req.Timeout = 10000;
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.3; Win64; x64; rv:84.0) Gecko/20100101 Firefox/84.0";
            req.ContentType = "application/json";
            if (auth) req.Headers.Add(HttpRequestHeader.Authorization, Settings.UserToken);

            if ((data == null && method == "POST") || (data != null))
                using (var sw = new StreamWriter(req.GetRequestStream())) await sw.WriteAsync($"{data ?? ""}");

            using (HttpWebResponse res = (HttpWebResponse)(req.GetResponse()))
            {
                using (StreamReader sr = new StreamReader(res.GetResponseStream()))
                {
                    string raw = await sr.ReadToEndAsync();
                    return raw;
                }
            }
        }

        public static async Task<string> RequestHttp(string url, string method, bool auth, string data)
        {
            StringContent content = new StringContent(data ?? "", Encoding.UTF8, "application/json");
            if (auth) content.Headers.Add(nameof(HttpRequestHeader.Authorization), Settings.UserToken);
            if (method == "GET")
            {
                return await http.GetStringAsync(url);
            }
            else if (method == "POST")
            {
                HttpResponseMessage response = await http.PostAsync(url, content);
                return await response.Content.ReadAsStringAsync();
            }
            return "ERR";
        }

        public static async Task<string> UploadRequest(string url, string data, string file, string paramName, string contentType)
        {
            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;
            request.Credentials = CredentialCache.DefaultCredentials;

            byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            byte[] endBoundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--");

            MemoryStream memStream = new MemoryStream();

            if (data != null)
            {
                foreach (string d in data.Split('&'))
                {
                    (string, string) pair = (d.Split('=')[0], d.Split('=')[1]);

                    string formitem = $"\r\n--{boundary}\r\nContent-Disposition: form-data; name=\"{pair.Item1}\";\r\n\r\n{pair.Item2}";
                    byte[] formitembytes = Encoding.UTF8.GetBytes(formitem);
                    memStream.Write(formitembytes, 0, formitembytes.Length);
                }
            }

            memStream.Write(boundarybytes, 0, boundarybytes.Length);
            var header = $"Content-Disposition: form-data; name=\"{paramName}\"; filename=\"{new FileInfo(file).Name}\"\r\n" +
                $"Content-Type: {contentType}\r\n\r\n";
            var headerbytes = Encoding.UTF8.GetBytes(header);

            memStream.Write(headerbytes, 0, headerbytes.Length);

            using (FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                fileStream.CopyTo(memStream);
            }

            memStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
            request.ContentLength = memStream.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                memStream.Position = 0;
                memStream.CopyTo(requestStream);
                memStream.Close();
            }

            using (var sr = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                return await sr.ReadToEndAsync();
            }
        }
    }
}
