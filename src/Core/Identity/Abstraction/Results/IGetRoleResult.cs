namespace   ModularArchitecture.Identity.Core
{
    public interface IGetRoleResult : IIdentityResult
    {
        public IRole Role { get; set; }
    }
}