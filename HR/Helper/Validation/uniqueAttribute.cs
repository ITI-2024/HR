using HR.Models;
using HR.Models;
using System.ComponentModel.DataAnnotations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
            if (name != null && validationContext.ObjectInstance is RoleName r)
            {
                var db = (HRDbcontext)validationContext.GetService(typeof(HRDbcontext));
                if (!db.Roles.Any(x => x.GroupName == name && x.Id != r.Id)) 
                    return ValidationResult.Success;
            }
            return new ValidationResult("Group Name must be unique");
        }
    }
}
