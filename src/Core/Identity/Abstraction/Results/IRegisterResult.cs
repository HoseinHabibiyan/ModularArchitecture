namespace   ModularArchitecture.Identity.Core
{
    public interface IRegisterResult : IIdentityResult
    {
        public IUser User { get; set; }
    }
}