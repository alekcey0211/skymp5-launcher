using Newtonsoft.Json;

namespace UpdatesClient.Core.Models
{
    public struct SkympClientSettingsModel
    {
        [JsonProperty("server-ip")]
        public string IP { get; set; }

        [JsonProperty("server-port")]
        public int Port { get; set; }

        [JsonProperty("show-me")]
        public bool IsShowMe { get; set; }

        [JsonProperty("enable-console")]
        public bool IsEnableConsole { get; set; }

        [JsonProperty("gameData")]
        public object GameData { get; set; }
    }
}
