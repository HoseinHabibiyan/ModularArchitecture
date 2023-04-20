using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace  ModularArchitecture.Identity.Core
{
    public class Role : IdentityRole, IRole
    {
        [Required]
        public DateTime? ExpireDate { get; set; }
        public int SortOrder { get; set; }
        public string ParentId { get; set; }
    }
}
