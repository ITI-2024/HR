using HR.DTO;
using HR.Models;
using Microsoft.AspNetCore.Authorization;
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
  //  [Authorize(Roles = "Admin")]
    public class SalaryReportController : ControllerBase
    {

        private readonly HRDbcontext db;
        public SalaryReportController(HRDbcontext _db)
        {
            this.db = _db;
        }
        [HttpGet]
      //  [Authorize(Roles = "SalaryReport.View")]
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
                    if (i != 0)
                    {
                        if (empAttendece[i].dayDate.Year > empAttendece[i - 1].dayDate.Year || empAttendece[i].dayDate.Month > empAttendece[i - 1].dayDate.Month)
                        {
                            var existingMonthEntry = item.AttendencperMonths.FirstOrDefault(month => month.Monthofyear.Month == empAttendece[i - 1].dayDate.Month && month.Monthofyear.Year == empAttendece[i - 1].dayDate.Year);
                            if (existingMonthEntry == null)
                            {
                                //March //March
                                informationAttendencperMonth element = new informationAttendencperMonth();
                                element.extraTime = extraTime; //extraTime before add Setting extra
                                element.discountTime = discountTime; //discount before add Setting discount
                                element.Monthofyear = empAttendece[i - 1].dayDate;
                                element.totalNetSalary = Math.Round(item.salary + ((extraTime * settings.extraHours) * salaryPerHour) - ((discountTime * settings.deductionHours) * salaryPerHour), 2);
                                element.attendofDay = attend;
                                element.absentday = absent;
                                element.nameofMonth = empAttendece[i - 1].dayDate.ToString("MMMM", CultureInfo.InvariantCulture);


                                if (item.AttendencperMonths == null)
                                    item.AttendencperMonths = new List<informationAttendencperMonth>();
                                item.AttendencperMonths.Add(element);
                                db.SaveChanges();
                                

                            }
                            extraTime = 0;
                            discountTime = 0;
                            attend = 0;
                            absent = 0;
                            if (empAttendece[i].leavingTime != null && empAttendece[i].arrivingTime != null)
                            {

                                attend += 1;
                                if (empAttendece[i].arrivingTime > item.arrivingTime)
                                { discountTime += Math.Abs(empAttendece[i].arrivingTime.Value.Hour - item.arrivingTime.Hour); }
                                if (empAttendece[i].leavingTime > item.leavingTime)
                                { extraTime += Math.Abs(empAttendece[i].leavingTime.Value.Hour - item.leavingTime.Hour); }
                                if (empAttendece[i].leavingTime < item.leavingTime)
                                { discountTime += Math.Abs(empAttendece[i].leavingTime.Value.Hour - item.leavingTime.Hour); }
                            }
                            else
                            {
                                absent += 1;
                            }

                        }
                        else
                        {
                            if (empAttendece[i].leavingTime != null && empAttendece[i].arrivingTime != null)
                            {

                                attend += 1;
                                if (empAttendece[i].arrivingTime > item.arrivingTime)
                                { discountTime += Math.Abs(empAttendece[i].arrivingTime.Value.Hour - item.arrivingTime.Hour); }
                                if (empAttendece[i].leavingTime > item.leavingTime)
                                { extraTime += Math.Abs(empAttendece[i].leavingTime.Value.Hour - item.leavingTime.Hour); }
                                if (empAttendece[i].leavingTime < item.leavingTime)
                                { discountTime += Math.Abs(empAttendece[i].leavingTime.Value.Hour - item.leavingTime.Hour); }
                            }
                            else
                            {
                                absent += 1;
                            }
                        }

                    }
                    else
                    {
                        if (empAttendece[i].leavingTime != null && empAttendece[i].arrivingTime != null)
                        {

                            attend += 1;
                            if (empAttendece[i].arrivingTime > item.arrivingTime)
                            { discountTime += Math.Abs(empAttendece[i].arrivingTime.Value.Hour - item.arrivingTime.Hour); }
                            if (empAttendece[i].leavingTime > item.leavingTime)
                            { extraTime += Math.Abs(empAttendece[i].leavingTime.Value.Hour - item.leavingTime.Hour); }
                            if (empAttendece[i].leavingTime < item.leavingTime)
                            { discountTime += Math.Abs(empAttendece[i].leavingTime.Value.Hour - item.leavingTime.Hour); }
                        }
                        else
                        {
                            absent += 1;
                        }
                    }

                }
                //////////////update 
                if (item.AttendencperMonths != null && item.AttendencperMonths.Count != 0 && DateOnly.FromDateTime(DateTime.Now).Day == 1 && item.AttendencperMonths[item.AttendencperMonths.Count - 1].Monthofyear.Month != ((DateOnly.FromDateTime(DateTime.Now).Month) - 1))
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
                var salaryPerDay = Math.Round((emp.salary / 30), 2);
                //calcu salary per hour
                var salaryPerHour = Math.Round((salaryPerDay / (emp.leavingTime.Hour - emp.arrivingTime.Hour)), 2);
                foreach (var empMonth in emp.AttendencperMonths)
                {

                    var SalaryDTo = new SalaryReportDto
                    {
                        nameMonth = empMonth.Monthofyear.ToString("MMMM"),
                        nameYear = empMonth.Monthofyear.Year,
                        empName = emp.name,
                        deptName = emp.dept.Name,
                        mainSalary = emp.salary,
                        attendDay = empMonth.attendofDay,
                        absentDay = empMonth.absentday,
                        extraHours = empMonth.extraTime * settings.extraHours,
                        dedectionHours = empMonth.discountTime * settings.extraHours,
                        extraTimebeforSetting = empMonth.extraTime,
                        discountTimebeforSetting = empMonth.discountTime,
                        totalExtra = Math.Round(empMonth.extraTime * settings.extraHours* salaryPerHour, 2),
                        totalDiscount = Math.Round(empMonth.discountTime * settings.extraHours* salaryPerHour, 2),
                        totalNetSalary = empMonth.totalNetSalary,



                    };
                    SalaryReport.Add(SalaryDTo);

                }
            }

            return Ok(SalaryReport);
        }


        [HttpGet("{name}")]
       // [Authorize(Roles = "SalaryReport.View")]
        public IActionResult SearchByname(string name)
        {
            string fullname = name;
            var empReport = db.Employees.Include(e => e.dept).Include(e => e.Attendence).Include(e => e.AttendencperMonths).Where(e => e.name.ToLower().Contains(fullname.ToLower())).ToList();
            var setting = db.PublicSettings.FirstOrDefault();
            if (empReport == null) return NotFound("please Enter ValidName");
            List<SalaryReportDto> SalaryFilterByName = new List<SalaryReportDto>();
            foreach (var emp in empReport)
            {
                var salaryPerDay = Math.Round((emp.salary / 30), 2);
                //calcu salary per hour
                var salaryPerHour = Math.Round((salaryPerDay / (emp.leavingTime.Hour - emp.arrivingTime.Hour)), 2);
                foreach (var empMonth in emp.AttendencperMonths)
                {

                    SalaryReportDto salaryReportDto = new SalaryReportDto()
                    {
                        nameMonth = empMonth.Monthofyear.ToString("MMMM"),
                        nameYear = empMonth.Monthofyear.Year,
                        empName = emp.name,
                        deptName = emp.dept.Name,
                        mainSalary = emp.salary,
                        attendDay = empMonth.attendofDay,
                        absentDay = empMonth.absentday,
                        extraHours = empMonth.extraTime * setting.extraHours,
                        dedectionHours = empMonth.discountTime * setting.extraHours,
                        extraTimebeforSetting = empMonth.extraTime,
                        discountTimebeforSetting = empMonth.discountTime,
                        totalExtra = Math.Round(empMonth.extraTime * setting.extraHours * salaryPerHour, 2),
                        totalDiscount = Math.Round(empMonth.discountTime * setting.extraHours * salaryPerHour, 2),
                        totalNetSalary = empMonth.totalNetSalary,


                    };
                    SalaryFilterByName.Add(salaryReportDto);
                }
            }
            return Ok(SalaryFilterByName);

        }
        [HttpGet("{year:int}/{month:alpha}")]
       // [Authorize(Roles = "SalaryReport.View")]
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
                var salaryPerDay = Math.Round((idemp.salary / 30), 2);
                //calcu salary per hour
                var salaryPerHour = Math.Round((salaryPerDay / (idemp.leavingTime.Hour - idemp.arrivingTime.Hour)), 2);
                SalaryReportDto salaryReportDto = new SalaryReportDto()
                {
                    nameMonth = item.Monthofyear.ToString("MMMM"),
                    nameYear = item.Monthofyear.Year,
                    empName = idemp.name,
                    deptName = idemp.dept.Name,
                    mainSalary = idemp.salary,
                    attendDay = item.attendofDay,
                    absentDay = item.absentday,
                    extraHours = item.extraTime * setting.extraHours ,
                    dedectionHours = item.discountTime * setting.deductionHours,
                    extraTimebeforSetting = item.extraTime,
                    discountTimebeforSetting = item.discountTime,
                    totalExtra = Math.Round(item.extraTime * setting.extraHours* salaryPerHour, 2),
                    totalDiscount = Math.Round(item.discountTime * setting.deductionHours* salaryPerHour, 2),
                    totalNetSalary = item.totalNetSalary,



                };
                SalaryFilterByName.Add(salaryReportDto);
            }

            return Ok(SalaryFilterByName);

        }

        [HttpGet("BythreeEele")]
        //[Authorize(Roles = "SalaryReport.View")]
        public IActionResult SearchbythreeElement(int year, string month, string name)
        {

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

            var reportResult = empReport.Where(e => e.name.ToLower().Contains(name.ToLower())).SelectMany(e => e.AttendencperMonths)
            .Where(a => a.nameofMonth.ToLower() == month.ToLower() && a.Monthofyear.Year == year)
                           .ToList();

            if (reportResult.Count == 0)
            {
                return NotFound("Data not found.");
            }

            var setting = db.PublicSettings.FirstOrDefault();


            foreach (var item in reportResult)
            {
                var idemp = db.Employees.Include(e => e.dept).FirstOrDefault(e => e.id == item.idemp);
                var salaryPerDay = Math.Round((idemp.salary / 30), 2);
                //calcu salary per hour
                var salaryPerHour = Math.Round((salaryPerDay / (idemp.leavingTime.Hour - idemp.arrivingTime.Hour)), 2);
                SalaryReportDto salaryReportDto = new SalaryReportDto()
                {
                    nameMonth = item.Monthofyear.ToString("MMMM"),
                    nameYear = item.Monthofyear.Year,
                    empName = idemp.name,
                    deptName = idemp.dept.Name,
                    mainSalary = idemp.salary,
                    attendDay = item.attendofDay,
                    absentDay = item.absentday,
                    extraHours = (item.extraTime * setting.extraHours),
                    dedectionHours = item.discountTime * setting.deductionHours,
                    extraTimebeforSetting = item.extraTime,
                    discountTimebeforSetting = item.discountTime,
                    totalExtra = Math.Round(item.extraTime * setting.extraHours * salaryPerHour, 2),
                    totalDiscount = Math.Round(item.discountTime * setting.deductionHours * salaryPerHour, 2),
                    totalNetSalary = item.totalNetSalary,



                };
                SalaryFilterByName.Add(salaryReportDto);
            }

            return Ok(SalaryFilterByName);
        }
    }
}