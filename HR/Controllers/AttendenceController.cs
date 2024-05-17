using HR.DTO;
using HR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using OfficeOpenXml;

namespace HR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(Roles = "Admin")]
    public class AttendenceController : ControllerBase
    {
        private HRDbcontext db { get; }
        public AttendenceController(HRDbcontext _db)
        {
            db = _db;
        }
        [HttpGet]
        // [Authorize(Roles = "Attendance.View")]
        public ActionResult getAttendenceReport()
        {
            var attendence = db.AttendenceEmployees.Include(a => a.Emp).Include(a => a.department).ToList();
            var AttendenceDTO = attendence.Select(a => new AttendeceWithDepartmentDTO
            {
                ID = a.Id,
                DepartmentName = a.department.Name,
                EmployeeName = a.Emp.name,
                ArrivingTime = a.arrivingTime,
                LeavingTime = a.leavingTime,
                DayDate = a.dayDate,
                DayName = a.dayDate.DayOfWeek.ToString()

            }).OrderBy(a => a.DayDate).ToList();
            return Ok(AttendenceDTO);
        }

        [HttpPost]
        // [Authorize(Roles = "Attendance.Create")]
        public IActionResult AddEmployeeAttendence(AddAttendenceDTO emp)
        {
            if (emp == null) return NotFound();
            if (ModelState.IsValid)
            {
                var currentDate = DateOnly.FromDateTime(DateTime.Now);
                if (emp.DayDate > currentDate) return BadRequest("Not allowed date in the future");
                if (emp.DayDate.Month != currentDate.Month || emp.DayDate.Year != currentDate.Year) return BadRequest("Not allowed to add attendence in this month");
                var employee = db.Employees.Where(e => e.name == emp.EmployeeName).FirstOrDefault();
                if (employee == null) return NotFound();
                var officialHoliday = db.Holidays.Where(h => h.HolidayDate == emp.DayDate).FirstOrDefault();
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
                    if (secondHoliday != null && secondHoliday.ToLower() == attendenceDayName.ToLower())
                    {
                        return BadRequest("this is weekend holiday");
                    }
                }
                AttendenceEmployee addEmpData = new AttendenceEmployee();
                addEmpData.dayDate = emp.DayDate;
                addEmpData.arrivingTime = emp.ArrivingTime;
                addEmpData.leavingTime = emp.LeavingTime;
                addEmpData.idDept = employee.idDept;
                addEmpData.idemp = employee.id;
                var temp = db.AttendenceEmployees.Where(e => (e.idemp == employee.id && e.dayDate == emp.DayDate)).FirstOrDefault();
                if (temp != null) return BadRequest("This attendence already exist");
                db.AttendenceEmployees.Add(addEmpData);
                db.SaveChanges();
                return Ok(addEmpData);
            }
            return BadRequest();
        }
        [HttpGet("{id:int}")]
        //  [Authorize(Roles = "Attendance.View")]
        public IActionResult GetById(int id)
        {
            var empAttendence = db.AttendenceEmployees.Where(a => a.Id == id).FirstOrDefault();
            if (empAttendence == null) return NotFound();
            return Ok(empAttendence);
        }

        [HttpGet("search")]
        //[Authorize(Roles = "Attendance.View")]
        public IActionResult Search(DateOnly? fromDate, DateOnly? toDate, string? name)
        {
            var SearchResult = new List<AttendeceWithDepartmentDTO>();
            var searchNameList = new List<AttendenceEmployee>();
            searchNameList = [];
            var searchName = "";
            if (name != null)
            {
                var filterAttendenceByEmp = db.AttendenceEmployees.Include(a => a.department).Include(a => a.Emp).Where(a => a.Emp.name.ToLower().Contains(name.ToLower())).ToList();
                var filterAttendenceByDept = db.AttendenceEmployees.Include(a => a.department).Include(a => a.Emp).Where(a => a.department.Name.ToLower().Contains(name.ToLower())).ToList();
                if (filterAttendenceByEmp == null || filterAttendenceByEmp.Count == 0)
                {
                    if (filterAttendenceByDept == null || filterAttendenceByDept.Count == 0)
                        return BadRequest("Invalid Employee or Department name");
                    searchNameList = filterAttendenceByDept;
                    searchName = name;

                }
                else
                {
                    searchNameList = filterAttendenceByEmp;
                    searchName = name;
                }
            }
            if (fromDate > toDate) return BadRequest("Enter valid date");
            if (searchName != "")
            {
                if (fromDate != null && toDate != null)
                {
                    var filterAttendenceByDate = db.AttendenceEmployees.Include(a => a.department).Include(a => a.Emp).Where(a => (a.dayDate >= fromDate && a.dayDate <= toDate)).ToList();
                    filterAttendenceByDate = filterAttendenceByDate.Where(f => (f.Emp.name.ToLower().Contains(searchName.ToLower()) || f.department.Name.ToLower().Contains(searchName.ToLower()))).ToList();
                    SearchResult = filterAttendenceByDate.Select(a => new AttendeceWithDepartmentDTO
                    {
                        ID = a.Id,
                        DepartmentName = a.department.Name,
                        EmployeeName = a.Emp.name,
                        ArrivingTime = a.arrivingTime,
                        LeavingTime = a.leavingTime,
                        DayDate = a.dayDate,
                        DayName = a.dayDate.DayOfWeek.ToString()

                    }).OrderBy(a => a.DayDate).ToList();
                }
                else
                {
                    SearchResult = searchNameList.Select(a => new AttendeceWithDepartmentDTO
                    {
                        ID = a.Id,
                        DepartmentName = a.department.Name,
                        EmployeeName = a.Emp.name,
                        ArrivingTime = a.arrivingTime,
                        LeavingTime = a.leavingTime,
                        DayDate = a.dayDate,
                        DayName = a.dayDate.DayOfWeek.ToString()

                    }).OrderBy(a => a.DayDate).ToList();
                }
            }
            else
            {
                if (fromDate == null && toDate == null) return BadRequest("This Fields Required");
                var filterAttendenceByDate = db.AttendenceEmployees.Include(a => a.department).Include(a => a.Emp).Where(a => (a.dayDate >= fromDate && a.dayDate <= toDate)).ToList();
                SearchResult = filterAttendenceByDate.Select(a => new AttendeceWithDepartmentDTO
                {
                    ID = a.Id,
                    DepartmentName = a.department.Name,
                    EmployeeName = a.Emp.name,
                    ArrivingTime = a.arrivingTime,
                    LeavingTime = a.leavingTime,
                    DayDate = a.dayDate,
                    DayName = a.dayDate.DayOfWeek.ToString()

                }).OrderBy(a => a.DayDate).ToList();
            }
            return Ok(SearchResult);
        }

        [HttpPut("{id:int}")]
        public IActionResult EditEmployeeAttendence(int id, AddAttendenceDTO empAttendence)
        {
            if (empAttendence == null) return BadRequest();
            if (ModelState.IsValid)
            {
                var employeeAttendence = db.AttendenceEmployees.Where(a => a.Id == id).FirstOrDefault();
                var currentDate = DateOnly.FromDateTime(DateTime.Now);
                if (employeeAttendence != null && employeeAttendence.dayDate.Month != currentDate.Month || employeeAttendence.dayDate.Year != currentDate.Year) return BadRequest("Not allowed to update attendence in this month");
                if (empAttendence.DayDate > currentDate) return BadRequest("Not allowed date in the future");
                if (empAttendence.DayDate.Month != currentDate.Month || (empAttendence.DayDate.Month == currentDate.Month && empAttendence.DayDate.Year != currentDate.Year)) return BadRequest("Not allowed to update attendence");
                if (employeeAttendence == null) return NotFound();
                var employee = db.Employees.Where(e => e.name == empAttendence.EmployeeName).FirstOrDefault();
                if (employee == null) return NotFound();
                var temp = db.AttendenceEmployees.Where(e => (e.Id != id && e.idemp == employee.id && e.dayDate == empAttendence.DayDate)).FirstOrDefault();
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
        // [Authorize(Roles = "Delete.View")]
        public IActionResult DeleteEmployeeAttendence(int id)
        {
            var empAttendence = db.AttendenceEmployees.Find(id);
            if (empAttendence is null) return NotFound();
            db.AttendenceEmployees.Remove(empAttendence);
            db.SaveChanges();
            return Ok(empAttendence);

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

                var attendances = new List<AttendenceEmployee>();

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);

                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++) // Assuming row 1 is header
                        {
                            var dateValue = worksheet.Cells[row, 1].GetValue<DateTime>();
                            var arrivingTimeValue = worksheet.Cells[row, 2].GetValue<TimeSpan>(); // Assuming the Excel cell contains a TimeSpan value
                            var leavingTimeValue = worksheet.Cells[row, 3].GetValue<TimeSpan>(); // Assuming the Excel cell contains a TimeSpan value
                            var idemp = worksheet.Cells[row, 4].GetValue<string>();
                            var idDept = worksheet.Cells[row, 5].GetValue<int>();

                            // Convert TimeSpan to TimeOnly
                            TimeOnly? arrivingTime = null;
                            TimeOnly? leavingTime = null;

                            if (arrivingTimeValue != TimeSpan.Zero)
                            {
                                arrivingTime = new TimeOnly(arrivingTimeValue.Hours, arrivingTimeValue.Minutes, arrivingTimeValue.Seconds);
                            }
                            if (leavingTimeValue != TimeSpan.Zero)
                            {
                                leavingTime = new TimeOnly(leavingTimeValue.Hours, leavingTimeValue.Minutes, leavingTimeValue.Seconds);
                            }

                            var attendance = new AttendenceEmployee
                            {
                                dayDate = DateOnly.FromDateTime(dateValue),
                                arrivingTime = arrivingTime,
                                leavingTime = leavingTime,
                                idemp = idemp,
                                idDept = idDept
                            };

                            // Check for existing attendance to prevent duplicates
                            var exists = await db.AttendenceEmployees
                                                 .AnyAsync(a => a.idemp == idemp && a.dayDate == attendance.dayDate);

                            if (!exists)
                            {
                                attendances.Add(attendance);
                            }
                        }
                    }
                }

                if (attendances.Count > 0)
                {
                    await db.AttendenceEmployees.AddRangeAsync(attendances);
                    await db.SaveChangesAsync();
                }

                return Ok(new { ImportResult = true, RecordsAdded = attendances.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
