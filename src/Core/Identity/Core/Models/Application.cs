using System.ComponentModel.DataAnnotations;

namespace  ModularArchitecture.Identity.Core
{
    public class Application
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
