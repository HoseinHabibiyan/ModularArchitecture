using ModularArchitecture.Identity.Abstraction.Results;

namespace ModularArchitecture.Identity.Core.Results
{
    public class RegisterResult : IdentityResult, IRegisterResult
    {
        public IUser User { get; set; }
        public string EmailConfirmLink { get; set; }
    }
}