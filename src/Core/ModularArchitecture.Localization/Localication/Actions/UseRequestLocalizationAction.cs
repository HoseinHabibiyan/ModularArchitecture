using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ModularArchitecture.Infrastructure;
using ModularArchitecture.Localization.Localication;

namespace ModularArchitecture.Localization.Localication.Actions
{
    public class UseRequestLocalizationAction : IConfigureMiddleware
    {
        private IOptions<LocalizationOption> _options;
        public int Priority => 10001;

        public void Execute(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider)
        {
            _options = serviceProvider.GetService<IOptions<LocalizationOption>>();

            applicationBuilder.UseRequestLocalization(options =>
            {
                options.RequestCultureProviders = _options.Value.RequestCultureProviders;
                options
                    .SetDefaultCulture(_options.Value.DefaultCulture ?? _options.Value.SupportedCultures[0])
                    .AddSupportedCultures(_options.Value.SupportedCultures)
                    .AddSupportedUICultures(_options.Value.SupportedCultures)
                    .AddInitialRequestCultureProvider(new CustomRequestCultureProvider(async context =>
                    {
                        var currentCulture = _options.Value.DefaultCulture ?? _options.Value.SupportedCultures[0];

                        if (context.Request.RouteValues.ContainsKey("culture"))
                        {
                            currentCulture = context.Request.RouteValues["culture"].ToString();
                        }
                        else
                        {
                            var segments = context.Request.Path.Value.Split(new[] { '/' },
                                StringSplitOptions.RemoveEmptyEntries);

                            if (segments.Length >= 1)
                            {
                                var cultureSegment = segments[0];
                                if (cultureSegment == "api")
                                {
                                    cultureSegment = segments[1];
                                }

                                if (cultureSegment.Length == 2 || cultureSegment.Length == 5 && Regex.IsMatch(cultureSegment, "\\w{2}-\\w{2}"))
                                {
                                    currentCulture = cultureSegment;
                                }
                            }
                        }

                        var requestCulture = new ProviderCultureResult(currentCulture);
                        CultureInfo.CurrentCulture = new CultureInfo(currentCulture);
                        CultureInfo.CurrentUICulture = new CultureInfo(currentCulture);

                        return requestCulture;
                    }));
            });
        }
    }

    public static class RequestLocalizationOptionsExtensions
    {
        /// <summary>
        /// Adds a new <see cref="RequestCultureProvider"/> to the <see cref="RequestLocalizationOptions.RequestCultureProviders"/>.
        /// </summary>
        /// <param name="requestLocalizationOptions">The cultures to be added.</param>
        /// <param name="requestCultureProvider">The cultures to be added.</param>
        /// <returns>The <see cref="RequestLocalizationOptions"/>.</returns>
        /// <remarks>This method ensures that <paramref name="requestCultureProvider"/> has priority over other <see cref="RequestCultureProvider"/> instances in <see cref="RequestLocalizationOptions.RequestCultureProviders"/>.</remarks>
        public static RequestLocalizationOptions AddInitialRequestCultureProvider(
            this RequestLocalizationOptions requestLocalizationOptions,
            RequestCultureProvider requestCultureProvider)
        {
            if (requestLocalizationOptions == null)
            {
                throw new ArgumentNullException(nameof(requestLocalizationOptions));
            }

            if (requestCultureProvider == null)
            {
                throw new ArgumentNullException(nameof(requestCultureProvider));
            }

            requestLocalizationOptions.RequestCultureProviders.Insert(0, requestCultureProvider);

            return requestLocalizationOptions;
        }
    }
}
