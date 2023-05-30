using ModularArchitecture.Identity.Abstraction.Results;

namespace ModularArchitecture.Identity.Core.Results
{
    public class GeneratePasswordResetTokenResult : IdentityResult, IGeneratePasswordResetTokenResult
    {
        public string Code { get; set; }
    }
}
