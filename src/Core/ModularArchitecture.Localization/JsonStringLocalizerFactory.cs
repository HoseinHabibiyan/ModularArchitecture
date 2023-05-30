using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;

namespace ModularArchitecture.Infrastructure.Localization;
public class JsonStringLocalizerFactory : IStringLocalizerFactory
{
    private readonly IMemoryCache _cache;
    private readonly IWebHostEnvironment _env;

    public JsonStringLocalizerFactory(IMemoryCache cache, IWebHostEnvironment env)
    {
        _cache = cache;
        _env = env;
    }

    public IStringLocalizer Create(Type resourceSource) =>
        new JsonStringLocalizer(_cache, _env);

    public IStringLocalizer Create(string baseName, string location) =>
        new JsonStringLocalizer(_cache, _env);
}

