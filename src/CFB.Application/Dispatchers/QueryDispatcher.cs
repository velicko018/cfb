using CFB.Application.QueryHandlers;
using CFB.Domain.Queries;
using System;
using System.Threading.Tasks;

namespace CFB.Application.Dispatchers
{
    public class QueryDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public QueryDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task<T> Dispatch<T>(IQuery<T> query)
        {
            var type = typeof(IQueryHandler<,>);
            var typeArgs = new [] { query.GetType(), typeof(T) };
            var handlerType = type.MakeGenericType(typeArgs);

            dynamic handler = _serviceProvider.GetService(handlerType);
            T result = await handler.Handle((dynamic)query);

            return result;
        }
    }
}