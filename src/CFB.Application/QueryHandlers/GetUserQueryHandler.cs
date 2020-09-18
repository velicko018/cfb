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
    class GetUserQueryHandler : IQueryHandler<GetUserQuery, UserDto>
    {
        private readonly IGremlinClient _gremlinClient;
        public GetUserQueryHandler(IGremlinClient gremlinClient)
        {
            _gremlinClient = gremlinClient;
        }
        public async Task<UserDto> Handle(GetUserQuery getUserQuery)
        {
            var query = new GremlinQueryBuilder()
                .Vertex(getUserQuery.UserId.ToString())
                .Build();

            var gremlinResult = await _gremlinClient.SubmitAsyncQuery(query);

            return gremlinResult.ToObject<Vertex>()
                .ToList<UserDto>()
                .FirstOrDefault();
        }
    }
}