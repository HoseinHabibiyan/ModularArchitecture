namespace ModularArchitecture.Identity.Abstraction.Inputs
{
    public interface IVerifyRegistrationCodeModel
    {
        public string Cell { get; set; }
        public string Code { get; set; }
    }
}