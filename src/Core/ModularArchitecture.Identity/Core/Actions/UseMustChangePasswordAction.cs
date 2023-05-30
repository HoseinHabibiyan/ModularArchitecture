using System;
using Microsoft.AspNetCore.Builder;
using ModularArchitecture.Identity.Core.Extensions;
using ModularArchitecture.Infrastructure;

namespace ModularArchitecture.Identity.Core.Actions
{
    public class UseMustChangePasswordAction : IConfigureMiddleware
    {
        public int Priority => 10030;

        public void Execute(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider)
        {
            applicationBuilder.UseMustChangePassword();
        }
    }
}
