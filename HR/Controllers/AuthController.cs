using HR.DTO;
using HR.Models;
using HR.Repository;
using HR.serviec;
using HR.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace HR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize(Roles = "Admin")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServies _authService;
        private HRDbcontext db { get; }
        public readonly IRoleNameRepository _roleNameRepository;
        public AuthController(IAuthServies authServies, HRDbcontext _db, IRoleNameRepository _roleNameRepository)
        {
            this._authService = authServies;
            db = _db;
            this._roleNameRepository = _roleNameRepository;

        }
       
        [HttpGet]
       // [Authorize(Roles = "Users.View")]
        public async Task<IActionResult> GetAllUsersAsync()
        {

            
            var users = await _authService.GetAllUsersAsync();
            List <UsersAllDTO> usersAllDTO = new List<UsersAllDTO>();
            foreach (var user in users)
            {
                UsersAllDTO usersDTO = new UsersAllDTO();
                usersDTO.fallName = user.Fullname;
                usersDTO.Username = user.UserName;
                usersDTO.Email = user.Email;
                var rolename = await _roleNameRepository.GetRoleNameById(user.roleId);
                if (rolename != null)
                {
                    usersDTO.RoleName = rolename.GroupName;
                }
                else
                {
                    usersDTO.RoleName = "Admin";
                }
                usersAllDTO.Add(usersDTO);

            }
            return Ok(usersAllDTO);
        }

        [HttpPost]
       // [Authorize(Roles = "Users.Create")]
        public async Task<IActionResult> addUserAsync(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.RegisterAsync(model);
                if (!result.IsAuthenticated)
                    return BadRequest(result.Message);
                return Ok(result);
            }
            return BadRequest(ModelState);
        }
        [HttpPost("login")]
       // [AllowAnonymous]
        public async Task<IActionResult> LoginAsync(Login model)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.LoginAsync(model);
                if (!result.IsAuthenticated)
                    return BadRequest(result.Message);
                return Ok(result);
            }
            return BadRequest(ModelState);
        }
    }
}
