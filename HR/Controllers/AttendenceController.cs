using HR.DTO;
using HR.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace HR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendenceController : ControllerBase
    {
        private HRDbcontext db { get; }
        public AttendenceController(HRDbcontext _db)
        {
            db = _db;
        }
        [HttpGet]
        public ActionResult getAttendenceReport()
        {
            var attendence = db.AttendenceEmployees.Include(a => a.Emp).Include(a=>a.department).ToList();
            var AttendenceDTO = attendence.Select(a => new AttendeceWithDepartmentDTO
            {
                ID = a.Id,
                DepartmentName = a.department.Name,
                EmployeeName = a.Emp.name,
                ArrivingTime=a.arrivingTime,
                LeavingTime=a.leavingTime,
                DayDate=a.dayDate,
                DayName =a.dayDate.DayOfWeek.ToString()

        }).OrderBy(a=>a.DayDate).ToList();
            return Ok(AttendenceDTO);
        }

        [HttpPost]
        public IActionResult AddEmployeeAttendence(AddAttendenceDTO emp)
        {
            if (emp == null) return NotFound();
            if (ModelState.IsValid)
            {
                var currentDate = DateOnly.FromDateTime(DateTime.Now);
                if (emp.DayDate > currentDate) return BadRequest("Not allowed date in the future");
                if (emp.DayDate.Month != currentDate.Month || emp.DayDate.Year != currentDate.Year) return BadRequest("Not allowed to add attendence in this month");
                var employee=db.Employees.Where(e=>e.name==emp.EmployeeName).FirstOrDefault();
                if (employee == null) return NotFound();
                var officialHoliday =db.Holidays.Where(h=>h.HolidayDate==emp.DayDate).FirstOrDefault(); 
                if (officialHoliday != null) return BadRequest("This day is official holiday");
                var publicSetting = db.PublicSettings.FirstOrDefault();
                if (publicSetting != null)
                {
                    var firstHoliday = publicSetting.firstWeekend;
                    var secondHoliday = publicSetting.secondWeekend;
                    var attendenceDayName = emp.DayDate.DayOfWeek.ToString();
                    if (firstHoliday.ToLower() == attendenceDayName.ToLower())
                    {
                        return BadRequest("this is weekend holiday");
                    }
                    if(secondHoliday!= null && secondHoliday.ToLower() == attendenceDayName.ToLower())
                    {
                        return BadRequest("this is weekend holiday");
                    }
                }
                AttendenceEmployee addEmpData=new AttendenceEmployee();
                addEmpData.dayDate = emp.DayDate;
                addEmpData.arrivingTime = emp.ArrivingTime;
                addEmpData.leavingTime = emp.LeavingTime;
                addEmpData.idDept = employee.idDept;
                addEmpData.idemp = employee.id;
                var temp = db.AttendenceEmployees.Where(e =>( e.idemp == employee.id && e.dayDate==emp.DayDate)).FirstOrDefault();
                if (temp != null) return BadRequest("This attendence already exist");
                db.AttendenceEmployees.Add(addEmpData);
                db.SaveChanges();
                return Ok(addEmpData);
            }
            return BadRequest();
        }
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var empAttendence = db.AttendenceEmployees.Where(a => a.Id == id).FirstOrDefault();
            if (empAttendence == null) return NotFound();
            return Ok(empAttendence);
        }

        [HttpGet("{name:alpha}")]
        public IActionResult SearchByName(string name) {
            var employeeAttendence = db.AttendenceEmployees.Include(a => a.Emp).Include(a => a.department).ToList();
            var filterAttendence= employeeAttendence.Where(a=>(a.department.Name.ToLower().Contains(name.ToLower()) || a.Emp.name.ToLower().Contains(name.ToLower()))).ToList();
            if (filterAttendence == null|| filterAttendence.Count==0) return BadRequest("Invalid Employee or department name");
            return Ok(filterAttendence);
        }
        [HttpGet("{fromDate:datetime},{toDate:datetime}")]
        public IActionResult SearchByDate(DateOnly fromDate ,DateOnly toDate)
        {
            if (fromDate > toDate) return BadRequest("Enter valid date");
            var filterAttendence=db.AttendenceEmployees.Where(a=>(a.dayDate>=fromDate && a.dayDate<=toDate)).ToList();
            return Ok(filterAttendence);
        }
        [HttpPut("{id:int}")]
        public IActionResult EditEmployeeAttendence(int id,AddAttendenceDTO empAttendence)
        {
            if (empAttendence == null) return BadRequest();
            if (ModelState.IsValid)
            {
                var employeeAttendence = db.AttendenceEmployees.Where(a => a.Id == id).FirstOrDefault();
                var currentDate = DateOnly.FromDateTime(DateTime.Now);
                if (employeeAttendence !=null && employeeAttendence.dayDate.Month != currentDate.Month || employeeAttendence.dayDate.Year != currentDate.Year) return BadRequest("Not allowed to update attendence in this month");
                if (empAttendence.DayDate > currentDate) return BadRequest("Not allowed date in the future");
                if (empAttendence.DayDate.Month != currentDate.Month ||(empAttendence.DayDate.Month == currentDate.Month && empAttendence.DayDate.Year != currentDate.Year )) return BadRequest("Not allowed to update attendence");
                if (employeeAttendence == null) return NotFound();
                var employee = db.Employees.Where(e => e.name == empAttendence.EmployeeName).FirstOrDefault();
                if(employee == null) return NotFound();
                var temp = db.AttendenceEmployees.Where(e => ( e.Id!= id && e.idemp == employee.id && e.dayDate == empAttendence.DayDate)).FirstOrDefault();
                if (temp != null) return BadRequest("This Attendence Already Exist");
                var officialHoliday = db.Holidays.Where(h => h.HolidayDate == empAttendence.DayDate).FirstOrDefault();
                if (officialHoliday != null) return BadRequest("This day is official holiday");
                var publicSetting = db.PublicSettings.FirstOrDefault();
                if (publicSetting != null)
                {
                    var firstHoliday = publicSetting.firstWeekend;
                    var secondHoliday = publicSetting.secondWeekend;
                    var attendenceDayName = empAttendence.DayDate.DayOfWeek.ToString();
                    if (firstHoliday.ToLower() == attendenceDayName.ToLower())
                    {
                        return BadRequest("this is weekend holiday");
                    }
                    if (secondHoliday != null && secondHoliday.ToLower() == attendenceDayName.ToLower())
                    {
                        return BadRequest("this is weekend holiday");
                    }
                }
                employeeAttendence.dayDate = empAttendence.DayDate;
                employeeAttendence.arrivingTime = empAttendence.ArrivingTime;
                employeeAttendence.leavingTime = empAttendence.LeavingTime;
                employeeAttendence.idDept = employee.idDept;
                employeeAttendence.idemp = employee.id;
                db.SaveChanges();
                return Ok(employeeAttendence);
            }
            return BadRequest();
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteEmployeeAttendence(int id)
        {
            var empAttendence = db.AttendenceEmployees.Find(id);
            if (empAttendence is null) return NotFound();
            db.AttendenceEmployees.Remove(empAttendence);
            db.SaveChanges();
            return Ok(empAttendence);

        }
    }
}
