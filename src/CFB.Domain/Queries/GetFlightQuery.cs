using CFB.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CFB.Domain.Queries
{
    public class GetFlightQuery : IQuery<FlightDto>
    {
        public string Id { get; set; }
    }
}
