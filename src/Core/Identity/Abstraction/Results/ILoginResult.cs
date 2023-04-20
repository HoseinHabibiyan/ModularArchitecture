namespace ModularArchitecture.Identity.Abstraction.Results
{
    public interface ILoginResult : IIdentityResult
    {
        public IAuthToken Token { get; set; }
        public IUser User { get; set; }
    }
}