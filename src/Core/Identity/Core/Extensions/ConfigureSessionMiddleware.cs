using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ModularArchitecture.Identity.Abstraction;

namespace ModularArchitecture.Identity.Core.Extensions
{
    public class ConfigureSessionMiddleware
    {
        private readonly RequestDelegate _next;

        public ConfigureSessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUserSession session)
        {
            if (context.User.Identities.Any(id => id.IsAuthenticated))
            {
                session.Id = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                session.Roles = context.User.Claims.Where(x => x.Type == ClaimTypes.Role)?.Select(x => x.Value).ToList();
                session.UserName = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
                session.Mobile = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.MobilePhone)?.Value;
                session.FullName = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
            }

            // Call the next delegate/middleware in the pipeline
            await _next.Invoke(context);
        }
    }
}
