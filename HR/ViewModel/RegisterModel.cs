using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.ViewModel
{
    public class RegisterModel
    {
        [Required, StringLength(100)]
        public string Fullname { get; set; }
        [Required, StringLength(50)]
        public string Username { get; set; }
        [Required, StringLength(100)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required, StringLength(50)]
        public string Email { get; set; }
        [Required]
        public int Roleid { get; set; }

    }
}
