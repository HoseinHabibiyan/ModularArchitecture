using System;
using System.Collections.Generic;

namespace  ModularArchitecture.Identity
{
    public interface IUser
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string EnglishFullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public List<string> Roles { get; set; }
        public bool IsEnabled { get; set; }
        public string ProfileImageUrl { get; set; }
        public int Code { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool IsSuperUser { get; set; }
        public bool IsDeleted { get; set; }
        public byte Level { get; set; }
        public DateTime JoinDate { get; set; }
    }
}
