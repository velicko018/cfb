using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace CFB.Infrastructure.Logging
{
    public class CosmosLoggerProvider : ILoggerProvider
    {
        private readonly CosmosLoggingOptions _cosmosLoggingOptions;
        private readonly IMemoryCache _memoryCache;

        public CosmosLoggerProvider(CosmosLoggingOptions cosmosLoggingOptions, IMemoryCache memoryCache)
        {
            _cosmosLoggingOptions = cosmosLoggingOptions;
            _memoryCache = memoryCache;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new CosmosLogger(_cosmosLoggingOptions, _memoryCache);
        }

        public void Dispose()
        {
            
        }
    }
}
