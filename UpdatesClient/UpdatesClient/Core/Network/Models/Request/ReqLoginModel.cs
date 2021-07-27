using Newtonsoft.Json;

namespace UpdatesClient.Core.Network.Models.Request
{
    public struct ReqLoginModel
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
