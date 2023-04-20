using Microsoft.AspNetCore.Identity;

namespace  ModularArchitecture.Identity.Core
{
    public class UserRole
    {
        public string UserId { get; set; }
        public IdentityRole Role { get; set; }
    }
}
