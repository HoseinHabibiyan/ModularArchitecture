using System;
using Microsoft.Extensions.DependencyInjection;
using ModularArchitecture.Infrastructure;

namespace ModularArchitecture.Localization
{
    public class AddLocalizationAction : IConfigureServicesAction
    {
        public int Priority => 1000;
        public void Execute(IServiceCollection services, IServiceProvider serviceProvider)
        {
            services.AddJsonLocalization(options => options.ResourcesPath = "Resources");
        }
    }
}
