using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using Microsoft.Extensions.Caching.Distributed;
using CFB.Application.Models;

namespace CFB.Application.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(string username, string userId, string role);
        string GenerateRefreshToken();
        bool InvalidateOrCheckAccessToken(string token, bool check = false);
        ClaimsPrincipal GetPrincipalFromToken(string token);
        Claim GetUsernameClaimFromToken(string token);
    }

    public class TokenService : ITokenService
    {
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        private readonly JwtOptions _jwtOptions;
        private readonly IDistributedCache _cache;

        public TokenService(IOptions<JwtOptions> jwtOptions, IDistributedCache cache)
        {
            _jwtOptions = jwtOptions.Value;
            _cache = cache;

            if (_jwtSecurityTokenHandler is null)
            {
                _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            }
        }


        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(randomNumber);

                return Convert.ToBase64String(randomNumber);
            }
        }

        public string GenerateAccessToken(string username, string userId, string role)
        {
            var claims = new[]
            {
                 new Claim(JwtRegisteredClaimNames.Sub, username),
                 new Claim(JwtRegisteredClaimNames.Jti, _jwtOptions.Jti),
                 new Claim(ClaimTypes.Name, username),
                 new Claim(ClaimTypes.NameIdentifier, userId),
                 new Claim(ClaimTypes.Role, role)
            };

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            return _jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);
        }

        public bool InvalidateOrCheckAccessToken(string token, bool check = false)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            var principals = GetPrincipalFromToken(token);
            var jtiClaim = principals?.Claims.SingleOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti);
            var expClaim = principals?.Claims.SingleOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp);

            if (jtiClaim != null)
            {
                if (!double.TryParse(expClaim.Value, out double tokenExpirationTimestamp))
                {
                    return false;
                }

                if (check)
                {
                    return _cache.Get(jtiClaim.Value) is null;
                }

                var memoryCacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.UtcNow + GetExpirationFromTimestamp(tokenExpirationTimestamp)
                };

                _cache.Set(jtiClaim.Value, Encoding.UTF8.GetBytes(token), memoryCacheOptions);

                return true;
            }

            return false;
        }

        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            token = token.Replace("Bearer ", string.Empty);
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.SecretKey));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtOptions.Issuer,

                ValidateAudience = true,
                ValidAudience = _jwtOptions.Audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            var claimsPrincipals = _jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (!(securityToken is JwtSecurityToken jwtSecurityToken)
                || (jwtSecurityToken.Issuer != _jwtOptions.Issuer)
                || (jwtSecurityToken.ValidTo < DateTime.UtcNow))
            {
                throw new SecurityTokenException("The token is not valid.");
            }

            return claimsPrincipals;
        }

        public Claim GetUsernameClaimFromToken(string token)
        {
            token = token.Replace("Bearer ", string.Empty);

            var jwtToken = _jwtSecurityTokenHandler.ReadJwtToken(token);

            return jwtToken.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Name);
        }

        private TimeSpan GetExpirationFromTimestamp(double unixTimestamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0)
                .AddSeconds(unixTimestamp)
                .Subtract(DateTime.UtcNow);
        }
    }
}
