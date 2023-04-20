using ModularArchitecture.Identity.Core.Result;

namespace  ModularArchitecture.Identity.Core
{
    public class GeneratePasswordResetTokenResult : IdentityResult, IGeneratePasswordResetTokenResult
    {
        public string Code { get; set; }
    }
}
