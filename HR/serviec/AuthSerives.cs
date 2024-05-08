using HR.Contant;
using HR.Helper;
using HR.Models;
using HR.Repository;
using HR.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
namespace HR.serviec
{
    public class AuthSerives : IAuthServies
    {
        private readonly UserManager<ApplictionUsers> _userManager;
        private readonly Jwt _Jwt;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRoleNameRepository roleNameRepository;
        private string _roleName;
        public AuthSerives(UserManager<ApplictionUsers> _userManager, IOptions<Jwt> jwt, RoleManager<IdentityRole> _roleManager, IRoleNameRepository _roleNameRepository)
        {

            this._userManager = _userManager;
            this._Jwt = jwt.Value;
            this._roleManager = _roleManager;

            this.roleNameRepository = _roleNameRepository;

        }

        public async Task<AuthModel> LoginAsync(Login model)
        {
            var authModel = new AuthModel();
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Email or password is wrong";
                return authModel;
            }
            var JwtSecurityToken = await CreateJwtToken(user);
            authModel.Email = user.Email;
            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(JwtSecurityToken);
            authModel.ExpiresOn = JwtSecurityToken.ValidTo;
            authModel.Username = user.UserName;
            var RoleList = await _userManager.GetRolesAsync(user);
            authModel.Roles = RoleList.ToList();
            var roleName = await GetRoleName(user.roleId);
            authModel.RoleName = roleName;
            return authModel;

        }
        private async Task<string> GetRoleName(int? roleId)
        {
            if (roleId.HasValue)
            {
                var roleName = await roleNameRepository.GetRoleNameById(roleId.Value);
                return roleName?.GroupName ?? string.Empty;
            }
            return string.Empty;
        }


        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
            {
                return new AuthModel { Message = "Email is already Exist", IsAuthenticated = false };
            }
            if (await _userManager.FindByNameAsync(model.Username) is not null)
            {
                return new AuthModel { Message = "Username is already Exist", IsAuthenticated = false };
            }
            var user = new ApplictionUsers
            {
                UserName = model.Username,
                Email = model.Email,
                Fullname = model.Fullname,

                roleId = model.Roleid,
              


            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors = $"{error.Description},";

                }
                return new AuthModel { Message = errors };

            }
            if (!await _roleManager.RoleExistsAsync(UserRole.User.ToString()))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRole.User.ToString()));
            }
            await _userManager.AddToRoleAsync(user, UserRole.User.ToString());
            if (user.roleId != null)
            {
                List<string> roles = await AddUserRoles(user.roleId.Value);
                foreach (var role in roles)
                {
                    await _userManager.AddToRoleAsync(user, role);
                }

            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var JwtSecurityToken = await CreateJwtToken(user);
            return new AuthModel
            {
                Email = user.Email,
                ExpiresOn = JwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = userRoles.ToList(),
                Token = new JwtSecurityTokenHandler().WriteToken(JwtSecurityToken),
                Username = user.UserName,
                Message = "it is created",
                RoleName = _roleName
            };


        }

        public async Task<List<string>> AddUserRoles(int roleId)
        {
            List<string> nameoftableperm = new List<string>();

            RoleName roleName = await roleNameRepository.GetRoleNameById(roleId);
            _roleName = roleName.GroupName;
            if (roleName is not null)
            {
                foreach (var perm in roleName.Permissions)
                {
                    var p = PermissionGeneret.GeneratePermissionsList(perm.name, perm.create, perm.update, perm.delete, perm.view);
                    // var p = PermissionGeneret.GeneratePermissionsList(perm.name, false, true, false, true);
                    foreach (var per in p)
                    {
                        if (!await _roleManager.RoleExistsAsync(per))
                            await _roleManager.CreateAsync(new IdentityRole(per));
                        nameoftableperm.Add(per);
                    }

                }

            }
            return nameoftableperm;
        }

        private async Task<JwtSecurityToken> CreateJwtToken(ApplictionUsers user)
        {

            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>()
                {

                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id),


            };

            foreach (var roleName in roles)
            {

                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {

                    claims.Add(new Claim(ClaimTypes.Role, roleName));

                }

            }

            SecurityKey symmetricSecrityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Jwt?.key));

            SigningCredentials SigningCredintials = new SigningCredentials(symmetricSecrityKey, SecurityAlgorithms.HmacSha256);

            var JwtSecurityToken = new JwtSecurityToken(
                issuer: _Jwt.Issuer,
                audience: _Jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_Jwt.DurationInDays),
                signingCredentials: SigningCredintials);


            return JwtSecurityToken;
        }


        public async Task<List<ApplictionUsers>> GetAllUsersAsync()
        {

            return await _userManager.Users.ToListAsync();
        }
    }
}