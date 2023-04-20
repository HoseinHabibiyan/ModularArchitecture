using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModularArchitecture.Identity.EntityFramework;
using ModularArchitecture.Infrastructure;

namespace ModularArchitecture.Identity.EntityFramework.Extensions
{
    /// <summary>
    /// Contains the extension methods of the <see cref="IIdentityContext">IStorageContext</see> interface.
    /// </summary>
    public static class IdentityContextExtensions
    {
        /// <summary>
        /// Registers the entities from all the extensions inside the single Entity Framework storage context
        /// by finding all the implementations of the <see cref="IIdentityEntityRegistrar">IEntityRegistrar</see> interface.
        /// </summary>
        /// <param name="identityContext">The Entity Framework identity context.</param>
        /// <param name="modelBuilder">The Entity Framework model builder.</param>
        /// <param name="logger"></param>
        public static void RegisterEntities(this IIdentityContext identityContext, ModelBuilder modelBuilder,
            ILogger logger)
        {
            foreach (IIdentityEntityRegistrar entityRegistrar in ExtensionManager.GetInstances<IIdentityEntityRegistrar>(null, false, logger))
            {
                logger.LogError(entityRegistrar.GetType().FullName);
                entityRegistrar.RegisterEntities(modelBuilder);
            }
        }
    }
}