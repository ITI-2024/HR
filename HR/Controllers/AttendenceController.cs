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
                if (employee == null) return NotFound();
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
    }
}
