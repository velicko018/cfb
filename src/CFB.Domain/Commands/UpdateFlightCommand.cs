using System;

namespace CFB.Domain.Commands
{
    public class UpdateFlightCommand : ICommand
    {
        public Guid Id { get; set; }
        public DateTime Departure { get; set; }
        public TimeSpan Duration { get; set; }
    }
}