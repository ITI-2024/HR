using HR.DTO;
using HR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Net;
using System.Reflection;

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
            var employee = db.Employees.Where(e => (e.name == emp.name && e.id != emp.id)).FirstOrDefault();
            if (employee != null) return BadRequest("There is another employee with the same name");
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
        [HttpPost("import-excel")]
        public async Task<IActionResult> ImportExcelFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length <= 0)
                {
                    return BadRequest("File is empty or missing.");
                }

                var employees = new List<Employee>();

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++) // Assuming row 1 is header
                        {
                            // Validate and parse each cell value
                            var id = worksheet.Cells[row, 1].GetValue<string>();
                            var name = worksheet.Cells[row, 2].GetValue<string>();
                            var gender = worksheet.Cells[row, 3].GetValue<string>();
                            var birthDate = worksheet.Cells[row, 4].GetValue<DateTime>();
                            var address = worksheet.Cells[row, 5].GetValue<string>();
                            var phoneNumber = worksheet.Cells[row, 6].GetValue<string>();
                            var nationality = worksheet.Cells[row, 7].GetValue<string>();
                            var contractDate = worksheet.Cells[row, 8].GetValue<DateTime>();
                            var arrivingTimeValue = worksheet.Cells[row, 9].GetValue<TimeSpan>();
                            var leavingTimeValue = worksheet.Cells[row, 10].GetValue<TimeSpan>();
                            var salary = worksheet.Cells[row, 11].GetValue<double>();
                            var departmentId = worksheet.Cells[row, 12].GetValue<int>();

                            // Validate and convert TimeSpan to TimeOnly
                            var arrivingTime = new TimeOnly(arrivingTimeValue.Hours, arrivingTimeValue.Minutes, arrivingTimeValue.Seconds);
                            var leavingTime = new TimeOnly(leavingTimeValue.Hours, leavingTimeValue.Minutes, leavingTimeValue.Seconds);

                            // Create Employee object
                            var employee = new Employee
                            {
                                id = id,
                                name = name,
                                gender = gender,
                                birthDate = DateOnly.FromDateTime(birthDate),
                                phoneNumber = phoneNumber,
                                address = address,
                                nationality = nationality,
                                contractDate = DateOnly.FromDateTime(contractDate),
                                arrivingTime = arrivingTime,
                                leavingTime = leavingTime,
                                salary = salary,
                                idDept = departmentId
                            };

                            employees.Add(employee);
                        }
                    }
                }

                // Add employees to the database
                await db.Employees.AddRangeAsync(employees);
                await db.SaveChangesAsync();

                return Ok(new { ImportResult = true });
            }
            catch (Exception ex)
            {
                // Log the exception
                

                // Return error response
                return StatusCode(500, $"An error occurred while importing the Excel file: {ex.Message}");
            }
        }

        
    }
}


            


