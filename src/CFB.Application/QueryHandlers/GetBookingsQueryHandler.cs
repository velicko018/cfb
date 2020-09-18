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
    public class GetBookingsQueryHandler : IQueryHandler<GetBookingsQuery, IEnumerable<BookingDto>>
    {
        private readonly IGremlinClient _gremlinClient;

        public GetBookingsQueryHandler(IGremlinClient gremlinClient)
        {
            _gremlinClient = gremlinClient;
        }
        public async Task<IEnumerable<BookingDto>> Handle(GetBookingsQuery getBookingsQuery)
        {
            var gremlinQuery = new GremlinQueryBuilder()
                .Vertex()
                .HasLabel("booking")
                .Range(getBookingsQuery.RangeFrom, getBookingsQuery.RangeTo)
                .Build();

            var result = await _gremlinClient.SubmitAsyncQuery(gremlinQuery);

            return result.ToObject<Vertex>()
                .ToList<BookingDto>();
        }
    }
}
