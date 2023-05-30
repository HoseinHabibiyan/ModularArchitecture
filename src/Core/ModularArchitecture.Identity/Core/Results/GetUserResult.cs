using ModularArchitecture.Identity.Abstraction.Results;

namespace ModularArchitecture.Identity.Core.Results
{
    public class GetUserResult : IdentityResult, IGetUserResult
    {
        public IUser User { get; set; }
    }
}