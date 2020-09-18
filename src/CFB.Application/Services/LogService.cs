using System.Threading.Tasks;
using CFB.Application.Dispatchers;
using CFB.Common.DTOs;
using CFB.Domain.Queries;
using CFB.Infrastructure.Persistence.CosmosGremlinClient;
using Microsoft.Extensions.Caching.Memory;

namespace CFB.Application.Services
{
    public interface ILogService
    {
        Task<LogsDto> GetLogsAsync(PaginationParameters paginationParameters);
    }

    public class LogService : ILogService
    {
        private readonly QueryDispatcher _queryDispatcher;
        private readonly IMemoryCache _memoryCache;

        public LogService(QueryDispatcher queryDispatcher, IMemoryCache memoryCache)
        {
            _queryDispatcher = queryDispatcher;
            _memoryCache = memoryCache;
        }

        public async Task<LogsDto> GetLogsAsync(PaginationParameters paginationParameters)
        {
            var query = new GetLogsQuery
            {
                RangeFrom =paginationParameters.PageIndex * paginationParameters.PageSize,
                RangeTo = paginationParameters.PageIndex * paginationParameters.PageSize + paginationParameters.PageSize
            };
            var logs = await _queryDispatcher.Dispatch(query);
            
            _memoryCache.TryGetValue(CacheKeys.Logs, out long total);

            return new LogsDto
            {
                Logs = logs,
                Total = total
            };
        }
    }
}
