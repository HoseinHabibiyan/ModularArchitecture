using ModularArchitecture.Identity.Abstraction.Results;

namespace ModularArchitecture.Identity.Core.Results
{
    public class GetRoleResult : IdentityResult, IGetRoleResult
    {
        public IRole Role { get; set; }
    }
}