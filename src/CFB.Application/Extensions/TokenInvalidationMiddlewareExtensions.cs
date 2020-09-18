using CFB.Application.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace CFB.Application.Extensions
{
    public static class TokenInvalidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenInvalidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenInvalidationMiddleware>();
        }
    }
}
