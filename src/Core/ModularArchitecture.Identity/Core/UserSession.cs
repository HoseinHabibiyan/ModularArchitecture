
using ModularArchitecture.Identity.Abstraction;

namespace ModularArchitecture.Identity.Core
{
    public class UserSession : IUserSession
    {
        public string Id { get; set; }
        public List<string> Roles { get; set; }
        public string UserName { get; set; }
        public bool DisableSoftDeleteFilter { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
    }
}