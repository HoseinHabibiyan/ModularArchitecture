using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Reflection;

namespace ModularArchitecture.Infrastructure
{
    public static class ExtensionManager
    {
        private static IEnumerable<Assembly> assemblies;
        private static ConcurrentDictionary<Type, IEnumerable<Type>> types;

        /// <summary>
        /// Gets the cached assemblies that have been set by the SetAssemblies method.
        /// </summary>
        public static IEnumerable<Assembly> Assemblies
        {
            get
            {
                return assemblies;
            }
        }


        /// <summary>
        /// Gets the implementations of the type specified by the type parameter and located in the assemblies
        /// filtered by the predicate.
        /// </summary>
        /// <typeparam name="T">The type parameter to find implementations of.</typeparam>
        /// <param name="predicate">The predicate to filter the assemblies.</param>
        /// <param name="useCaching">
        ///     Determines whether the type cache should be used to avoid assemblies scanning next time,
        ///     when the same type(s) is requested.
        /// </param>
        /// <param name="logger"></param>
        /// <returns>Found implementations of the given type.</returns>
        public static IEnumerable<Type> GetImplementations<T>(Func<Assembly, bool> predicate, bool useCaching = false,
            ILogger logger = null)
        {
            Type type = typeof(T);

            logger?.LogInformation("GetImplementations of type: " + type.FullName);

            if (useCaching && types.ContainsKey(type))
                return types[type];

            List<Type> implementations = new List<Type>();

            foreach (Assembly assembly in GetAssemblies(predicate))
            {
                foreach (Type exportedType in assembly.GetExportedTypes())
                {
                    if (type.GetTypeInfo().IsAssignableFrom(exportedType) && exportedType.GetTypeInfo().IsClass)
                        implementations.Add(exportedType);
                }
            }

            if (useCaching)
                types[type] = implementations;

            return implementations;
        }



        /// <summary>
        /// Gets the new instance (using constructor that matches the arguments) of the first implementation
        /// of the type specified by the type parameter and located in the assemblies filtered by the predicate
        /// or null if no implementations found.
        /// </summary>
        /// <typeparam name="T">The type parameter to find implementation of.</typeparam>
        /// <param name="predicate">The predicate to filter the assemblies.</param>
        /// <param name="useCaching">
        /// Determines whether the type cache should be used to avoid assemblies scanning next time,
        /// when the instance(s) of the same type(s) is requested.
        /// </param>
        /// <param name="args">The arguments to be passed to the constructor.</param>
        /// <returns>The instance of the first found implementation of the given type.</returns>
        public static T GetInstance<T>(Func<Assembly, bool> predicate, bool useCaching = false, params object[] args)
        {
            return GetInstances<T>(predicate, useCaching, null, args).FirstOrDefault();
        }

        /// <summary>
        /// Gets the new instances of the implementations of the type specified by the type parameter
        /// or empty enumeration if no implementations found.
        /// </summary>
        /// <typeparam name="T">The type parameter to find implementations of.</typeparam>
        /// <param name="useCaching">
        /// Determines whether the type cache should be used to avoid assemblies scanning next time,
        /// when the instance(s) of the same type(s) is requested.
        /// </param>
        /// <returns>The instances of the found implementations of the given type.</returns>
        public static IEnumerable<T> GetInstances<T>(bool useCaching = false)
        {
            return GetInstances<T>(null, useCaching, null, new object[] { });
        }


        /// <summary>
        /// Gets the new instances (using constructor that matches the arguments) of the implementations
        /// of the type specified by the type parameter and located in the assemblies filtered by the predicate
        /// or empty enumeration if no implementations found.
        /// </summary>
        /// <typeparam name="T">The type parameter to find implementations of.</typeparam>
        /// <param name="predicate">The predicate to filter the assemblies.</param>
        /// <param name="useCaching">
        /// Determines whether the type cache should be used to avoid assemblies scanning next time,
        /// when the instance(s) of the same type(s) is requested.
        /// </param>
        /// <param name="args">The arguments to be passed to the constructors.</param>
        /// <returns>The instances of the found implementations of the given type.</returns>
        public static IEnumerable<T> GetInstances<T>(Func<Assembly, bool> predicate, bool useCaching = false, ILogger logger = null, params object[] args)
        {
            List<T> instances = new List<T>();

            foreach (Type implementation in GetImplementations<T>(predicate, useCaching, logger))
            {
                if (!implementation.GetTypeInfo().IsAbstract)
                {
                    T instance = (T)Activator.CreateInstance(implementation, args);

                    instances.Add(instance);
                }
            }

            return instances;
        }

        private static IEnumerable<Assembly> GetAssemblies(Func<Assembly, bool> predicate)
        {
            if (predicate == null)
                return new Assembly[] { };

            return Assemblies.Where(predicate);
        }
    }
}