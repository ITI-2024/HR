using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Models
{
    public class ApplictionUsers:IdentityUser
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Fullname { get; set; }


        [ForeignKey("RoleNames")]
        public int? roleId {  get; set; }

        public RoleName? RoleNames { get; set; }

    }
}
