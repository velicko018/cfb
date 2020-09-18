using System;
using System.Collections.Generic;
using System.Text;

namespace CFB.Domain.Commands
{
    public class DeleteFlightCommand : ICommand
    {
        public Guid FlightId { get; set; }
    }
}
