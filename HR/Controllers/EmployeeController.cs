using HR.DTO;
using HR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

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
    //  [Authorize(Roles ="Employee.View")]
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
        //[HttpPost("import-excel")]
        //public async Task<IActionResult> ImportExcelFile(IFormFile file)
        //{
        //    try
        //    {
            
        //        if (file == null || file.Length <= 0)
        //         {
        //                return BadRequest("File is empty or missing.");
        //            }
        //        var employees = new List<Employee>();
        //        using (var stream = new MemoryStream())
        //        {
        //            await file.CopyToAsync(stream);
        //            using (var package = new ExcelPackage(stream))
        //            {
        //                var worksheet = package.Workbook.Worksheets[0];
        //                var rowCount = worksheet.Dimension.Rows;

        //                for (int row = 2; row <= rowCount; row++) // Assuming row 1 is header
        //                {
        //                    var nationality = worksheet.Cells[row, 1].GetValue<string>();
        //                    var name = worksheet.Cells[row, 2].GetValue<string>();
        //                    var birthDate = worksheet.Cells[row, 3].GetValue<DateTime>();
        //                    var arrivingTimeValue = worksheet.Cells[row, 2].GetValue<TimeSpan>(); // Assuming the Excel cell contains a TimeSpan value
        //                    var leavingTimeValue = worksheet.Cells[row, 3].GetValue<TimeSpan>(); // Assuming the Excel cell contains a TimeSpan value
        //                    var idemp = worksheet.Cells[row, 4].GetValue<string>();
        //                    var idDept = worksheet.Cells[row, 5].GetValue<int>();
        //                    var employee = new Employee
        //                    {
        //                        id = worksheet.Cells[row, 1].GetValue<string>(),
                               
        //                        address = worksheet.Cells[row, 3].GetValue<string>(),
        //                        phoneNumber = worksheet.Cells[row, 4].GetValue<string>(),
        //                        birthDate = worksheet.Cells[row, 5].GetValue<DateTime>(), // Assuming the cell contains a valid Date value
        //                        gender = worksheet.Cells[row, 6].GetValue<string>(),
                            
        //                        contractDate = worksheet.Cells[row, 8].GetValue<DateTime>(), // Assuming the cell contains a valid Date value
        //                        arrivingTime = worksheet.Cells[row, 9].GetValue<TimeSpan>(), // Assuming the cell contains a valid TimeSpan value
        //                        leavingTime = worksheet.Cells[row, 10].GetValue<TimeSpan>(), // Assuming the cell contains a valid TimeSpan value
        //                        salary = worksheet.Cells[row, 11].GetValue<double>(), // Assuming the cell contains a valid double value
        //                        idDept = worksheet.Cells[row, 12].GetValue<int>() // Assuming the cell contains a valid int value
        //                    };

        //                    employees.Add(employee);
        //                }
        //            }


        //            }


    }

}
