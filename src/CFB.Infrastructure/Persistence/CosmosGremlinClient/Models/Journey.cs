using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace CFB.Infrastructure.Persistence.CosmosGremlinClient.Models
{
    public class Journey
    {
        private IEnumerable<VertexEdge> _vertexEdges;

        [JsonProperty("objects")]
        public IEnumerable<VertexEdge> Flights 
        { 
            get { return _vertexEdges; } 
            set { _vertexEdges = value.Where(f => f.Label == "flight" && f.Type == "vertex"); } }
    }

}
    