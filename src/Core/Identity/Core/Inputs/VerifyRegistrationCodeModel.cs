using    ModularArchitecture.Identity.Core;

namespace  ModularArchitecture.Identity.Core
{
    public class VerifyRegistrationCodeModel : IVerifyRegistrationCodeModel
    {
        public string Cell { get; set; }
        public string Code { get; set; }
    }
}
