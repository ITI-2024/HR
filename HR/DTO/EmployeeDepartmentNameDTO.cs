using HR.Helper.Validation;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace HR.DTO
{
    public class EmployeeDepartmentNameDTO
    {
        public string NationalID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public DateOnly BirthDate { get; set; }
        public string Gender { get; set; }
        public string Nationality { get; set; }
        public DateOnly ContractDate { get; set; }
        public TimeOnly ArrivingTime { get; set; }
        public TimeOnly LeavingTime { get; set; }
        public double Salary { get; set; }
        public string DepartmentName {  get; set; }

    }
}
