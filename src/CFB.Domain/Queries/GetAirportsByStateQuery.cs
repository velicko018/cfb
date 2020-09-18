using CFB.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CFB.Domain.Queries
{
    public class GetAirportsByStateQuery : IQuery<IEnumerable<AirportDto>>
    {
        public string State { get; set; }
    }
}
