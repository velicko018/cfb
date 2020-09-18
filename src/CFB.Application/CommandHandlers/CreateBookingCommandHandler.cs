using CFB.Domain.Commands;
using CFB.Common.Utilities;
using CFB.Infrastructure.Persistence.CosmosGremlinClient;
using Gremlin.Net.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CFB.Application.Models;

namespace CFB.Application.CommandHandlers
{
    public class CreateBookingCommandHandler : ICommandHandler<CreateBookingCommand>
    {
        private readonly IGremlinClient _gremlinClient;
        private readonly ILogger<CreateBookingCommandHandler> _logger;

        public CreateBookingCommandHandler(IGremlinClient gremlinClient, ILogger<CreateBookingCommandHandler> logger)
        {
            _gremlinClient = gremlinClient;
            _logger = logger;
        }

        public async Task<Result> Handle(CreateBookingCommand createBookingCommand)
        {
            try
            {
                var bookingId = Guid.NewGuid();
                var createBookingQuery = new GremlinQueryBuilder()
                    .AddVertex("booking")
                    .Property("id", bookingId)
                    .Property("type", "booking")
                    .Property("ownerId", createBookingCommand.OwnerId)
                    .Property("numberOfSeats", createBookingCommand.Passangers.Count())
                    .Property("numberOfStops", createBookingCommand.FlightIds.Count())
                    .AddEdge("has_owner")
                    .To(createBookingCommand.OwnerId).InVertex();

                foreach (var flightId in createBookingCommand.FlightIds)
                {
                    createBookingQuery
                        .AddEdge("has_booking")
                        .From(flightId.ToString())
                        .OutVertex();
                }


                foreach (var passanger in createBookingCommand.Passangers)
                {
                    createBookingQuery
                        .AddVertex("passanger", true)
                        .Property("type", "passanger")
                        .Property("id", Guid.NewGuid())
                        .Property("firstName", passanger.FirstName)
                        .Property("lastName", passanger.LastName)
                        .Property("phoneNumber", passanger.PhoneNumber)
                        .Property("passportNumber", passanger.PassportNumber)
                        .AddEdge("has_passanger")
                        .From(bookingId.ToString());
                }

                var result = await _gremlinClient.SubmitAsyncQuery(createBookingQuery.Build());

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
