using Microsoft.Extensions.Options;

namespace CFB.Infrastructure.Identity
{
    public class CosmosIdentityOptions : IOptions<CosmosIdentityOptions>
    {
        public CosmosIdentityOptions Value => this;
        public string AccountEndpoint { get;  set; }
        public string AuthKey { get;  set; }
    }
}
