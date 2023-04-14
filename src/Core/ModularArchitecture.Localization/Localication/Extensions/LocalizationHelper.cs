using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.Localization;

namespace ModularArchitecture.Localization
{
    public static class LocalizationHelper
    {
        public static List<string> GetLocalizationFromKey([NotNull]this List<LocalizedString> localizedString,[NotNull]List<string> keys)
        {
            var messages = new List<string>();
            foreach (var key in keys)
            {
                var item = localizedString.FirstOrDefault(x => x.Name.ToLower().Trim() == key.ToLower().Trim());
                messages.Add(item?.Value);
            }

            return messages;
        }
        
        public static string GetLocalizationFromKey([NotNull]this List<LocalizedString> localizedString,[NotNull]string key)
        {
            var item = localizedString.FirstOrDefault(x => x.Name.ToLower().Trim() == key.ToLower().Trim());
            return item?.Value;
        }
    }
}