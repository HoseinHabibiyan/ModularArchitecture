namespace   ModularArchitecture.Identity.Core
{
    public interface IGetUserResult : IIdentityResult
    {
        public IUser User { get; set; }
    }
}