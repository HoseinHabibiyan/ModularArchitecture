namespace  ModularArchitecture.Identity
{
    public interface IRegisterModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        string FirstName { get; set; }
        string MiddleName { get; set; }
        string LastName { get; set; }
        string CallbackUrl { get; set; }
        string EnglishFullName { get; set; }
    }
}