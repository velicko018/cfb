using CFB.Application.Models;
using CFB.Common.Utilities;
using CFB.Domain.Commands;
using CFB.Infrastructure.Persistence.CosmosGremlinClient;
using Gremlin.Net.Driver;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CFB.Application.CommandHandlers
{
    public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand>
    {
        private readonly IGremlinClient _gremlinClient;
        private readonly ILogger<UpdateUserCommandHandler> _logger;

        public UpdateUserCommandHandler(IGremlinClient gremlinClient, ILogger<UpdateUserCommandHandler> logger)
        {
            _gremlinClient = gremlinClient;
            _logger = logger;
        }
        public async Task<Result> Handle(UpdateUserCommand command)
        {
            try
            {
                var query = new GremlinQueryBuilder()
                    .Vertex(command.Id)
                    .Property("email", command.Email)
                    .Property("firstName", command.FirstName)
                    .Property("lastName", command.LastName)
                    .Property("address", command.Address)
                    .Property("city", command.City)
                    .Property("zip", command.Zip)
                    .Build();

                var result = await _gremlinClient.SubmitAsyncQuery(query);

                if (result.StatusCode >= 200 && result.StatusCode <= 299)
                {
                    return Result.Success;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return Result.Failure;
        }
    }
}
