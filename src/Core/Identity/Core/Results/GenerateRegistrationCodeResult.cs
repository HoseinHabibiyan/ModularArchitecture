
using ModularArchitecture.Identity.Core.Result;

namespace  ModularArchitecture.Identity.Core
{
    public class GenerateRegistrationCodeResult : IdentityResult, IGenerateRegistrationCodeResult
    {
        public int Code { get; set; }
        public string Cell { get; set; }
        public string Hashed { get; set; }
    }
}