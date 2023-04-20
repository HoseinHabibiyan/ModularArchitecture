namespace ModularArchitecture.Localization.Json.Caching
{
    public interface IResourceNamesCache
    {
        IList<string> GetOrAdd(string name, Func<string, IList<string>> valueFactory);
    }
}
