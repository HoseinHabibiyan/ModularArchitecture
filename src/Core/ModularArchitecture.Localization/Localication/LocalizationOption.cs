using Microsoft.AspNetCore.Localization;
using System.Collections.Generic;

namespace ModularArchitecture.Localization
{
    public class LocalizationOption
    {
        public string[] SupportedCultures { get; set; }
        public string DefaultCulture { get; set; }
        public IList<IRequestCultureProvider> RequestCultureProviders { get; set; } = new List<IRequestCultureProvider>
        {
            new QueryStringRequestCultureProvider(),
            new CookieRequestCultureProvider(),
            new AcceptLanguageHeaderRequestCultureProvider()
        };
    }
}
