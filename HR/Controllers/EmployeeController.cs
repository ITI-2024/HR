using HR.DTO;
using HR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class EmployeeController : ControllerBase
    {
        private HRDbcontext db { get; }
        public EmployeeController(HRDbcontext _db)
        {
            db = _db;
        }
        [HttpGet]
      //[Authorize(Roles ="Employee.View")]
        public ActionResult getAllEmployees()
        {
            var employees = db.Employees.Include(e => e.dept).ToList();
            var employeesDTO = employees.Select(e => new EmployeeDepartmentNameDTO
            {
                NationalID = e.id,
                Name = e.name,
                Address = e.address,
                PhoneNumber = e.phoneNumber,
                BirthDate = e.birthDate,
                Gender = e.gender,
                Nationality = e.nationality,
                ContractDate = e.contractDate,
                ArrivingTime = e.arrivingTime,
                LeavingTime = e.leavingTime,
                Salary = e.salary,
                DepartmentName = e.dept.Name
            }).ToList();
            return Ok(employeesDTO);
        }
        [HttpGet("id")]
       // [Authorize(Roles = "Employee.View")]
        public IActionResult GetEmplyeeById(string id)
        {
            var emp = db.Employees.Where(a => a.id == id).FirstOrDefault();
            if (emp == null) return NotFound();
            return Ok(emp);
        }
        [HttpPost]
       // [Authorize(Roles = "Employee.Create")]
        public IActionResult AddEmployee(Employee emp)
        {
            var employee = db.Employees.Where(e => e.id == emp.id).FirstOrDefault();
            if (employee != null) return BadRequest("Employee already exist");
            employee = db.Employees.Where(e => e.name == emp.name).FirstOrDefault();
            if (employee != null) return BadRequest("There is another employee with the same name");
            if (emp == null) return NotFound();
            if (ModelState.IsValid)
            {
                db.Employees.Add(emp);
                db.SaveChanges();
                return Ok(emp);
            }
            return BadRequest();
        }
        [HttpPut]
       // [Authorize(Roles = "Employee.Update")]
        public IActionResult EditEmployee(Employee emp)
        {
            if (emp == null) return BadRequest();
            db.Employees.Update(emp);
            db.SaveChanges();
            return Ok(emp);
        }
     
   
        [HttpDelete("employees/{id}")]
        // [Authorize(Roles = "Employee.delete")]
        public async Task<IActionResult> DeleteEmployees(string id)
        {
            var employee = await db.Employees
                .Include(e => e.Attendence)
                .Include(a => a.AttendencperMonths)
                .FirstOrDefaultAsync(e => e.id == id);

            if (employee == null)
            {
                return NotFound();
            }

            // Remove related records
            foreach (var attendance in employee.AttendencperMonths)
            {
                db.Attendencpermonth.RemoveRange(attendance);
            }
            db.AttendenceEmployees.RemoveRange(employee.Attendence);
            db.Employees.Remove(employee);

            await db.SaveChangesAsync();

            return NoContent();
        }
    }
}
