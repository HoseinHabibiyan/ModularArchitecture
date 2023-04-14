using Microsoft.Extensions.DependencyInjection;

namespace ModularArchitecture.Infrastructure
{
    /// <summary>
    /// Contains the extension methods of the <see cref="IServiceCollection">IServiceCollection</see> interface.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static void AddAppServices(this IServiceCollection services)
        {
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            foreach (IConfigureServicesAction action in ExtensionManager.GetInstances<IConfigureServicesAction>().OrderBy(a => a.Priority))
            {
                action.Execute(services, serviceProvider);
                serviceProvider = services.BuildServiceProvider();
            }
        }
    }
}