using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CFB.Common.DTOs
{
    public class AirportsDto
    {
        [JsonProperty("airports")]
        public IEnumerable<AirportDto> Airports { get; set; }

        [JsonProperty("total")]
        public long Total { get; set; }
    }
    public class AirportDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [Required]
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [Required]
        [JsonProperty("city")]
        public string City { get; set; }
        
        [Required]
        [JsonProperty("state")]
        public string State { get; set; }
        
        [Required]
        [JsonProperty("latitude")]
        public double Latitude { get; set; }
        
        [Required]
        [JsonProperty("longitude")]
        public double Longitude { get; set; }
        
        [Required]
        [JsonProperty("iata")]
        public string IATA { get; set; }
        
        [Required]
        [JsonProperty("icao")]
        public string ICAO { get; set; }
    }

    public class CreateAirportDto
    {
        [Required]
        [JsonProperty("name")]
        public string Name { get; set; }

        [Required]
        [JsonProperty("city")]
        public string City { get; set; }

        [Required]
        [JsonProperty("state")]
        public string State { get; set; }

        [Required]
        [JsonProperty("iata")]
        public string IATA { get; set; }

        [Required]
        [JsonProperty("icao")]
        public string ICAO { get; set; }

        [Required]
        [JsonProperty("latitude")]
        public double Latitude { get; set; }
        
        [Required]
        [JsonProperty("longitude")]
        public double Longitude { get; set; }
    }
}
