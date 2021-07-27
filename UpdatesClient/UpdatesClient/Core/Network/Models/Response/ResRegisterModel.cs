using Newtonsoft.Json;

namespace UpdatesClient.Core.Network.Models.Response
{
    public struct ResRegisterModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
