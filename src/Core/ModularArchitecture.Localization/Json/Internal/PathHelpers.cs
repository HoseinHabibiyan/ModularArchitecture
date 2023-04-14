using System.IO;
using System.Reflection;

namespace ModularArchitecture.Localization
{
    public static class PathHelpers
    {
        public static string GetApplicationRoot()
            => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }
}