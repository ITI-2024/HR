using HR.Helper.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Models
{
    public class Employee
    {
        [Key]
        [Required]
        [RegularExpression(@"^[2|3]\d{13}")]
        public string id { get; set; }
        [Required, StringLength(70, MinimumLength = 3)]
        public string name { get; set; }
        [Required, StringLength(100, MinimumLength = 3)]

        public string address { get; set; }
        [Required]
        [RegularExpression(@"^01[0|1|2|5]{1}[0-9]{8}$")]
        [DataType(DataType.PhoneNumber)]
        public string phoneNumber { get; set; }
        [Required]
        [BirthDateValidation(ErrorMessage ="Age Must Older then or Equal 20")]
        [DataType(DataType.Date)]
        public DateOnly birthDate { get; set; }
        [Required]
        [RegularExpression("(?i)Male|Female")]
        public string gender { get; set; }
        [Required]
        [StringLength(100)]
        public string nationality { get; set; }
        [Required]
        [ContractDate(ErrorMessage ="The company started in 2008")]
        [DataType(DataType.Date)]
        public DateOnly contractDate { get; set; }
        [Required]
        [DataType(DataType.Time)]
        public TimeOnly arrivingTime  { get; set; }
        [Required]
        [DataType(DataType.Time)]
        public TimeOnly leavingTime { get; set; }
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Please enter a valid salary.")]
        public double salary { get; set; }
        [ForeignKey("dept")]

        [Display(Name = "DepartmentName")]
        public  int? idDept { get; set; }

        public virtual department? dept { get; set; }

        public virtual List<AttendenceEmployee>? Attendence { get; set; }

    }
}
