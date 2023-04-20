using ModularArchitecture.Identity.Abstraction.Results;

namespace ModularArchitecture.Identity.Core.Results
{
    public class LoginResult : IdentityResult, ILoginResult
    {
        public LoginResult(string message)
        {
            Message = message;
        }

        public LoginResult(IAuthToken token, IUser user)
        {
            Token = token;
            User = user;
            Success = true;
        }

        public IAuthToken Token { get; set; }
        public IUser User { get; set; }
    }
}