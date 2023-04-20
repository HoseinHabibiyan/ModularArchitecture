
namespace  ModularArchitecture.Identity.Core
{
    public class AuthToken : IAuthToken
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}