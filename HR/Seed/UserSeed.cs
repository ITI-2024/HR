using HR.Contant;
using HR.Models;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Security.Claims;

namespace HR.Seed
{
    public static class UserSeed
    {
        public static async Task SeedBasicUserAsync(UserManager<ApplictionUsers> _userManager,RoleManager<IdentityRole>_roleManager)
        {
            var usersCount = _userManager.Users.Count();
            if (usersCount <= 0)
            {
                var Adminuser = new ApplictionUsers()
                {
                    UserName = "Admin", //Admin
                    Email = "Admain@gmail.com", //ashraf@pioneers-solutions.com
                    Fullname = "AdminUser", //"Ashraf Nouh CEOAdmin
                    EmailConfirmed = true,
                    PhoneNumber = "123456",
                    PhoneNumberConfirmed = true
                };
    
                    await _userManager.CreateAsync(Adminuser, "Admin@123");
                    await _userManager.AddToRoleAsync(Adminuser, UserRole.Admin.ToString());
            }
        }
    }
  

}
