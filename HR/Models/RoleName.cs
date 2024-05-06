using System.ComponentModel.DataAnnotations;

namespace HR.Models
{
    public class RoleName
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string GroupName { get; set; }
        public List<permission>? Permissions { get; set; }
    }
}
