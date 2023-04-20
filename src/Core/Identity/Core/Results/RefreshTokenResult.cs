
using ModularArchitecture.Identity.Core.Result;

namespace  ModularArchitecture.Identity.Core
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
