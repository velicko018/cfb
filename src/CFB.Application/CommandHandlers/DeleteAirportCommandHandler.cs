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
    public class DeleteAirportCommandHandler : ICommandHandler<DeleteAirportCommand>
    {
        private readonly IGremlinClient _gremlinClient;
        private readonly ILogger<DeleteAirportCommandHandler> _logger;

        public DeleteAirportCommandHandler(IGremlinClient gremlinClient, ILogger<DeleteAirportCommandHandler> logger)
        {
            _gremlinClient = gremlinClient;
            _logger = logger;
        }

        public async Task<Result> Handle(DeleteAirportCommand deleteAirportCommand)
        {
            try
            {
                var query = new GremlinQueryBuilder()
                    .Vertex(deleteAirportCommand.AirportId.ToString())
                    .Drop()
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
