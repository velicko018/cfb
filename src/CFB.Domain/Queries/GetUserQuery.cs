using CFB.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CFB.Domain.Queries
{
    public class GetUserQuery : IQuery<UserDto>
    {
        public Guid UserId { get; set; }
    }
}
