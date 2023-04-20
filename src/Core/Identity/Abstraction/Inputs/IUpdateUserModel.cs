using System;

namespace ModularArchitecture.Identity.Abstraction.Inputs
{
    public interface IUpdateUserModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string ProfileImageUrl { get; set; }
        DateTime? LastLogin { get; set; }
        string FirstName { get; set; }
        string MiddleName { get; set; }
        string LastName { get; set; }
        string EnglishFullName { get; set; }
    }
}