using Newtonsoft.Json;

namespace UpdatesClient.Core.Network.Models.Request
{
    public struct ReqVerifyRegisterModel
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("pin")]
        public string Pin { get; set; }
    }
}
