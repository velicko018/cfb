using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace CFB.Infrastructure.Logging
{
    public static class CosmosLoggerExtensions
    {
        public static ILoggerFactory AddCosmosLogger(this ILoggerFactory loggerFactory, CosmosLoggingOptions cosmosLoggingOptions, IMemoryCache memoryCache)
        {
            loggerFactory.AddProvider(new CosmosLoggerProvider(cosmosLoggingOptions, memoryCache));

            return loggerFactory;
        }
    }
}
