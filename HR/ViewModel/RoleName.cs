using System.ComponentModel.DataAnnotations;

namespace HR.ViewModel
{
    public class RoleName
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string GroupName { get; set; }
    }
}
