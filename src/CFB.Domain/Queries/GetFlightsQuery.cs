using CFB.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CFB.Domain.Queries
{
    public class GetFlightsQuery : IQuery<IEnumerable<FlightDto>>
    {
        public int RangeFrom { get; set; }
        public int RangeTo { get; set; }
    }
}
