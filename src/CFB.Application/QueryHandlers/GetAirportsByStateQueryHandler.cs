using CFB.Common.DTOs;
using CFB.Common.Utilities;
using CFB.Domain.Queries;
using CFB.Infrastructure.Persistence.CosmosGremlinClient;
using CFB.Infrastructure.Persistence.CosmosGremlinClient.Models;
using Gremlin.Net.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CFB.Application.QueryHandlers
{
    public class GetAirportsByStateQueryHandler : IQueryHandler<GetAirportsByStateQuery, IEnumerable<AirportDto>>
    {
        private readonly IGremlinClient _gremlinClient;

        public GetAirportsByStateQueryHandler(IGremlinClient gremlinClient)
        {
            _gremlinClient = gremlinClient;
        }
        
        public async Task<IEnumerable<AirportDto>> Handle(GetAirportsByStateQuery getAirportsByStateQuery)
        {
            var query = new GremlinQueryBuilder()
                .Vertex()
                .HasLabel("airport")
                .Has("state", getAirportsByStateQuery.State)
                .Build();

            var gremlinResult = await _gremlinClient.SubmitAsyncQuery(query);

            return gremlinResult.ToObject<Vertex>()
                .ToList<AirportDto>();
        }
    }
}
