using System.ComponentModel.DataAnnotations;

namespace HR.DTO
{
    public class AttendeceWithDepartmentDTO
    {
        public int ID { get; set; }
        public string DepartmentName { get; set; }
        public string EmployeeName {  get; set; }
        public TimeOnly? ArrivingTime { get; set; }
        public TimeOnly? LeavingTime { get; set; }
        public string DayName { get; set; }
        public DateOnly DayDate { get; set; }

    }
}
