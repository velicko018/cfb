using CFB.Application.Services;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CFB.Application.Middlewares
{
    public class TokenInvalidationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenInvalidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, ITokenService tokenService)
        {
            if (httpContext.Request.Headers.TryGetValue("Authorization", out var token))
            {
                if (tokenService.InvalidateOrCheckAccessToken(token, true))
                {
                    await _next(httpContext);

                    return;
                }

                httpContext.Response.StatusCode = 401;
                httpContext.Response.ContentType = "application/json";

                await httpContext.Response.WriteAsync("The token is not valid.");
            }
            else
            {
                await _next(httpContext);
            }
        }
    }
}
