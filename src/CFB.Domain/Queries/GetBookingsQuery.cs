using CFB.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CFB.Domain.Queries
{
    public class GetBookingsQuery : IQuery<IEnumerable<BookingDto>>
    {
        public int RangeFrom { get; set; }
        public int RangeTo { get; set; }
    }
}
