using Microsoft.AspNetCore.Identity;
using System;

namespace ModularArchitecture.Identity.Core.Models
{
    public class ApplicationRole : IdentityRole
    {
        public string ApplicationId { get; set; }
        public DateTime? ExpireDate { get; set; }
        public int SortOrder { get; set; }
        public string ParentId { get; set; }
    }
}