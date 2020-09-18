using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace CFB.Infrastructure.Identity.Models
{
    public class CosmosIdentityRole
    {
        public const string Admin = "Admin";
        public const string User = "User";

        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("normalizedRoleName")]
        public string NormalizedName { get; set; }

        [JsonProperty("claims", NullValueHandling = NullValueHandling.Ignore)]
        public IList<Claim> Claims { get; set; }
    }
}
