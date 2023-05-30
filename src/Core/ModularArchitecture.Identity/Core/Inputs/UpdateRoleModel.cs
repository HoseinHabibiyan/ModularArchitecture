using ModularArchitecture.Identity.Abstraction.Inputs;

namespace ModularArchitecture.Identity.Core.Inputs
{
    public class UpdateRoleModel : IUpdateRoleModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public int SortOrder { get; set; }
        public string ApplicationId { get; set; }
    }
}
