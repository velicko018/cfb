using CFB.Common.DTOs;
using CFB.Common.Utilities;
using CFB.Domain.Queries;
using CFB.Infrastructure.Persistence.CosmosGremlinClient;
using CFB.Infrastructure.Persistence.CosmosGremlinClient.Models;
using Gremlin.Net.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CFB.Application.QueryHandlers
{
    public class FlightSerchQueryHandler : IQueryHandler<FlightSearchQuery, IEnumerable<object>>
    {
        private readonly IGremlinClient _gremlinClient;

        public FlightSerchQueryHandler(IGremlinClient gremlinClient)
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

            foreach (var journay in journays)
            {
                var journayCanBeIncluded = true;
                var flight = journay[0];

                for (var i = 1; journay.Count > i; i++)
                {
                    if (flight.Departure.AddHours(flight.Duration.Add(TimeSpan.FromHours(1.0)).Hours) > journay[i].Departure)
                    {
                        journayCanBeIncluded = false;
                        break;
                    }
                }

                if (journayCanBeIncluded)
                {
                    journaysToReturn.Add(journay);
                }
            }

            return journaysToReturn;
        }
    }
}
