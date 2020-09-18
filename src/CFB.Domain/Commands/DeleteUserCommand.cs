using System;
using System.Collections.Generic;
using System.Text;

namespace CFB.Domain.Commands
{
    public class DeleteUserCommand : ICommand
    {
        public Guid UserId { get; set; }
    }
}
