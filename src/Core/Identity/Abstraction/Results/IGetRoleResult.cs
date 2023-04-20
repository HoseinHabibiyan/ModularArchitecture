namespace ModularArchitecture.Identity.Abstraction.Results
{
    public interface IGetRoleResult : IIdentityResult
    {
        public IRole Role { get; set; }
    }
}