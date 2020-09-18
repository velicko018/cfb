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
    public class CreateFlightCommandHandler : ICommandHandler<CreateFlightCommand>
    {
        private readonly IGremlinClient _gremlinClient;
        private readonly ILogger<CreateFlightCommandHandler> _logger;

        public CreateFlightCommandHandler(IGremlinClient gremlinClient, ILogger<CreateFlightCommandHandler> logger)
        {
            _gremlinClient = gremlinClient;
            _logger = logger;
        }
        public async Task<Result> Handle(CreateFlightCommand createFlightCommand)
        {
            try
            {
                var flightId = Guid.NewGuid();
                var createFlightQuery = new GremlinQueryBuilder()
                    .AddVertex("flight")
                    .Property("type", "flight")
                    .Property("id", flightId)
                    .Property("from", createFlightCommand.From)
                    .Property("originAirportInfo", createFlightCommand.OriginAirportInfo)                    
                    .Property("to", createFlightCommand.To)
                    .Property("destinationAirportInfo", createFlightCommand.DestinationAirportInfo)
                    .Property("departure", createFlightCommand.Departure.ToIsoFormat())
                    .Property("duration", createFlightCommand.Duration)
                    .AddEdge("has_flight")
                    .From(createFlightCommand.From)
                    .VertexAppend(flightId.ToString())
                    .AddEdge("flight_to")
                    .To(new Guid(createFlightCommand.To))
                    .Build();

                var result = await _gremlinClient.SubmitAsyncQuery(createFlightQuery);
                
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
