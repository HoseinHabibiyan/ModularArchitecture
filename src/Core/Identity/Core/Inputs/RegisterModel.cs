using ModularArchitecture.Identity.Abstraction.Inputs;

namespace ModularArchitecture.Identity.Core.Inputs
{
    public class RegisterModel : IRegisterModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string CallbackUrl { get; set; }
        public string EnglishFullName { get; set; }
        public bool MustChangePassword { get; set; }
    }
}
