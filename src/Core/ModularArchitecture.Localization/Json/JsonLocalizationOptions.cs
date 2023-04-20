using Microsoft.Extensions.Localization;

namespace ModularArchitecture.Localization.Json
{
    public class JsonLocalizationOptions : LocalizationOptions
    {
        public ResourcesType ResourcesType { get; set; } = ResourcesType.TypeBased;
    }
}