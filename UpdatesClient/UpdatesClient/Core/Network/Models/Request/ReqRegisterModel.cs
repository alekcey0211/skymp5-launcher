using Newtonsoft.Json;

namespace UpdatesClient.Core.Network.Models.Request
{
    public struct ReqRegisterModel
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
