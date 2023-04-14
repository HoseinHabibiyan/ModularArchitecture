﻿using System;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

namespace ModularArchitecture.Localization
{
    public static class JsonLocalizationServiceCollectionExtensions
    {
        public static IServiceCollection AddJsonLocalization(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();

            AddJsonLocalizationServices(services);

            return services;
        }

        public static IServiceCollection AddJsonLocalization(
           this IServiceCollection services,
           Action<JsonLocalizationOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }
            
            AddJsonLocalizationServices(services, setupAction);

            return services;
        }

        internal static void AddJsonLocalizationServices(IServiceCollection services)
        {
            services.TryAddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
            services.AddSingleton<IHtmlLocalizerFactory, JsonHtmlLocalizerFactory>();
            services.TryAddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));
            services.TryAddTransient(typeof(IStringLocalizer), typeof(StringLocalizer));
        }

        internal static void AddJsonLocalizationServices(
            IServiceCollection services,
            Action<JsonLocalizationOptions> setupAction)
        {
            AddJsonLocalizationServices(services);
            services.Configure(setupAction);
        }
    }
}