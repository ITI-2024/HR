using HR.Helper;
using HR.Models;
using HR.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HR.serviec
{
    public class AuthSerives : IAuthServies
    {
        private readonly UserManager<ApplictionUsers> _userManager;
        private readonly Jwt _Jwt;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AuthSerives(UserManager<ApplictionUsers>_userManager, IOptions<Jwt>jwt,RoleManager<IdentityRole>_roleManager) 
        {

            this._userManager = _userManager;
            this._Jwt = jwt.Value;
            this._roleManager = _roleManager;



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
            return authModel;

        }


        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            if(await _userManager.FindByEmailAsync(model.Email) is not null)
            {
                return new AuthModel { Message = "Email is already Exist", IsAuthenticated = false };
            }
            if(await _userManager.FindByNameAsync(model.Username) is not null)
            {
                return new AuthModel { Message = "Username is already Exist", IsAuthenticated = false };
            }
            var user = new ApplictionUsers
            {
                UserName = model.Username,
                Email = model.Email,
                Fullname = model.Fullname,
                

            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach(var error in result.Errors)
                {
                    errors = $"{error.Description},";

                }
                return new AuthModel { Message=errors};

            }
            await _userManager.AddToRoleAsync(user, model.Rolename);
            var JwtSecurityToken = await CreateJwtToken(user);
            return new AuthModel
            {
                Email = user.Email,
                ExpiresOn = JwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { model.Rolename },
                Token = new JwtSecurityTokenHandler().WriteToken(JwtSecurityToken),
                Username = user.UserName,
                Message = "it is created"
            };


        }
       private async Task<JwtSecurityToken> CreateJwtToken(ApplictionUsers user)
        {
            var UserClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            foreach (var role in roles)
            {
                roleClaims.Add(new Claim("roles", role));
            }
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)

            }
            .Union(UserClaims)
            .Union(roleClaims);

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
    }
}
