using Microsoft.AspNetCore.Builder;

namespace ModularArchitecture.Infrastructure
{
    public static class MiddlewareExtensions
  {
    public static void UseAppMiddlewares(this IApplicationBuilder applicationBuilder)
    {
      foreach (IConfigureMiddleware action in ExtensionManager.GetInstances<IConfigureMiddleware>().OrderBy(a => a.Priority))
      {
        action.Execute(applicationBuilder, applicationBuilder.ApplicationServices);
      }
    }
  }
}