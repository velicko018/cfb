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
    public class GetBookingsByUserIdQueryHandler : IQueryHandler<GetBookingsByUserIdQuery, IEnumerable<BookingDto>>
    {
        private readonly IGremlinClient _gremlinClient;

        public GetBookingsByUserIdQueryHandler(IGremlinClient gremlinClient)
        {
            _gremlinClient = gremlinClient;
        }

        public async Task<IEnumerable<BookingDto>> Handle(GetBookingsByUserIdQuery getBookingsByUserIdQuery)
        {
            var gremlinQuery = new GremlinQueryBuilder()
                .Vertex()
                .HasLabel("booking")
                .Has("ownerId", getBookingsByUserIdQuery.UserId.ToString())
                //.Range(getBookingsByUserIdQuery.RangeFrom, getBookingsByUserIdQuery.RangeTo)
                .Build();

            var result = await _gremlinClient.SubmitAsyncQuery(gremlinQuery);

            return result.ToObject<Vertex>()
                .ToList<BookingDto>();
        }
    }
}
