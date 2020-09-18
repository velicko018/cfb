using CFB.Application.Models;
using CFB.Domain.Commands;
using System.Threading.Tasks;

namespace CFB.Application.CommandHandlers
{
    public interface ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        Task<Result> Handle(TCommand command);
    }
}
