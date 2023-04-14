using System;
using System.Collections.Generic;

namespace ModularArchitecture.Localization
{
    public interface IResourceNamesCache
    {
        IList<string> GetOrAdd(string name, Func<string, IList<string>> valueFactory);
    }
}
