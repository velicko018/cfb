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
    class CreateAirportCommandHandler : ICommandHandler<CreateAirportCommand>
    {
        private readonly IGremlinClient _gremlinClient;
        private readonly ILogger<CreateAirportCommandHandler> _logger;

        public CreateAirportCommandHandler(IGremlinClient gremlinClient, ILogger<CreateAirportCommandHandler> logger)
        {
            _gremlinClient = gremlinClient;
            _logger = logger;
        }


        public async Task<Result> Handle(CreateAirportCommand command)
        {
            try
            {
                var query = new GremlinQueryBuilder()
                    .AddVertex("airport")
                    .Property("type", "airport")
                    .Property("id", command.Id)
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
