using CFB.Common.DTOs;
using CFB.Domain.Queries;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CFB.Application.QueryHandlers
{
    public class GetLogsQueryHandler : IQueryHandler<GetLogsQuery, IEnumerable<LogDto>>
    {
        private readonly IMongoCollection<LogDto> _logs;

        public GetLogsQueryHandler(IMongoClient mongoClient)
        {
            _logs = mongoClient
                .GetDatabase("logs")
                .GetCollection<LogDto>("logs");
        }
        public async Task<IEnumerable<LogDto>> Handle(GetLogsQuery query)
        {
            var logs = await _logs.Find(_ => true)
                .Skip(query.RangeFrom)
                .Limit(query.RangeTo - query.RangeFrom)
                .ToListAsync();

            return logs;
        }
    }
}
