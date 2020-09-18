using CFB.Common.DTOs;
using System.Collections.Generic;

namespace CFB.Domain.Queries
{
    public class GetAirportsQuery : IQuery<IEnumerable<AirportDto>>
    {
        public int RangeFrom { get; set; }
        public int RangeTo { get; set; }
    }
}
