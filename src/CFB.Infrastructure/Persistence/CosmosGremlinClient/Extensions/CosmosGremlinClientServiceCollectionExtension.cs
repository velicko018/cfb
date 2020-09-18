

using Gremlin.Net.Driver;
using Gremlin.Net.Structure.IO.GraphSON;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CFB.Infrastructure.Persistence.CosmosGremlinClient.Extensions
{
    public static class CosmosGremlinClientServiceCollectionExtension
    {
        public static IServiceCollection AddCosmosGremlinClient(this IServiceCollection services, IConfiguration configuration)
        {
            var cosmosGremlinOptions = configuration
                .GetSection(nameof(CosmosGremlinOptions))
                .Get<CosmosGremlinOptions>();

            services.AddSingleton<IGremlinClient>(factory =>
            {
                var gremlinServer = new GremlinServer(
                    hostname: cosmosGremlinOptions.Hostname,
                    port: cosmosGremlinOptions.Port,
                    enableSsl: false,
                    username: cosmosGremlinOptions.Username,
                    password: cosmosGremlinOptions.Password);

                return new GremlinClient(
                    gremlinServer,
                    new GraphSON2Reader(),
                    new GraphSON2Writer(),
                    GremlinClient.GraphSON2MimeType);
            });

            return services;
        }
    }
}
