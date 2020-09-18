using CFB.Common.DTOs;
using System;
using System.Collections.Generic;

namespace CFB.Domain.Queries
{
    public class GetBookingsByUserIdQuery : IQuery<IEnumerable<BookingDto>>
    {
        public Guid UserId { get; set; }
        public int RangeFrom { get; set; }
        public int RangeTo { get; set; }
    }
}
