using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace CFB.Infrastructure.Persistence.CosmosGremlinClient.Models
{
    public class VertexEdge
    {
        private IDictionary<string, ICollection<VertexProperty>> _properties;

        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public IDictionary<string, ICollection<VertexProperty>> Properties
        {
            get => _properties;
            set => _properties = value ?? new Dictionary<string, ICollection<VertexProperty>>(); 
        }
        public T ToObject<T>() where T : class
        {
            var serilizer = new JsonSerializer();
            var contract = serilizer.ContractResolver.ResolveContract(typeof(T)) as JsonObjectContract;
            var vertexProperties = new Dictionary<string, object>
            {
                ["id"] = Id,
            };

            foreach (var property in Properties)
            {
                var propertyContract = contract.Properties.GetClosestMatchProperty(property.Key);

                vertexProperties[property.Key] = propertyContract == null || !(typeof(string) == propertyContract.PropertyType || !typeof(IEnumerable).IsAssignableFrom(propertyContract.PropertyType))
                    ? property.Value.Select(pv => pv.Value).ToList()
                    : property.Value.FirstOrDefault()?.Value;
            }

            var result = JObject.FromObject(vertexProperties)
                .ToObject(typeof(T));

            return result as T;
        }
    }
}
    