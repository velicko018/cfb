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
    class UpdateAirportCommandHandler : ICommandHandler<UpdateAirportCommand>
    {
        private readonly IGremlinClient _gremlinClient;
        private readonly ILogger<UpdateAirportCommandHandler> _logger;

        public UpdateAirportCommandHandler(IGremlinClient gremlinClient, ILogger<UpdateAirportCommandHandler> logger)
        {
            _gremlinClient = gremlinClient;
            _logger = logger;
        }

        public async Task<Result> Handle(UpdateAirportCommand command)
        {
            try
            {
                var query = new GremlinQueryBuilder()
                    .Vertex(command.Id.ToString())
                    .Property("name", command.Name)
                    .Property("city", command.City)
                    .Property("state", command.State)
                    .Property("latitude", command.Latitude)
                    .Property("longitude", command.Longitude)
                    .Property("icao", command.ICAO)
                    .Property("iata", command.IATA)
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

