using Microsoft.Extensions.Localization;

namespace ModularArchitecture.Localization
{
    public class JsonLocalizationOptions : LocalizationOptions
    {
        public ResourcesType ResourcesType { get; set; } = ResourcesType.TypeBased;
    }
}