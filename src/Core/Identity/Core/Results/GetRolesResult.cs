
using ModularArchitecture.Identity.Core.Result;

namespace ModularArchitecture.Identity.Core
{
    public class GetRolesResult : IdentityResult, IGetRolesResult
    {
        public List<IRole> Roles { get; set; }
    }
}