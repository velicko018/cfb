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
    public class GetFlightQueryHandler : IQueryHandler<GetFlightQuery, FlightDto>
    {
        private readonly IGremlinClient _gremlinClient;
        public GetFlightQueryHandler(IGremlinClient gremlinClient)
        {
            _gremlinClient = gremlinClient;
        }

        public async Task<FlightDto> Handle(GetFlightQuery getFlightQuery)
        {
            var query = new GremlinQueryBuilder()
                .Vertex(getFlightQuery.Id)
                .Build();

            var gremlinResult = await _gremlinClient.SubmitAsyncQuery(query);

            return gremlinResult.ToObject<Vertex>()
                .ToList<FlightDto>()
                .FirstOrDefault();
        }
    }
}
