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
    public class GetAirportsQueryHandler : IQueryHandler<GetAirportsQuery, IEnumerable<AirportDto>>
    {
        private readonly IGremlinClient _gremlinClient;

        public GetAirportsQueryHandler(IGremlinClient gremlinClient)
        {
            _gremlinClient = gremlinClient;
        }

        public async Task<IEnumerable<AirportDto>> Handle(GetAirportsQuery getAirportsQuery)
        {
            var query = new GremlinQueryBuilder()
                .Vertex()
                .HasLabel("airport")
                .Range(getAirportsQuery.RangeFrom, getAirportsQuery.RangeTo)
                .Build();

            var result = await _gremlinClient.SubmitAsyncQuery(query);

            return result.ToObject<Vertex>()
                .ToList<AirportDto>();
        }
    }
}
