using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace ModularArchitecture.Identity.Core.Extensions;

public class MustChangePasswordMiddleware
{
    private readonly RequestDelegate _next;

    public MustChangePasswordMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity is { IsAuthenticated: true } &&
            !context.Request.Path.Value.EndsWith("/account/ChangePassword") &&
            !context.Request.Path.Value.EndsWith("/account/refreshToken") &&
            ((ClaimsIdentity)context.User.Identity).HasClaim(c => c.Type == "MustChangePassword"))
        {
            context.Response.StatusCode = 498;
            return;
        }
        await _next(context).ConfigureAwait(true);
    }
}