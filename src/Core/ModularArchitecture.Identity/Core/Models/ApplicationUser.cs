using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace ModularArchitecture.Identity.Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        private string _fullName;

        [MaxLength(300)]
        public string FullName
        {
            get => string.IsNullOrEmpty(_fullName) ? $"{FirstName} {MiddleName} {LastName}" : _fullName;
            set => _fullName = string.IsNullOrEmpty(value) ? $"{FirstName} {MiddleName} {LastName}" : value;
        }

        [MaxLength(300)]
        public string EnglishFullName { get; set; }

        [MaxLength(100)]
        public string FirstName { get; set; }

        [MaxLength(100)]
        public string MiddleName { get; set; }

        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        public byte Level { get; set; } = 3;

        [Required]
        public DateTime JoinDate { get; set; }

        public string ProfileImageUrl { get; set; }

        public DateTime? LastLogin { get; set; }

        public bool IsSuperUser { get; set; }

        public bool IsDeleted { get; set; }

        public string MobileNumber { get; set; }

        public int Code { get; set; }

        public bool IsEnabled { get; set; }
        public bool MustChangePassword { get; set; }
    }
}
