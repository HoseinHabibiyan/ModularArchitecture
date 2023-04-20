namespace   ModularArchitecture.Identity.Core
{
    public interface IGeneratePasswordResetTokenResult:IIdentityResult
    {
        public string Code { get; set; }
    }
}