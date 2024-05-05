using HR.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics.Arm;

namespace HR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly HRDbcontext db;
        public DepartmentController(HRDbcontext _db)
        {
            this.db=_db;
            
        }
        [HttpGet]
        public IActionResult GetDepartment()
        {
            var department = db.Departments.ToList();
            if (department==null)
            {
                return BadRequest();
            }
            return Ok(department);

        }
        [HttpPost]
        public IActionResult AddDepartment(department dept)
        {
            if (dept == null) BadRequest();
            if (ModelState.IsValid)
            {
              // var deptnew = db.Departments.ToList();
                db.Departments.Add(dept);
                db.SaveChanges();
                return Ok(dept);
         }
            return BadRequest();

        }
        [HttpGet("id")]
        public IActionResult GetDeptById(int id)
        {
            var dept = db.Departments.Where(d => d.Id==id).FirstOrDefault();
            if (dept == null) return NotFound();
            return Ok(dept);
        }


    }
}
