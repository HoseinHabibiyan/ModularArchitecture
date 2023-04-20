using Microsoft.AspNetCore.Identity;

namespace ModularArchitecture.Identity.Core.Results
{
    public class UserRole
    {
        public string UserId { get; set; }
        public IdentityRole Role { get; set; }
    }
}
