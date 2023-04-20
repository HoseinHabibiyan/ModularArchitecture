using System.Security.Claims;

namespace ModularArchitecture.Identity.Core
{
    public static class IdentityExtensions
    {
        public static string GetUserId(this System.Security.Principal.IIdentity identity)
        {
            return identity is ClaimsIdentity identity1 ? identity1.FindFirst(ClaimTypes.NameIdentifier)?.Value : null;
        }

        public static List<string> GetRoles(this System.Security.Principal.IIdentity identity)
        {
            return identity is ClaimsIdentity identity1 ? identity1.FindAll(ClaimTypes.Role)?.Select(c => c.Value)
                .Distinct()
                .ToList() : new List<string>();
        }

        public static string GetUserFullName(this System.Security.Principal.IIdentity identity)
        {
            return identity is ClaimsIdentity identity1 ? identity1.FindFirst(ClaimTypes.GivenName)?.Value : null;
        }
    }
}
