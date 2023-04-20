
namespace  ModularArchitecture.Identity.Core
{
    public class CreateRoleModel : ICreateRoleModel
    {
        public string Name { get; set; }
        public string ParentId { get; set; }
        public int SortOrder { get; set; }
        public string ApplicationId { get; set; }
    }
}
