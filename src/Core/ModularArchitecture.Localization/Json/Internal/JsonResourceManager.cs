﻿using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ModularArchitecture.Localization
{
    public class JsonResourceManager
    {
        private ConcurrentDictionary<string, ConcurrentDictionary<string, string>> _resourcesCache = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();

        public JsonResourceManager(string resourcesPath, string extensionResourcesPath, string resourceName = null)
        {
            ResourcesPath = resourcesPath;
            ExtensionResourcesPath = extensionResourcesPath;
            ResourceName = resourceName;
        }

        public string ResourceName { get; }

        public string ResourcesPath { get; }
        public string ExtensionResourcesPath { get; }

        public string ResourcesFilePath { get; private set; }

        public virtual ConcurrentDictionary<string, string> GetResourceSet(CultureInfo culture, bool tryParents)
        {
            TryLoadResourceSet(culture);

            if (!_resourcesCache.ContainsKey(culture.Name))
            {
                return null;
            }

            if (tryParents)
            {
                var allResources = new ConcurrentDictionary<string, string>();
                do
                {
                    if (_resourcesCache.TryGetValue(culture.Name, out ConcurrentDictionary<string, string> resources))
                    {
                        foreach (var entry in resources)
                        {
                            allResources.TryAdd(entry.Key, entry.Value);
                        }
                    }

                    culture = culture.Parent;
                } while (culture != CultureInfo.InvariantCulture);

                return allResources;
            }
            else
            {
                _resourcesCache.TryGetValue(culture.Name, out ConcurrentDictionary<string, string> resources);

                return resources;
            }
        }

        public virtual string GetString(string name)
        {
            var culture = CultureInfo.CurrentUICulture;
            GetResourceSet(culture, tryParents: true);

            if (_resourcesCache.Count == 0)
            {
                return null;
            }

            do
            {
                if (_resourcesCache.ContainsKey(culture.Name))
                {
                    if (_resourcesCache[culture.Name].TryGetValue(name, out string value))
                    {
                        return value;
                    }
                }

                culture = culture.Parent;
            } while (culture != culture.Parent);

            return null;
        }

        public virtual string GetString(string name, CultureInfo culture)
        {
            GetResourceSet(culture, tryParents: true);

            if (_resourcesCache.Count == 0)
            {
                return null;
            }

            if (!_resourcesCache.ContainsKey(culture.Name))
            {
                return null;
            }

            return _resourcesCache[culture.Name].TryGetValue(name, out string value)
                ? value
                : null;
        }

        private void TryLoadResourceSet(CultureInfo culture)
        {
            if (string.IsNullOrEmpty(ResourceName))
            {
                var file = Path.Combine(ResourcesPath, $"{culture.Name}.json");
                var resources = LoadJsonResources(file);
                if (!resources.Any())
                {
                    file = Path.Combine(ExtensionResourcesPath, $"{culture.Name}.json");
                    resources = LoadJsonResources(file);
                }
                _resourcesCache.TryAdd(culture.Name, new ConcurrentDictionary<string, string>(resources.ToDictionary(r => r.Key, r => r.Value)));
            }
            else
            {
                List<string> resourceFiles = new List<string>();
                var rootCulture = culture.Name.Substring(0, 2);
                if (ResourceName.Contains("."))
                {
                    try
                    {
                        resourceFiles = Directory.EnumerateFiles(ResourcesPath, $"{ResourceName}.{rootCulture}*.json").ToList();
                    }
                    catch (Exception e)
                    {

                    }

                    if (!resourceFiles.Any())
                    {
                        resourceFiles = GetResourceFiles(rootCulture).ToList();

                        if (!resourceFiles.Any())
                        {
                            resourceFiles = Directory.EnumerateFiles(ExtensionResourcesPath, $"{ResourceName}.{rootCulture}*.json", SearchOption.AllDirectories).ToList();

                            if (!resourceFiles.Any())
                            {
                                resourceFiles = GetExtensionResourceFiles(rootCulture).ToList();
                            }
                        }
                    }
                }
                else
                {
                    resourceFiles = GetResourceFiles(rootCulture).ToList();
                }

                foreach (var file in resourceFiles)
                {
                    var resources = LoadJsonResources(file);
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    var cultureName = fileName.Substring(fileName.LastIndexOf(".") + 1);

                    culture = CultureInfo.GetCultureInfo(cultureName);

                    if (_resourcesCache.ContainsKey(culture.Name))
                    {
                        foreach (var resource in resources)
                        {
                            _resourcesCache[culture.Name].TryAdd(resource.Key, resource.Value);
                        }
                    }
                    else
                    {
                        _resourcesCache.TryAdd(culture.Name, new ConcurrentDictionary<string, string>(resources.ToDictionary(r => r.Key, r => r.Value)));
                    }
                }
            }

            IEnumerable<string> GetResourceFiles(string culture)
            {
                var resourcePath = ResourceName.Replace('.', Path.AltDirectorySeparatorChar);
                var resourcePathLastDirectorySeparatorIndex = resourcePath.LastIndexOf(Path.AltDirectorySeparatorChar);
                var resourceName = resourcePath.Substring(resourcePathLastDirectorySeparatorIndex + 1);
                string resourcesPath = null;
                if (resourcePathLastDirectorySeparatorIndex == -1)
                {
                    resourcesPath = ResourcesPath;
                }
                else
                {
                    resourcesPath = Path.Combine(ResourcesPath, resourcePath.Substring(0, resourcePathLastDirectorySeparatorIndex));
                }

                return Directory.Exists(resourcesPath)
                    ? Directory.EnumerateFiles(resourcesPath, $"{resourceName}.{culture}*.json")
                    : Enumerable.Empty<string>();
            }

            IEnumerable<string> GetExtensionResourceFiles(string culture)
            {
                var resourcePath = ResourceName.Replace('.', Path.AltDirectorySeparatorChar);
                var resourcePathLastDirectorySeparatorIndex = resourcePath.LastIndexOf(Path.AltDirectorySeparatorChar);
                var resourceName = resourcePath.Substring(resourcePathLastDirectorySeparatorIndex + 1);
                string resourcesPath = null;
                if (resourcePathLastDirectorySeparatorIndex == -1)
                {
                    resourcesPath = ResourcesPath;
                }
                else
                {
                    resourcesPath = Path.Combine(ExtensionResourcesPath, resourcePath.Substring(0, resourcePathLastDirectorySeparatorIndex));
                }

                return Directory.Exists(resourcesPath)
                    ? Directory.EnumerateFiles(resourcesPath, $"{resourceName}.{culture}*.json")
                    : Enumerable.Empty<string>();
            }
        }

        private static IDictionary<string, string> LoadJsonResources(string filePath)
        {
            if (File.Exists(filePath))
            {
                var builder = new ConfigurationBuilder()
                    .AddJsonFile(filePath, optional: true, reloadOnChange: true);

                return new Dictionary<string, string>(builder.Build().AsEnumerable());
            }
            else
            {
                return new Dictionary<string, string>();
            }
        }
    }
}
