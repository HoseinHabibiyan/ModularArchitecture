using   ModularArchitecture.Identity.Core;

namespace  ModularArchitecture.Identity.Core
{
    public interface IUpdateRoleModel : IRole
    {
        string ApplicationId { get; set; }
    }
}