using ModularArchitecture.Identity.Abstraction.Inputs;

namespace ModularArchitecture.Identity.Core.Inputs
{
    public class VerifyRegistrationCodeModel : IVerifyRegistrationCodeModel
    {
        public string Cell { get; set; }
        public string Code { get; set; }
    }
}
