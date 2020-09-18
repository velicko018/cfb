using CFB.Application.Dispatchers;
using Microsoft.Extensions.DependencyInjection;

namespace CFB.Application.Extensions
{
    public static class AddDispatchersExtensions
    {
        public static IServiceCollection AddDispatchers(this IServiceCollection services)
        {
            services.AddSingleton<QueryDispatcher>();
            services.AddSingleton<CommandDispatcher>();

            return services;
        }
    }
}
