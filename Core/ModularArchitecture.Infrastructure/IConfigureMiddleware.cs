using Microsoft.AspNetCore.Builder;

namespace ModularArchitecture.Infrastructure
{
  public interface IConfigureMiddleware
  {
    int Priority { get; }

    void Execute(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider);
  }
}