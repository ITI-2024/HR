using System.ComponentModel.DataAnnotations;

namespace HR.Models
{
    public class department
    {
        [Key]
        public int Id { get; set; }

        [Required,StringLength(100)]
        public string Name { get; set; }
        public virtual List<Employee>? Employees { get; set; }
    }
}
