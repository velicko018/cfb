using System.Threading.Tasks;
using CFB.Infrastructure.Persistence.CosmosGremlinClient.Models;
using Gremlin.Net.Driver;
using Newtonsoft.Json.Linq;

namespace CFB.Infrastructure.Persistence.CosmosGremlinClient
{
    public static class CosmosGremlinClientExtensions
    {
        public static async Task<GremlinResult> SubmitAsyncQuery(this IGremlinClient gremlinClient, string query)
        { 
            var resultSet = await gremlinClient.SubmitAsync<JToken>(query);

            return new GremlinResult(resultSet);
        }
    }
}
