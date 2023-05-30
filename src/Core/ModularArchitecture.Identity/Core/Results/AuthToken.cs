using ModularArchitecture.Identity.Abstraction.Results;

namespace ModularArchitecture.Identity.Core.Results
{
    public class AuthToken : IAuthToken
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}