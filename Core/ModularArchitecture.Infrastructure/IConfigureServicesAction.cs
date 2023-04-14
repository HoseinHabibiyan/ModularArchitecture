using Microsoft.Extensions.DependencyInjection;

namespace ModularArchitecture.Infrastructure { 

  public interface IConfigureServicesAction
  {
    int Priority { get; }
    void Execute(IServiceCollection services, IServiceProvider serviceProvider);
  }
}