using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ModularArchitecture.Identity.Core.Extensions
{
    public static class HttpContextAccessorExtensions
    {
        public static string GetUserId(this IHttpContextAccessor httpContextAccessor)
        {
            var claims = httpContextAccessor.HttpContext.User.Claims.ToList();
            var userId = claims.Any() ? claims.First(i => i.Type == ClaimTypes.NameIdentifier)?.Value : null;

            return userId;
        }

        public static List<string> GetRoles(this IHttpContextAccessor httpContextAccessor)
        {
            var claims = httpContextAccessor.HttpContext.User.Claims;

            return claims.Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .Distinct()
                .ToList();
        }

        public static string GetUserFullName(this IHttpContextAccessor httpContextAccessor)
        {
            var claims = httpContextAccessor.HttpContext.User.Claims;

            return claims.Where(c => c.Type == ClaimTypes.GivenName)
                .Select(c => c.Value)
                .Distinct()
                .FirstOrDefault();
        }

        public static string GetUserEmail(this IHttpContextAccessor httpContextAccessor)
        {
            var claims = httpContextAccessor.HttpContext.User.Claims;

            return claims.Where(c => c.Type == ClaimTypes.Email)
                .Select(c => c.Value)
                .Distinct()
                .FirstOrDefault();
        }

        public static bool IsSuperAdmin(this IHttpContextAccessor httpContextAccessor)
        {
            var roles = httpContextAccessor.GetRoles();
            return roles.Contains("SuperAdmin");
        }

        public static bool IsAdmin(this IHttpContextAccessor httpContextAccessor)
        {
            var roles = httpContextAccessor.GetRoles();
            return roles.Contains("Admin");
        }

        public static bool IsOperator(this IHttpContextAccessor httpContextAccessor)
        {
            var roles = httpContextAccessor.GetRoles();
            return roles.Contains("Operator");
        }

        public static bool IsAdminOrSuperAdmin(this IHttpContextAccessor httpContextAccessor)
        {
            return httpContextAccessor.IsSuperAdmin() || httpContextAccessor.IsAdmin();
        }

        public static bool IsAdminOrSuperAdminOrOperator(this IHttpContextAccessor httpContextAccessor)
        {
            return httpContextAccessor.IsSuperAdmin() || httpContextAccessor.IsAdmin() || httpContextAccessor.IsOperator();
        }
    }
}
