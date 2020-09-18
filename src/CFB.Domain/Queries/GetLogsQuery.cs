using CFB.Common.DTOs;
using System.Collections.Generic;

namespace CFB.Domain.Queries
{
    public class GetLogsQuery : IQuery<IEnumerable<LogDto>>
    {
        public int RangeFrom { get; set; }
        public int RangeTo { get; set; }
    }
}
