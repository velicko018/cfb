using Newtonsoft.Json;

namespace CFB.Infrastructure.Persistence.CosmosGremlinClient.Models
{
    public class VertexProperty
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("value")]
        public object Value { get; set; }
    }
}