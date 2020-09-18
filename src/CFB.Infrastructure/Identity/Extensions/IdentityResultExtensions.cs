using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace CFB.Infrastructure.Identity.Extensions
{
    public static class IdentityResultExtensions
    {
        public static string ToFormatedString(this IdentityResult identityResult)
        {
            return string.Join(", ", identityResult.Errors.Select(e => e.Description));
        }
    }
}
