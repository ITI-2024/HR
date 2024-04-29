using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.Xml;
using System.Text.Json.Serialization;

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
        public TimeOnly? arrivingTime { get; set; }
        [Required]
        [DataType(DataType.Time)]
        public TimeOnly? leavingTime { get; set; }

        [ForeignKey("Emp")]
        public string? idemp { get; set; }
        [JsonIgnore]
        public virtual Employee? Emp { get; set; }

        [ForeignKey("department")]
        public int? idDept { get; set; }

        public virtual department? department { get; set; }
    }
}
