using Microsoft.AspNetCore.Builder;

namespace ModularArchitecture.Identity.Core.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseConfigureSession(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ConfigureSessionMiddleware>();
        }

        public static IApplicationBuilder UseMustChangePassword(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MustChangePasswordMiddleware>();
        }
    }


}
