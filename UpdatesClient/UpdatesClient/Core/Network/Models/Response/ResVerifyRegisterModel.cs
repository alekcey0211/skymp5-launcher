using Newtonsoft.Json;

namespace UpdatesClient.Core.Network.Models.Response
{
    public struct ResVerifyRegisterModel
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
