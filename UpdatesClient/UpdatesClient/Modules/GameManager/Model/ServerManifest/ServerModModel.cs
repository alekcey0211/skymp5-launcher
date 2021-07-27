using Newtonsoft.Json;

namespace UpdatesClient.Modules.GameManager.Models.ServerManifest
{
    public class ServerModModel
    {
        [JsonProperty("crc32")]
        public int CRC32 { get; set; }
        [JsonProperty("filename")]
        public string FileName { get; set; }
        [JsonProperty("size")]
        public long Size { get; set; }
    }
}
