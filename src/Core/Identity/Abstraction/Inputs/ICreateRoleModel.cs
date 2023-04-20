namespace ModularArchitecture.Identity.Abstraction.Inputs
{
    public interface ICreateRoleModel
    {
        public string Name { get; set; }
        public string ParentId { get; set; }
        public int SortOrder { get; set; }
        string ApplicationId { get; set; }
    }
}