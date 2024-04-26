using System.ComponentModel.DataAnnotations;

namespace HR.Helper.Validation
{
    public class BirthDateValidationAttribute: ValidationAttribute
    {
        public BirthDateValidationAttribute()
        {
            
        }
        public override bool IsValid(object? value)
        {
            var currentDate = DateTime.Now.Date; // Get the current date without the time component

            if (value is DateOnly birthDate)
            {
                var age = currentDate.Year - birthDate.Year;
                if (age >= 20)
                    return true;
            }

            return false;
        }

        }
}
