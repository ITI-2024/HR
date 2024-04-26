using System.ComponentModel.DataAnnotations;

namespace HR.Models
{
    public class PublicSetting
    {
        [Key]
        public int id { get; set; }
        [Required]
        [Range(1,4)]
        public int extraHours { get; set; }
        [Required]
        [Range(1,4)]
        public int deductionHours { get; set; }
        [Required]
        [StringLength(30)]
        [RegularExpression("^(Monday|Tuesday|Wednesday|Thursday|Friday|Saturday|Sunday)$", ErrorMessage = "Please enter a valid day of the week.")]
        public string firstWeekend { get; set; }
        [RegularExpression("^(Monday|Tuesday|Wednesday|Thursday|Friday|Saturday|Sunday)$", ErrorMessage = "Please enter a valid day of the week.")]
        [StringLength(30)]
        public string? secondWeekend { get; set;}
       
    }
}
