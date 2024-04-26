using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.Xml;

namespace HR.Models
{
    public class AttendenceEmployee
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateOnly dayDate {  get; set; }
        [Required]
        [DataType(DataType.Time)]
        public TimeOnly arrivingTime { get; set; }
        [Required]
        [DataType(DataType.Time)]
        public TimeOnly leavingTime { get; set; }

        [ForeignKey("Emp")]
        public string? idemp { get; set; }
        public virtual Employee? Emp { get; set; }
    }
}
