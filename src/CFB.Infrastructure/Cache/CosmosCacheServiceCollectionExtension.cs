using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CFB.Infrastructure.Cache
{
    public static class CosmosCacheServiceCollectionExtension
    {
            /// <summary>
            /// Adds Cosmos distributed caching services to the specified <see cref="IServiceCollection" />.
            /// </summary>
            /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
            /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
            public static IServiceCollection AddDistributedCosmosCache(this IServiceCollection services)
            {
                if (services is null)
                {
                    throw new ArgumentNullException(nameof(services));
                }

                services.Add(ServiceDescriptor.Singleton<IDistributedCache, CosmosCache>());

                return services;
            }
        }
}
