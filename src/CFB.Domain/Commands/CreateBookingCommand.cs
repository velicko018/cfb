using CFB.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CFB.Domain.Commands
{
    public class CreateBookingCommand : ICommand
    {
        public IEnumerable<Guid> FlightIds { get; set; }

        public IEnumerable<PassangerDto> Passangers { get; set; }
        public Guid OwnerId { get; set; }
    }
}
