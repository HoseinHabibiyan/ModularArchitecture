namespace ModularArchitecture.Identity.Abstraction.Results
{
    public interface IRegisterResult : IIdentityResult
    {
        public IUser User { get; set; }
    }
}