using MVCLap3.Helpers;
using System.ComponentModel.DataAnnotations;

namespace HR.DTO
{
    public class RoleDTO
    {
        public Guid Id { get; set; }
        [Required]
        [unique]
        public string Name { get; set; }
        public List<PermissionDTO>? Permissions { get; set; }
    }
}
