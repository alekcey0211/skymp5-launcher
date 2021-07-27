using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UpdatesClient.Modules.Configs;

namespace UpdatesClient.Core.Models
{
    public struct ServerModel
    {
        private static ServerModel nullServer = new ServerModel();
        public static int NullID => nullServer.ID;

        [JsonProperty("ip")]
        public string IP { get; set; }

        [JsonProperty("port")]
        public int Port { get; set; }
        public int DataPort { get => Port == 7777 ? 3000 : Port + 1; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("maxPlayers")]
        public int MaxPlayers { get; set; }

        [JsonProperty("online")]
        public int Online { get; set; }

        public string Address => $"{IP}:{Port}";
        public string AddressData => $"{IP}:{DataPort}";

        public int ID => Address.GetHashCode();

        public bool IsEmpty() => ID == NullID;

        public override string ToString() => Name + " (" + Online + " / " + MaxPlayers + ")";

        public bool IsSameServer(SkympClientSettingsModel s) => $"{s.IP}:{s.Port}".GetHashCode() == ID;

        public SkympClientSettingsModel ToSkympClientSettings(SkympClientSettingsModel oldSettings)
        {
            return new SkympClientSettingsModel
            {
                IP = IP,
                IsEnableConsole = oldSettings.IsEnableConsole,
                IsShowMe = oldSettings.IsShowMe,
                Port = Port
            };
        }

        public static List<ServerModel> ParseServersToList(string jArrayServerList)
        {
            return JArray.Parse(jArrayServerList).ToObject<List<ServerModel>>();
        }

        public static Task<string> GetServers()
        {
            return Net.Request(Net.URL_SERVERS, "GET", false, null);
        }

        public static string Load()
        {
            if (File.Exists(DefaultPaths.PathToSavedServerList))
            {
                return File.ReadAllText(DefaultPaths.PathToSavedServerList);
            }
            else
            {
                File.WriteAllText(DefaultPaths.PathToSavedServerList, "[{}]");
                return "[{}]";
            }
        }

        public static void Save(string serverList)
        {
            File.WriteAllText(DefaultPaths.PathToSavedServerList, serverList);
        }
    }
}
