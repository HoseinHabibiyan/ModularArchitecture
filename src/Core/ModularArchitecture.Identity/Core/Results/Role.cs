using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using ModularArchitecture.Identity.Abstraction.Results;

namespace ModularArchitecture.Identity.Core.Results
{
    public class Role : IdentityRole, IRole
    {
        [Required]
        public DateTime? ExpireDate { get; set; }
        public int SortOrder { get; set; }
        public string ParentId { get; set; }
    }
}
