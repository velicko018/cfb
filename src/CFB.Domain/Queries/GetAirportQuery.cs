using CFB.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CFB.Domain.Queries
{
    public class GetAirportQuery : IQuery<AirportDto>
    {
        public string Id { get; set; }
    }
}
