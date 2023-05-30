using ModularArchitecture.Identity.Abstraction.Results;

namespace ModularArchitecture.Identity.Core.Results
{
    public class RefreshTokenResult : IdentityResult, IRefreshTokenResult
    {
        public RefreshTokenResult(AuthToken token)
        {
            Token = token;
            Success = true;
        }

        public RefreshTokenResult(string message) : base(message)
        {
        }

        public IAuthToken Token { get; set; }
        public IUser User { get; set; }

    }
}
