using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace CFB.Infrastructure.Logging
{
    public static class CosmosMongoClientServiceCollectionExtension
    {
        public static IServiceCollection AddCosmosMongoClient(this IServiceCollection services, IConfiguration configuration)
        {
            var cosmosLoggingOptions = configuration
                .GetSection(nameof(CosmosLoggingOptions))
                .Get<CosmosLoggingOptions>();

            services.AddSingleton<IMongoClient>(new MongoClient(cosmosLoggingOptions.ConnectionString));

            return services;
        }
    }
}
