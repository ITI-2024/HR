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
        public RoleNameController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
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
                } }
            return BadRequest();
            }
    }
}
