using System;

namespace CFB.Domain.Commands
{
    public class DeleteAirportCommand : ICommand
    {
        public Guid AirportId { get; set; }
    }
}
