namespace HR.DTO
{
    public class SalaryReportDto
    {
        public string empName { get; set; }
        public string deptName { get; set; }
        public double mainSalary { get; set; }
        public int attendDay { get; set; }
        public int absentDay { get; set; }
        public int extraHours { get; set; }
        public int dedectionHours { get; set; }
        public int extraTimebeforSetting { get; set; }
        public string nameMonth { get; set; }
        public int discountTimebeforSetting { get; set; }
        public int totalExtra { get; set; }
        public int totalDiscount { get; set; }
        public double totalNetSalary { get; set; }
    }
}
