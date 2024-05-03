using System.ComponentModel.DataAnnotations;

namespace HR.Models
{
    public class HolidaySetting
    {

        public int id { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        public string? dayName { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateOnly HolidayDate { get; set; }
    }
}
