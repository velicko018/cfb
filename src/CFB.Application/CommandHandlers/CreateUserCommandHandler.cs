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
    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
    {
        private readonly IGremlinClient _gremlinClient;
        private readonly ILogger<CreateUserCommandHandler> _logger;

        public CreateUserCommandHandler(IGremlinClient gremlinClient, ILogger<CreateUserCommandHandler> logger)
        {
            _gremlinClient = gremlinClient;
            _logger = logger;
        }
        
        public async Task<Result> Handle(CreateUserCommand createUserCommand)
        {
            try
            {
                var query = new GremlinQueryBuilder()
                    .AddVertex("user")
                    .Property("type", "user")
                    .Property("id", createUserCommand.Id)
                    .Property("email", createUserCommand.Email)
                    .Property("username", createUserCommand.UserName)
                    .Property("firstName", createUserCommand.FirstName)
                    .Property("lastName", createUserCommand.LastName)
                    .Property("address", createUserCommand.Address)
                    .Property("city", createUserCommand.City)
                    .Property("zip", createUserCommand.Zip)
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
