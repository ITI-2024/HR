using HR.Models;
using HR.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleNameController : ControllerBase
    {
        public RoleManager<IdentityRole> roleManager { get; }
        public readonly HRDbcontext db;
        public RoleNameController(RoleManager<IdentityRole> roleManager, HRDbcontext _dbl)
        {
            this.roleManager = roleManager;
            this.db = _dbl;
        }
        //  [HttpPost]
        //public async Task<IActionResult> addRole(RoleName vm)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        IdentityRole role = new IdentityRole();
        //        role.Name = vm.GroupName;
        //        IdentityResult result = await roleManager.CreateAsync(role);
        //        if (result.Succeeded)
        //        {
        //            return Ok(role.Name);
        //        }
        //        else
        //        {
        //            foreach (var e in result.Errors)
        //            {
        //                ModelState.AddModelError("", e.Description);
        //            }
        //        }
        //    }
        //    return BadRequest();
        //}
        [HttpPost]
        public IActionResult CreateRole([FromBody] group role)
        {
            role.NormalizedName = role.Name.ToUpper();
            role.ConcurrencyStamp = Guid.NewGuid().ToString();
            if (role == null) return BadRequest();
            db.Roles.Add(role);
            db.SaveChanges();
            return Ok(role);
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = db.Roles.ToList();
            return Ok(roles);
        }
        [HttpGet("{id}")]
        public IActionResult GetRoleById(string id) {
            var role = db.Roles.FirstOrDefault(x => x.Id==id);
            return Ok(role);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteRoleById(string id)
        {
            var role = db.Roles.FirstOrDefault(y => y.Id==id);
            if(role == null) return NotFound();
            db.Roles.Remove(role);
            db.SaveChanges();
            return Ok(role);
        }
    }
}
