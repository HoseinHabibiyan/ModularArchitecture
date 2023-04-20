using ModularArchitecture.Identity.Core.Result;

namespace ModularArchitecture.Identity.Core
{
    public class GetUserResult : IdentityResult, IGetUserResult
    {
        public IUser User { get; set; }
    }
}