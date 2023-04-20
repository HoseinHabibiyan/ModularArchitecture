
using ModularArchitecture.Identity.Core.Result;

namespace  ModularArchitecture.Identity.Core
{
    public class GetRoleResult : IdentityResult, IGetRoleResult
    {
        public IRole Role { get; set; }
    }
}