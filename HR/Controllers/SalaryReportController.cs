using HR.DTO;
using HR.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;
using System.Xml.Linq;

namespace HR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalaryReportController : ControllerBase
    {

        private readonly HRDbcontext db;
        public SalaryReportController(HRDbcontext _db)
        {
            this.db = _db;
        }
        [HttpGet]

        public IActionResult GetReport()
        {
            var Employee = db.Employees.Include(e => e.dept).Include(e => e.Attendence).Include(e=>e.AttendencperMonths).ToList();
            var settings = db.PublicSettings.FirstOrDefault();
            List<SalaryReportDto> SalaryReport = new List<SalaryReportDto>();

            foreach (var item in Employee) //emp1
            {

                int extraTime = 0; 
                int discountTime = 0;
                //calcu salary per Day 
                var salaryPerDay = Math.Round((item.salary / 30), 2);
                //calcu salary per hour
                var salaryPerHour = Math.Round((salaryPerDay / (item.leavingTime.Hour - item.arrivingTime.Hour)), 2);
                var TotalDiscountSalaryPerHours=0.0;
                var TotalAdditionalSalaryPerHours = 0.0;
                var TotslNetSalary = 0.0;
                var attend = 0;
                var empAttendece = db.AttendenceEmployees.Where(e => e.idemp == item.id).OrderBy(e => e.dayDate).ToList(); //order Attendec
                for (int i = 0; i < empAttendece.Count(); i++) // of years and months
                {
                    if (empAttendece[i].leavingTime != null && empAttendece[i].arrivingTime != null)
                    { 
                        attend += 1;
                        if (i != 0) 
                        {
                            if (empAttendece[i].dayDate.Month > empAttendece[i - 1].dayDate.Month || (empAttendece[i].dayDate.Month == 1 && empAttendece[i - 1].dayDate.Month == 12))
                            {
                                var existingMonthEntry = item.AttendencperMonths.FirstOrDefault(month => month.Monthofyear.Month == empAttendece[i - 1].dayDate.Month);
                                if (existingMonthEntry == null)
                                {
                                    informationAttendencperMonth element = new informationAttendencperMonth()
                                    {
                                        extraTime = extraTime, //extraTime before add Setting extra
                                        discountTime = discountTime, //discount before add Setting discount
                                        Monthofyear = empAttendece[i - 1].dayDate,
                                        totalNetSalary = Math.Round(item.salary + ((extraTime * settings.extraHours) * salaryPerHour) - ((discountTime * settings.deductionHours) * salaryPerHour), 2),
                                        attendofDay = attend
                           
                                    };

                                    if (item.AttendencperMonths == null)
                                        item.AttendencperMonths = new List<informationAttendencperMonth>();
                                    item.AttendencperMonths.Add(element);
                                    //total Addtional Salary per hour ال انا ضفتها
                                    //TotalAdditionalSalaryPerHours = Math.Round((element.extraTime * settings.extraHours) * salaryPerHour);
                                    //TotalDiscountSalaryPerHours = Math.Round((element.discountTime * settings.deductionHours) * salaryPerHour)

                                    db.SaveChanges();
                                    extraTime = 0;
                                    discountTime = 0;
                                    attend = 0;

                                }

                            }
                            else
                            {
                                if (empAttendece[i].leavingTime != null && empAttendece[i].arrivingTime != null)
                                {

                                    if (empAttendece[i].arrivingTime > item.arrivingTime)
                                    { discountTime += Math.Abs(empAttendece[i].arrivingTime.Value.Hour - item.arrivingTime.Hour); }
                                    if (empAttendece[i].leavingTime > item.leavingTime)
                                    { extraTime += Math.Abs(empAttendece[i].leavingTime.Value.Hour - item.leavingTime.Hour); }
                                    if (empAttendece[i].leavingTime < item.leavingTime)
                                    { discountTime += Math.Abs(empAttendece[i].leavingTime.Value.Hour - item.leavingTime.Hour); }
                                }
                            }

                        }
                        else
                        {
                     
                            if (empAttendece[i].arrivingTime > item.arrivingTime)
                                discountTime += Math.Abs(empAttendece[i].arrivingTime.Value.Hour - item.arrivingTime.Hour);
                            if (empAttendece[i].leavingTime > item.leavingTime)
                            {
                                extraTime += Math.Abs(empAttendece[i].leavingTime.Value.Hour - item.leavingTime.Hour);
                            }
                             if (empAttendece[i].leavingTime < item.leavingTime)
                            { discountTime += Math.Abs(empAttendece[i].leavingTime.Value.Hour - item.leavingTime.Hour); }

                        }
                    }
                }


            }



            foreach (var emp in Employee)
            {
                foreach(var empMonth in emp.AttendencperMonths)
                {

                    var SalaryDTo = new SalaryReportDto
                    {
                        nameMonth = empMonth.Monthofyear.ToString("MMMM"),
                        empName = emp.name,
                        deptName = emp.dept.Name,
                        mainSalary = emp.salary,
                        attendDay = empMonth.attendofDay,
                        absentDay = (22 - empMonth.attendofDay),
                        extraHours = settings.extraHours,
                        dedectionHours = settings.deductionHours,
                        extraTimebeforSetting = empMonth.extraTime,
                        discountTimebeforSetting = empMonth.discountTime,
                        totalExtra = empMonth.extraTime * settings.extraHours,
                        totalDiscount = empMonth.discountTime * settings.extraHours,
                        totalNetSalary = empMonth.totalNetSalary,
                        

                    };
                    SalaryReport.Add(SalaryDTo);

                }
            }
            
            
            return Ok(SalaryReport);
        }


}
}