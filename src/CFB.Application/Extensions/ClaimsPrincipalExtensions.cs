using System;
using System.Linq;
using System.Security.Claims;

namespace CFB.Application.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal User)
        {
            return Guid.Parse(User.Claims.First(i => i.Type == ClaimTypes.NameIdentifier).Value);
        }
    }
}
