using ModularArchitecture.Identity.Abstraction.Results;

namespace ModularArchitecture.Identity.Core.Results
{
    public class GetUsersResult : IdentityResult, IGetUsersResult
    {
        public List<IUser> Users { get; set; }
    }
}