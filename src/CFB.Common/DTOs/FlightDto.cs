using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CFB.Common.DTOs
{
    public class FlightsDto
    {
        [JsonProperty("flights")]
        public IEnumerable<FlightDto> Flights { get; set; }

        [JsonProperty("total")]
        public long Total { get; set; }
    }

    public class FlightSearchDto
    {
        [Required]
        public string From { get; set; }

        [Required]
        public string To { get; set; }

        [Required]
        public DateTime Departure { get; set; }

        [Required]
        public DateTime Return { get; set; }

        public int NumberOfStops { get; set; }
        public string FirstStop { get; set; }
        public string SecondStop { get; set; }
    }

    public class CreateFlightDto
    {
        [Required]
        public string OriginAirportInfo { get; set; }

        [Required]
        public string DestinationAirportInfo { get; set; }
        
        [Required]
        public string From { get; set; }

        [Required]
        public string To { get; set; }

        [Required]
        public DateTime Departure { get; set; }

        [Required]
        public TimeSpan Duration { get; set; }
    }

    public class UpdateFlightDto
    {
        [Required]
        public DateTime Departure { get; set; }
        
        [Required]
        public TimeSpan Duration { get; set; }
    }

    public class FlightDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string From { get; set; }

        [Required]
        public string To { get; set; }

        [Required]
        public DateTime Departure { get; set; }

        [Required]
        public TimeSpan Duration { get; set; }

        [Required]
        public string OriginAirportInfo { get; set; }

        [Required]
        public string DestinationAirportInfo { get; set; }

    }
}
