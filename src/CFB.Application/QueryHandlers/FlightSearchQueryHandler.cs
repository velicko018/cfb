using CFB.Common.DTOs;
using CFB.Common.Utilities;
using CFB.Domain.Queries;
using CFB.Infrastructure.Persistence.CosmosGremlinClient;
using CFB.Infrastructure.Persistence.CosmosGremlinClient.Models;
using Gremlin.Net.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFB.Application.QueryHandlers
{
    public class FlightSearchQueryHandler : IQueryHandler<FlightSearchQuery, IEnumerable<object>>
    {
        private readonly IGremlinClient _gremlinClient;

        public FlightSearchQueryHandler(IGremlinClient gremlinClient)
        {
            _gremlinClient = gremlinClient;
        }

        public async Task<IEnumerable<object>> Handle(FlightSearchQuery flightSearchQuery)
        {
            var gremlinQuery = new GremlinQueryBuilder()
                .Vertex()
                .HasLabel("flight")
                .Has("from", flightSearchQuery.From)
                .Has("departure", "gte", flightSearchQuery.Departure.ToIsoFormat())
                .RepeatOutSimplePath()
                .UntilHasId(flightSearchQuery.To)
                .Path()
                .Build();

            var gremlinResult = await _gremlinClient.SubmitAsyncQuery(gremlinQuery);

            var journays = gremlinResult.ToVEObject<Journey>()
                .ToVEList<FlightDto>();
            var journaysToReturn = new List<List<FlightDto>>();

            if (flightSearchQuery.NumberOfStops != -1)
            {
                journays = journays.Where(j => j.Count <= flightSearchQuery.NumberOfStops + 1)
                    .ToList();
            }

            foreach (var flights in journays)
            {
                var journayCanBeIncluded = true;
                var flight = flights.First();

                for (var i = 1; flights.Count > i; i++)
                {
                    if (flight.Departure.AddHours(flight.Duration.Add(TimeSpan.FromHours(1.0)).Hours) > flights[i].Departure)
                    {
                        journayCanBeIncluded = false;
                        break;
                    }
                }

                if (journayCanBeIncluded)
                {
                    journaysToReturn.Add(flights);
                }
            }

            return journaysToReturn;
        }
    }
}
