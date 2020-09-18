using Newtonsoft.Json;
using System.Collections.Generic;

namespace CFB.Infrastructure.Identity.Models
{
    public class CosmosIdentityUser : CosmosIdentityUser<CosmosIdentityRole> { }

    public class CosmosIdentityUser<TRole>
        where TRole : new ()
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("normalizedUserName")]
        public string NormalizedUserName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("normalizedEmail")]
        public string NormalizedEmail { get; set; }

        [JsonProperty("passwordHash")]
        public string PasswordHash { get; set; }

        [JsonProperty("emailConfirmed")]
        public bool EmailConfirmed { get; set; }

        [JsonProperty("roles", NullValueHandling = NullValueHandling.Ignore)]
        public IList<TRole> Roles { get; set; }

        [JsonProperty("refreshTokens", NullValueHandling = NullValueHandling.Ignore)]
        public IList<string> RefreshTokens { get; set; }

        public CosmosIdentityUser()
        {
            Roles = new List<TRole>();
            RefreshTokens = new List<string>();
        }
    }
}
