using Microsoft.Extensions.DependencyInjection;

namespace  ModularArchitecture.Identity.Core
{
    public static class AddTwoFactorRegistrationExtension
    {
        public static void AddTwoFactorRegistration(this IServiceCollection services)
        {
            services.AddScoped(typeof(ITwoFactorRegistrationService), typeof(TwoFactorRegistrationService));
        }
    }
}
