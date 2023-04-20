using System.Collections.Generic;
using System.Security.Claims;

namespace ModularArchitecture.Identity.Abstraction.Inputs
{
    public interface ILoginModel
    {
        string Username { get; set; }
        string Password { get; set; }
        string ClientId { get; set; }
        string ClientSecret { get; set; }
        List<Claim> Claims { get; set; }
    }
}