using CFB.Common.DTOs;
using CFB.Common.Utilities;
using CFB.Domain.Queries;
using CFB.Infrastructure.Persistence.CosmosGremlinClient;
using CFB.Infrastructure.Persistence.CosmosGremlinClient.Models;
using Gremlin.Net.Driver;
using System.Linq;
using System.Threading.Tasks;

namespace CFB.Application.QueryHandlers
{
    public class GetAirportQueryHandler : IQueryHandler<GetAirportQuery, AirportDto>
    {
        private readonly IGremlinClient _gremlinClient;

        public GetAirportQueryHandler(IGremlinClient gremlinClient)
        {
            _gremlinClient = gremlinClient;
        }

        public async Task<AirportDto> Handle(GetAirportQuery getAirportQuery)
        {
            var query = new GremlinQueryBuilder()
                .Vertex()
                .HasLabel("airport")
                .Has("id", getAirportQuery.Id)
                .Build();

            var gremlinResult = await _gremlinClient.SubmitAsyncQuery(query);

            return gremlinResult.ToObject<Vertex>()
                .ToList<AirportDto>()
                .FirstOrDefault();
        }
    }
}
