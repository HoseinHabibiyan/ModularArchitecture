namespace ModularArchitecture.Identity.Abstraction.Results
{
    public interface IGenerateRegistrationCodeResult : IIdentityResult
    {
        public int Code { get; set; }
        public string Cell { get; set; }
        public string Hashed { get; set; }
    }
}