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
    public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, IEnumerable<UserDto>>
    {
        private readonly IGremlinClient _gremlinClient;

        public GetUsersQueryHandler(IGremlinClient gremlinClient)
        {
            _gremlinClient = gremlinClient;
        }

        public async Task<IEnumerable<UserDto>> Handle(GetUsersQuery getUsersQuery)
        {
            var query = new GremlinQueryBuilder()
                .Vertex()
                .HasLabel("user")
                .Build();

            var gremlinResult = await _gremlinClient.SubmitAsyncQuery(query);

            return gremlinResult.ToObject<Vertex>()
                .ToList<UserDto>();
        }
    }
}
