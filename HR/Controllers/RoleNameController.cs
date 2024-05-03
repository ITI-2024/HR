using HR.Models;
using HR.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleNameController : ControllerBase
    {
        public RoleManager<IdentityRole> roleManager { get; }
        public readonly HRDbcontext dbl;
        public RoleNameController(RoleManager<IdentityRole> roleManager, HRDbcontext _dbl)
        {
            this.roleManager = roleManager;
            this.dbl = _dbl;
        }
        [HttpPost]
        public async Task<IActionResult> addRole(RoleName vm)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole();
                role.Name = vm.GroupName;
                IdentityResult result = await roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return Ok(role.Name);
                }
                else
                {
                    foreach (var e in result.Errors)
                    {
                        ModelState.AddModelError("", e.Description);
                    }
                }
            }
            return BadRequest();
        }
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = dbl.Roles.ToList();
            return Ok(roles);
        }
    }
}
