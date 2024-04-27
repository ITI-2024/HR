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

        }).ToList();
            return Ok(AttendenceDTO);
        }
 
        //[HttpPost]
        //public IActionResult AddEmployeeAttendence()
        //{
        //    if (emp == null) return NotFound();
        //    if (ModelState.IsValid)
        //    {
        //        db.Employees.Add(emp);
        //        db.SaveChanges();
        //        return Ok(emp);
        //    }
        //    return BadRequest();
        //}
    }
}
