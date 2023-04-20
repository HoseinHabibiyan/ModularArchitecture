using Microsoft.Extensions.DependencyInjection;
using ModularArchitecture.Identity.TwoFactorRegistration;

namespace ModularArchitecture.Identity.TwoFactorRegistration.Extensions
{
    public static class AddTwoFactorRegistrationExtension
    {
        public static void AddTwoFactorRegistration(this IServiceCollection services)
        {
            services.AddScoped(typeof(ITwoFactorRegistrationService), typeof(TwoFactorRegistrationService));
        }
    }
}
