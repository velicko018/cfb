using CFB.Infrastructure.Persistence.CosmosGremlinClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace CFB.Infrastructure.Logging
{
    public class CosmosLogger : ILogger
    {
        private readonly IMongoCollection<Log> _logs;
        private readonly CosmosLoggingOptions _cosmosLoggingOptions;
        private readonly IMemoryCache _memoryCache;

        public CosmosLogger(CosmosLoggingOptions cosmosLoggingOptions, IMemoryCache memoryCache)
        {
            _cosmosLoggingOptions = cosmosLoggingOptions;
            _memoryCache = memoryCache;
            var client = new MongoClient(_cosmosLoggingOptions.ConnectionString);
            var database = client.GetDatabase(_cosmosLoggingOptions.DatabaseName);
            var filter = new BsonDocument("name", _cosmosLoggingOptions.CollectionName);
            var collectionExists = database
                .ListCollections(new ListCollectionsOptions { Filter = filter })
                .Any();

            if (!collectionExists)
            {
                database.CreateCollection(_cosmosLoggingOptions.CollectionName);
            }

            _logs = database.GetCollection<Log>(_cosmosLoggingOptions.CollectionName);

            _memoryCache.Set(CacheKeys.Logs, _logs.CountDocuments(_ => true));
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _cosmosLoggingOptions.LogLevel == logLevel;
        }

        public void Log<TState>(LogLevel logLevel, 
            EventId eventId, 
            TState state, 
            Exception exception, 
            Func<TState, Exception, string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                var log = new Log
                {
                    Level = logLevel,
                    Message = state.ToString(),
                    Exception = exception?.ToString(),
                    ApplicationName = exception?.Source
                };

                _logs.InsertOne(log);

                if (_memoryCache.TryGetValue(CacheKeys.Logs, out long numberOfLogs))
                {
                    _memoryCache.Set(CacheKeys.Logs, ++numberOfLogs);
                }
            }
        }
    }
}
