namespace ModularArchitecture.Identity.Abstraction.Results
{
    public interface IGetRolesResult : IIdentityResult
    {
        public List<IRole> Roles { get; set; }
    }
}