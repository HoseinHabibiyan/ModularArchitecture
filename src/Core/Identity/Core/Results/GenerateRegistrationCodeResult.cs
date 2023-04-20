using ModularArchitecture.Identity.Abstraction.Results;

namespace ModularArchitecture.Identity.Core.Results
{
    public class GenerateRegistrationCodeResult : IdentityResult, IGenerateRegistrationCodeResult
    {
        public int Code { get; set; }
        public string Cell { get; set; }
        public string Hashed { get; set; }
    }
}