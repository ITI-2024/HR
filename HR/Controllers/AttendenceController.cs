using HR.DTO;
using HR.Models;
using Microsoft.AspNetCore.Http;
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
                var employee=db.Employees.Where(e=>e.name==emp.EmployeeName).FirstOrDefault();
                var officialHoliday =db.Holidays.Where(h=>h.HolidayDate==emp.DayDate).FirstOrDefault(); 
                if (employee == null) return NotFound();
                if (officialHoliday != null) return BadRequest("this day is Official Holiday");
                AttendenceEmployee addEmpData=new AttendenceEmployee();
                addEmpData.dayDate = emp.DayDate;
                addEmpData.arrivingTime = emp.ArrivingTime;
                addEmpData.leavingTime = emp.LeavingTime;
                addEmpData.idDept = employee.idDept;
                addEmpData.idemp = employee.id;
                var temp = db.AttendenceEmployees.Where(e =>( e.idemp == employee.id && e.dayDate==emp.DayDate)).FirstOrDefault();
                if (temp != null) return BadRequest("This Attendence Already Exist");
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
        [HttpPut("{id:int}")]
        public IActionResult EditEmployeeAttendence(int id,AddAttendenceDTO empAttendence)
        {
            if (empAttendence == null) return BadRequest();
            if (ModelState.IsValid)
            {
                var employeeAttendence = db.AttendenceEmployees.Where(a => a.Id == id).FirstOrDefault();
                if (employeeAttendence == null) return NotFound();
                var employee = db.Employees.Where(e => e.name == empAttendence.EmployeeName).FirstOrDefault();
                if(employee == null) return NotFound();
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
