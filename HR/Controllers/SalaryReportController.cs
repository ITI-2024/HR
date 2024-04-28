using HR.DTO;
using HR.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

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
            var Employee = db.Employees.Include(e => e.dept).Include(e => e.Attendence).ToList();
            var settings = db.PublicSettings.FirstOrDefault();
            List < SalaryReportDto> SalaryReport= new List<SalaryReportDto>();
           
            
            foreach (var item in Employee)
            {
                int additionalHours = 0;
                int discountHours = 0;
                foreach (var item2 in item.Attendence)
                {
                    if (item2.arrivingTime == null && item2.leavingTime == null)
                    {
                        additionalHours += 0;
                        discountHours += 0;

                    }
                    else
                    {
                        if (item2.arrivingTime.Minute > 50)
                            additionalHours += (item2.arrivingTime.Hour + 1)-(item.arrivingTime.Hour);
                        else
                            additionalHours += (item2.arrivingTime.Hour) - (item.arrivingTime.Hour);
                        if(item2.leavingTime.Minute > 50)
                            discountHours += (item2.leavingTime.Hour + 1) - (item.leavingTime.Hour);
                        else
                            discountHours += (item2.leavingTime.Hour) - (item.leavingTime.Hour);
                    }
                  
                }
                var absent = 22 - item.Attendence.Count();
                //holidays
              
                    var salaryPerDay = Math.Round((item.salary / 30),2);
                    var salaryPerHour =Math.Round(( salaryPerDay / (item.leavingTime.Hour - item.arrivingTime.Hour)),2);
                    var TotalAdditionalSalaryPerHours = Math.Round((additionalHours * settings.extraHours)*salaryPerHour);
                    var TotalDiscountSalaryPerHours = Math.Round((discountHours * settings.deductionHours)*salaryPerHour);
                   var TotslNetSalary=item.salary+TotalAdditionalSalaryPerHours-TotalDiscountSalaryPerHours;

                   
                
                var SalaryDTo = new SalaryReportDto
                {
                    empName = item.name,
                    deptName = item.dept.Name,
                    mainSalary = item.salary,
                    attendDay = item.Attendence.Count(),
                    absentDay = absent,
                    extraHours = settings.extraHours,
                    dedectionHours = settings.deductionHours,
                    totalExtra = additionalHours,
                    totalDiscount = discountHours,
                    totalNetSalary = TotslNetSalary

                };
                SalaryReport.Add(SalaryDTo);

            }
            return Ok(SalaryReport);
     }
    }
}
