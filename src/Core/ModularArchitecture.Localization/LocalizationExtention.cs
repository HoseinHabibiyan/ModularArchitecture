using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace ModularArchitecture.Infrastructure.Localization;

public static class LocalizationExtention
{
    public static void AddSihriLocalization(this IServiceCollection services)
    {
        services.AddLocalization();
        services.AddSingleton<IStringLocalizer, StringLocalizer>();
        services.AddSingleton<LocalizationMiddleware>();
        services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
    }

    public static IApplicationBuilder UseSihriLocalization(this IApplicationBuilder builder)
    {
        var options = new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(new CultureInfo("en-US")),
        };
        builder.UseRequestLocalization(options);
        return builder.UseMiddleware<LocalizationMiddleware>();
    }
}

