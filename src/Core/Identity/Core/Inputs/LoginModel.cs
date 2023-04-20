using ModularArchitecture.Identity.Abstraction.Inputs;
using System.Collections.Generic;
using System.Security.Claims;

namespace ModularArchitecture.Identity.Core.Inputs
{
    public class LoginModel : ILoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public List<Claim> Claims { get; set; }
    }
}
