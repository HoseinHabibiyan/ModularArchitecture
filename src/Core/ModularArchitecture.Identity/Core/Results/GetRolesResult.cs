using ModularArchitecture.Identity.Abstraction.Results;

namespace ModularArchitecture.Identity.Core.Results
{
    public class GetRolesResult : IdentityResult, IGetRolesResult
    {
        public List<IRole> Roles { get; set; }
    }
}