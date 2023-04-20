using ModularArchitecture.Identity.Abstraction.Inputs;

namespace ModularArchitecture.Identity.Core.Inputs
{
    public class UpdateUserModel : IUpdateUserModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string ProfileImageUrl { get; set; }
        public List<string> Roles { get; set; }
        public DateTime? LastLogin { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string EnglishFullName { get; set; }
    }
}
