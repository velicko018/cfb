using System;
using System.Collections.Generic;

namespace CFB.Domain.Queries
{
    public class FlightSearchQuery : IQuery<IEnumerable<object>>
    {
        public string From { get; set; }
        public string To { get; set; }
        public DateTime Departure { get; set; }
        public DateTime Return { get; set; }
        public int NumberOfStops { get; set; }
    }
}
