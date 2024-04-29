using HR.DTO;
using HR.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private HRDbcontext db { get; }
        public EmployeeController(HRDbcontext _db)
        {
            db = _db;
        }
        [HttpGet]
        public ActionResult getAllEmployees()
        {
            var employees= db.Employees.Include(e=>e.dept).ToList();
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
        [HttpPost]
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
        public IActionResult EditEmployee(Employee emp)
        {
            if (emp == null) return BadRequest();
            db.Employees.Update(emp);
            db.SaveChanges();
            return Ok(emp);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteEmployee(string id)
        {
            var emp = db.Employees.Find(id);
            if (emp is null) return NotFound();
            db.Employees.Remove(emp);
            db.SaveChanges();
            return Ok(emp);

        }
    }
}
