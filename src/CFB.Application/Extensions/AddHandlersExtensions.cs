using CFB.Application.QueryHandlers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace CFB.Application.Extensions
{
    public static class AddHandlersExtensions
    {
        public static IServiceCollection AddHandlers(this IServiceCollection serviceCollection)
        {
            var handlerTypes = typeof(IQueryHandler<,>)
                .Assembly.GetTypes()
                .Where(x => x.Name.EndsWith("Handler"))
                .ToList();

            foreach (var handlerType in handlerTypes)
            {
                AddHandler(serviceCollection, handlerType);
            }

            return serviceCollection;
        }

        private static void AddHandler(IServiceCollection serviceCollection, Type handlerType)
        {
            var handlerInterface = handlerType.GetInterfaces().First();
            
            serviceCollection.AddTransient(handlerInterface, handlerType);
        }
    }
}
