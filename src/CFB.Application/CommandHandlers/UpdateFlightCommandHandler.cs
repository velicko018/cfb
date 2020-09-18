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
    class UpdateFlightCommandHandler : ICommandHandler<UpdateFlightCommand>
    {
        private readonly IGremlinClient _gremlinClient;
        private readonly ILogger<UpdateFlightCommandHandler> _logger;

        public UpdateFlightCommandHandler(IGremlinClient gremlinClient, ILogger<UpdateFlightCommandHandler> logger)
        {
            _gremlinClient = gremlinClient;
            _logger = logger;
        }

        public async Task<Result> Handle(UpdateFlightCommand command)
        {
            try
            {
                var query = new GremlinQueryBuilder()
                    .Vertex(command.Id.ToString())
                    .Property("departure", command.Departure.ToIsoFormat())
                    .Property("duration", command.Duration)
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
