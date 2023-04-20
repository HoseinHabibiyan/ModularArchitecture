namespace   ModularArchitecture.Identity.Core
{
    public interface IRole
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public int SortOrder { get; set; }
    }
}