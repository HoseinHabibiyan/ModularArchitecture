using ModularArchitecture.Identity.Core.Result;

namespace ModularArchitecture.Identity.Core
{
    public class GetUsersResult : IdentityResult, IGetUsersResult
    {
        public List<IUser> Users { get; set; }
    }
}