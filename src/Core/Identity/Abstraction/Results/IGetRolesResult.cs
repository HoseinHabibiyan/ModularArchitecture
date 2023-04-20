namespace ModularArchitecture.Identity.Core
{
    public interface IGetRolesResult : IIdentityResult
    {
        public List<IRole> Roles { get; set; }
    }
}