using HR.Contant;
using HR.Models;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace HR.Seed
{
    public static class DefaultRole
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManger)
        {
            if (!roleManger.Roles.Any())
            {
                await roleManger.CreateAsync(new IdentityRole(UserRole.Admin.ToString()));
              
            }
        }
    }
}
