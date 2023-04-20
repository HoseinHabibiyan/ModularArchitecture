namespace ModularArchitecture.Identity.Abstraction.Results
{
    public interface IGeneratePasswordResetTokenResult : IIdentityResult
    {
        public string Code { get; set; }
    }
}