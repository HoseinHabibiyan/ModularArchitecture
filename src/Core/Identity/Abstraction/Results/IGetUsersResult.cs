using System.Collections.Generic;

namespace ModularArchitecture.Identity.Abstraction.Results
{
    public interface IGetUsersResult : IIdentityResult
    {
        public List<IUser> Users { get; set; }
    }
}