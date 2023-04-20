
namespace  ModularArchitecture.Identity.Core
{
    public class UsersInRoleModel : IUsersInRoleModel
    {
        public string Id { get; set; }
        public List<string> EnrolledUsers { get; set; }
        public List<string> RemovedUsers { get; set; }
    }
}
