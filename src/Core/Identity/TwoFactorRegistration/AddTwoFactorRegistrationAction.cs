using Microsoft.Extensions.DependencyInjection;
using ModularArchitecture.Infrastructure;

namespace ModularArchitecture.Identity.Core
{
    public class AddTwoFactorRegistrationAction : IConfigureServicesAction
    {
        public int Priority { get; } = 10000;
        public void Execute(IServiceCollection services, IServiceProvider serviceProvider)
        {
            services.AddScoped(typeof(ITwoFactorRegistrationService), typeof(TwoFactorRegistrationService));
        }
    }
}