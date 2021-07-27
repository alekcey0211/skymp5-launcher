using Newtonsoft.Json;

namespace UpdatesClient.Core.Network.Models.Request
{
    public struct ReqResetPassword
    {
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
