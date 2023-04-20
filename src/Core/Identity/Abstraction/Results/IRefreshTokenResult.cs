namespace   ModularArchitecture.Identity.Core
{
    public interface IRefreshTokenResult : IIdentityResult
    {
        public IAuthToken Token { get; set; }
        public IUser User { get; set; }
    }
}