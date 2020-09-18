using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace CFB.Infrastructure.Identity.Extensions
{
    public static class CosmosClientServiceCollectionExtension
    {
        public static void AddCosmosClient(this IServiceCollection service, IConfiguration configuration)
        {
            var cosmosIdentityoptions = configuration
                .GetSection(nameof(CosmosIdentityOptions))
                .Get<CosmosIdentityOptions>();
            var cosmosClient = new CosmosClient(cosmosIdentityoptions.AccountEndpoint, cosmosIdentityoptions.AuthKey);

            SeedData(cosmosClient).Wait();

            service.AddSingleton(cosmosClient);
        }

        private static async Task SeedData(CosmosClient cosmosClient)
        {
            var cfbDatabaseResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync("cfb");

            await cfbDatabaseResponse.Database.CreateContainerIfNotExistsAsync("cfb", "/type");

            var identityDatabaseResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync("identity");

            await identityDatabaseResponse.Database.CreateContainerIfNotExistsAsync("users", "/user");
            await identityDatabaseResponse.Database.CreateContainerIfNotExistsAsync("roles", "/role");
        }
    }
}
