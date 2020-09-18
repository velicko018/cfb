using CFB.Domain.Queries;
using System.Threading.Tasks;

namespace CFB.Application.QueryHandlers
{
    public interface IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        Task<TResult> Handle(TQuery query);
    }
}
