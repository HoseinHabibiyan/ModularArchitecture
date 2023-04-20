using System;
using Microsoft.Extensions.DependencyInjection;
using ModularArchitecture.Infrastructure;
using ModularArchitecture.Localization.Json;

namespace ModularArchitecture.Localization.Json.Actions
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
