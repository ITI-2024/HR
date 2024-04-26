using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HR.Models
{
    public class ApplictionUsers:IdentityUser
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Fullname { get; set; }
        //[Required]
        //public string permission { get; set; }
    }
}
