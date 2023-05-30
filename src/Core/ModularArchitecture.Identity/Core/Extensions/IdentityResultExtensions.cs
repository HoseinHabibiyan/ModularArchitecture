using Microsoft.AspNetCore.Identity;
using System;

namespace ModularArchitecture.Identity.Core.Extensions
{
    public static class IdentityResultExtensions
    {
        public static string GetErrors(this IdentityResult identityResult)
        {
            var result = "";
            foreach (var error in identityResult.Errors)
            {
                result += error.Description + Environment.NewLine;
            }

            return result;
        }
    }
}
