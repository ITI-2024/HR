using System.ComponentModel.DataAnnotations;

namespace HR.Helper.Validation
{
    public class ContractDateAttribute : ValidationAttribute
    {
        public ContractDateAttribute() { }
        public override bool IsValid(object? value)
        { 
        if(value is DateOnly date){
                return date.Year > 2008;
            }
        return false;
        }
        }
}
