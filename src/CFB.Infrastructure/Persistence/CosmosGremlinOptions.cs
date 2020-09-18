using Microsoft.Extensions.Options;
using System;

namespace CFB.Infrastructure.Persistence
{
    public class CosmosGremlinOptions : IOptions<CosmosGremlinOptions>
    {
        public int Port { get; set; }
        public string Hostname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public CosmosGremlinOptions Value => this;
    }
}
