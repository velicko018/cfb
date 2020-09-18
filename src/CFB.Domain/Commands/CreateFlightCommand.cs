using System;
using System.Collections.Generic;
using System.Text;

namespace CFB.Domain.Commands
{
    public class CreateFlightCommand : ICommand
    {
        public string From { get; set; }
        public string To { get; set; }
        public DateTime Departure { get; set; }
        public TimeSpan Duration { get; set; }
        public string DestinationAirportInfo { get; set; }
        public string OriginAirportInfo { get; set; }
    }
}
