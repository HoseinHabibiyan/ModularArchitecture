using ModularArchitecture.Identity.Core.Result;

namespace ModularArchitecture.Identity.Core
{
    public class RegisterResult : IdentityResult, IRegisterResult
    {
        public IUser User { get; set; }
        public string EmailConfirmLink { get; set; }
    }
}