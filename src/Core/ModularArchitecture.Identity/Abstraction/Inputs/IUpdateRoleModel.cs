using ModularArchitecture.Identity.Abstraction.Results;

namespace ModularArchitecture.Identity.Abstraction.Inputs
{
    public interface IUpdateRoleModel : IRole
    {
        string ApplicationId { get; set; }
    }
}