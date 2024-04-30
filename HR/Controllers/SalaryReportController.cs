using HR.DTO;
using HR.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage;
using System.Globalization;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
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
            var absent = 0;
            var Employee = db.Employees.Include(e => e.dept).Include(e => e.Attendence).Include(e => e.AttendencperMonths).ToList();
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
                var TotalDiscountSalaryPerHours = 0.0;
                var TotalAdditionalSalaryPerHours = 0.0;
                var TotslNetSalary = 0.0;
                var attend = 0;
                absent = 0;
                var empAttendece = db.AttendenceEmployees.Where(e => e.idemp == item.id).OrderBy(e => e.dayDate).ToList(); //order Attendec
                for (int i = 0; i < empAttendece.Count(); i++) // of years and months
                {
                    if (empAttendece[i].leavingTime != null && empAttendece[i].arrivingTime != null)
                    {


                        if (i != 0)
                        {
                            if (empAttendece[i].dayDate.Month > empAttendece[i - 1].dayDate.Month || (empAttendece[i].dayDate.Month == 1 && empAttendece[i - 1].dayDate.Month == 12))
                            {
                                var existingMonthEntry = item.AttendencperMonths.FirstOrDefault(month => month.Monthofyear.Month == empAttendece[i - 1].dayDate.Month && month.Monthofyear.Year == empAttendece[i - 1].dayDate.Year);
                                if (existingMonthEntry == null)
                                {
                                    //March //March
                                    informationAttendencperMonth element = new informationAttendencperMonth()
                                    {
                                        extraTime = extraTime, //extraTime before add Setting extra
                                        discountTime = discountTime, //discount before add Setting discount
                                        Monthofyear = empAttendece[i - 1].dayDate,
                                        totalNetSalary = Math.Round(item.salary + ((extraTime * settings.extraHours) * salaryPerHour) - ((discountTime * settings.deductionHours) * salaryPerHour), 2),
                                        attendofDay = attend,
                                        absentday = absent,
                                        nameofMonth = empAttendece[i - 1].dayDate.ToString("MMMM", CultureInfo.InvariantCulture)

                                    };

                                    if (item.AttendencperMonths == null)
                                        item.AttendencperMonths = new List<informationAttendencperMonth>();
                                    item.AttendencperMonths.Add(element);


                                    db.SaveChanges();
                                    extraTime = 0;
                                    discountTime = 0;
                                    attend = 0;
                                    absent = 0;

                                }

                            }
                            else
                            {
                                attend += 1;
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
                            attend += 1;
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
                    else
                    {
                        absent += 1;
                    }
                }
                //////////////update 
                if (item.AttendencperMonths != null && DateOnly.FromDateTime(DateTime.Now).Day == 1 && item.AttendencperMonths[item.AttendencperMonths.Count - 1].Monthofyear.Month != ((DateOnly.FromDateTime(DateTime.Now).Month) - 1))
                {

                    informationAttendencperMonth element = new informationAttendencperMonth()
                    {
                        extraTime = extraTime, //extraTime before add Setting extra
                        discountTime = discountTime, //discount before add Setting discount
                        Monthofyear = item.AttendencperMonths[item.AttendencperMonths.Count - 1].Monthofyear.AddMonths(1),
                        totalNetSalary = Math.Round(item.salary + ((extraTime * settings.extraHours) * salaryPerHour) - ((discountTime * settings.deductionHours) * salaryPerHour), 2),
                        attendofDay = attend,
                        absentday = absent,
                        nameofMonth = item.AttendencperMonths[item.AttendencperMonths.Count - 1].Monthofyear.AddMonths(1).ToString("MMMM", CultureInfo.InvariantCulture)

                    };

                    if (item.AttendencperMonths == null)
                        item.AttendencperMonths = new List<informationAttendencperMonth>();
                    item.AttendencperMonths.Add(element);


                    db.SaveChanges();
                }
            }



            foreach (var emp in Employee)
            {
                foreach (var empMonth in emp.AttendencperMonths)
                {

                    var SalaryDTo = new SalaryReportDto
                    {
                        nameMonth = empMonth.Monthofyear.ToString("MMMM"),
                        empName = emp.name,
                        deptName = emp.dept.Name,
                        mainSalary = emp.salary,
                        attendDay = empMonth.attendofDay,
                        absentDay = empMonth.absentday,
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


        [HttpGet("{name}")]
        public IActionResult SearchByname(string name)
        {
            string fullname = name;
            var empReport = db.Employees.Include(e => e.dept).Include(e => e.Attendence).Include(e => e.AttendencperMonths).Where(e => e.name.ToLower() == fullname.ToLower()).ToList();
            var setting = db.PublicSettings.FirstOrDefault();
            if (empReport == null) return NotFound("please Enter ValidName");
            List<SalaryReportDto> SalaryFilterByName = new List<SalaryReportDto>();
            foreach (var emp in empReport)
            {
                foreach (var item in emp.AttendencperMonths)
                {
                    SalaryReportDto salaryReportDto = new SalaryReportDto()
                    {
                        nameMonth = item.Monthofyear.ToString("MMMM"),
                        empName = emp.name,
                        deptName = emp.dept.Name,
                        mainSalary = emp.salary,
                        attendDay = item.attendofDay,
                        absentDay = item.absentday,
                        extraHours = setting.extraHours,
                        dedectionHours = setting.deductionHours,
                        extraTimebeforSetting = item.extraTime,
                        discountTimebeforSetting = item.discountTime,
                        totalExtra = item.extraTime * setting.extraHours,
                        totalDiscount = item.discountTime * setting.deductionHours,
                        totalNetSalary = item.totalNetSalary,


                    };
                    SalaryFilterByName.Add(salaryReportDto);
                }
            }
            return Ok(SalaryFilterByName);

        }
        [HttpGet("{year:int}/{month:alpha}")]
        public IActionResult SearchByDateRepot(int year, string month)
        {
            if (year < 2008 || year > DateOnly.FromDateTime(DateTime.Now).Year) return NotFound("Please enter Correct year");
            var setting = db.PublicSettings.FirstOrDefault();
            var empReport = db.Employees
                            .Include(e => e.dept)
                            .Include(e => e.Attendence)
                            .Include(e => e.AttendencperMonths)
                            .ToList();
            var reportResult = empReport.SelectMany(e => e.AttendencperMonths)
.Where(a => a.nameofMonth.ToLower() == month.ToLower() && a.Monthofyear.Year == year)
                           .ToList();



            if (reportResult == null) return NotFound("The date is not Exist");
            List<SalaryReportDto> SalaryFilterByName = new List<SalaryReportDto>();

            foreach (var item in reportResult)
            {
                var idemp = db.Employees.Include(e => e.dept).FirstOrDefault(e => e.id == item.idemp);
                SalaryReportDto salaryReportDto = new SalaryReportDto()
                {
                    nameMonth = item.Monthofyear.ToString("MMMM"),
                    empName = idemp.name,
                    deptName = idemp.dept.Name,
                    mainSalary = idemp.salary,
                    attendDay = item.attendofDay,
                    absentDay = item.absentday,
                    extraHours = setting.extraHours,
                    dedectionHours = setting.deductionHours,
                    extraTimebeforSetting = item.extraTime,
                    discountTimebeforSetting = item.discountTime,
                    totalExtra = item.extraTime * setting.extraHours,
                    totalDiscount = item.discountTime * setting.deductionHours,
                    totalNetSalary = item.totalNetSalary,



                };
                SalaryFilterByName.Add(salaryReportDto);
            }

            return Ok(SalaryFilterByName);

        }

        [HttpGet("BythreeEele")]
        public IActionResult SearchbythreeElement(int? year, string? month, string? name)
        {
            // Check if any of the parameters are null
            if (name == null && year == null && month == null)
            {
                return BadRequest(" must enter at least one parameter");
            }

            // Check if the year parameter is within a valid range
            if (year < 2008 || year > DateOnly.FromDateTime(DateTime.Now).Year)
            {
                return NotFound("Please enter a correct year.");
            }

            // Retrieve data from the database
            var empReport = db.Employees
                                .Include(e => e.dept)
                                .Include(e => e.Attendence)
                                .Include(e => e.AttendencperMonths)
                                .ToList();

            List<SalaryReportDto> SalaryFilterByName = new List<SalaryReportDto>();

            // Filter the empReport based on parameters
            var reportResult = empReport.Where(e =>
                (name == null || e.name == name) &&             // Filter by name if provided
                (year == null || e.AttendencperMonths.Any(a => a.Monthofyear.Year == year)) &&  // Filter by year if provided
                (month == null || e.AttendencperMonths.Any(a => a.nameofMonth.ToLower() == month.ToLower()))  // Filter by month if provided
            ).SelectMany(e => e.AttendencperMonths).ToList();  // Flatten the list of AttendencperMonths

            if (reportResult.Count == 0)
            {
                return NotFound("Data not found.");
            }

            var setting = db.PublicSettings.FirstOrDefault();
            

            foreach (var item in reportResult)
            {
                var idemp = db.Employees.Include(e => e.dept).FirstOrDefault(e => e.id == item.idemp);
                SalaryReportDto salaryReportDto = new SalaryReportDto()
                {
                    nameMonth = item.Monthofyear.ToString("MMMM"),
                    empName = idemp.name,
                    deptName = idemp.dept.Name,
                    mainSalary = idemp.salary,
                    attendDay = item.attendofDay,
                    absentDay = item.absentday,
                    extraHours = setting.extraHours,
                    dedectionHours = setting.deductionHours,
                    extraTimebeforSetting = item.extraTime,
                    discountTimebeforSetting = item.discountTime,
                    totalExtra = item.extraTime * setting.extraHours,
                    totalDiscount = item.discountTime * setting.deductionHours,
                    totalNetSalary = item.totalNetSalary,



                };
                SalaryFilterByName.Add(salaryReportDto);
            }

            return Ok(SalaryFilterByName);
        }
    }
}