using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace ModularArchitecture.Localization
{
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IResourceNamesCache _resourceNamesCache = new ResourceNamesCache();
        private readonly ConcurrentDictionary<string, JsonStringLocalizer> _localizerCache = new ConcurrentDictionary<string, JsonStringLocalizer>();
        private readonly string _resourcesRelativePath;
        private readonly ResourcesType _resourcesType = ResourcesType.TypeBased;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IWebHostEnvironment _env;

        public JsonStringLocalizerFactory(
            IOptions<JsonLocalizationOptions> localizationOptions,
            ILoggerFactory loggerFactory,
            IWebHostEnvironment env)
        {
            if (localizationOptions == null)
            {
                throw new ArgumentNullException(nameof(localizationOptions));
            }

            _resourcesRelativePath = localizationOptions.Value.ResourcesPath ?? string.Empty;
            _resourcesType = localizationOptions.Value.ResourcesType;
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _env = env;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            if (resourceSource == null)
            {
                throw new ArgumentNullException(nameof(resourceSource));
            }

            string resourcesPath = string.Empty;
            string extensionResourcesPath = string.Empty;

            // TODO: Check why an exception happen before the host build
            if (resourceSource.Name == "Controller")
            {
                resourcesPath = Path.Combine(PathHelpers.GetApplicationRoot(), GetResourcePath(resourceSource.Assembly));
                extensionResourcesPath = Path.Combine(Path.Combine(_env.ContentRootPath,"Resources"), GetResourcePath(resourceSource.Assembly));

                return _localizerCache.GetOrAdd(resourceSource.Name,
                    _ => CreateJsonStringLocalizer(resourcesPath, extensionResourcesPath,
                        TryFixInnerClassPath("Controller")));
            }

            var typeInfo = resourceSource.GetTypeInfo();
            var assembly = typeInfo.Assembly;
            var assemblyName = resourceSource.Assembly.GetName().Name;
            var typeName= typeInfo.FullName;

            try
            {
                typeName = $"{assemblyName}.{typeInfo.Name}" == typeInfo.FullName
                    ? typeInfo.Name
                    : typeInfo.FullName.Substring(assemblyName.Length + 1);
            }
            catch (Exception e)
            {
                var logger = _loggerFactory.CreateLogger<JsonStringLocalizer>();
                logger.LogError(e, e.Message);
            }

            resourcesPath = Path.Combine(PathHelpers.GetApplicationRoot(), GetResourcePath(assembly));
            extensionResourcesPath = Path.Combine(Path.Combine(_env.ContentRootPath, "Resources"), GetResourcePath(assembly));
            typeName = TryFixInnerClassPath(typeName);

            return _localizerCache.GetOrAdd($"culture={CultureInfo.CurrentUICulture.Name}, typeName={typeName}",
                _ => CreateJsonStringLocalizer(resourcesPath, extensionResourcesPath, typeName));
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            if (baseName == null)
            {
                throw new ArgumentNullException(nameof(baseName));
            }

            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            return _localizerCache.GetOrAdd($"baseName={baseName},location={location}", _ =>
            {
                var assemblyName = new AssemblyName(location);
                var assembly = Assembly.Load(assemblyName);
                var resourcesPath = Path.Combine(PathHelpers.GetApplicationRoot(), GetResourcePath(assembly));
                var extensionResourcesPath = Path.Combine(Path.Combine(_env.ContentRootPath, "Resources"), GetResourcePath(assembly));
                string resourceName = null;
                if (baseName == string.Empty)
                {
                    resourceName = baseName;

                    return CreateJsonStringLocalizer(resourcesPath, extensionResourcesPath, resourceName);
                }

                if (_resourcesType == ResourcesType.TypeBased)
                {
                    baseName = TryFixInnerClassPath(baseName);
                    resourceName = TrimPrefix(baseName, location + ".");
                }

                return CreateJsonStringLocalizer(resourcesPath, extensionResourcesPath, resourceName);
            });
        }

        protected virtual JsonStringLocalizer CreateJsonStringLocalizer(string resourcesPath,
            string extensionResourcesPath,
            string resourceName)
        {
            var resourceManager = _resourcesType == ResourcesType.TypeBased
                ? new JsonResourceManager(resourcesPath, extensionResourcesPath, resourceName)
                : new JsonResourceManager(resourcesPath, extensionResourcesPath);
            var logger = _loggerFactory.CreateLogger<JsonStringLocalizer>();

            return new JsonStringLocalizer(resourceManager, _resourceNamesCache, logger);
        }

        private string GetResourcePath(Assembly assembly)
        {
            var resourceLocationAttribute = assembly.GetCustomAttribute<ResourceLocationAttribute>();

            return resourceLocationAttribute == null
                ? _resourcesRelativePath
                : resourceLocationAttribute.ResourceLocation;
        }

        private static string TrimPrefix(string name, string prefix)
        {
            if (name.StartsWith(prefix, StringComparison.Ordinal))
            {
                return name.Substring(prefix.Length);
            }

            return name;
        }

        private string TryFixInnerClassPath(string path)
        {
            const char innerClassSeparator = '+';
            var fixedPath = path;

            if (path.Contains(innerClassSeparator.ToString()))
            {
                fixedPath = path.Replace(innerClassSeparator, '.');
            }

            return fixedPath;
        }
    }
}