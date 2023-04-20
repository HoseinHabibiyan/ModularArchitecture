using System.Collections.Generic;

namespace ModularArchitecture.Identity
{
    public interface IUserSession
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
