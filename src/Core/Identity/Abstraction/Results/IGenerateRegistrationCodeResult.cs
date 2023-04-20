namespace   ModularArchitecture.Identity.Core
{
    public interface IGenerateRegistrationCodeResult : IIdentityResult
    {
        public int Code { get; set; }
        public string Cell { get; set; }
        public string Hashed { get; set; }
    }
}