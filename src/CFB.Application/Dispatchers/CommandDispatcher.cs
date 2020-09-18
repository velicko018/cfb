using System;
using System.Threading.Tasks;
using CFB.Application.CommandHandlers;
using CFB.Application.Models;
using CFB.Domain.Commands;

namespace CFB.Application.Dispatchers
{
    public class CommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<Result> Dispatch(ICommand command)
        {
            var type = typeof(ICommandHandler<>);
            var typeArgs = new[] { command.GetType() };
            var handlerType = type.MakeGenericType(typeArgs);
            dynamic handler = _serviceProvider.GetService(handlerType);

            return handler.Handle((dynamic)command);
        }
    }
}
