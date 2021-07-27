using Newtonsoft.Json;

namespace UpdatesClient.Core.Network.Models.Response
{
    public struct ResLoginModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
