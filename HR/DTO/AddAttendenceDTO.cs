namespace HR.DTO
{
    public class AddAttendenceDTO
    {
        public string EmployeeName { get; set; }
        public TimeOnly ArrivingTime { get; set; }
        public TimeOnly LeavingTime { get; set; }
        public DateOnly DayDate { get; set; }
    }
}
