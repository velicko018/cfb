using System;

namespace CFB.Domain.Commands
{
    public class CreateAirportCommand : ICommand
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string IATA { get; set; }
        public string ICAO { get; set; }
    }
}
