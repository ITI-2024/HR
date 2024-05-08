using HR.Models;
using HR.Models;
using System.ComponentModel.DataAnnotations;

namespace MVCLap3.Helpers
{
    public class uniqueAttribute:ValidationAttribute
    {
        HRDbcontext db {  get; set; }
        public uniqueAttribute()
        {
    
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string? name=value?.ToString();
            if (name != null )
            {
                var db = (HRDbcontext)validationContext.GetService(typeof(HRDbcontext));
                if (!db.Roles.Any(x => x.GroupName == name)) 
                    return ValidationResult.Success;
            }
            return new ValidationResult("Namw of Group must be unique");
        }
    }
}
