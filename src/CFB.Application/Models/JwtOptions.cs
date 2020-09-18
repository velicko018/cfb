using Microsoft.IdentityModel.Tokens;
using System;

namespace CFB.Application.Models
{
    public class JwtOptions
    {
        public string SecretKey { get; set; }

        public string Issuer { get; set; }

        public string Subject { get; set; }

        public string Audience { get; set; }

        public DateTime Expiration => IssuedAt.Add(ValidFor);

        public DateTime NotBefore => DateTime.UtcNow;

        public DateTime IssuedAt => DateTime.UtcNow;

        public TimeSpan ValidFor { get; set; } = TimeSpan.FromMinutes(120);

        public string Jti => Guid.NewGuid().ToString();

        public SigningCredentials SigningCredentials { get; set; }
    }
}