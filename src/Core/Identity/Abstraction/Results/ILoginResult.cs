namespace   ModularArchitecture.Identity.Core
{
    public interface ILoginResult : IIdentityResult
    {
        public IAuthToken Token { get; set; }
        public IUser User { get; set; }
    }
}