using System.Collections.Generic;
using System.Security.Claims;

namespace  ModularArchitecture.Identity.Core
{
    public class ExternalLoginModel
    {
        public string ProviderKey { get; set; }
        public string LoginProvider { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public List<Claim> Claims { get; set; }
    }
}