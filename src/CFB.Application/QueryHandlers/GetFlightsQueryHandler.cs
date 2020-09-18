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
    class GetFlightsQueryHandler : IQueryHandler<GetFlightsQuery, IEnumerable<FlightDto>>
    {
        private readonly IGremlinClient _gremlinClient;

        public GetFlightsQueryHandler(IGremlinClient gremlinClient)
        {
            _gremlinClient = gremlinClient;
        }
        public async Task<IEnumerable<FlightDto>> Handle(GetFlightsQuery getFlightsQuery)
        {
            var query = new GremlinQueryBuilder()
                .Vertex()
                .HasLabel("flight")
                .Range(getFlightsQuery.RangeFrom, getFlightsQuery.RangeTo)
                .Build();

            var result = await _gremlinClient.SubmitAsyncQuery(query);

            return result.ToObject<Vertex>()
                .ToList<FlightDto>();
        }
    }
}
