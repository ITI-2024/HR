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
                if (emp.DayDate > currentDate) return BadRequest("This date in the future");
                var employee=db.Employees.Where(e=>e.name==emp.EmployeeName).FirstOrDefault();
                var officialHoliday =db.Holidays.Where(h=>h.HolidayDate==emp.DayDate).FirstOrDefault(); 
                if (employee == null) return NotFound();
                if (officialHoliday != null) return BadRequest("This day is official holiday");
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
                var currentDate = DateOnly.FromDateTime(DateTime.Now);
                if (empAttendence.DayDate > currentDate) return BadRequest("This date in the future");
                var officialHoliday = db.Holidays.Where(h => h.HolidayDate == empAttendence.DayDate).FirstOrDefault();
                if (officialHoliday != null) return BadRequest("this day is Official Holiday");
                var employeeAttendence = db.AttendenceEmployees.Where(a => a.Id == id).FirstOrDefault();
                if (employeeAttendence == null) return NotFound();
                var employee = db.Employees.Where(e => e.name == empAttendence.EmployeeName).FirstOrDefault();
                if(employee == null) return NotFound();
                var temp = db.AttendenceEmployees.Where(e => ( e.Id!= id && e.idemp == employee.id && e.dayDate == empAttendence.DayDate)).FirstOrDefault();
                if (temp != null) return BadRequest("This Attendence Already Exist");
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
