using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CFB.Common.DTOs
{
    public class BookingsDto
    {
        [JsonProperty("bookings")]
        public IEnumerable<BookingDto> Bookings { get; set; }

        [JsonProperty("total")]
        public long Total { get; set; }
    }
    public class BookingDto
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("numberOfSeats")]
        public int NumberOfSeats { get; set; }

        [JsonProperty("numberOfStops")]
        public int NumberOfStops { get; set; }
    }

    public class CreateBookingDto
    {
        [Required]
        [JsonProperty("flightIds")]
        public IEnumerable<Guid> FlightIds { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(5)]
        [JsonProperty("passangers")]    
        public IEnumerable<PassangerDto> Passangers { get; set; }
    }

    public class PassangerDto
    {
        [Required]
        [JsonProperty("firstName")]
        public string FirstName { get; set; }
        
        [Required]
        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [Required]
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [Required]
        [JsonProperty("passportNumber")]
        public string PassportNumber { get; set; }
    }
}
