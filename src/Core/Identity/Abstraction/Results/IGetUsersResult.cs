using System.Collections.Generic;

namespace  ModularArchitecture.Identity.Core
{
    public interface IGetUsersResult : IIdentityResult
    {
        public List<IUser> Users { get; set; }
    }
}