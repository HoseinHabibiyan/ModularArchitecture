namespace ModularArchitecture.Identity.Abstraction.Results
{
    public interface IGetUserResult : IIdentityResult
    {
        public IUser User { get; set; }
    }
}