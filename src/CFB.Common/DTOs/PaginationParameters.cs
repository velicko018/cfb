using System;
using System.Collections.Generic;
using System.Text;

namespace CFB.Common.DTOs
{
    public class PaginationParameters
    {
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 10;
    }
}
