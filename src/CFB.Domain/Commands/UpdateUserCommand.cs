using System;
using System.Collections.Generic;
using System.Text;

namespace CFB.Domain.Commands
{
    public class UpdateUserCommand : ICommand
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
    }
}
