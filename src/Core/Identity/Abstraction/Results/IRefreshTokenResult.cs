namespace ModularArchitecture.Identity.Abstraction.Results
{
    public interface IRefreshTokenResult : IIdentityResult
    {
        public IAuthToken Token { get; set; }
        public IUser User { get; set; }
    }
}